using SigmaTestApp2.Models;

class Program
{
    static void Main()
    {
        var tree = new AddressTree();
        string csvPath = "test_addresses.csv";
        if (!File.Exists(csvPath))
        {
            Console.WriteLine("Ошибка: Файл не найден! Проверьте путь:");
            Console.WriteLine(Path.GetFullPath(csvPath));
            return;
        }

        tree.ImportFromCsv(csvPath);

        var address = new[] { "Российская Федерация", "город Москва", "улица Ленина", "дом 1" };
        var found = tree.FindElement(address);
        Console.WriteLine(found != null ? "Адрес найден!" : "Адрес не найден");

        var moscowAddresses = tree.SearchElementsBySubstring("Москва");
        Console.WriteLine($"Найдено адресов в Москве: {moscowAddresses.Count}");

        var spbAddress1 = new[] { "Российская Федерация", "город Санкт-Петербург", "Невский проспект", "дом 5" };
        var spbAddress2 = new[] { "Российская Федерация", "город Санкт-Петербург", "Литейный проспект", "дом 15" };
        var element1 = tree.FindElement(spbAddress1);
        var element2 = tree.FindElement(spbAddress2);

        var ancestor = tree.FindCommonAncestor(new[] { element1, element2 });
        Console.WriteLine($"Общий предок: {ancestor.FullPath}");
    }
}