using System.Data.Common;
using System.Numerics;
using System.Transactions;
using System.Xml.Linq;

using System;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.VisualBasic.FileIO;

SortedDictionary<int, (Tuple<string, string, DateTime> userInfo, Tuple<int, int, int> accounts)> users =
    new SortedDictionary<int, (Tuple<string, string, DateTime> userInfo, Tuple<int, int, int> accounts)>();

users.Add(1, (new Tuple<string, string, DateTime>("Ante", "Antic", new DateTime(2001, 5, 21)),
              new Tuple<int, int, int>(800, -100, -200)));

users.Add(2, (new Tuple<string, string, DateTime>("Mate", "Matic", new DateTime(1980, 2, 1)),
              new Tuple<int, int, int>(190, 50, -100)));

SortedDictionary<int, Tuple<int, int, string, int, string, DateTime, string>> transactions =
    new SortedDictionary<int, Tuple<int, int, string, int, string, DateTime, string>>();

transactions.Add(1, new Tuple<int, int, string, int, string, DateTime, string>(1, 1, "prihod", 800, "plaća",
    new DateTime(2023, 4, 21), "Posao"));
transactions.Add(2, new Tuple<int, int, string, int, string, DateTime, string>(1, 1, "rashod", 100, "sport",
    DateTime.Now, "Članarina"));
transactions.Add(3, new Tuple<int, int, string, int, string, DateTime, string>(1, 2, "rashod", 100, "sport",
    new DateTime(2024, 5, 11), "Članarina"));
transactions.Add(4, new Tuple<int, int, string, int, string, DateTime, string>(1, 3, "rashod", 200, "hrana",
    new DateTime(2024, 1, 1), "Konzum"));
transactions.Add(5, new Tuple<int, int, string, int, string, DateTime, string>(2, 1, "prihod", 100, "poklon",
    new DateTime(2024, 6, 20), "standardna transakcija"));
transactions.Add(6, new Tuple<int, int, string, int, string, DateTime, string>(2, 1, "rashod", 10, "prijevoz",
    DateTime.Now, "Gorivo"));
transactions.Add(7, new Tuple<int, int, string, int, string, DateTime, string>(2, 2, "prihod", 50, "honorar",
    new DateTime(2024, 7, 2), "standardna transakcija"));
transactions.Add(8, new Tuple<int, int, string, int, string, DateTime, string>(2, 3, "rashod", 100, "hrana",
    new DateTime(2024, 5, 25), "Lidl"));


//components
void BackButton()
{
    Console.Write("\nPritisni bilo koju tipku za povratak nazad...");
    Console.ReadKey();
    Console.Clear();
}
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
            Console.Clear();
            Console.WriteLine("Unos nije validan. Molimo pokušajte ponovo.\n");
            continue;
        }
        break;
    } while (true);

    Console.Clear();

    return option;
}
void UserListDisplayBody(IEnumerable<KeyValuePair<int, (Tuple<string, string, DateTime> userInfo,
    Tuple<int, int, int> accounts)>> userList, string message)
{
    if (userList.Any())
    {
        Console.WriteLine("Id - Ime - Prezime - Datum rođenja");
        foreach (var user in userList)
        {
            int id = user.Key;
            string name = user.Value.userInfo.Item1;
            string surname = user.Value.userInfo.Item2;
            DateTime birthDate = user.Value.userInfo.Item3;

            Console.WriteLine($"{id} - {name} - {surname} - {birthDate.ToString("dd.MM.yyyy")}");
        }
    }
    else Console.WriteLine(message);
}
void TransactionListDisplayBody(IEnumerable<KeyValuePair<int, Tuple<int, int, string, int, string, DateTime, string>>> transactionList, string message)
{
    if (transactionList.Any())
    {
        Console.WriteLine("Id - Tip - Iznos - Kategorija - Opis - Datum");
        foreach (var transaction in transactionList)
        {
            string type = transaction.Value.Item3;
            int price = transaction.Value.Item4;
            string category = transaction.Value.Item5;
            string summary = transaction.Value.Item7;
            DateTime transactionDate = transaction.Value.Item6;

            Console.WriteLine($"{transaction.Key} - {type} - {price} - {category} - {summary} - {transactionDate.ToString("dd.MM.yyyy")}");
        }
    }
    else Console.WriteLine(message);
}
DateTime DateTimeBody()
{
    int day, month, year;
    DateTime birthDate;
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
    return birthDate;
}
(string, int, string, DateTime, string) TransactionBody()
{
    var typeNum = "";
    var option = "";
    int amount;
    var category = "";
    var summary = "standarna transakcija";
    DateTime dateTime = DateTime.Now;

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
        if (typeNum == "1") Console.Write("Kategorija (plaća, honorar, poklon): ");
        else Console.Write("Kategorija (hrana, prijevoz, sport): ");
        category = Console.ReadLine();
        if (typeNum == "1" && (category == "plaća" || category == "honorar" || category == "poklon") ||
            typeNum == "2" && (category == "hrana" || category == "prijevoz" || category == "sport")) break;
        Console.WriteLine("Kategorija ne postoji. Pokušajte ponovo.");
    }

    Console.Write("Opis: ");
    var text = Console.ReadLine();
    if (text != "") summary = text;

    while (true)
    {
        Console.Write("Pritisni 1 za postavljanje transakcije na današnji dan ili 2 za postavljanje transakcije na određeni datum: ");
        option = Console.ReadLine();
        if (option == "1" || option == "2") break;
        Console.WriteLine("Niste pritisnuli 1 ili 2. Pokušajte ponovo.");
    }

    if (option == "2") dateTime = DateTimeBody();

    return (typeNum, amount, category, dateTime, summary);
}

(string, string, DateTime) UserBody()
{
    var name = "";
    var surname = "";
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
    Console.WriteLine("Datum rođenja");
    birthDate = DateTimeBody();

    return (name, surname, birthDate);
}
void AccountValue(int userId, int accountId, int amount)
{
    var user = users[userId];
    int accountBalance = 0;

    if (accountId == 1) accountBalance = user.accounts.Item1;
    else if (accountId == 2) accountBalance = user.accounts.Item2;
    else if (accountId == 3) accountBalance = user.accounts.Item3;

    accountBalance += amount;

    if (accountId == 1)
        users[userId] = (user.userInfo, new Tuple<int, int, int>(accountBalance, user.accounts.Item2, user.accounts.Item3));
    else if (accountId == 2)
        users[userId] = (user.userInfo, new Tuple<int, int, int>(user.accounts.Item1, accountBalance, user.accounts.Item3));
    else if (accountId == 3)
        users[userId] = (user.userInfo, new Tuple<int, int, int>(user.accounts.Item1, user.accounts.Item2, accountBalance));

}
void RemoveTransactions(List<int> keysToRemove, int userId, int accountId)
{
    var user = users[userId];
    int accountBalance = 0;

    if (accountId == 1) accountBalance = user.accounts.Item1;
    else if (accountId == 2) accountBalance = user.accounts.Item2;
    else if (accountId == 3) accountBalance = user.accounts.Item3;

    foreach (var key in keysToRemove)
    {
        var transaction = transactions[key];
        var transactionType = transaction.Item3;
        var amount = transaction.Item4;

        if (transactionType == "prihod") accountBalance -= amount;
        else if (transactionType == "rashod") accountBalance += amount;

        transactions.Remove(key);
    }

    if (accountId == 1)
        users[userId] = (user.userInfo, new Tuple<int, int, int>(accountBalance, user.accounts.Item2, user.accounts.Item3));
    else if (accountId == 2)
        users[userId] = (user.userInfo, new Tuple<int, int, int>(user.accounts.Item1, accountBalance, user.accounts.Item3));
    else if (accountId == 3)
        users[userId] = (user.userInfo, new Tuple<int, int, int>(user.accounts.Item1, user.accounts.Item2, accountBalance));

    Console.WriteLine($"Izbrisano {keysToRemove.Count} transakcija.");
    Accounts(userId);
}

void RemoveTransactionsDialog(List<int> keysToRemove, int userId, int accountId)
{
    var dialog = "";
    while (true)
    {
        Console.Write($"\nIzbrisati {keysToRemove.Count} transakcije (y za da ili n za ne)? ");
        dialog = Console.ReadLine();

        if (dialog == "y")
        {
            Console.Clear();
            RemoveTransactions(keysToRemove, userId, accountId);
            break;
        }
        else if (dialog == "n")
        {
            Console.Clear();
            break;
        }
        else Console.WriteLine("\nNiste upisali y ili n. Molimo pokušajte ponovo.\n");
    }
}

//functions
void AddTransaction(int userId, int accountId)
{
    var (typeNum, amount, category, dateTime, summary) = TransactionBody();
    int newId = transactions.Keys.Count != 0 ? transactions.Keys.Max() + 1 : 1;
    transactions.Add(newId, new Tuple<int, int, string, int, string, DateTime, string>(userId, accountId, typeNum == "1" ? "prihod" : "rashod", amount, category, dateTime, summary));
    AccountValue(userId, accountId, typeNum == "1" ? amount : -amount);
    Console.Clear();
    Console.WriteLine("Transakcija dodana.\n");
    Transactions(userId, accountId);
}
void DeleteTransactionById(int userId, int accountId)
{
    var userTransactions = transactions.Where(x => x.Value.Item1 == userId && x.Value.Item2 == accountId).ToList();
    TransactionListDisplayBody(userTransactions, "Nema transakcija.");
    var userKeys = transactions.Where(transaction => transaction.Value.Item1 == userId && transaction.Value.Item2 == accountId)
        .Select(transaction => transaction.Key)
        .ToList();
    var dialog = "";
    int option;

    while (true)
    {
        Console.Write("\nUpiši id transakcije koje želiš izmijeniti: ");
        int.TryParse(Console.ReadLine(), out option);

        if (userKeys.Contains(option)) break;
        else Console.WriteLine("\nTransakcija s tim id-om ne postoji. Molimo pokušajte ponovo.\n");
    }
    while (true)
    {
        Console.Write($"\nIzbrisati transakciju s id-om {option} (y za da ili n za ne)? ");
        dialog = Console.ReadLine();

        if (dialog == "y")
        {
            transactions.Remove(option);
            Console.Clear();
            Console.WriteLine($"Izbrisana transakcije s id-om {option}.");
            break;
        }
        else if (dialog == "n")
        {
            Console.Clear();
            break;
        }
        else Console.WriteLine("\nNiste upisali y ili n. Molimo pokušajte ponovo.\n");
    }
    Accounts(userId);
}
void DeleteTransactionsUnderCertainAmount(int userId, int accountId)
{
    int amount;
    while (true)
    {
        Console.Write("Iznos: ");
        if (int.TryParse(Console.ReadLine(), out amount)) break;
        Console.WriteLine("Iznos mora biti broj. Pokušajte ponovo.");
    }
    var keysToRemove = transactions
        .Where(transaction => transaction.Value.Item4 < amount && transaction.Value.Item1 == userId && transaction.Value.Item2 == accountId)
        .Select(transaction => transaction.Key)
        .ToList();

    RemoveTransactionsDialog(keysToRemove, userId, accountId);
}
void DeleteTransactionsAboveCertainAmount(int userId, int accountId)
{
    int amount;
    while (true)
    {
        Console.Write("Iznos: ");
        if (int.TryParse(Console.ReadLine(), out amount)) break;
        Console.WriteLine("Iznos mora biti broj. Pokušajte ponovo.");
    }
    var keysToRemove = transactions
        .Where(transaction => transaction.Value.Item4 > amount && transaction.Value.Item1 == userId && transaction.Value.Item2 == accountId)
        .Select(transaction => transaction.Key)
        .ToList();

    RemoveTransactionsDialog(keysToRemove, userId, accountId);
}
void DeleteIncomeTransactions(int userId, int accountId)
{
    var keysToRemove = transactions
        .Where(transaction => transaction.Value.Item3 == "prihod" && transaction.Value.Item1 == userId && transaction.Value.Item2 == accountId)
        .Select(transaction => transaction.Key)
        .ToList();

    RemoveTransactionsDialog(keysToRemove, userId, accountId);
}
void DeleteExpenseTransactions(int userId, int accountId)
{
    var keysToRemove = transactions
        .Where(transaction => transaction.Value.Item3 == "rashod" && transaction.Value.Item1 == userId && transaction.Value.Item2 == accountId)
        .Select(transaction => transaction.Key)
        .ToList();

    RemoveTransactionsDialog(keysToRemove, userId, accountId);
}
void DeleteTransactionsOfSelectedCategory(int userId, int accountId)
{
    var category = "";
    while (true)
    {
        Console.Write("Kategorija (plaća, honorar, poklon, hrana, prijevoz, sport): ");
        category = Console.ReadLine();
        if (category == "plaća" || category == "honorar" || category == "poklon" || 
            category == "hrana" || category == "prijevoz" || category == "sport") break;
        Console.WriteLine("Kategorija ne postoji. Pokušajte ponovo.");
    }
    var keysToRemove = transactions
        .Where(transaction => transaction.Value.Item5 == category && transaction.Value.Item1 == userId && transaction.Value.Item2 == accountId)
        .Select(transaction => transaction.Key)
        .ToList();

    RemoveTransactionsDialog(keysToRemove, userId, accountId);
}

void DeleteTransaction(int userId, int accountId)
{
    string[] options = { "Povratak\n", "Brisanje transakcije po id-u", "Brisanje ispod unesenog iznosa", "Brisanje iznad unesenog iznosa", "Brisanje svih prihoda", "Brisanje svih rashoda", "Brisanje svih transakcija za odabranu kategoriju" };
    int option = Display(options);

    switch (option)
    {
        case 0:
            Transactions(userId, accountId);
            break;
        case 1:
            DeleteTransactionById(userId, accountId);
            break;
        case 2:
            DeleteTransactionsUnderCertainAmount(userId, accountId);
            break;
        case 3:
            DeleteTransactionsAboveCertainAmount(userId, accountId);
            break;
        case 4:
            DeleteIncomeTransactions(userId, accountId);
            break;
        case 5:
            DeleteExpenseTransactions(userId, accountId);
            break;
        case 6:
            DeleteTransactionsOfSelectedCategory(userId, accountId);
            break;
    }
}
void UpdateTransaction(int userId, int accountId)
{
    var userTransactions = transactions.Where(x => x.Value.Item1 == userId && x.Value.Item2 == accountId).ToList();
    TransactionListDisplayBody(userTransactions, "Nema transakcija.");
    var userKeys = transactions.Where(transaction => transaction.Value.Item1 == userId && transaction.Value.Item2 == accountId)
        .Select(transaction => transaction.Key)
        .ToList();
    int option;
    var dialog = "";

    while (true)
    {
        Console.Write("\nUpiši id transakcije koje želiš izmijeniti: ");
        int.TryParse(Console.ReadLine(), out option);

        if (userKeys.Contains(option)) break;
        else Console.WriteLine("\nTransakcija s tim id-om ne postoji. Molimo pokušajte ponovo.\n");
    }
    var oldAmount = transactions[option].Item4;
    var (typeNum, amount, category, dateTime, summary) = TransactionBody();
    while (true)
    {
        Console.Write($"\nAžurirati transakciju s id-om {option} (y za da ili n za ne)? ");
        dialog = Console.ReadLine();

        if (dialog == "y")
        {
            transactions[option] = new Tuple<int, int, string, int, string, DateTime, string>
                (userId, accountId, typeNum == "1" ? "prihod" : "rashod", amount, category, dateTime, summary);
            Console.Clear();
            Console.WriteLine($"\nTransakcija s ID-om {option} izmijenjena.\n");
            AccountValue(userId, accountId, amount - oldAmount);
            break;
        }
        else if (dialog == "n") 
        { 
            Console.Clear(); 
            break; 
        }
        else Console.WriteLine("\nNiste upisali y ili n. Molimo pokušajte ponovo.\n");
    }

    Transactions(userId, accountId);
}
void TransactionListAscendingAmount(int userId, int accountId)
{
    var sortedTransactions = transactions.Where(x => x.Value.Item1 == userId && x.Value.Item2 == accountId).OrderBy(transaction => transaction.Value.Item4).ToList();
    TransactionListDisplayBody(sortedTransactions, "Nema transakcija.");
}
void TransactionListBySummary(int userId, int accountId)
{
    var sortedTransactions = transactions.Where(x => x.Value.Item1 == userId && x.Value.Item2 == accountId).OrderBy(transaction => transaction.Value.Item7).ToList();
    TransactionListDisplayBody(sortedTransactions, "Nema transakcija.");
}
void TransactionListDescendingAmount(int userId, int accountId)
{
    var sortedTransactions = transactions.Where(x => x.Value.Item1 == userId && x.Value.Item2 == accountId).OrderByDescending(transaction => transaction.Value.Item4).ToList();
    TransactionListDisplayBody(sortedTransactions, "Nema transakcija.");
}
void TransactionListAscendingDate(int userId, int accountId)
{
    var sortedTransactions = transactions.Where(x => x.Value.Item1 == userId && x.Value.Item2 == accountId).OrderBy(transaction => transaction.Value.Item6).ToList();
    TransactionListDisplayBody(sortedTransactions, "Nema transakcija.");
}
void TransactionListDescendingDate(int userId, int accountId)
{
    var sortedTransactions = transactions.Where(x => x.Value.Item1 == userId && x.Value.Item2 == accountId).OrderByDescending(transaction => transaction.Value.Item6).ToList();
    TransactionListDisplayBody(sortedTransactions, "Nema transakcija.");
}
void TransactionListIncome(int userId, int accountId)
{
    var foundTransactions = transactions.Where(x => x.Value.Item1 == userId && x.Value.Item2 == accountId && x.Value.Item3 == "prihod").ToList();
    TransactionListDisplayBody(foundTransactions, "Nema transakcija.");
}
void TransactionListExpense(int userId, int accountId)
{
    var foundTransactions = transactions.Where(x => x.Value.Item1 == userId && x.Value.Item2 == accountId && x.Value.Item3 == "rashod").ToList();
    TransactionListDisplayBody(foundTransactions, "Nema transakcija.");
}
void TransactionListByCategory(int userId, int accountId)
{
    var category = "";
    while (true)
    {
        Console.Write("Kategorija (plaća, honorar, poklon, hrana, prijevoz, sport): ");
        category = Console.ReadLine();
        if (category == "plaća" || category == "honorar" || category == "poklon" ||
            category == "hrana" || category == "prijevoz" || category == "sport") break;
        Console.WriteLine("Kategorija ne postoji. Pokušajte ponovo.");
    }
    var foundTransactions = transactions.Where(x => x.Value.Item1 == userId && x.Value.Item2 == accountId && x.Value.Item5 == category).ToList();
    TransactionListDisplayBody(foundTransactions, "Nema transakcija.");
}

void TransactionListRegular(int userId, int accountId)
{
    var userTransactions = transactions.Where(x => x.Value.Item1 == userId && x.Value.Item2 == accountId).ToList();
    TransactionListDisplayBody(userTransactions, "Nema transakcija.");
}
void TransactionList(int userId, int accountId)
{
    string[] options = { "Povratak\n", "Sve transakcije", "Sve transakcije sortirane po iznosu uzlazno", 
        "Sve transakcije sortirane po iznosu silazno", "Sve transakcije sortirane po opisu abecedno",
        "Sve transakcije sortirane po datumu uzlazno", "Sve transakcije sortirane po datumu silazno",
        "Svi prihodi", "Svi rashodi", "Sve transakcije za odabranu kategoriju" };
    int option = Display(options);

    switch (option)
    {
        case 0:
            Transactions(userId, accountId);
            break;
        case 1:
            TransactionListRegular(userId, accountId);
            break;
        case 2:
            TransactionListAscendingAmount(userId, accountId);
            break;
        case 3:
            TransactionListDescendingAmount(userId, accountId);
            break;
        case 4:
            TransactionListBySummary(userId, accountId);
            break;
        case 5:
            TransactionListAscendingDate(userId, accountId);
            break;
        case 6:
            TransactionListDescendingDate(userId, accountId);
            break;
        case 7:
            TransactionListIncome(userId, accountId);
            break;
        case 8:
            TransactionListExpense(userId, accountId);
            break;
        case 9:
            TransactionListByCategory(userId, accountId);
            break;
    }

    if (option > 0 && option < 10)
    {
        BackButton();
        TransactionList(userId, accountId);
    }
}

void CurrentAccountBalance(int userId, int accountId)
{
    var sum = users[userId].accounts.Item1 + users[userId].accounts.Item2 + users[userId].accounts.Item3;
    if (accountId == 1) Console.WriteLine($"Trenutno stanje računa: {users[userId].accounts.Item1} eura");
    else if (accountId == 2) Console.WriteLine($"Trenutno stanje računa: {users[userId].accounts.Item2} eura");
    else if (accountId == 3) Console.WriteLine($"Trenutno stanje računa: {users[userId].accounts.Item3} eura");
}
void TransactionsCount(int userId, int accountId)
{
    var userTransactions = transactions.Where(t => t.Value.Item1 == userId && t.Value.Item2 == accountId).ToList();
    Console.WriteLine($"Ukupan broj transakcija: {userTransactions.Count}");
}
void IncomeAndExpensesForChosenPeriod(int userId, int accountId)
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

    var userTransactions = transactions.Where(t => t.Value.Item1 == userId && t.Value.Item2 == accountId && 
    t.Value.Item6.Month == month && t.Value.Item6.Year == year);
    if (userTransactions.Any())
    {
        foreach (var transaction in userTransactions)
        {
            string type = transaction.Value.Item3;
            int amount = transaction.Value.Item4;

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
void PercentageExpenseForChosenCategory(int userId, int accountId)
{
    var category = "";
    var chosenIncome = 0;
    var income = 0;
    while (true)
    {
        Console.Write("Kategorija (hrana, prijevoz, sport): ");
        category = Console.ReadLine();
        if (category == "hrana" || category == "prijevoz" || category == "sport") break;
        Console.WriteLine("Kategorija ne postoji. Pokušajte ponovo.");
    }
    var userTransactions = transactions.Where(t => t.Value.Item1 == userId && t.Value.Item2 == accountId && t.Value.Item3 == "rashod");
    if (userTransactions.Any())
    {
        foreach (var transaction in userTransactions)
        {
            int amount = transaction.Value.Item4;
            if (transaction.Value.Item5 == category)
                chosenIncome += amount;
            income += amount;
        }
        Console.WriteLine($"Postotak ukupnih troškova koji odlazi na kategoriju {category}: {(int)Math.Round((double)(100 * chosenIncome) / income)}%");
    }
    else Console.WriteLine($"Nema transakcija s kategorijom {category}.");
}
void AverageBalanceTransactionForChosenPeriod(int userId, int accountId)
{
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

    var userTransactions = transactions.Where(t => t.Value.Item1 == userId && t.Value.Item2 == accountId && t.Value.Item6.Month == month && t.Value.Item6.Year == year);
    if (userTransactions.Any())
    {
        foreach (var transaction in userTransactions)
        {
            string type = transaction.Value.Item3;
            int amount = transaction.Value.Item4;

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
void AverageBalanceTransactionForChosenCategory(int userId, int accountId)
{
    var category = "";
    var balance = 0;
    while (true)
    {
        Console.Write("Kategorija (plaća, honorar, poklon, hrana, prijevoz, sport): ");
        category = Console.ReadLine();
        if (category == "plaća" || category == "honorar" || category == "poklon" || 
            category == "hrana" || category == "prijevoz" || category == "sport") break;
        Console.WriteLine("Kategorija ne postoji. Pokušajte ponovo.");
    }
    var userTransactions = transactions.Where(t => t.Value.Item1 == userId && t.Value.Item2 == accountId && t.Value.Item5 == category);
    if (userTransactions.Any())
    {
        foreach (var transaction in userTransactions)
        {
            string type = transaction.Value.Item3;
            int amount = transaction.Value.Item4;

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

void FinancialReport(int userId, int accountId)
{
    string[] options = { "Povratak\n", "Trenutno stanje računa", "Ukupan broj transakcija",
        "Ukupan iznos prihoda i rashoda za odabrani mjesec i godinu", "Postotak udjela rashoda za odabranu kategoriju",
        "Prosječni iznos transakcije za odabrani mjesec i godinu", "Prosječni iznos transakcije za odabranu kategoriju" };
    int option = Display(options);

    switch (option)
    {
        case 0:
            Transactions(userId, accountId);
            break;
        case 1:
            CurrentAccountBalance(userId, accountId);
            break;
        case 2:
            TransactionsCount(userId, accountId);
            break;
        case 3:
            IncomeAndExpensesForChosenPeriod(userId, accountId);
            break;
        case 4:
            PercentageExpenseForChosenCategory(userId, accountId);
            break;
        case 5:
            AverageBalanceTransactionForChosenPeriod(userId, accountId);
            break;
        case 6:
            AverageBalanceTransactionForChosenCategory(userId, accountId);
            break;
    }

    if (option > 0 && option < 7)
    {
        BackButton();
        FinancialReport(userId, accountId);
    }
}

void Transactions(int userId, int accountId)
{
    string[] options = { "Povratak\n", "Unos nove transakcije", "Brisanje transakcije",
        "Uređivanje transakcije", "Pregled transakcija", "Financijsko izvješće" };
    int option = Display(options);

    switch (option)
    {
        case 0:
            Accounts(userId);
            break;
        case 1:
            AddTransaction(userId, accountId);
            break;
        case 2:
            DeleteTransaction(userId, accountId);
            break;
        case 3:
            UpdateTransaction(userId, accountId);
            break;
        case 4:
            TransactionList(userId, accountId);
            break;
        case 5:
            FinancialReport(userId, accountId);
            break;
    }
}

void Accounts(int userId)
{
    string[] options = { "Povratak u glavni izbornik\n", "Tekući", "Žiro", "Prepaid" };
    int option = Display(options);

    switch (option)
    {
        case 0:
            MainMenu();
            break;
        case 1:
            Transactions(userId, option);
            break;
        case 2:
            Transactions(userId, option);
            break;
        case 3:
            Transactions(userId, option);
            break;
    }
}
void UserListBySurname()
{
    var sortedUsers = users.OrderBy(user => user.Value.userInfo.Item2).ToList();
    UserListDisplayBody(sortedUsers, "Nema korisnika.");
}

void UserListByAge()
{
    var usersOver30 = users.Where(user => (DateTime.Now.Year - user.Value.userInfo.Item3.Year) > 30).ToList();
    UserListDisplayBody(usersOver30, "Nema korisnika starijih od 30.");
}

void UserListWithNegativeAccount()
{
    var usersWithNegativeAccount = users
        .Where(user => user.Value.accounts.Item1 < 0 || user.Value.accounts.Item2 < 0 || user.Value.accounts.Item3 < 0)
        .ToList();
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

    if (option > 0 && option < 4)
    {
        BackButton();
        Users();
    }
}

void UpdateUser()
{
    UserListBySurname();
    int option;
    var dialog = "";
    while (true)
    {
        Console.Write("\nUpiši id korisnika kojeg želiš izmijeniti: ");
        int.TryParse(Console.ReadLine(), out option);

        if (users.ContainsKey(option)) break;
        else Console.WriteLine("\nKorisnik s tim id-om ne postoji. Molimo pokušajte ponovo.\n");
    }
    var (name, surname, birthDate) = UserBody();
    var currentAccounts = users[option].accounts;
    while (true)
    {
        Console.Write($"\nAžurirati korisnika s id-om {option} (y za da ili n za ne)? ");
        dialog = Console.ReadLine();

        if (dialog == "y")
        {
            users[option] = (new Tuple<string, string, DateTime>(name, surname, birthDate), currentAccounts);
            Console.Clear();
            Console.WriteLine($"Korisnik s ID-om {option} izmijenjen.\n");
            break;
        }
        else if (dialog == "n")
        {
            Console.Clear();
            break;
        }
        else Console.WriteLine("\nNiste upisali y ili n. Molimo pokušajte ponovo.");
    }
    MainMenu();
}

void DeleteUserById()
{
    UserListBySurname();
    int option;
    var dialog = "";
    while (true)
    {
        Console.Write("\nUpiši id korisnika kojeg želiš izbrisati: ");
        int.TryParse(Console.ReadLine(), out option);

        if (users.ContainsKey(option)) break;
        else Console.WriteLine("\nKorisnik s tim id-om ne postoji. Molimo pokušajte ponovo.\n");
    }
    while (true)
    {
        Console.Write($"\nIzbrisati korisnika s id-om {option} (y za da ili n za ne)? ");
        dialog = Console.ReadLine();

        if (dialog == "y")
        {
            users.Remove(option);
            Console.Clear();
            Console.WriteLine($"Korisnik s ID-om {option} izbrisan.\n");
            break;
        }
        else if (dialog == "n")
        {
            Console.Clear();
            break;
        }
        else Console.WriteLine("\nNiste upisali y ili n. Molimo pokušajte ponovo.");
    }
    MainMenu();
}

void DeleteUserByFirstnameAndLastname()
{
    UserListBySurname();
    Console.Write("\nUnesi ime korisnika kojeg želiš izbrisati: ");
    var name = Console.ReadLine();
    Console.Write("Unesi prezime korisnika kojeg želiš izbrisati: ");
    var surname = Console.ReadLine();
    var dialog = "";

    var usersToDelete = users
        .Where(u => u.Value.userInfo.Item1 == name && u.Value.userInfo.Item2 == surname)
        .ToList();

    if (usersToDelete.Count == 0)
    {
        Console.Clear(); 
        Console.WriteLine("Korisnik s tim imenom i prezimenom nije pronađen.\n");
    }
    else
    {
        while (true)
        {
            Console.Write($"\nIzbrisati {usersToDelete.Count} korisnika (y za da ili n za ne)? ");
            dialog = Console.ReadLine();

            if (dialog == "y")
            {
                Console.Clear();
                foreach (var user in usersToDelete)
                {
                    users.Remove(user.Key);
                    Console.WriteLine($"Korisnik {name} {surname} izbrisan.");
                }
                break;
            }
            else if (dialog == "n")
            {
                Console.Clear();
                break;
            }
            else Console.WriteLine("\nNiste upisali y ili n. Molimo pokušajte ponovo.");
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
    users.Add(newId, (new Tuple<string, string, DateTime>(name, surname, birthDate),
                      new Tuple<int, int, int>(100, 0, 0)));
    Console.Clear();
    Console.WriteLine($"Korisnik {name} {surname} dodan s id-om {newId}.\n");
    MainMenu();
}
void Users()
{
    string[] options = { "Povratak u glavni izbornik\n", "Unos novog korisnika", "Brisanje korisnika",
        "Uređivanje korisnika", "Pregled korisnika" };
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
