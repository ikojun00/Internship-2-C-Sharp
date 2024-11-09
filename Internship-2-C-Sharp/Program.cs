SortedDictionary<int, Tuple <string, string, DateTime>> users = new SortedDictionary<int, Tuple<string, string, DateTime>>();
users.Add(1, new Tuple<string, string, DateTime>("Ante", "Antic", new DateTime(2001, 5, 21)));
users.Add(2, new Tuple<string, string, DateTime>("Mate", "Matic", new DateTime(2003, 2, 1)));
users.Add(3, new Tuple<string, string, DateTime>("Jure", "Juric", new DateTime(1998, 12, 21)));
users.Add(4, new Tuple<string, string, DateTime>("Ivo", "Ivic", new DateTime(2000, 7, 6)));

void UserList()
{
    Console.WriteLine("Id - Ime - Prezime - Datum rođenja");

    var sortedUsers = users.OrderBy(user => user.Value.Item2).ToList();

    foreach (var user in sortedUsers)
    {
        int id = user.Key;
        string name = user.Value.Item1;
        string surname = user.Value.Item2;
        DateTime birthDate = user.Value.Item3;

        Console.WriteLine($"{id} - {name} - {surname} - {birthDate.ToString("dd.MM.yyyy")}");
    }
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
            Console.WriteLine("Brisanje po id-u");
            break;
        case 2:
            Console.WriteLine("Brisanje po imenu i prezimenu");
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

        users.Add(newId, new Tuple<string, string, DateTime>(name, surname, new DateTime(year, month, day)));

        Console.WriteLine($"\nKorisnik {name} {surname} dodan s ID-om {newId}.\n");
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
            Console.WriteLine("Uređivanje korisnika");
            break;
        case 4:
            UserList();
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
            Console.WriteLine("Računi");
            break;
        case 3:
            Console.WriteLine("Odjava.");
            break;
    }
}

MainMenu();
