namespace Test.Services;

/// <summary>
/// Utility extensions.
/// </summary>
public static class Extensions
{
  /// <summary>
  /// Ammeds enumerable with context information about 
  /// index and previous and following items.
  /// </summary>
  /// <typeparam name="T">A source item type.</typeparam>
  /// <param name="source">A source enumerable.</param>
  /// <returns>Enumerable with context.</returns>
  public static IEnumerable<(T current, int index, T? prev, T? next)> WithContext<T>(this IEnumerable<T> source)
  {
    T? prev = default;
    T? current = default;
    var index = -1;

    foreach(var item in source)
    {
      if (index >= 0)
      {
        yield return (current!, index, prev, item);
      }

      ++index;
      (prev, current) = (current, item);
    }

    if (index >= 0)
    {
      yield return (current!, index, prev, default);
    }
  }
}
