using System.Runtime.ExceptionServices;
using System.Threading.Tasks.Dataflow;

namespace Test.Services;

/// <summary>
/// <para>Utility class to simplify refactoring serial code into parallel.</para>
/// <para>Main idea spins around refactoring serial cycles into parallel.</para>
/// <para>Consider following use cases:</para>
/// <code>
/// foreach(source)
/// {
///   A code that does not depend on previous iterations 
///   nor requires order of execution.
/// }
/// </code>
/// <para>This code is refactored as:</para>
/// <code>
/// using var parallel = new Parallel(...);
///
/// ...
/// parallel.ForEachAsync(
///   source,
///   item =>
///   {
///     A code that does not depend on previous iterations 
///     nor requires order of execution.
///   });
/// </code>
/// <para>Consider more complex use case:</para>
/// <code>
/// foreach(source)
/// {
///   1. A code that does not depend on previous iterations 
///   nor requires order of execution.
///   
///   2. A code that needs to be executed serially, like file write, 
///   or aggregation of data.
/// }
/// </code>
/// <para>The refactored code is:</para>
/// <code>
/// ...
/// parallel.ForEachAsync(
///   source,
///   item =>
///   {
///     1. A code that does not depend on previous iterations 
///     nor requires order of execution.
///     
///     parallel.PostSync(data, () =>
///     {
///       2. A code that needs to be executed serially, like file write, 
///       or aggregation of data.
///     });
///   });
/// </code>
public class Parallel: IDisposable
{
  /// <summary>
  /// Creates a <see cref="Parallel"/> instance.
  /// </summary>
  /// <param name="parallelism">A level of parallelism.</param>
  /// <param name="bufferSize">A queue size of pending steps.</param>
  public Parallel(int? parallelism = null, int? bufferSize = null)
  {
    var dop = parallelism ?? Environment.ProcessorCount;
    var capacity = bufferSize ?? dop * 2;

    actions = new(
      f => f(),
      new()
      {
        MaxDegreeOfParallelism = dop,
        BoundedCapacity = capacity
      });
  }

  /// <summary>
  /// Current goto target.
  /// </summary>
  public object? Target => Volatile.Read(ref target);

  /// <summary>
  /// Disposes this instance.
  /// Calls <see cref="Join"/> as part of disposal.
  /// </summary>
  public void Dispose()
  {
    try
    {
      Join();
    }
    finally
    {
      try
      {
        Complete();
      }
      finally
      {
        threadState.Dispose();
      }
    }
  }

  /// <summary>
  /// Waits for all async and sync steps to complete.
  /// </summary>
  public void Join()
  {
    if (threadState.Value != null)
    {
      return;
    }

    List<Exception>? exceptions = null;

    while(true)
    {
      try
      {
        ProcessSync();

        Task completionTask;

        lock(sync)
        {
          if (states.Count == 0)
          {
            break;
          }

          completionTask = AsyncCompletionTask();
        }

        Task.WaitAny(actions.Completion, completionTask);
      }
      catch(Exception e)
      {
        exceptions ??= new(1);
        exceptions.Add(e);
      }
    }

    if (exceptions != null)
    {
      if (exceptions.Count == 1)
      {
        ExceptionDispatchInfo.Throw(exceptions[0]);
      }
      else
      {
        throw new AggregateException(exceptions);
      }
    }
  }

  /// <summary>
  /// Requests to complete the execution.
  /// </summary>
  public void Complete()
  {
    Volatile.Write(ref completed, true);
    Volatile.Write(ref target, null);

    lock(sync)
    {
      states.Clear();
    }

    actions?.Complete();
  }

  /// <summary>
  /// Goes to a label.
  /// </summary>
  public void Goto(object label) => 
    PostSync(() => Volatile.Write(ref target, label));

  /// <summary>
  /// Goes to return.
  /// </summary>
  public void Return() => Goto("return");

  /// <summary>
  /// Goes to the break.
  /// </summary>
  public void Break() => Goto("break");

  /// <summary>
  /// Goes to the continue.
  /// </summary>
  public void Continue() => Goto("continue");

  /// <summary>
  /// Marks a label.
  /// </summary>
  public void Label(object label) => PostSync(() => { }, label);

  /// <summary>
  /// Processes pending sync steps.
  /// </summary>
  public void ProcessSync()
  {
    if (Volatile.Read(ref completed) || threadState.Value != null)
    {
      return;
    }

    while(true)
    {
      State? state;
      Action? step;
      int refCount;

      lock(sync)
      {
        if (!states.TryPeek(out state))
        {
          return;
        }

        (step, refCount, state.step) = (state.step, state.refCount, null);

        if (step == null)
        {
          if (refCount != 0)
          {
            return;
          }

          states.Dequeue();

          continue;
        }
      }

      try
      {
        step();
      }
      catch
      {
        Complete();

        throw;
      }
      finally
      {
        lock(sync)
        {
          if (state is { step: null, refCount: 0 })
          {
            states.TryDequeue(out state);
          }
        }
      }
    }
  }

  /// <summary>
  /// Returns a <see cref="Task"/> that completes when any 
  /// async step is complete.
  /// </summary>
  /// <returns>
  /// A <see cref="Task"/> that completes when any async step is complete.
  /// </returns>
  public Task AsyncCompletionTask()
  {
    if (Volatile.Read(ref completed))
    { 
      return Task.CompletedTask;
    }

    lock(sync)
    {
      return (asyncCompletionSource ??= new()).Task;
    }
  }

  /// <summary>
  /// Posts a synchronous step.
  /// </summary>
  /// <param name="step">A step to execute.</param>
  /// <param name="label">Optional goto target label.</param>
  public void PostSync(Action step, object? label = null)
  {
    if (Volatile.Read(ref completed))
    {
      return;
    }

    var state = threadState.Value;

    if (state is not null)
    {
      lock(sync)
      {
        state.step += run;
      }
    }
    else
    {
      run();
    }

    void run()
    {
      var target = Volatile.Read(ref this.target);

      if (target != null)
      {
        if (target != label)
        {
          return;
        }

        target = null;
      }

      (var prev, threadState.Value) = (threadState.Value, state);

      try
      {
        step();
      }
      finally
      {
        threadState.Value = prev;
      }
    }
  }

  /// <summary>
  /// Passes values from source called in this thread to target 
  /// called sync thread.
  /// </summary>
  /// <typeparam name="T">Type of data to pass to step.</typeparam>
  /// <param name="data">Data to pass into a step.</param>
  /// <param name="step">A step to execute.</param>
  /// <param name="label">Optional goto target label.</param>
  public void PostSync<T>(T data, Action<T> step, object? label = null) =>
    PostSync(() => step(data), label);

  /// <summary>
  /// Posts an asynchronous step.
  /// </summary>
  /// <param name="step">A step to execute.</param>
  /// <param name="isolated">
  /// Indicates whether to use isolated state, or reuse existing one, 
  /// if available.
  /// </param>
  /// <param name="escapes">Optional escape targets to break the step.</param>
  public void PostAsync(Action step,
    bool isolated = true,
    params object[] escapes)
  {
    if (Volatile.Read(ref completed) || MatchesTarget(escapes))
    {
      return;
    }

    var stepCompleted = false;
    var state = isolated ? null : threadState.Value;

    if (state == null)
    {
      state = new State();

      lock(sync)
      {
        states.Enqueue(state);
      }
    }
    else
    {
      lock(sync)
      {
        ++state.refCount;
      }
    }

    try
    {
      var acceptedTask = actions.SendAsync(() =>
      {
        if (Volatile.Read(ref completed) || MatchesTarget(escapes))
        {
          return;
        }

        (var prev, threadState.Value) = (threadState.Value, state);

        try
        {
          step();
        }
        catch(Exception e)
        {
          state.step += () => ExceptionDispatchInfo.Throw(e);
        }
        finally
        {
          try
          {
            threadState.Value = prev;
          }
          finally
          {
            completeStep();
          }
        }
      });

      do
      {
        ProcessSync();
        Task.WaitAny(acceptedTask, AsyncCompletionTask());
      }
      while(!acceptedTask.IsCompleted);

      if (!acceptedTask.Result)
      {
        Complete();

        throw new InvalidOperationException();
      }
    }
    catch
    {
      completeStep();

      throw;
    }

    void completeStep()
    {
      TaskCompletionSource? completionSource;

      lock(sync)
      {
        if (!stepCompleted)
        {
          stepCompleted = true;
          --state.refCount;
        }

        completionSource = asyncCompletionSource;
        asyncCompletionSource = null;
      }

      completionSource?.SetResult();
    }
  }

  /// <summary>
  /// Posts an asynchronous step.
  /// </summary>
  /// <typeparam name="T">Type of data to pass to step.</typeparam>
  /// <param name="data">Data to pass into a step.</param>
  /// <param name="step">A step to execute.</param>
  /// <param name="isolated">
  /// Indicates whether to use isolated state, or reuse existing one, 
  /// if available.
  /// </param>
  /// <param name="escapes">Optional escape targets to break the step.</param>
  public void PostAsync<T>(
    T data,
    Action<T> step,
    bool isolated = true,
    params object[] escapes) =>
    PostAsync(() => step(data), isolated, escapes);

  /// <summary>
  /// Enumerates actions from a source and post for async steps.
  /// After completions call Join().
  /// </summary>
  /// <param name="source">A source of items (items are discarded).</param>
  /// <param name="step">A step to execute.</param>
  /// <param name="escapes">Optional escape targets to break the step.</param>
  /// <typeparam name="T">Item type of source.</typeparam>
  public void ForEachAsync<T>(
    IEnumerable<T> source,
    Action<T> step,
    params object[] escapes)
  {
    if (Volatile.Read(ref completed))
    {
      return;
    }

    var defaultEscapes = new object[] { "break" };

    foreach(var item in source)
    {
      if (Volatile.Read(ref completed) || 
        MatchesTarget(escapes) || 
        MatchesTarget(defaultEscapes))
      {
        break;
      }

      PostAsync(
        item,
        data =>
        {
          step(data);
          Label("continue");
        },
        escapes: escapes);

    }

    Join();
    Label("break");
  }

  /// <summary>
  /// Tests whether current goto target matches on of the values.
  /// </summary>
  /// <param name="targets">Optional array of goto targets.</param>
  /// <returns>
  /// true if current goto target matches one of targets, and false otherwise.
  /// </returns>
  private bool MatchesTarget(object[]? targets)
  {
    if (!(targets?.Length > 0))
    {
      return false;
    }

    var target = Volatile.Read(ref this.target);

    return target is "return" ||
      target is not null && Array.IndexOf(targets, target) >= 0;
  }

  /// <summary>
  /// Interanl sync state of an async step.
  /// </summary>
  private class State
  {
    /// <summary>
    /// Reference count.
    /// </summary>
    public int refCount = 1;

    /// <summary>
    /// Pending sync steps.
    /// </summary>
    public Action? step;
  }

  /// <summary>
  /// An action block to run async steps.
  /// </summary>
  private readonly ActionBlock<Action> actions;

  /// <summary>
  /// An internal synchronization.
  /// </summary>
  private readonly object sync = new();

  /// <summary>
  /// A queue of sync steps.
  /// </summary>
  private readonly Queue<State> states = new();

  /// <summary>
  /// A thread state.
  /// </summary>
  private readonly ThreadLocal<State?> threadState = new();

  /// <summary>
  /// Indicates whether processing has completed.
  /// </summary>
  private bool completed;

  /// <summary>
  /// Current goto target.
  /// </summary>
  private object? target;

  /// <summary>
  /// A completion source for the async step.
  /// </summary>
  private TaskCompletionSource? asyncCompletionSource;
}
