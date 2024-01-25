using Microsoft.Extensions.Options;

namespace Test.Services;

public class ParallelProcessor(
  IDataService dataService, 
  IOptions<ParallelProcessor.Settings> options): IReportProcessor
{
  public record Settings
  { 
    public int Parallelism { get; set; }
  }

  public void CreateReport(StringWriter writer)
  {
    var parallel = new Parallel(options.Value.Parallelism);
    var index = 0;

    parallel.ForEachAsync(
      dataService.
        GetTransactions().
        OrderBy(item => (item.At, item.SourceAccountId)),
      transaction =>
      {
        var sourceAccount = dataService.GetAccount(transaction.SourceAccountId);
        var targetAccount = transaction.TargetAccountId != null ?
          dataService.GetAccount(transaction.TargetAccountId) : null;

        parallel.PostSync(
          (
            transaction,
            sourceAccount,
            targetAccount
          ),
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
      });
  }
}
