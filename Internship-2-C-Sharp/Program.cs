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
//accounts

void Accounts(int optionForId)
{
    //TODO
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
        Console.Write("Upiši id korisnika kojeg želiš izmijeniti: ");
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
        Console.Write("Upiši id korisnika kojeg želiš izbrisati: ");
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
    int option;
    do
    {
        Console.WriteLine("1 - Brisanje po id-u");
        Console.WriteLine("2 - Brisanje po imenu i prezimenu");
        Console.WriteLine("\n0 - Povratak u glavni izbornik\n");
        Console.Write("Tvoj izbor: ");

        if (!int.TryParse(Console.ReadLine(), out option) || (option < 0 || option > 2))
        {
            Console.WriteLine("\nUnos nije validan. Molimo pokušajte ponovo.\n");
            continue;
        }
        break;

    } while (true);

    Console.Clear();

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
    int option;
    do
    {
        Console.WriteLine("1 - Unos novog korisnika");
        Console.WriteLine("2 - Brisanje korisnika");
        Console.WriteLine("3 - Uređivanje korisnika");
        Console.WriteLine("4 - Pregled izbornika");
        Console.WriteLine("\n0 - Povratak u glavni izbornik\n");
        Console.Write("Tvoj izbor: ");

        if (!int.TryParse(Console.ReadLine(), out option) || (option < 0 || option > 4))
        {
            Console.WriteLine("\nUnos nije validan. Molimo pokušajte ponovo.\n");
            continue;
        }
        break;

    } while (true);

    Console.Clear();

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
    int option;
    do
    {
        Console.WriteLine("1 - Korisnici");
        Console.WriteLine("2 - Računi");
        Console.WriteLine("3 - Izlaz iz aplikacije");
        Console.Write("Tvoj izbor: ");

        if (!int.TryParse(Console.ReadLine(), out option) || (option < 1 || option > 3))
        {
            Console.WriteLine("\nUnos nije validan. Molimo pokušajte ponovo.\n");
            continue;
        }
        break;

    } while (true);

    Console.Clear();

    switch (option)
    {
        case 1:
            Users();
            break;
        case 2:
            UserList();
            int optionForId;
            do
            {
                Console.Write("Upiši id korisnika čijem računu pristupaš: ");
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
        case 3:
            Console.WriteLine("Odjava.");
            break;
    }
}

MainMenu();
