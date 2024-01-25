using Test.Model;

namespace Test.Services;

public class MockDataService: IDataService
{
  public MockDataService()
  {
    var random = new Random(12345);

    var accountList = AccountsData.
      Split(Environment.NewLine).
      Skip(1).
      Select(line => line.Split(",")).
      Select((row, index) => new Account
      {
        Id = $"{random.Next()}-{index}",
        Name = row[0],
        BranchId = int.Parse(row[1]),
        Description = row[2]
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

  private const string AccountsData = @"Id,Name,BranchId,Description
John Smith,1,Personal Account
Sarah Johnson,1,Checking Account
Michael Davis,2,Savings Account
Jessica Anderson,3,Money Market Account
Christopher Lee,2,Term Deposit Account
Amanda Baker,4,Joint Account
Matthew Wright,3,Certificate of Deposit Account
Stephanie Martinez,1,Student Account
David Thompson,4,Investment Account
Ashley Young,2,Personal Account
Brian Clark,3,Checking Account
Katherine Lewis,1,Savings Account
Jonathan White,4,Money Market Account
Christine Hill,2,Term Deposit Account
Andrew Walker,3,Joint Account
Melissa Turner,1,Certificate of Deposit Account
Joshua Robinson,4,Student Account
Laura Harris,2,Investment Account
Robert King,3,Personal Account
Michelle Turner,4,Checking Account
William Scott,1,Savings Account
Amanda Gonzalez,2,Money Market Account
Jason Rodriguez,3,Term Deposit Account
Emily Lewis,4,Joint Account
Christopher Adams,1,Certificate of Deposit Account
Rebecca Parker,2,Student Account
Daniel Garcia,3,Investment Account
Julia Stewart,4,Personal Account
James Morgan,1,Checking Account
Natalie Carter,2,Savings Account
Matthew Perez,3,Money Market Account
Alexander Howard,4,Term Deposit Account
Samantha Turner,1,Joint Account
Steven Johnson,2,Certificate of Deposit Account
Amy Adams,3,Student Account
Jonathan White,4,Investment Account
Olivia Anderson,1,Personal Account
Benjamin Mitchell,2,Checking Account
Elizabeth Martinez,3,Savings Account
David Lewis,4,Money Market Account
Rachel Thompson,1,Term Deposit Account
Thomas Wright,2,Joint Account
Emily Clark,3,Certificate of Deposit Account
Nathan Scott,4,Student Account
Jessica King,1,Investment Account
Ryan Stewart,2,Personal Account
Megan Turner,3,Checking Account
Brian Johnson,4,Savings Account
Jennifer Davis,1,Money Market Account
Brandon Gonzalez,2,Term Deposit Account
Christina Brown,3,Joint Account
Nicholas Perez,4,Certificate of Deposit Account
Amanda Adams,1,Student Account
Jason Wilson,2,Investment Account
Stephanie Walker,3,Personal Account
Thomas Turner,4,Checking Account
Erica Thompson,1,Savings Account
James Martinez,2,Money Market Account
Sarah Johnson,3,Term Deposit Account
Matthew Hernandez,4,Joint Account
Alexander Scott,1,Certificate of Deposit Account
Jennifer Clark,2,Student Account
Megan Harris,3,Investment Account
Laura Adams,4,Personal Account
Brandon Turner,1,Checking Account
Emily Thompson,2,Savings Account
Christopher Davis,3,Money Market Account
Erica Martinez,4,Term Deposit Account
Jason Wilson,1,Joint Account
Kimberly Turner,2,Certificate of Deposit Account
Eric Adams,3,Student Account
Kelly Walker,4,Investment Account
Joshua Perez,1,Personal Account
Lauren Gonzalez,2,Checking Account
Matthew Hernandez,3,Savings Account
Samantha Robinson,4,Money Market Account
Melissa Walker,1,Term Deposit Account
Brian Scott,2,Joint Account
David Wright,3,Certificate of Deposit Account
Nicole Turner,4,Student Account
Andrew Adams,1,Investment Account
Brittany Thompson,2,Personal Account
William Wilson,3,Checking Account
Katherine Lewis,4,Savings Account
John Martinez,1,Money Market Account
Courtney Clark,2,Term Deposit Account
Michael Walker,3,Joint Account
Amy Roberts,4,Certificate of Deposit Account
Steven Johnson,1,Student Account
Alexandra Turner,2,Investment Account
Christopher Davis,3,Personal Account
Alexis Roberts,4,Checking Account
Michael Thompson,1,Savings Account
Catherine Wright,2,Money Market Account
Jonathan Parker,3,Term Deposit Account
Anna Hernandez,4,Joint Account
Benjamin Wright,1,Certificate of Deposit Account
Sarah Turner,2,Student Account
Robert Garcia,3,Investment Account
Hannah Martinez,4,Personal Account
Megan Adams,1,Checking Account
Thomas Scott,2,Savings Account
Emily Wilson,3,Money Market Account
Daniel Turner,4,Term Deposit Account
Kimberly Clark,1,Joint Account
Joshua Robinson,2,Certificate of Deposit Account
Jennifer Harris,3,Student Account
Jason Thompson,4,Investment Account
Jennifer Turner,1,Personal Account
Michael White,2,Checking Account
Amanda Martinez,3,Savings Account
James Anderson,4,Money Market Account
Natalie Davis,1,Term Deposit Account
Matthew Wright,2,Joint Account
Amy Thompson,3,Certificate of Deposit Account
Jonathan Walker,4,Student Account
Emily Wright,1,Investment Account
David Turner,2,Personal Account
Sarah Martinez,3,Checking Account
Jonathan Lewis,4,Savings Account
Daniel Walker,1,Money Market Account
Brittany Wright,2,Term Deposit Account
Robert Adams,3,Joint Account
Hannah Turner,4,Certificate of Deposit Account
Katherine Scott,1,Student Account
David Thompson,2,Investment Account
Emily Harris,3,Personal Account
Alexander Clark,4,Checking Account
Kimberly Wilson,1,Savings Account
Jonathan Gonzalez,2,Money Market Account
Benjamin Adams,3,Term Deposit Account
Rebecca Walker,4,Joint Account
Brian Lewis,1,Certificate of Deposit Account
Amanda Parker,2,Student Account
Matthew Smith,3,Investment Account
Kristen Turner,4,Personal Account
Michael Adams,1,Checking Account
Catherine Harris,2,Savings Account
Jonathan Wright,3,Money Market Account
Laura Thompson,4,Term Deposit Account
Nicole Clark,1,Joint Account
Daniel Turner,2,Certificate of Deposit Account
Ashley Wilson,3,Student Account
Joshua Thompson,4,Investment Account
Matthew Roberts,1,Personal Account
Brittany Walker,2,Checking Account
Ashley Scott,3,Savings Account
Evan Hernandez,4,Money Market Account
Nicole Lewis,1,Term Deposit Account
Christopher Adams,2,Joint Account
Amanda Turner,3,Certificate of Deposit Account
Kyle Thompson,4,Student Account
Chelsea Walker,1,Investment Account
Michael Turner,2,Personal Account
Amber Martinez,3,Checking Account
Katherine Davis,4,Savings Account
Jonathan Wright,1,Money Market Account
Megan Thompson,2,Term Deposit Account
Joshua Adams,3,Joint Account
Rebecca Turner,4,Certificate of Deposit Account
Daniel Scott,1,Student Account
Amanda Roberts,2,Investment Account
Jeffrey Wilson,3,Personal Account
Jasmine Walker,4,Checking Account
Katherine Thompson,1,Savings Account
Michael Johnson,2,Money Market Account
Christopher Lewis,3,Term Deposit Account
Anna Turner,4,Joint Account
Kimberly Clark,1,Certificate of Deposit Account
Nicole Smith,2,Student Account
Steven Thompson,3,Investment Account
Brandon Wright,4,Personal Account
Melissa Hernandez,1,Checking Account
Jonathan Davis,2,Savings Account
Amanda Turner,3,Money Market Account
David Adams,4,Term Deposit Account
Melissa Turner,1,Joint Account
Nicole Clark,2,Certificate of Deposit Account
Justin Thompson,3,Student Account
Matthew Martinez,4,Investment Account
Ashley Walker,1,Personal Account
Daniel Johnson,2,Checking Account
Emily Harris,3,Savings Account
Amanda Turner,4,Money Market Account
Jonathan Lewis,1,Term Deposit Account
Lauren Walker,2,Joint Account
Brian Thompson,3,Certificate of Deposit Account
David Scott,4,Student Account
Jonathan Clark,1,Investment Account
Megan Wright,2,Personal Account
Brittany Turner,3,Checking Account
Michael Davis,4,Savings Account
Jonathan Wright,1,Money Market Account
Katherine Thompson,2,Term Deposit Account
Matthew Turner,3,Joint Account
Amanda King,4,Certificate of Deposit Account
Jonathan Hernandez,1,Student Account
Robert Turner,2,Investment Account
Nicole Wright,3,Personal Account
Andrew Walker,4,Checking Account
Jonathan Harris,1,Savings Account
Christopher Turner,2,Money Market Account
Lauren Lewis,3,Term Deposit Account
Joshua Turner,4,Joint Account
Rebecca Clark,1,Certificate of Deposit Account
Kimberly Wilson,2,Student Account
Matthew Thompson,3,Investment Account
John Davis,4,Personal Account
Ashley Turner,1,Checking Account
Michael Johnson,2,Savings Account
Jonathan Wright,3,Money Market Account
Emily Thompson,4,Term Deposit Account
Amanda Turner,1,Joint Account
Matthew King,2,Certificate of Deposit Account
Benjamin Adams,3,Student Account
Katherine Wright,4,Investment Account
Jason Turner,1,Personal Account
Jessica Walker,2,Checking Account
Megan Martinez,3,Savings Account
Jonathan Davis,4,Money Market Account
Joshua Turner,1,Term Deposit Account
Amanda Lewis,2,Joint Account
David Thompson,3,Certificate of Deposit Account
Emily Wright,4,Student Account
Ashley Martinez,1,Investment Account
David Turner,2,Personal Account
Brittany Harris,3,Checking Account
Michael Wilson,4,Savings Account
John Wright,1,Money Market Account
Emily Martinez,2,Term Deposit Account
Matthew Turner,3,Joint Account
Anna Clark,4,Certificate of Deposit Account
Nicole Adams,1,Student Account
Charles Thompson,2,Investment Account
Katherine Walker,3,Personal Account
Nicole Davis,4,Checking Account
Christopher Harris,1,Savings Account
John Turner,2,Money Market Account
Emily Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
David Clark,1,Certificate of Deposit Account
Megan Anderson,2,Student Account
Amanda Wright,3,Investment Account
Michael Turner,4,Personal Account
Jonathan Walker,1,Checking Account
Daniel Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brian Turner,1,Joint Account
David Thompson,2,Certificate of Deposit Account
Kimberly Clark,3,Student Account
Thomas Adams,4,Investment Account
Amanda Turner,1,Personal Account
Ashley Wilson,2,Checking Account
Matthew Harris,3,Savings Account
Jonathan Turner,4,Money Market Account
Rebecca Thompson,1,Term Deposit Account
Joshua Turner,2,Joint Account
Matthew Clark,3,Certificate of Deposit Account
Ashley Davis,4,Student Account
Jonathan Wright,1,Investment Account
Michael Walker,2,Personal Account
Amanda Hernandez,3,Checking Account
Jonathan Martinez,4,Savings Account
Katherine Davis,1,Money Market Account
John Wright,2,Term Deposit Account
Brittany Turner,3,Joint Account
David Clark,4,Certificate of Deposit Account
Emily Adams,1,Student Account
Alexander Thompson,2,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,1,Personal Account
Nicole Wilson,2,Checking Account
Matthew Thompson,3,Savings Account
Jonathan Turner,4,Money Market Account
Rebecca Thompson,1,Term Deposit Account
Joshua Turner,2,Joint Account
Matthew Clark,3,Certificate of Deposit Account
John Davis,4,Student Account
Amanda Wright,1,Investment Account
Michael Walker,2,Personal Account
Amanda Hernandez,3,Checking Account
Jonathan Martinez,4,Savings Account
Katherine Davis,1,Money Market Account
John Wright,2,Term Deposit Account
Brittany Turner,3,Joint Account
David Clark,4,Certificate of Deposit Account
Emily Adams,1,Student Account
Alexander Thompson,2,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,1,Personal Account
Nicole Wilson,2,Checking Account
Matthew Thompson,3,Savings Account
Jonathan Turner,4,Money Market Account
Rebecca Thompson,1,Term Deposit Account
Joshua Turner,2,Joint Account
Matthew Clark,3,Certificate of Deposit Account
John Davis,4,Student Account
Amanda Wright,1,Investment Account
Michael Walker,2,Personal Account
Amanda Hernandez,3,Checking Account
Jonathan Martinez,4,Savings Account
Katherine Davis,1,Money Market Account
John Wright,2,Term Deposit Account
Brittany Turner,3,Joint Account
David Clark,4,Certificate of Deposit Account
Emily Adams,1,Student Account
Alexander Thompson,2,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account
Katherine Davis,3,Money Market Account
John Wright,4,Term Deposit Account
Brittany Turner,1,Joint Account
David Clark,2,Certificate of Deposit Account
Emily Adams,3,Student Account
Alexander Thompson,4,Investment Account
Jonathan Turner,3,Personal Account
Nicole Wilson,4,Checking Account
Matthew Thompson,1,Savings Account
Jonathan Turner,2,Money Market Account
Rebecca Thompson,3,Term Deposit Account
Joshua Turner,4,Joint Account
Matthew Clark,1,Certificate of Deposit Account
John Davis,2,Student Account
Amanda Wright,3,Investment Account
Michael Walker,4,Personal Account
Amanda Hernandez,1,Checking Account
Jonathan Martinez,2,Savings Account";
}
