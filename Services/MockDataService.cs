using Test.Model;

namespace Test.Services;

public class MockDataService: IDataService
{
  public MockDataService()
  {
    var random = new Random(12345);

    var accountList = Enumerable.
      Range(0, 1_000).
      Select(index => new Account
      {
        Id = $"{random.Next()}-{index}",
        Name = $"{Names[random.Next(Names.Length)]} {Surnames[random.Next(Surnames.Length)]}",
        BranchId = random.Next() % 1000
      }).
      ToList();

    accounts = accountList.ToDictionary(account => account.Id);

    transactions = Enumerable.
      Range(0, 3_000).
      Select(index => new Transaction
      {
        Id = $"{random.Next()}-{index}",
        Type = random.Next(1000),
        At = new DateTime(2024, 1, random.Next(1, 32)),
        Amount = random.Next(10, 1001),
        SourceAccountId = accountList[random.Next(accountList.Count)].Id,
        TargetAccountId = accountList[random.Next(accountList.Count)].Id,
      }).
      ToList();
  }

  public Account? GetAccount(string id)
  {
    Thread.Sleep(1);

    return accounts.TryGetValue(id, out var account) ? account : null;
  }

  public IEnumerable<Transaction> GetTransactions()
  {
    Thread.Sleep(1);

    return transactions;
  }

  private readonly List<Transaction> transactions;
  private readonly Dictionary<string, Account> accounts;

  private static readonly string[] Names =
  [
    "Michael", "Emma", "David", "Sophia", "Ethan",
    "Olivia", "Matthew", "Ava", "Daniel", "Isabella",
    "Alexander", "Mia", "Andrew", "Charlotte", "Joseph",
    "Amelia", "Lucas", "Harper", "Henry", "Evelyn",
    "Samuel", "Abigail", "Anthony", "Emily", "Benjamin",
    "Madison", "Jack", "Luna", "Sebastian", "Grace",
    "William", "Sofia", "James", "Avery", "Owen",
    "Ella", "Gabriel", "Scarlett", "Carter", "Chloe",
    "Ryan", "Layla", "Dylan", "Riley", "Nathan",
    "Zoey", "Caleb", "Lily", "Thomas", "Ellie",
    "Leo", "Hannah", "Isaac", "Addison", "Julian",
    "Nora", "Luke", "Zoe", "Grayson", "Stella",
    "Christopher", "Natalie", "Jack", "Penelope", "Mason",
    "Violet", "Logan", "Lillian", "Elijah", "Elizabeth",
    "Oliver", "Lucy", "Jacob", "Kinsley", "Aiden",
    "Ruby", "Joshua", "Aubrey", "Isaiah", "Kayla",
    "Adam", "Paisley", "Adrian", "Samantha", "Aaron",
    "Anna", "Kevin", "Leah", "Tyler", "Audrey",
    "Brian", "Aurora", "Nicholas", "Brooklyn", "Gavin",
    "Camila", "Christian", "Sadie", "Evan", "Serenity",
    "Jordan", "Allison", "Zachary", "Savannah", "Ian",
    "Alexa", "Jason", "Claire", "Austin", "Khloe",
    "Connor", "Victoria", "Hunter", "Jasmine", "Bradley",
    "Lauren", "Jose", "Mackenzie", "Xavier", "Bella"
  ];

  private static readonly string[] Surnames =
  [
    "Smith", "Johnson", "Williams", "Brown", "Jones",
    "Garcia", "Miller", "Davis", "Rodriguez", "Martinez",
    "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson",
    "Thomas", "Taylor", "Moore", "Jackson", "Martin",
    "Lee", "Perez", "Thompson", "White", "Harris",
    "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson",
    "Walker", "Young", "Allen", "King", "Wright",
    "Scott", "Torres", "Nguyen", "Hill", "Flores",
    "Green", "Adams", "Nelson", "Baker", "Hall",
    "Rivera", "Campbell", "Mitchell", "Carter", "Roberts",
    "Gomez", "Phillips", "Evans", "Turner", "Diaz",
    "Parker", "Cruz", "Edwards", "Collins", "Reyes",
    "Stewart", "Morris", "Morales", "Murphy", "Cook",
    "Rogers", "Gutierrez", "Ortiz", "Morgan", "Cooper",
    "Peterson", "Bailey", "Reed", "Kelly", "Howard",
    "Ramos", "Kim", "Cox", "Ward", "Richardson",
    "Watson", "Brooks", "Chavez", "Wood", "James",
    "Bennett", "Gray", "Mendoza", "Ruiz", "Hughes",
    "Price", "Alvarez", "Castillo", "Sanders", "Patel",
    "Myers", "Long", "Ross", "Foster", "Jimenez"  
  ];
}
