using System.Data.Common;
using System.Numerics;
using System.Transactions;
using System.Xml.Linq;

SortedDictionary<int, Tuple<string, string, int, DateTime>> users = new SortedDictionary<int, Tuple<string, string, int, DateTime>>();
users.Add(1, new Tuple<string, string, int, DateTime>("Ante", "Antic", 800, new DateTime(2001, 5, 21)));
users.Add(2, new Tuple<string, string, int, DateTime>("Mate", "Matic", -200, new DateTime(1980, 2, 1)));
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
void UserListDisplayBody(IEnumerable<KeyValuePair<int, Tuple<string, string, int, DateTime>>> userList, string message)
{
    if (userList.Any())
    {
        Console.WriteLine("Id - Ime - Prezime - Datum rođenja");
        foreach (var user in userList)
        {
            int id = user.Key;
            string name = user.Value.Item1;
            string surname = user.Value.Item2;
            DateTime birthDate = user.Value.Item4;

            Console.WriteLine($"{id} - {name} - {surname} - {birthDate.ToString("dd.MM.yyyy")}");
        }
    }
    else Console.WriteLine(message);
}
void TransactionListDisplayBody(IEnumerable<KeyValuePair<int, Tuple<int, string, int, string, DateTime>>> transactionList, string message)
{
    if (transactionList.Any())
    {
        Console.WriteLine("Id - Tip - Iznos - Kategorija - Datum");
        foreach (var transaction in transactionList)
        {
            string type = transaction.Value.Item2;
            int price = transaction.Value.Item3;
            string category = transaction.Value.Item4;
            DateTime transactionDate = transaction.Value.Item5;

            Console.WriteLine($"{transaction.Key} - {type} - {price} - {category} - {transactionDate.ToString("dd.MM.yyyy")}");
        }
    }
    else Console.WriteLine(message);
}
(string, int, string) TransactionBody()
{
    var typeNum = "";
    int amount;
    var category = "";

    while (true)
    {
        Console.Write("Pritisni 1 za prihod ili 2 za rashod: ");
        typeNum = Console.ReadLine();
        if (typeNum == "1" || typeNum == "2") break;
        Console.WriteLine("Niste pritisnuli 1 za prihod ili 2 za rashod. Pokušajte ponovo.");
    }

    while (true)
    {
        Console.Write("Iznos: ");
        var amountInput = Console.ReadLine();
        if (int.TryParse(amountInput, out amount) && amount > 0) break;
        Console.WriteLine("Iznos mora biti pozitivan broj. Pokušajte ponovo.");
    }

    while (true)
    {
        Console.Write("Kategorija: ");
        category = Console.ReadLine();
        if (typeNum == "1" && (category == "plaća" || category == "honorar" || category == "poklon") ||
            typeNum == "2" && (category == "hrana" || category == "prijevoz" || category == "sport")) break;
        Console.WriteLine("Kategorija ne postoji. Pokušajte ponovo.");
    }

    return (typeNum, amount, category);
}

(string, string, DateTime) UserBody()
{
    var name = "";
    var surname = "";
    int day;
    int month;
    int year;
    DateTime birthDate;

    while (true)
    {
        Console.Write("Ime: ");
        name = Console.ReadLine();
        if (name != "") break;
        Console.WriteLine("Ime ne može biti prazno. Pokušajte ponovo.");
    }

    while (true)
    {
        Console.Write("Prezime: ");
        surname = Console.ReadLine();
        if (surname != "") break;
        Console.WriteLine("Prezime ne može biti prazno. Pokušajte ponovo.");
    }

    while (true)
    {
        Console.Write("Dan: ");
        if (!int.TryParse(Console.ReadLine(), out day) || day < 1 || day > 31)
        {
            Console.WriteLine("Dan mora biti broj između 1 i 31. Pokušajte ponovo.");
            continue;
        }

        Console.Write("Mjesec: ");
        if (!int.TryParse(Console.ReadLine(), out month) || month < 1 || month > 12)
        {
            Console.WriteLine("Mjesec mora biti broj između 1 i 12. Pokušajte ponovo.");
            continue;
        }

        Console.Write("Godina: ");
        if (!int.TryParse(Console.ReadLine(), out year) || year < 1900 || year > DateTime.Now.Year)
        {
            Console.WriteLine($"Godina mora biti između 1900 i {DateTime.Now.Year}. Pokušajte ponovo.");
            continue;
        }

        try
        {
            birthDate = new DateTime(year, month, day);
            break;
        }
        catch
        {
            Console.WriteLine("Nevažeći datum. Pokušajte ponovo.");
        }
    }

    return (name, surname, birthDate);
}
void AccountValue(int optionForId)
{
    var userTransactions = transactions.Where(t => t.Value.Item1 == optionForId);
    int balance = 100;
    foreach (var transaction in userTransactions)
    {
        string type = transaction.Value.Item2;
        int amount = transaction.Value.Item3;

        if (type == "prihod")
            balance += amount;
        else if (type == "rashod")
            balance -= amount;
    }

    users[optionForId] = new Tuple<string, string, int, DateTime>(
        users[optionForId].Item1,
        users[optionForId].Item2,
        balance,
        users[optionForId].Item4
    );
}
void RemoveTransactions(List<int> keysToRemove, int optionForId)
{
    foreach (var key in keysToRemove) transactions.Remove(key);
    Console.WriteLine($"Izbrisano {keysToRemove.Count} transakcija.");
    Accounts(optionForId);
}
//functions
void AddTransaction(int optionForId)
{
    var (typeNum, amount, category) = TransactionBody();
    int newId = transactions.Keys.Count != 0 ? transactions.Keys.Max() + 1 : 1;
    transactions.Add(newId, new Tuple<int, string, int, string, DateTime>(optionForId, typeNum == "1" ? "prihod" : "rashod", amount, category, DateTime.Now));
    AccountValue(optionForId);
    Accounts(optionForId);
}
void DeleteTransactionById(int optionForId)
{
    var userTransactions = transactions.Where(x => x.Value.Item1 == optionForId).ToList();
    TransactionListDisplayBody(userTransactions, "Nema transakcija.");
    var userKeys = transactions.Where(transaction => transaction.Value.Item1 == optionForId)
        .Select(transaction => transaction.Key)
        .ToList();
    int option;

    while (true)
    {
        Console.Write("\nUpiši id transakcije koje želiš izmijeniti: ");
        int.TryParse(Console.ReadLine(), out option);

        if (userKeys.Contains(option)) break;
        else Console.WriteLine("\nTransakcija s tim id-om ne postoji. Molimo pokušajte ponovo.\n");
    }
    transactions.Remove(option);
    Console.WriteLine($"Izbrisana transakcije s id-om {option}.");
    Accounts(optionForId);
}
void DeleteTransactionsUnderCertainAmount(int optionForId)
{
    int amount;
    while (true)
    {
        Console.Write("Iznos: ");
        if (int.TryParse(Console.ReadLine(), out amount)) break;
        Console.WriteLine("Iznos mora biti broj. Pokušajte ponovo.");
    }
    var keysToRemove = transactions
        .Where(transaction => transaction.Value.Item3 < amount && transaction.Value.Item1 == optionForId)
        .Select(transaction => transaction.Key)
        .ToList();

    RemoveTransactions(keysToRemove, optionForId);
}
void DeleteTransactionsAboveCertainAmount(int optionForId)
{
    int amount;
    while (true)
    {
        Console.Write("Iznos: ");
        if (int.TryParse(Console.ReadLine(), out amount)) break;
        Console.WriteLine("Iznos mora biti broj. Pokušajte ponovo.");
    }
    var keysToRemove = transactions
        .Where(transaction => transaction.Value.Item3 > amount && transaction.Value.Item1 == optionForId)
        .Select(transaction => transaction.Key)
        .ToList();

    RemoveTransactions(keysToRemove, optionForId);
}
void DeleteIncomeTransactions(int optionForId)
{
    var keysToRemove = transactions
        .Where(transaction => transaction.Value.Item2 == "prihod" && transaction.Value.Item1 == optionForId)
        .Select(transaction => transaction.Key)
        .ToList();

    RemoveTransactions(keysToRemove, optionForId);
}
void DeleteExpenseTransactions(int optionForId)
{
    var keysToRemove = transactions
        .Where(transaction => transaction.Value.Item2 == "rashod" && transaction.Value.Item1 == optionForId)
        .Select(transaction => transaction.Key)
        .ToList();

    RemoveTransactions(keysToRemove, optionForId);
}
void DeleteTransactionsOfSelectedCategory(int optionForId)
{
    var category = "";
    while (true)
    {
        Console.Write("Kategorija: ");
        category = Console.ReadLine();
        if (category == "plaća" || category == "honorar" || category == "poklon" || category == "hrana" || category == "prijevoz" || category == "sport") break;
        Console.WriteLine("Kategorija ne postoji. Pokušajte ponovo.");
    }
    var keysToRemove = transactions
        .Where(transaction => transaction.Value.Item4 == category && transaction.Value.Item1 == optionForId)
        .Select(transaction => transaction.Key)
        .ToList();

    RemoveTransactions(keysToRemove, optionForId);
}

void DeleteTransaction(int optionForId)
{
    string[] options = { "Povratak\n", "Brisanje transakcije po id-u", "Brisanje ispod unesenog iznosa", "Brisanje iznad unesenog iznosa", "Brisanje svih prihoda", "Brisanje svih rashoda", "Brisanje svih transakcija za odabranu kategoriju" };
    int option = Display(options);

    switch (option)
    {
        case 0:
            Accounts(optionForId);
            break;
        case 1:
            DeleteTransactionById(optionForId);
            break;
        case 2:
            DeleteTransactionsUnderCertainAmount(optionForId);
            break;
        case 3:
            DeleteTransactionsAboveCertainAmount(optionForId);
            break;
        case 4:
            DeleteIncomeTransactions(optionForId);
            break;
        case 5:
            DeleteExpenseTransactions(optionForId);
            break;
        case 6:
            DeleteTransactionsOfSelectedCategory(optionForId);
            break;
    }
}
void UpdateTransaction(int optionForId)
{
    var userTransactions = transactions.Where(x => x.Value.Item1 == optionForId).ToList();
    TransactionListDisplayBody(userTransactions, "Nema transakcija.");
    var userKeys = transactions.Where(transaction => transaction.Value.Item1 == optionForId)
        .Select(transaction => transaction.Key)
        .ToList();
    int option;

    while (true)
    {
        Console.Write("\nUpiši id transakcije koje želiš izmijeniti: ");
        int.TryParse(Console.ReadLine(), out option);

        if (userKeys.Contains(option)) break;
        else Console.WriteLine("\nTransakcija s tim id-om ne postoji. Molimo pokušajte ponovo.\n");
    }

    var (typeNum, amount, category) = TransactionBody();
    transactions[option] = new Tuple<int, string, int, string, DateTime>(optionForId, typeNum == "1" ? "prihod" : "rashod", amount, category, DateTime.Now);
    Console.WriteLine($"\nTransakcija s ID-om {option} izmijenjena.\n");

    AccountValue(optionForId);
    Accounts(optionForId);
}
void TransactionListAscendingAmount(int optionForId)
{
    var sortedTransactions = transactions.Where(x => x.Value.Item1 == optionForId).OrderBy(transaction => transaction.Value.Item3).ToList();
    TransactionListDisplayBody(sortedTransactions, "Nema transakcija.");
    TransactionList(optionForId);
}
void TransactionListDescendingAmount(int optionForId)
{
    var sortedTransactions = transactions.Where(x => x.Value.Item1 == optionForId).OrderByDescending(transaction => transaction.Value.Item3).ToList();
    TransactionListDisplayBody(sortedTransactions, "Nema transakcija.");
    TransactionList(optionForId);
}
void TransactionListAscendingDate(int optionForId)
{
    var sortedTransactions = transactions.Where(x => x.Value.Item1 == optionForId).OrderBy(transaction => transaction.Value.Item5).ToList();
    TransactionListDisplayBody(sortedTransactions, "Nema transakcija.");
    TransactionList(optionForId);
}
void TransactionListDescendingDate(int optionForId)
{
    var sortedTransactions = transactions.Where(x => x.Value.Item1 == optionForId).OrderByDescending(transaction => transaction.Value.Item5).ToList();
    TransactionListDisplayBody(sortedTransactions, "Nema transakcija.");
    TransactionList(optionForId);
}
void TransactionListIncome(int optionForId)
{
    var foundTransactions = transactions.Where(x => x.Value.Item1 == optionForId && x.Value.Item2 == "prihod").ToList();
    TransactionListDisplayBody(foundTransactions, "Nema transakcija.");
    TransactionList(optionForId);
}
void TransactionListExpense(int optionForId)
{
    var foundTransactions = transactions.Where(x => x.Value.Item1 == optionForId && x.Value.Item2 == "rashod").ToList();
    TransactionListDisplayBody(foundTransactions, "Nema transakcija.");
    TransactionList(optionForId);
}
void TransactionListByCategory(int optionForId)
{
    var category = "";
    while (true)
    {
        Console.Write("Kategorija: ");
        category = Console.ReadLine();
        if (category == "plaća" || category == "honorar" || category == "poklon" ||
            category == "hrana" || category == "prijevoz" || category == "sport") break;
        Console.WriteLine("Kategorija ne postoji. Pokušajte ponovo.");
    }
    var foundTransactions = transactions.Where(x => x.Value.Item1 == optionForId && x.Value.Item4 == category).ToList();
    TransactionListDisplayBody(foundTransactions, "Nema transakcija.");
    TransactionList(optionForId);
}

void TransactionListRegular(int optionForId)
{
    var userTransactions = transactions.Where(x => x.Value.Item1 == optionForId).ToList();
    TransactionListDisplayBody(userTransactions, "Nema transakcija.");
    TransactionList(optionForId);
}
void TransactionList(int optionForId)
{
    string[] options = { "Povratak u glavni izbornik\n", "Sve transakcije", 
        "Sve transakcije sortirane po iznosu uzlazno", "Sve transakcije sortirane po iznosu silazno", 
        "Sve transakcije sortirane po datumu uzlazno", "Sve transakcije sortirane po datumu silazno", 
        "Svi prihodi", "Svi rashodi", "Sve transakcije za odabranu kategoriju" };
    int option = Display(options);

    switch (option)
    {
        case 0:
            Accounts(optionForId);
            break;
        case 1:
            TransactionListRegular(optionForId);
            break;
        case 2:
            TransactionListAscendingAmount(optionForId);
            break;
        case 3:
            TransactionListDescendingAmount(optionForId);
            break;
        case 4:
            TransactionListAscendingDate(optionForId);
            break;
        case 5:
            TransactionListDescendingDate(optionForId);
            break;
        case 6:
            TransactionListIncome(optionForId);
            break;
        case 7:
            TransactionListExpense(optionForId);
            break;
        case 8:
            TransactionListByCategory(optionForId);
            break;
    }
}
void CurrentAccountBalance(int optionForId) {
    var userTransactions = transactions.Where(t => t.Value.Item1 == optionForId);
    int balance = 100;
    foreach (var transaction in userTransactions)
    {
        string type = transaction.Value.Item2;
        int amount = transaction.Value.Item3;

        if (type == "prihod")
            balance += amount;
        else if (type == "rashod")
            balance -= amount;
    }
    Console.WriteLine($"Trenutno stanje računa: {balance} eura");
}
void TransactionsCount(int optionForId) 
{
    var userTransactions = transactions.Where(t => t.Value.Item1 == optionForId).ToList();
    Console.WriteLine($"Ukupan broj transakcija: {userTransactions.Count}");
}
void IncomeAndExpensesForChosenPeriod(int optionForId)
{
    int month;
    int year;
    var expense = 0;
    var income = 0;
    while (true)
    {
        Console.Write("Mjesec: ");
        if (!int.TryParse(Console.ReadLine(), out month) || month < 1 || month > 12)
        {
            Console.WriteLine("Mjesec mora biti broj između 1 i 12. Pokušajte ponovo.");
            continue;
        }

        Console.Write("Godina: ");
        if (!int.TryParse(Console.ReadLine(), out year) || year < 1900 || year > DateTime.Now.Year)
        {
            Console.WriteLine($"Godina mora biti između 1900 i {DateTime.Now.Year}. Pokušajte ponovo.");
            continue;
        }
        break;
    }
    
    var userTransactions = transactions.Where(t => t.Value.Item1 == optionForId && t.Value.Item5.Month == month && t.Value.Item5.Year == year);
    if (userTransactions.Any())
    {
        foreach (var transaction in userTransactions)
        {
            string type = transaction.Value.Item2;
            int amount = transaction.Value.Item3;

            if (type == "prihod")
                income += amount;
            else if (type == "rashod")
                expense -= amount;
        }
        Console.WriteLine($"Prihod: {income} eura");
        Console.WriteLine($"Rashod: {expense} eura");
    }
    else Console.WriteLine($"Nema transakcija u {month}. mjesecu i {year}. godini.");
}
void PercentageExpenseForChosenCategory(int optionForId) {
    var category = "";
    var chosenIncome = 0;
    var income = 0;
    while (true)
    {
        Console.Write("Kategorija: ");
        category = Console.ReadLine();
        if (category == "hrana" || category == "prijevoz" || category == "sport") break;
        Console.WriteLine("Kategorija ne postoji. Pokušajte ponovo.");
    }
    var userTransactions = transactions.Where(t => t.Value.Item1 == optionForId && t.Value.Item2 == "rashod");
    if (userTransactions.Any())
    {
        foreach (var transaction in userTransactions)
        {
            int amount = transaction.Value.Item3;
            if (transaction.Value.Item4 == category)
                chosenIncome += amount;
            income += amount;
        }
        Console.WriteLine($"Postotak ukupnih troškova koji odlazi na kategoriju {category}: {(int)Math.Round((double)(100 * chosenIncome) / income)}%");
    }
    else Console.WriteLine($"Nema transakcija s kategorijom {category}.");
}
void AverageBalanceTransactionForChosenPeriod(int optionForId) {
    int month;
    int year;
    var balance = 0;
    while (true)
    {
        Console.Write("Mjesec: ");
        if (!int.TryParse(Console.ReadLine(), out month) || month < 1 || month > 12)
        {
            Console.WriteLine("Mjesec mora biti broj između 1 i 12. Pokušajte ponovo.");
            continue;
        }

        Console.Write("Godina: ");
        if (!int.TryParse(Console.ReadLine(), out year) || year < 1900 || year > DateTime.Now.Year)
        {
            Console.WriteLine($"Godina mora biti između 1900 i {DateTime.Now.Year}. Pokušajte ponovo.");
            continue;
        }
        break;
    }

    var userTransactions = transactions.Where(t => t.Value.Item1 == optionForId && t.Value.Item5.Month == month && t.Value.Item5.Year == year);
    if (userTransactions.Any())
    {
        foreach (var transaction in userTransactions)
        {
            string type = transaction.Value.Item2;
            int amount = transaction.Value.Item3;

            if (type == "prihod")
                balance += amount;
            else if (type == "rashod")
                balance -= amount;
        }
        Console.WriteLine($"Prosječni iznos transakcije u {month}. mjesecu i {year}. godini: " +
            $"{(int)Math.Round((double)balance / userTransactions.Count())} eura");
    }
    else Console.WriteLine($"Nema transakcija u {month}. mjesecu i {year}. godini.");
}
void AverageBalanceTransactionForChosenCategory(int optionForId) {
    var category = "";
    var balance = 0;
    while (true)
    {
        Console.Write("Kategorija: ");
        category = Console.ReadLine();
        if (category == "plaća" || category == "honorar" || category == "poklon" || category == "hrana" || category == "prijevoz" || category == "sport") break;
        Console.WriteLine("Kategorija ne postoji. Pokušajte ponovo.");
    }
    var userTransactions = transactions.Where(t => t.Value.Item1 == optionForId && t.Value.Item4 == category);
    if (userTransactions.Any())
    {
        foreach (var transaction in userTransactions)
        {
            string type = transaction.Value.Item2;
            int amount = transaction.Value.Item3;

            if (type == "prihod")
                balance += amount;
            else if (type == "rashod")
                balance -= amount;
        }
        Console.WriteLine($"Prosječni iznos transakcije u kategoriji {category}: " +
            $"{(int)Math.Round((double)balance / userTransactions.Count())} eura");
    }
    else Console.WriteLine($"Nema transakcija s kategorijom {category}.");
}

void FinancialReport(int optionForId)
{
    string[] options = { "Povratak\n", "Trenutno stanje računa", "Ukupan broj transakcija",
        "Ukupan iznos prihoda i rashoda za odabrani mjesec i godinu", "Postotak udjela rashoda za odabranu kategoriju",
        "Prosječni iznos transakcije za odabrani mjesec i godinu", "Prosječni iznos transakcije za odabranu kategoriju" };
    int option = Display(options);

    switch (option)
    {
        case 0:
            Accounts(optionForId);
            break;
        case 1:
            CurrentAccountBalance(optionForId);
            break;
        case 2:
            TransactionsCount(optionForId);
            break;
        case 3:
            IncomeAndExpensesForChosenPeriod(optionForId);
            break;
        case 4:
            PercentageExpenseForChosenCategory(optionForId);
            break;
        case 5:
            AverageBalanceTransactionForChosenPeriod(optionForId);
            break;
        case 6:
            AverageBalanceTransactionForChosenCategory(optionForId);
            break;
    }
}

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
            AddTransaction(optionForId);
            break;
        case 2:
            DeleteTransaction(optionForId);
            break;
        case 3:
            UpdateTransaction(optionForId);
            break;
        case 4:
            TransactionList(optionForId);
            break;
        case 5:
            FinancialReport(optionForId);
            Accounts(optionForId);
            break;
    }
}

void UserListBySurname()
{
    var sortedUsers = users.OrderBy(user => user.Value.Item2).ToList();
    UserListDisplayBody(sortedUsers, "Nema korisnika.");
}

void UserListByAge()
{
    var usersOver30 = users.Where(user => (DateTime.Now.Year - user.Value.Item4.Year) > 30);
    UserListDisplayBody(usersOver30, "Nema korisnika starijih od 30.");
}

void UserListWithNegativeAccount()
{
    var usersWithNegativeAccount = users.Where(user => user.Value.Item3 < 0);
    UserListDisplayBody(usersWithNegativeAccount, "Nema korisnika sa negativnim računom.");
}

void UserList()
{
    string[] options = { "Povratak u glavni izbornik\n", "Svi korisnici abecedno po prezimenu", "Svi korisnici koji imaju više od 30 godina", "Svi korisnici koji imaju račun u minusu" };
    int option = Display(options);

    switch (option)
    {
        case 0:
            Users();
            break;
        case 1:
            UserListBySurname();
            break;
        case 2:
            UserListByAge();
            break;
        case 3:
            UserListWithNegativeAccount();
            break;
    }
}

void UpdateUser()
{
    UserListBySurname();
    int option;
    while (true)
    {
        Console.Write("\nUpiši id korisnika kojeg želiš izmijeniti: ");
        int.TryParse(Console.ReadLine(), out option);

        if (users.ContainsKey(option)) break;
        else Console.WriteLine("\nKorisnik s tim id-om ne postoji. Molimo pokušajte ponovo.\n");
    }

    var (name, surname, birthDate) = UserBody();
    var account = users[option].Item3;
    users[option] = new Tuple<string, string, int, DateTime>(name, surname, account, birthDate);
    Console.WriteLine($"\nKorisnik s ID-om {option} izmijenjen.\n");
    MainMenu();
}

void DeleteUserById()
{
    UserListBySurname();
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
    UserListBySurname();
    Console.Write("Unesi ime korisnika kojeg želiš izbrisati: ");
    var name = Console.ReadLine();
    Console.Write("Unesi prezime korisnika kojeg želiš izbrisati: ");
    var surname = Console.ReadLine();

    var usersToDelete = users.Where(u => u.Value.Item1 == name && u.Value.Item2 == surname).ToList();

    if (usersToDelete.Count == 0) Console.WriteLine("Korisnik s tim imenom i prezimenom nije pronađen.");
    else
    {
        foreach (var user in usersToDelete)
        {
            users.Remove(user.Key);
            Console.WriteLine($"Korisnik {user.Value.Item1} {user.Value.Item2} s ID-om {user.Key} izbrisan.");
        }
    }

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
    var (name, surname, birthDate) = UserBody();
    int newId = users.Keys.Count != 0 ? users.Keys.Max() + 1 : 1;
    users.Add(newId, new Tuple<string, string, int, DateTime>(name, surname, 100, birthDate));
    Console.WriteLine($"\nKorisnik {name} {surname} dodan s id-om {newId}.\n");
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
            UserListBySurname();
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
