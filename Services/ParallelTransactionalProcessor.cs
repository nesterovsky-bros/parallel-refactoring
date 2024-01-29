using Microsoft.Extensions.Options;

namespace Test.Services;

public class ParallelTransactionalProcessor(IDataService dataService, IOptions<ParallelSettings> options) : IReportProcessor
{
  public void CreateReport(StringWriter writer)
  {
    using var parallel = new Parallel(options.Value.Parallelism);
    using var _ = dataService.CreateTransaction();
    var index = 0;

    writer.WriteLine("index,transactionId,at,type,amount,sourceAccountId,sourceName,targetAccountId,targetName");

    parallel.ForEachAsync(
      dataService.
        GetTransactions().
        OrderBy(item => (item.At, item.SourceAccountId)),
      transaction =>
      {
        using var _ = dataService.CreateTransaction();
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
      });
  }
}
