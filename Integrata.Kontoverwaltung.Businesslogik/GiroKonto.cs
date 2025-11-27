namespace Integrata.Kontoverwaltung.Businesslogik;
public class GiroKonto : Konto
{
    /// <summary>
    /// Initialisiert ein neues Girokonto für den angegebenen Besitzer.
    /// </summary>
    /// <param name="inhaber">Kontoinhabers.</param>
    public GiroKonto(IInhaber inhaber) : base(inhaber)
    {
        // Girokonto-spezifische Initialisierungen können hier hinzugefügt werden
    }

    public double Gebuehren { get; set; } = 3.9;

    public override double Monatsabschluss()
    {
        AktuellerSaldo -= Gebuehren;
        return AktuellerSaldo;
    }
}
