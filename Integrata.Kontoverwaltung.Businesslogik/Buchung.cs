using Integrata.Kontoverwaltung.Businesslogik.Enums;

namespace Integrata.Kontoverwaltung.Businesslogik;
public class Buchung
{
    public Buchung(double betrag, Transaktionsart art)
    {
        Betrag = betrag;
        Transaktionsart = art;
        Buchungsdatum = DateTime.Now;
    }

    public DateTime Buchungsdatum { get; init; }
    public double Betrag { get; init; }

    public Transaktionsart  Transaktionsart { get; init; }

    public string? Buchungstext { get; set; }
}
