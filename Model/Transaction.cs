namespace Test.Model;

public record Transaction
{
  public string Id { get; init; } = null!;
  public DateTime At { get; init; }
  public int Type { get; init; }
  public decimal Amount { get; init; }
  public string SourceAccountId { get; init; } = null!;
  public string? TargetAccountId { get; init; }
  public string? Notes { get; init; }
}
