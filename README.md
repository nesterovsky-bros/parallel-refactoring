### Experience of parallel refactoring

#### Introduction

We migrate code out of MF to Azure.
Tool we use produces plain good functionally equivalent C# code.
But it turns it's not enough!

So, what's the problem?

Converted code is very slow, especially for batch processing, 
where MF completes job, say in 30 minutes, while converted code
finishes in 8 hours.

At this point usually someone appears and whispers in the ear:

> Look, those old technologies are proven by the time. It worth to stick to old good Cobol, or better to Assembler if you want to do the real thing.

We're curious though: why is there a difference?

Turns out the issue lies in differences of network topology between MF and Azure solutions.
On MF all programs, database and file storage virtually sit in a single box, thus network latency is negligible. 

It's rather usual to see chatty SQL programs on MF that are doing a lot of small SQL queries.

In Azure - programs, database, file storage are different services most certainly sitting in different physical boxes. 
You should be thankfull if they are co-located in a single datacenter. 
So, network latency immediately becomes a factor. 
Even if it just adds 1 millisecond per SQL roundtrip, it adds up in loops, and turns in the showstopper.

There is no easy workaround on the hardware level.

People advice to write programs differently: "[Tune applications and databases for performance in Azure SQL Database](https://learn.microsoft.com/en-us/azure/azure-sql/database/performance-guidance)".

That's a good advice for a new development but discouraging for migration done by a tool.

So, what is the way forward?

Well, there is one. While accepting weak sides of Azure we can exploit its strong sides.

#### Parallel refactoring

Before continuing let's consider a [code](./Services/SerialProcessor.cs#L5-L29) demoing the problem:

```C#
  public void CreateReport(StringWriter writer)
  {
    var index = 0;

    foreach(var transaction in dataService.
      GetTransactions().
      OrderBy(item => (item.At, item.SourceAccountId)))
    {
      var sourceAccount = dataService.GetAccount(transaction.SourceAccountId);
      var targetAccount = transaction.TargetAccountId != null ?
        dataService.GetAccount(transaction.TargetAccountId) : null;

      ++index;

      if (index % 100 == 0)
      { 
        Console.WriteLine(index);
      }

      writer.WriteLine($"{index},{transaction.Id},{
        transaction.At},{transaction.Type},{transaction.Amount},{
        transaction.SourceAccountId},{sourceAccount?.Name},{
        transaction.TargetAccountId},{targetAccount?.Name}");
    }
  }
```

This cycle queries transactions, along with two more small queries to get source and target accounts for each transaction. Results are printed into a report.

If we assume query latency just 1 millisecond, and try to run such code for 100K transactions we easily come to 200+ seconds of execution. 

Reality turns to be much worse. Program spends most of its lifecycle waiting for database results, and iterations don't advance until all work of previous iterations is complete.

We could do better even without trying to rewrite all code!
Let's articulate our goals:

1. To make code fast.
2. To leave code recognizable.

The idea is to form two processing pipelines:
- (a) one that processes data in parallel out of order;
- (b) other that processes data serially, in original order;

Each pipeline may post sub-tasks and immutable or a copy of data to the other, so (a) runs its tasks in parallel unordered, while (b) runs its tasks as if everything was running serially.

So, parallel plan would be like this:
1. Queue parallel sub-tasks (a) for each transaction.
2. Parallel sub-task in (a) reads source and target accounts, and queues serial sub-task (b) passing transaction and accounts.
3. Serial sub-task (b) increments index, and writes report record.
4. Wait for all tasks to complete.

To reduce burden of task piplelines we use [Dataflow (Task Parallel Library)](https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/dataflow-task-parallel-library), and encapsulate everything in a small wrapper.

Consider refactored [code](./Services/ParallelProcessor.cs#L14-L48):

```C#
  public void CreateReport(StringWriter writer)
  {
    using var parallel = new Parallel(options.Value.Parallelism); // 	⬅️ 1
    var index = 0;

    parallel.ForEachAsync( // 	⬅️ 2
      dataService.
        GetTransactions().
        OrderBy(item => (item.At, item.SourceAccountId)),
      transaction => // 	⬅️ 3
      {
        var sourceAccount = dataService.GetAccount(transaction.SourceAccountId);
        var targetAccount = transaction.TargetAccountId != null ?
          dataService.GetAccount(transaction.TargetAccountId) : null;

        parallel.PostSync(  // 	⬅️ 4
          (transaction, sourceAccount, targetAccount),  // 	⬅️ 5
          data =>
          {
            var (transaction, sourceAccount, targetAccount) = data; // 	⬅️ 6

            ++index;

            if (index % 100 == 0)
            {
              Console.WriteLine(index);
            }

            writer.WriteLine($"{index},{transaction.Id},{
              transaction.At},{transaction.Type},{transaction.Amount},{
              transaction.SourceAccountId},{sourceAccount?.Name},{
              transaction.TargetAccountId},{targetAccount?.Name}");
          });
      });
  }
```

Consider ⬅️ points:
1. We create `Parallel` utility class passing degree of parallelism requested.
2. We iterate transactions using `parallel.ForEachAsync()` that queues parallel sub-tasks for each transaction, and then waits until all tasks are complete.
3. Each parallel sub-task recieves a transaction. It may be called from a different thread.
4. Having recieved required accounts we queue a sub-task for synchronous execution using `parallel.PostSync()`, and
5. Pass there data collected in parallel sub-task: transaction and accounts.
6. We deconstruct data passed into variables, and then proceed with serial logic.

What we achieve with this refactoring:
1. Top level query that brings transactions is done and iterated serially.
2. But each iteration body is run in parallel. By default we set it up to allow up to 100 parallel executions.
   All those parallel sub-task do not wait on each other so their waitings do not add up.
3. Sync sub-tasks are queued and executed in order of their serial appearance, so increments and report records are not a subject of race conditions, nor a subject of reordering of output records.

We think that such refactored code is still recognizible.

As for performance this is what log shows:

```log
SerialProcessor
100
...
Execution time: 00:01:33.8152540

ParallelProcessor
100
...
Execution time: 00:00:05.8705468
```

#### Complications

Now consider a less trivial code where iteration depends previous values. 
To make iterations parallel we need to try to make the independant first. Consider the [code](./Services/SerialDependantProcessor.cs):

```C#
  public void CreateReport(StringWriter writer)
  {
    var index = 0;
    string? prevSourceAccountId = null; //  ⬅️ 1
    var subIndex = 0;

    foreach(var transaction in dataService.
      GetTransactions().
      OrderBy(item => (item.SourceAccountId, item.At)))
    {
      if (transaction.SourceAccountId != prevSourceAccountId) //  ⬅️ 2
      {
        subIndex = 0;
      }

      var sourceAccount = dataService.GetAccount(transaction.SourceAccountId);
      var targetAccount = transaction.TargetAccountId != null ?
        dataService.GetAccount(transaction.TargetAccountId) : null;

      ++index;
      ++subIndex;

      if (index % 100 == 0)
      { 
        Console.WriteLine(index);
      }

      writer.WriteLine($"{index},{subIndex},{transaction.Id},{
        transaction.At},{transaction.Type},{transaction.Amount},{
        transaction.SourceAccountId},{sourceAccount?.Name},{
        transaction.TargetAccountId},{targetAccount?.Name}");

      prevSourceAccountId = transaction.SourceAccountId; //  ⬅️ 3
    }
  }
```

Here we remember previous account, and count index within account. In this and similar cases there is an easy workaround to track item and previous item within enumerable.
With simple LINQ extension [WithContext()](./Services/Extensions.cs#L15) we do just this, so the refactored [code](./Services/ParallelDependantProcessor.cs) looks like:

```C#
  public void CreateReport(StringWriter writer)
  {
    using var parallel = new Parallel(options.Value.Parallelism);
    var index = 0;
    string? prevSourceAccountId = null;
    var subIndex = 0;

    writer.WriteLine("index,subIndex,transactionId,at,type,amount,sourceAccountId,sourceName,targetAccountId,targetName");

    parallel.ForEachAsync(
      dataService.GetTransactions().
        OrderBy(item => (item.SourceAccountId, item.At)).
        WithContext(), //  ⬅️ 1
      item =>
      {
        (var transaction, prevSourceAccountId) = (item.current, item.prev?.SourceAccountId); //  ⬅️ 2

        if (transaction.SourceAccountId != prevSourceAccountId)  //  ⬅️ 3
        {
          parallel.PostSync(() =>
          {
            subIndex = 0;
          });
        }

        var sourceAccount = dataService.GetAccount(transaction.SourceAccountId);
        var targetAccount = transaction.TargetAccountId != null ?
          dataService.GetAccount(transaction.TargetAccountId) : null;

        parallel.PostSync(
          (transaction, sourceAccount, targetAccount),
          data =>
          {
            var (transaction, sourceAccount, targetAccount) = data;

            ++index;
            ++subIndex;

            if (index % 100 == 0)
            { 
              Console.WriteLine(index);
            }

            writer.WriteLine($"{index},{subIndex},{transaction.Id},{
              transaction.At},{transaction.Type},{transaction.Amount},{
              transaction.SourceAccountId},{sourceAccount?.Name},{
              transaction.TargetAccountId},{targetAccount?.Name}");

            prevSourceAccountId = transaction.SourceAccountId;
          });
      });
  }
```

See we apply [WithContext()](./Services/Extensions.cs#L15), and then deconstruct `transaction` and `prevSourceAccountId` from `item`.

Let's consider another complication that appears in such a code.

#### Transactions

Above samples are intentionally simplified to focus on the idea. 
In reallity when you deal with the database there are (explicit or implicit) transactions, 
which you should pay for. When you wrap code into a transaction its cost is minimal but 
with parallel refactoring we put each iteration in a separate transaction. 
So cost of transaction, even if it's small might raise.

So, consider another serial [code](./Services/SerialTransactionalProcessor.cs) that uses a transaction:

```C#
  public void CreateReport(StringWriter writer)
  {
    using var _ = dataService.CreateTransaction(); //  ⬅️ 1
    var index = 0;

    foreach(var transaction in dataService.
      GetTransactions().
      OrderBy(item => (item.At, item.SourceAccountId)))
    {
      var sourceAccount = dataService.GetAccount(transaction.SourceAccountId);
      var targetAccount = transaction.TargetAccountId != null ?
        dataService.GetAccount(transaction.TargetAccountId) : null;

      ++index;

      if (index % 100 == 0)
      { 
        Console.WriteLine(index);
      }

      writer.WriteLine($"{index},{transaction.Id},{
        transaction.At},{transaction.Type},{transaction.Amount},{
        transaction.SourceAccountId},{sourceAccount?.Name},{
        transaction.TargetAccountId},{targetAccount?.Name}");
    }
  }
```

Mark ⬅️ 1 points to a transaction resource created at start and disposed at the end.
Let's, again, assume for the purpose of discussion it add 1 millisecond and each side.

So, just 2 milliseconds?   
Who cares?  
Right?

Now consider [code](./Services/ParallelTransactionalProcessor.cs) for parallel refactoring:

```C#
    using var parallel = new Parallel(options.Value.Parallelism);
    using var _ = dataService.CreateTransaction(); //  ⬅️ 1
    var index = 0;

    parallel.ForEachAsync(
      dataService.
        GetTransactions().
        OrderBy(item => (item.At, item.SourceAccountId)),
      transaction =>
      {
        using var _ = dataService.CreateTransaction(); //  ⬅️ 2
        var sourceAccount = dataService.GetAccount(transaction.SourceAccountId);
        var targetAccount = transaction.TargetAccountId != null ?
          dataService.GetAccount(transaction.TargetAccountId) : null;

        parallel.PostSync(
          (transaction, sourceAccount, targetAccount),
          data =>
          {
            var (transaction, sourceAccount, targetAccount) = data;

            ++index;

            if(index % 100 == 0)
            {
              Console.WriteLine(index);
            }

            writer.WriteLine($"{index},{transaction.Id},{transaction.At},{transaction.Type},{transaction.Amount},{transaction.SourceAccountId},{sourceAccount?.Name},{transaction.TargetAccountId},{targetAccount?.Name}");
          });
      });
  }
```

Notice transactions 1 and 2 that happen at top lelel and on each iteration.
So, each iteration pays cost of transaction.

Parallel code is still much faster than serial but can we do better?
Yes, we can amortize cost of transactions by processing data by chunks. We shall use LINK `Chunk()` to do it.
Consider another version of parallel [code](./Services/ParallelChunkingTransactionalProcessor.cs):

```C#
  public void CreateReport(StringWriter writer)
  {
    using var parallel = new Parallel(options.Value.Parallelism); //  ⬅️ 1
    using var _ = dataService.CreateTransaction();
    var index = 0;

    parallel.ForEachAsync(
      dataService.
        GetTransactions().
        OrderBy(item => (item.At, item.SourceAccountId)).
        Chunk(10), //  ⬅️ 2
      chunk =>
      {
        using var _ = dataService.CreateTransaction(); //  ⬅️ 3

        foreach(var transaction in chunk) //  ⬅️ 4
        {
          var sourceAccount = dataService.GetAccount(transaction.SourceAccountId);
          var targetAccount = transaction.TargetAccountId != null ?
            dataService.GetAccount(transaction.TargetAccountId) : null;

          parallel.PostSync(
            (transaction, sourceAccount, targetAccount),
            data =>
            {
              var (transaction, sourceAccount, targetAccount) = data;

              ++index;

              if(index % 100 == 0)
              {
                Console.WriteLine(index);
              }

              writer.WriteLine($"{index},{transaction.Id},{
                transaction.At},{transaction.Type},{transaction.Amount},{
                transaction.SourceAccountId},{sourceAccount?.Name},{
                transaction.TargetAccountId},{targetAccount?.Name}");
            });
        }
      });
  }
```

Marks show:
1. Top level transaction.
2. Chunk data by 10 items.
3. Iteration transaction for a chunk.
4. Scan items in the chunk and process.

So, what is the gain. Again, we shall look at log:

```log
SerialTransactionalProcessor
100
...
Execution time: 00:01:34.8674370

ParallelTransactionalProcessor
100
...
Execution time: 00:00:10.7874279

ParallelChunkingTransactionalProcessor
100
...
Execution time: 00:00:04.9963745
```

So, the gain is sensitive.

#### Conclusion

We're far from thinking that any code can be sped up this way.
E.g. you cannot speed more already refactored code. :-)
But in many cases it's possible, and gains worth the efforts.

While doing such refactoring you should seriously analyze whether parallel changes are permitted after all, as they cross transaction boundaries.
In some cases that is not an option.

Another point to note is that any communication between async and sync parts must be with immutable or with copies of objects.
You should never forget that code parts may run in different threads in parallel, so any sharing of data must be thread safe.

To avoid potential problem consider using static lambdas and passing all data as parameters.

#### Reference
Please take a look at project to understand implem nentation details, and in particular
[`Parallel`](./Services/Parallel.cs) class implementing API to post parallel and serial tasks, run cycles and some more.
