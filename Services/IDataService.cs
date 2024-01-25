using Test.Model;

namespace Test.Services;

public interface IDataService
{
  IEnumerable<Transaction> GetTransactions();

  Account? GetAccount(string id);
}
