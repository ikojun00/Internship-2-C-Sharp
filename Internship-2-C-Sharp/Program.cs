SortedDictionary<int, Tuple<string, string, int, DateTime>> users = new SortedDictionary<int, Tuple<string, string, int, DateTime>>();
users.Add(1, new Tuple<string, string, int, DateTime>("Ante", "Antic", 800, new DateTime(2001, 5, 21)));
users.Add(2, new Tuple<string, string, int, DateTime>("Mate", "Matic", -200, new DateTime(2003, 2, 1)));
users.Add(3, new Tuple<string, string, int, DateTime>("Jure", "Juric", 190, new DateTime(1998, 12, 21)));
users.Add(4, new Tuple<string, string, int, DateTime>("Ivo", "Ivic", 50, new DateTime(2000, 7, 6)));


SortedDictionary<int, Tuple<int, string, int, string, DateTime>> transactions = new SortedDictionary<int, Tuple<int, string, int, string, DateTime>>();
transactions.Add(1, new Tuple<int, string, int, string, DateTime>(1, "prihod", 800, "plaća", DateTime.Now));
transactions.Add(2, new Tuple<int, string, int, string, DateTime>(1, "rashod", 100, "sport", DateTime.Now));
transactions.Add(3, new Tuple<int, string, int, string, DateTime>(2, "rashod", 100, "sport", DateTime.Now));
transactions.Add(4, new Tuple<int, string, int, string, DateTime>(2, "rashod", 200, "hrana", DateTime.Now));
transactions.Add(5, new Tuple<int, string, int, string, DateTime>(3, "prihod", 100, "poklon", DateTime.Now));
transactions.Add(6, new Tuple<int, string, int, string, DateTime>(3, "rashod", 10, "prijevoz", DateTime.Now));
transactions.Add(7, new Tuple<int, string, int, string, DateTime>(4, "prihod", 50, "honorar", DateTime.Now));
transactions.Add(8, new Tuple<int, string, int, string, DateTime>(4, "rashod", 100, "hrana", DateTime.Now));

//components
int Display(string[] options)
{
    int option;
    do
    {
        for (int i = 0; i < options.Length; i++)
        {
            Console.WriteLine($"{i} - {options[i]}");
        }

        Console.Write("\nTvoj izbor: ");

        if (!int.TryParse(Console.ReadLine(), out option) || option < 0 || option >= options.Length)
        {
            Console.WriteLine("\nUnos nije validan. Molimo pokušajte ponovo.\n");
            continue;
        }
        break;
    } while (true);

    Console.Clear();

    return option;
}


//functions

void AddTransaction() { }
void DeleteTransaction() { }
void UpdateTransaction() { }
void TransactionList(int optionForId) 
{
    Console.WriteLine("Tip - Iznos - Kategorija - Datum");

    var userTransactions = transactions.Where(x => x.Value.Item1 == optionForId).ToList();
    foreach (var transaction in userTransactions)
    {
        string type = transaction.Value.Item2;
        int price = transaction.Value.Item3;
        string category = transaction.Value.Item4;
        DateTime transactionDate = transaction.Value.Item5;

        Console.WriteLine($"{type} - {price} - {category} - {transactionDate.ToString("dd.MM.yyyy")}");
    }
}
void FinancialReport() { }

void Accounts(int optionForId)
{
    string[] options = { "Povratak u glavni izbornik\n", "Unos nove transakcije", "Brisanje transakcije", "Uređivanje transakcije", "Pregled transakcija", "Financijsko izvješće" };
    int option = Display(options);

    switch (option)
    {
        case 0:
            MainMenu();
            break;
        case 1:
            AddTransaction();
            break;
        case 2:
            DeleteTransaction();
            break;
        case 3:
            UpdateTransaction();
            break;
        case 4:
            TransactionList(optionForId);
            MainMenu();
            break;
        case 5:
            FinancialReport();
            MainMenu();
            break;
    }
}

void UserList()
{
    Console.WriteLine("Id - Ime - Prezime - Datum rođenja");

    var sortedUsers = users.OrderBy(user => user.Value.Item2).ToList();

    foreach (var user in sortedUsers)
    {
        int id = user.Key;
        string name = user.Value.Item1;
        string surname = user.Value.Item2;
        DateTime birthDate = user.Value.Item4;

        Console.WriteLine($"{id} - {name} - {surname} - {birthDate.ToString("dd.MM.yyyy")}");
    }
}

void UpdateUser()
{
    UserList();
    int option;
    do
    {
        Console.Write("\nUpiši id korisnika kojeg želiš izmijeniti: ");
        int.TryParse(Console.ReadLine(), out option);

        if (!users.ContainsKey(option))
        {
            Console.WriteLine("\nKorisnik s tim id-om ne postoji. Molimo pokušajte ponovo.\n");
            continue;
        }
        Console.Write("Ime: ");
        var name = Console.ReadLine();
        Console.Write("Prezime: ");
        var surname = Console.ReadLine();
        while (true)
        {
            Console.WriteLine("Datum rođenja");
            Console.Write("Dan: ");
            if (!int.TryParse(Console.ReadLine(), out int day) || day < 1 || day > 31) continue;

            Console.Write("Mjesec: ");
            if (!int.TryParse(Console.ReadLine(), out int month) || month < 1 || month > 12) continue;

            Console.Write("Godina: ");
            if (!int.TryParse(Console.ReadLine(), out int year) || year < 1900 || year > DateTime.Now.Year) continue;

            var account = users[option].Item3;

            users[option] = new Tuple<string, string, int, DateTime>(name, surname, account, new DateTime(year, month, day));
            break;
        }
        Console.WriteLine($"\nKorisnik s ID-om {option} izmijenjen.\n");
        break;
    } while (true);
    MainMenu();
}

void DeleteUserById()
{
    UserList();
    int option;
    do
    {
        Console.Write("\nUpiši id korisnika kojeg želiš izbrisati: ");
        int.TryParse(Console.ReadLine(), out option);

        if (!users.ContainsKey(option))
        {
            Console.WriteLine("\nKorisnik s tim id-om ne postoji. Molimo pokušajte ponovo.\n");
            continue;
        }
        users.Remove(option);
        Console.WriteLine($"\nKorisnik s ID-om {option} izbrisan.\n");
        break;
    } while (true);
    MainMenu();
}

void DeleteUserByFirstnameAndLastname()
{
    UserList();
    // TO DO
    MainMenu();
}

void DeleteUser()
{
    string[] options = { "Povratak u glavni izbornik\n", "Brisanje po id-u", "Brisanje po imenu i prezimenu" };
    int option = Display(options);

    switch (option)
    {
        case 0:
            Users();
            break;
        case 1:
            DeleteUserById();
            break;
        case 2:
            DeleteUserByFirstnameAndLastname();
            break;
    }
}
void AddUser()
{
    Console.Write("Ime: ");
    var name = Console.ReadLine();
    Console.Write("Prezime: ");
    var surname = Console.ReadLine();
    while (true)
    {
        Console.WriteLine("Datum rođenja");
        Console.Write("Dan: ");
        if (!int.TryParse(Console.ReadLine(), out int day) || day < 1 || day > 31) continue;

        Console.Write("Mjesec: ");
        if (!int.TryParse(Console.ReadLine(), out int month) || month < 1 || month > 12) continue;

        Console.Write("Godina: ");
        if (!int.TryParse(Console.ReadLine(), out int year) || year < 1900 || year > DateTime.Now.Year) continue;

        int newId = users.Keys.Count != 0 ? users.Keys.Max() + 1 : 1;

        users.Add(newId, new Tuple<string, string, int, DateTime>(name, surname, 100, new DateTime(year, month, day)));

        Console.WriteLine($"\nKorisnik {name} {surname} dodan s id-om {newId}.\n");
        break;
    }
    MainMenu();
}
void Users()
{
    string[] options = { "Povratak u glavni izbornik\n", "Unos novog korisnika", "Brisanje korisnika", "Uređivanje korisnika", "Pregled izbornika" };
    int option = Display(options);

    switch (option)
    {
        case 0:
            MainMenu();
            break;
        case 1:
            AddUser();
            break;
        case 2:
            DeleteUser();
            break;
        case 3:
            UpdateUser();
            break;
        case 4:
            UserList();
            MainMenu();
            break;
    }
}
void MainMenu()
{
    string[] options = { "Izlaz iz aplikacije\n", "Korisnici", "Računi" };
    int option = Display(options);

    switch (option)
    {
        case 0:
            Console.WriteLine("Odjava.");
            break;
        case 1:
            Users();
            break;
        case 2:
            UserList();
            int optionForId;
            do
            {
                Console.Write("\nUpiši id korisnika čijem računu pristupaš: ");
                int.TryParse(Console.ReadLine(), out optionForId);

                if (!users.ContainsKey(optionForId))
                {
                    Console.WriteLine("\nKorisnik s tim id-om ne postoji. Molimo pokušajte ponovo.\n");
                    continue;
                }
                break;

            } while (true);
            Console.Clear();
            Accounts(optionForId);
            break;
    }
}

MainMenu();
