public static class Constants
{
    public const double DEFAULT_CREDIT_PERCENT = 100;
}

class Citizen
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public DateTime BirthDate { get; }

    public Citizen(string name, string surname, DateTime birthDate)
    {
        Name = name;
        Surname = surname;
        BirthDate = birthDate;
    }

    public override string ToString()
    {
        return $"Name: {Name}, Surname: {Surname}, Birth date: {BirthDate}, Age: {Age(DateTime.Now)}";
    }

    public int Age(DateTime date)
    {
        int age = date.Year - BirthDate.Year;
        if (date.Month < BirthDate.Month || (date.Month == BirthDate.Month && date.Day < BirthDate.Day))
        {
            age--;
        }
        return age;
    }
}

class BankAccount
{
    public int AccountNumber { get; }
    public double Money { get; set; }

    public BankAccount(int accountNumber, double money)
    {
        AccountNumber = accountNumber;
        Money = money;
    }

    public void AddMoney(double money)
    {
        Money += money;

        Console.WriteLine($"\n---\nAdded {money} to account {AccountNumber}. Total: {Money}\n---\n");
    }

    public void WithdrawMoney(double money)
    {
        if (money <= Money)
        {
            Money -= money;
            Console.WriteLine($"\n---\nWithdrawn {money} from account {AccountNumber}. Total: {Money}\n---\n");
        }
        else
        {
            Console.WriteLine("\n---\nNot enough money\n---\n");
        }
    }
}

class Client
{
    public Citizen Citizen { get; }
    public BankAccount BankAccount { get; }

    public Client(Citizen citizen, BankAccount bankAccount)
    {
        Citizen = citizen;
        BankAccount = bankAccount;
    }

    public override string ToString()
    {
        return $@"{Citizen}

Bank account: {BankAccount.AccountNumber}, Money: {BankAccount.Money}";
    }
}

class VipClient : Client
{
    public BankAccount CreditAccount { get; }

    private DateTime LastCreditDate { get; set; }


    public VipClient(Citizen citizen, BankAccount bankAccount, BankAccount creditAccount) : base(citizen, bankAccount)
    {
        CreditAccount = creditAccount;
    }

    public override string ToString()
    {
        return $@"{Citizen}

Bank account: {BankAccount.AccountNumber}
Money: {BankAccount.Money}

Credit account: {CreditAccount.AccountNumber}
Money: {CreditAccount.Money}
Last credit date if exists:  {LastCreditDate}

Total money: {BankAccount.Money + CreditAccount.Money}
Max credit amount: {MaxCreditAmount()}";
    }

    public void GetCredit(double money)
    {
        if (money <= MaxCreditAmount())
        {
            CreditAccount.AddMoney(money);
            LastCreditDate = DateTime.Now;
            Console.WriteLine("\n---\nCredit given\n---\n");
        }
        else
        {
            Console.WriteLine("Not enough money");
        }
    }

    public void ReturnCredit(double money)
    {
        var amountToReturn = money;

        if (amountToReturn <= CreditAccount.Money)
        {
            CreditAccount.WithdrawMoney(amountToReturn);
            Console.WriteLine("\n---\nCredit returned\n---\n");
            return;
        }
        else
        {
            amountToReturn -= CreditAccount.Money;
            CreditAccount.WithdrawMoney(CreditAccount.Money);

        }

        if (amountToReturn > 0)
        {
            BankAccount.WithdrawMoney(amountToReturn);
        }

        Console.WriteLine("\n---\nCredit returned\n---\n");

    }

    public double MaxCreditAmount()
    {
        if (Citizen.Age(DateTime.Now) >= 30 && Citizen.Age(DateTime.Now) <= 50)
        {
            return BankAccount.Money * Constants.DEFAULT_CREDIT_PERCENT;
        }
        else
        {
            return BankAccount.Money * Constants.DEFAULT_CREDIT_PERCENT / 2;
        }
    }
}

class Program
{
    public static void Main()
    {
        Client[] clients = {
            new Client(new Citizen("Rostyk", "M", new DateTime(1990, 1, 1)), new BankAccount(1, 1000)),
            new Client(new Citizen("Petro", "D", new DateTime(1980, 1, 1)), new BankAccount(2, 2000)),
            new VipClient(new Citizen("Vasyl", "U", new DateTime(1970, 1, 1)), new BankAccount(3, 3000), new BankAccount(4, 4000)),
            new VipClient(new Citizen("Danylo", "O", new DateTime(1960, 1, 1)), new BankAccount(5, 5000), new BankAccount(6, 6000)),
        };

        PrintClients(clients);

        clients[0].BankAccount.AddMoney(100);
        clients[1].BankAccount.WithdrawMoney(100);

        var vipClients = clients.OfType<VipClient>();

        foreach (var client in vipClients)
        {
            client.GetCredit(1000);
            client.ReturnCredit(100);
        }

        PrintClients(clients);
        GetCreditClients(clients);
    }

    public static void PrintClients(Client[] clients)
    {
        var sortedClients = clients.OrderBy(c => c.Citizen.Surname);

        Console.WriteLine("All clients:");
        foreach (var client in sortedClients)
        {
            Console.WriteLine(client);
            Console.WriteLine("\n---\n");
        }
    }

    public static void GetCreditClients(Client[] clients)
    {
        var creditClients = clients.OfType<VipClient>().Where(c => c.CreditAccount.Money > 0);
        var sortedClients = creditClients.OrderBy(c => c.CreditAccount.Money);

        Console.WriteLine("Clients with credits:");
        foreach (var client in sortedClients)
        {
            Console.WriteLine(client);
            Console.WriteLine("\n---\n");
        }
    }
}