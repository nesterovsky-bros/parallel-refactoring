using Test.Model;

namespace Test.Services;

public interface IDataService
{
  IDisposable CreateTransaction();

  IEnumerable<Transaction> GetTransactions();

  Account? GetAccount(string id);
}
