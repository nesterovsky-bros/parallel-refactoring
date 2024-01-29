namespace Test.Services;

public class SerialDependantProcessor(IDataService dataService) : IReportProcessor
{
  public void CreateReport(StringWriter writer)
  {
    writer.WriteLine("index,subIndex,transactionId,at,type,amount,sourceAccountId,sourceName,targetAccountId,targetName");

    var index = 0;
    string? prevSourceAccountId = null;
    var subIndex = 0;

    foreach(var transaction in dataService.
      GetTransactions().
      OrderBy(item => (item.SourceAccountId, item.At)))
    {
      if (transaction.SourceAccountId != prevSourceAccountId)
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

      prevSourceAccountId = transaction.SourceAccountId;
    }
  }
}
