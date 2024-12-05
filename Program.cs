using System.IO;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace ZadanieRekrutacyjne
{
    public class Program
    {
        static void Main(string[] args)
        {
            List<Towar> towary = WczytajDaneZPliku();
            Console.WriteLine("Dane zostały wczytane.");
            bool exit = false;

            while (!exit)
            {
                menuTekst();

                switch (Console.ReadLine())
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
                        string fraza = Console.ReadLine();
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
            var towary = new List<Towar>();

            Console.WriteLine("Podaj ścieżkę do pliku CSV:");

            string path = ZabespieczeniePrzedNull();

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
        static string ZabespieczeniePrzedNull()
        {
            string wprowadzonyTeskt;
            bool exit = false;
            do
            {
                wprowadzonyTeskt = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(wprowadzonyTeskt))
                {
                    Console.WriteLine("Ścieżka nie może być pusta. Proszę spróbować ponownie.");
                }
                else if (!File.Exists(wprowadzonyTeskt))
                {
                    Console.WriteLine($"Błędna ścieżka do pliku CSV - {wprowadzonyTeskt}");
                }
                else
                {                 
                    exit = true;
                }
            }
            while (!exit);
            return wprowadzonyTeskt;
        }
        static void ZapiszDoXmlPosortowane(List<Towar> towary)
        {
            var posortowaneTowary = towary.OrderBy(t => t.Nazwa).ToList();
            SaveToXml(posortowaneTowary, "TowaryPosortowaneNazwa.xml");
        }
        static void ZapiszDoXmlCenaWiekszaOd(List<Towar> towary, decimal cenaMin)
        {
            var posortowaneTowary = towary.Where(t => t.Cena > cenaMin).OrderByDescending(t => t.Cena).ToList();
            SaveToXml(posortowaneTowary, "TowaryPosortowaneCenaOd.xml");
        }
        static void WyszukajFrazeWOpisie(List<Towar> towary, string fraza)
        {
            var wyniki = towary.Where(t => t.Opis.A.Contains(fraza) 
                                        || t.Opis.B.Contains(fraza)).ToList();
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
        static void SaveToXml(List<Towar> towary, string nazwaPliku)
        {
            var xml = new XElement("Plik",
                new XElement("Towary",
                    from t in towary
                    select new XElement("Towar",
                        new XElement("Nazwa", t.Nazwa),
                        new XElement("Cena", t.Cena),
                        new XElement("Opis",
                            new XElement("A", t.Opis.A),
                            new XElement("B", t.Opis.B)
                            )
                        )
                    )
                );

            xml.Save(nazwaPliku);

            Console.WriteLine($"Plik XML zapisany jako {nazwaPliku}");
            System.Diagnostics.Process.Start("explorer", nazwaPliku); // Otwiera plik XML w eksploratorze
        }
        static void menuTekst()
        {
            Console.WriteLine("Wybierz opcję:");
            Console.WriteLine("1. Zapisz do XML posortowane wg nazwy");
            Console.WriteLine("2. Zapisz do XML gdzie cena większa od");
            Console.WriteLine("3. Wyszukaj frazę w opisie");
            Console.WriteLine("4. Koniec");
        }
    }
}

