namespace Test.Model;

public record Account
{
  public string Id { get; init; } = null!;
  public string Name { get; init; } = null!;
  public int BranchId { get; init; }
  public string? Description { get; init; }
}
