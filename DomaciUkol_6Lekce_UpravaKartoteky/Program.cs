// Zadání cesty pro ukládání souboru

using DomaciUkol_6Lekce_UpravaKartoteky;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

string cesta = @"C:\\text.txt";

//Console.WriteLine("Dobrý den,\nchcete zadat cestu k souboru ručně? (Odpovězte zadáním čísla) \n1 - ano\n2 - ne");

//string answPath = Console.ReadLine();
//bool tryAgainPath = true;

//while (tryAgainPath)
//{
//    try
//    {
//        switch (Int32.Parse(answPath))
//        {
//            case (int)TrueFalse.yes:
//                Console.WriteLine("Zadejte cestu:");
//                cesta = Console.ReadLine();
//                break;

//            case (int)TrueFalse.no:
//                cesta = @"C:\text.txt";
//                break;
//            default:
//                Console.WriteLine("Zřejmě jste jako odpověď nezadali 1 nebo 2, zkuste to znovu:");
//                answPath = Console.ReadLine();
//                break;
//        }
//        tryAgainPath = false;
//    }
//    catch (FormatException e)
//    {
//        Console.WriteLine("Zřejmě jste jako odpověď nezadali 1 nebo 2, zkuste to znovu:");
//        answPath = Console.ReadLine();
//    }
//}

// práce se slovníkem

Dictionary<string, int> osoby = File.ReadAllLines(cesta)
                                       .Select(x => x.Split('\t'))
                                       .ToDictionary(x => x[0], x => Convert.ToInt32(x[1]));

bool jeKonec = false;

Console.WriteLine("Vítejte v menu. Pro další akci zvolte číselné označení akce:");

while (!jeKonec)
{
    Console.WriteLine("\n1 - Pridat osobu");
    Console.WriteLine("2 - Smazat osobu");
    Console.WriteLine("3 - Vypsat osoby");
    Console.WriteLine("4 - Statistika kartotéky");
    Console.WriteLine("0 - Konec");

    string answ = Console.ReadLine();
    bool tryAgainFile = true;

    while (tryAgainFile)
    {
        try
        {
            switch (Int32.Parse(answ))
            {
                case (int)FileOperation.end:
                    jeKonec = true;
                    break;

                case (int)FileOperation.addPerson:
                    Osoba osoba = new Osoba();
                    Console.Write("Zadej cele jmeno (jmeno a prijmeni): ");
                    osoba.CeleJmeno = (Console.ReadLine()).ToUpper();
                    Console.Write("Zadej rok narozeni: ");

                    // kontrola roku narozeni
                    string rokNar = Console.ReadLine();
                    int rok;

                    while (!int.TryParse(rokNar, out rok) || rok < 1900)
                    {
                        Console.WriteLine("Zřejmě jste zadali špatný letopočet, zkuste to znovu:");
                        rokNar = Console.ReadLine();
                    }
                    osoba.RokNarozeni = rok;

                    try
                    {
                        osoby.Add(osoba.CeleJmeno, osoba.RokNarozeni);
                        File.AppendAllText(cesta, $"{osoba.CeleJmeno}\t{osoba.RokNarozeni}\n");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("\nDo slovníku se snažíš dostat osobu, která už byla jednou zadaná.");
                    }

                    break;

                case (int)FileOperation.deletePerson:

                    // funguje jenom v rámci slovníku, ale nezapíše se do souboru

                    Console.WriteLine("Zadejte cele jmeno osoby, kterou chcete smazat:");
                    string delPerson = (Console.ReadLine()).ToUpper();
                    if (osoby.ContainsKey(delPerson))
                    {
                        osoby.Remove(delPerson);
                        Console.WriteLine("Došlo ke smazání osoby delPerson");
                    }
                    else
                    {
                        Console.WriteLine("Zadaná osoba není ve slovníku.");
                    }

                    break;

                case (int)FileOperation.readFile:

                    Console.WriteLine("Od jakého roku narození chcete osoby vypsat?");
                    string answYear = Console.ReadLine();
                    int answYr;

                    while (!int.TryParse(answYear, out answYr) || answYr < 1900)
                    {
                        Console.WriteLine("Zřejmě jste zadali špatný letopočet, zkuste to znovu:");
                        answYear = Console.ReadLine();
                    }


                    int cnt = 0;
                    foreach (var r in osoby)
                    {
                        if (r.Value >= answYr)
                        {
                            Console.WriteLine($"{r.Key}\t{r.Value}");
                            cnt++;
                        }
                    }
                    if (cnt == 0)
                    {
                        Console.WriteLine("V kartotéce se nenachází osoby dle vašeho výběru roku narození.");
                    }

                    break;

                // práce s LINQ

                case (int)FileOperation.statisticks:

                    Console.WriteLine("\nCelkový počet osob v kartotéce: " + osoby.Count());
                    Console.WriteLine("\nNachází se v kartotéce nějaká žena? " + osoby.Any(x => x.Key.EndsWith("OVA")));
                    Console.WriteLine("Všechny osoby jsou starší 18ti let: " + osoby.All(x => x.Value < (DateTime.Now.Year - 18) ));
                    Console.WriteLine("Průměrný věk všech osob je: " + Math.Round(osoby.Average(x => DateTime.Now.Year - x.Value),0));
                    
                    Console.WriteLine("\nVýpis osob od nejmladší po nejstarší");
                    foreach (var i in osoby.OrderByDescending(x => x.Value))
                    {
                        Console.WriteLine($"{i.Key}\t{i.Value}");
                    }
                    
                    foreach (var skupina in osoby.GroupBy(x => x.Key.EndsWith("OVA")))
                    {
                        if (skupina.Key == true)
                        {
                            var jmenaZeny = skupina.Select(x => x.Key);
                            Console.WriteLine("\nVýpis všech žen:\n" + string.Join(", ", jmenaZeny));
                        }
                        else
                        {
                            var jmenaMuzi = skupina.Select(x => x.Key);
                            Console.WriteLine("\nVýpis všech mužů:\n" + string.Join(", ", jmenaMuzi));
                        }
                    }

                    var prvniPismeno = osoby.Where(x => x.Key.StartsWith("S"))
                                        .Select(x => x.Key.FirstOrDefault());
                    if(prvniPismeno != null)
                    {
                        Console.WriteLine("V kartotéce se nachází osoba s počátečním písmenem S.");
                    }
                    break;

                default:
                    Console.WriteLine("Zřejmě jste jako odpověď nedazali číslo ve škále od 0 do 3. Zkuste to znovu.");
                    answ = Console.ReadLine();
                    break;
            }

            tryAgainFile = false;

        }
        catch (FormatException e)
        {
            Console.WriteLine("Jako odpověď jste nezadali číslo. Zkuste to znovu.");
            answ = Console.ReadLine();
        }
    }

}













enum TrueFalse
{
    yes = 1,
    no = 2,
}

enum FileOperation
{
    end,
    addPerson,
    deletePerson,
    readFile,
    statisticks
}