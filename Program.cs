using System.IO;
using System.Xml.Serialization;
using ZadanieRekrutacyjne;

public class Program
{
    static void Main(string[] args)
    {
        List<Towar> towary = WczytajDaneZPliku();
        Console.WriteLine("Dane zostały wczytane.");

        Console.WriteLine("Wybierz opcję:");
        Console.WriteLine("1. Zapisz do XML posortowane wg nazwy");
        Console.WriteLine("2. Zapisz do XML gdzie cena większa od");
        Console.WriteLine("3. Wyszukaj frazę w opisie");
        Console.WriteLine("4. Koniec");
        bool exit = false;
        var wybor = Console.ReadLine();
        while (!exit)
        {
            switch (wybor)
            {
                case "1":
                    ZapiszDoXmlPosortowane(towary);
                    break;
                case "2":
                    Console.WriteLine("Podaj minimalną cenę:");
                    decimal cena = Convert.ToDecimal(Console.ReadLine());
                    ZapiszDoXmlCenaWiekszaOd(towary, cena);
                    break;
                case "3":
                    Console.WriteLine("Podaj frazę do wyszukania:");
                    string fraza = SprwdzanieNullWStringu(Console.ReadLine());
                    WyszukajFrazeWOpisie(towary, fraza);
                    break;
                case "4":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Niepoprawna opcja.");
                    break;
            }
        }
    }

    static List<Towar> WczytajDaneZPliku()
    {
        Console.WriteLine("Podaj ścieżkę do pliku CSV:");

        string path = SprwdzanieNullWStringu(Console.ReadLine());
        
        var towary = new List<Towar>();

        foreach (var line in File.ReadLines(path))
        {
            var columns = line.Split(';');
            var towar = new Towar
            {
                Nazwa = columns[0],
                Cena = decimal.Parse(columns[1]),
                Opis = new Opisy
                {
                    A = columns[2],
                    B = columns[3]
                }
            };
            towary.Add(towar);
        }

        return towary;
    }
    static string SprwdzanieNullWStringu(string wprowadzonyTeskt )
    {
        do
        {
            wprowadzonyTeskt = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(wprowadzonyTeskt))
            {
                Console.WriteLine("Ścieżka nie może być pusta. Proszę spróbować ponownie.");
            }
        }
        while (string.IsNullOrWhiteSpace(wprowadzonyTeskt));
        return wprowadzonyTeskt;
    }
    static void ZapiszDoXmlPosortowane(List<Towar> towary)
    {
        var posortowaneTowary = towary.OrderBy(t => t.Nazwa).ToList();
        ZapiszDoXml(posortowaneTowary, "TowaryPosortowaneNazwa.xml");
    }

    static void ZapiszDoXmlCenaWiekszaOd(List<Towar> towary, decimal cenaMin)
    {
        var posortowaneTowary = towary.Where(t => t.Cena > cenaMin).OrderByDescending(t => t.Cena).ToList();
        ZapiszDoXml(posortowaneTowary, "TowaryCenaWiekszaOd.xml");
    }
    static void WyszukajFrazeWOpisie(List<Towar> towary, string fraza)
    {
        var wyniki = towary.Where(t => t.Opis.A.Contains(fraza) || t.Opis.B.Contains(fraza)).ToList();

        if (wyniki.Any())
        {
            foreach (var towar in wyniki)
            {
                Console.WriteLine($"Nazwa: {towar.Nazwa}, Cena: {towar.Cena}, Opis: {towar.Opis.A}, {towar.Opis.B}");
            }
        }
        else
        {
            Console.WriteLine("Brak wyników.");
        }
    }
    static void ZapiszDoXml(List<Towar> towary, string fileName)
    {
        var xmlSerializer = new XmlSerializer(typeof(Plik));

        var plik = new Plik
        {
            Towary = towary
        };

        using (var writer = new StreamWriter(fileName))
        {
            xmlSerializer.Serialize(writer, plik);
        }

        Console.WriteLine($"Dane zostały zapisane do {fileName}");
        System.Diagnostics.Process.Start("explorer", fileName); // Otwiera plik XML w eksploratorze
    }
}

public class Plik
{
    [XmlArray("Towary")]
    [XmlArrayItem("Towar")]
    public List<Towar> Towary { get; set; }
}
