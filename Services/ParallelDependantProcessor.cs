using Microsoft.Extensions.Options;

namespace Test.Services;

public class ParallelDependantProcessor(IDataService dataService, IOptions<ParallelSettings> options) : IReportProcessor
{
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
        WithContext(),
      item =>
      {
        (var transaction, prevSourceAccountId) = (item.current, item.prev?.SourceAccountId);

        if (transaction.SourceAccountId != prevSourceAccountId)
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
}
