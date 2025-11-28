namespace Integrata.Kontoverwaltung.Businesslogik;
public class SparKonto:Konto
{
    /// <summary>
    /// Initialisiert ein neues Sparkonto für den angegebenen Besitzer.
    /// </summary>
    /// <param name="inhaber">Kontoinhabers.</param>
    public SparKonto(IInhaber inhaber) : base(inhaber)
    {
        Dispo = 0;  // Kein Dispo für Sparkonten
        Zinssatz = 1.5; // Beispielzinssatz
    }

    public double Zinssatz { get; protected set; }

    public Action<string> BonusNotification { get; set; } = (x) => { };  // automatischer Initialisierer mit leerer Lambda

    public override double Einzahlen(double betrag)
    {
        if (betrag > 10_000)
        {
            throw new InvalidOperationException("Einzahlungen über 10.000 sind auf Sparkonten nicht erlaubt.");
        }

        if (DateTime.Now.Month == 11 && DateTime.Now.Day == 27) // Weltspartag
        {
            TransaktionAusfuehren(2.0, Enums.Transaktionsart.Bonusgutschrift, "Einzhalung am Weltspartag");
            // Bonus von 2 Einheiten
            //BonusNotification?.Invoke($"Bonus von 2 {Waehrung} für Einzahlung am Weltspartag!");
            BonusNotification($"Bonus von 2 {Waehrung} für Einzahlung am Weltspartag!");
            //if (BonusNotification != null)
            //{
            //    BonusNotification($"Bonus von 2 {Waehrung} für Einzahlung am Weltspartag!");
            //}
        }
        return base.Einzahlen(betrag);
    }

    public override double Monatsabschluss()
    {
        double zins = GetZinsFromDatabase();
        return TransaktionAusfuehren(AktuellerSaldo * zins / 1200, Enums.Transaktionsart.Zinsgutschrift);
    }

    public virtual double GetZinsFromDatabase()
    {
        // hier der datenbankzugriff erfolgt;
        return 1.0;
    }
}
