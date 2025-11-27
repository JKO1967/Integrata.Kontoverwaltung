using Integrata.Kontoverwaltung.Businesslogik;

internal class Program
{
    private static void Main(string[] args)
    {
        using (Konto k = new GiroKonto(new PersonenInhaber("Jürgen")))
        {
            //k.AktuellerSaldo = 1000.0; // This line will cause a compilation error because the setter is private
            Console.WriteLine(k.AktuellerSaldo.ToString("c"));
            Console.WriteLine(k.Inhaber.Name);
            Console.WriteLine(k.Iban);
            Console.WriteLine(k.Kontonummer);
            Console.WriteLine(k.Waehrung.ToString());
            k.Einzahlen(250.0);
            Console.WriteLine(k.AktuellerSaldo.ToString("c"));
            k.Auszahlen(100.0);
            Console.WriteLine(k.AktuellerSaldo.ToString("c"));

            // Aufruf des Indexer
            Console.WriteLine(k[0].Transaktionsart.ToString());

            D d = new D("xx");
            SparKonto sk = new SparKonto(new PersonenInhaber("Mario"));
            // hier registriere ich mich für die Action Bonusnotification
            sk.BonusNotification = (x) => Console.WriteLine(x);  // Registrierung mittels Anonymer methode
            //sk.BonusNotification += Bonusnachricht; // Registrierung mittels Methodengruppe
            sk.Einzahlen(100);
            Console.WriteLine(sk.AktuellerSaldo.ToString("c"));

            GiroKonto gk = new GiroKonto(new PersonenInhaber("Christopher"));
            gk.Einzahlen(500);
            Console.WriteLine(gk.AktuellerSaldo.ToString("c"));

            List<Konto> konten = new List<Konto>();
            konten.Add(sk);
            konten.Add(gk);
            konten.Add(k);

            foreach (Konto konto in konten)
            {
                //SparKonto? ko2 = (SparKonto)konto;  // wäre Laufzeitfehler bei den Girokonto
                SparKonto? ko = konto as SparKonto;
                if (ko != null)
                {
                    ko.BonusNotification += Bonusnachricht;
                }

                konto.Auszahlen(konto.AktuellerSaldo);
                konto.Einzahlen(100);
                Console.WriteLine($"Konto vom Typ {konto.GetType().ToString()} vom Inhaber {konto.Inhaber} hat einen Saldo von {konto.AktuellerSaldo.ToString("c")}");

            }

            Console.WriteLine($"Gesamtsaldo = {gk + sk}");

            Console.WriteLine(ParamArraySample(true, "hallo", "welt", "wie", "gefällt", "euch", "c#"));
        }
    }

    private static string ParamArraySample(bool toUpper, params string[] args)
    {
        string result = "";
        foreach (string s in args)
        {
            result += s + "; ";
        }
        if (toUpper)
        {
            result = result.ToUpper();
        }
        return result;
    }


    public static void Bonusnachricht(string message)
    {
        Console.WriteLine($"Super, ich habe gerade einen Bonus bekommen: {message}");
    }

}
public class A
{
    public A()
    {
        Console.WriteLine("Ich bin der Konstruktor von A");
    }
}

public class B : A
{
    public B()
    {
        Console.WriteLine("Ich bin der Konstruktor von B");
    }
}

public class C : B
{
    public C(string x)
    {
        Console.WriteLine($"Ich bin der Konstruktor von C und heiße {x}");
    }
}

public class D : C
{
    public D(string x) : base(x)
    {
        Console.WriteLine("Ich bin der Konstruktor von D");
    }
}
