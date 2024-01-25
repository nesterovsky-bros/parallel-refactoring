namespace Test.Services;

public class SerialProcessor(IDataService dataService) : IReportProcessor
{
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
}
