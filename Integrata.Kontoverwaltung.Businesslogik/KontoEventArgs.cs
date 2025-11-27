
namespace Integrata.Kontoverwaltung.Businesslogik;
public class KontoEventArgs : EventArgs
{
    public double AlterSaldo { get; init; }
    public double NeuerSaldo { get; init; }
    public Konto Konto { get; init; } = null!;  // ich bin mir bewusst dass es evtl. Null sein kann, mit null! unterdrücke ich die Warnung
}
