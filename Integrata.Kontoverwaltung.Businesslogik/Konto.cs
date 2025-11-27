using Integrata.Kontoverwaltung.Businesslogik.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Integrata.Kontoverwaltung.Businesslogik
{
    /*
    Plan (Pseudocode / detaillierte Schritte):
    1. Für jede Klasse, Eigenschaft, Methode und eindeutige Feldbeschreibung XML-Dokumentationskommentare hinzufügen.
       - Klasse: kurze Beschreibung, Zweck.
       - Konstruktor: Parameter beschreiben, Standardwerte erklären.
       - Eigenschaften: Zweck, Zugriffsrechte und Besonderheiten (z. B. init-only, berechnet).
       - Felder: (privat) kurz beschreiben, falls Verhalten wichtig (z. B. Validierung über Property).
       - Methoden: Parameter, Rückgabewert, mögliche Ausnahmen dokumentieren.
    2. Kommentare in deutscher Sprache verfassen, passende XML-Tags verwenden:
       - <summary>, <param>, <returns>, <exception>, <value> (für Eigenschaften).
    3. Den existierenden Code nicht verändern, nur Kommentare ergänzen.
    4. Sicherstellen, dass die Kommentare präzise Auskunft über Seiteneffekte geben (z. B. Validierung des Dispo).
    5. Datei als vollständige Quelltextdatei ausgeben, unveränderte Implementierung beibehalten.
    */

    /// <summary>
    /// Repräsentiert ein Bankkonto mit grundlegenden Funktionen wie Einzahlen und Auszahlen.
    /// Beinhaltet Kontoinhaber, Kontonummer, Dispositionsrahmen, Währung sowie eine einfache IBAN-Darstellung.
    /// </summary>
    public abstract class Konto : IDisposable, INotifyPropertyChanged
    {
        /// <summary>
        /// Interner Speicher für den aktuellen Saldo des Kontos.
        /// Direkter Zugriff erfolgt über die Property <see cref="AktuellerSaldo"/>, die Validierung vornimmt.
        /// </summary>
        protected double _aktuellerSaldo;
        private bool disposedValue;

        public delegate void KontoEventHandler(object sender, KontoEventArgs e);

        public event KontoEventHandler? AktuellerSaldoChanged;
        public event KontoEventHandler? KontoUeberzogen;
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Initialisiert ein neues Konto für den angegebenen Besitzer.
        /// Erzeugt automatisch eine (pseudo-)zufällige Kontonummer, setzt einen Standarddispo und Standardwährung.
        /// </summary>
        /// <param name="name">Name des Kontoinhabers. Sollte nicht null oder leer sein (keine laufzeitbedingte Prüfung hier).</param>
        public Konto(IInhaber inhaber)
        {
            Inhaber = inhaber;
            Random random = new Random();
            Kontonummer = random.Next(100_000, int.MaxValue).ToString().PadLeft(10, '0');
            Dispo = 500.0;
            Waehrung = Waehrung.EUR;
        }

        // Spezielle Listentyp für Datenbindung in WPF
        //public ObservableCollection<Buchung> Buchungen { get; set; } = [];  // [] Shortcut für new List<Buchung>();
        public List<Buchung> Buchungen { get; set; } = [];  // [] Shortcut für new List<Buchung>();

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public Buchung this[int index]
        {
            get 
            {
                if (index < 0 || index >= Buchungen.Count)
                {
                    throw new IndexOutOfRangeException("Ungültiger Buchungsindex.");
                }
                return Buchungen[index]; 
            }
        }

        /// <summary>
        /// Kontoinhaber.
        /// </summary>
        /// <value>Der Kontoinhaber, der/die das Konto besitzt.</value>
        public IInhaber Inhaber { get; set; }

        /// <summary>
        /// Eindeutige Kontonummer im internen Format.
        /// Diese Eigenschaft ist nach der Initialisierung schreibgeschützt (<c>init</c>).
        /// </summary>
        /// <value>Eine zehnstellige Kontonummer als String, ggf. mit führenden Nullen aufgefüllt.</value>
        public string Kontonummer { get; init; }

        /// <summary>
        /// Vereinbarter Dispositionsrahmen (Überziehungsrahmen) in Kontowährung.
        /// </summary>
        /// <value>Positiver Wert, der angibt, wie weit das Konto überzogen werden darf (z.B. 500.0).</value>
        public double Dispo { get; set; }

        /// <summary>
        /// Währung des Kontos.
        /// </summary>
        /// <value>Standardmäßig auf <see cref="Waehrung.EUR"/> gesetzt.</value>
        public Waehrung Waehrung { get; init; }

        /// <summary>
        /// Bankleitzahl der fiktiven Bank (statisch).
        /// </summary>
        /// <value>String mit der festen BLZ "70070000".</value>
        public static string Blz => "70070000";
        
        /// <summary>
        /// Vereinfachte IBAN-Darstellung, zusammengesetzt aus Ländercode, BLZ und Kontonummer.
        /// Hinweis: Dies ist keine vollständige IBAN-Validierung oder -Berechnung.
        /// </summary>
        /// <value>String in der Form "DE89{Blz}{Kontonummer}".</value>
        public string Iban => $"DE89{Blz}{Kontonummer}";

        public string Anzeigename => $"{Inhaber.Name} - {Kontonummer} - {GetType().Name}";
        //public double AktuellerSaldo { get; set; }

        /// <summary>
        /// Aktueller Kontostand. Der Setter ist privat und überprüft, ob der Dispo überschritten wird.
        /// </summary>
        /// <value>Der aktuelle Saldo des Kontos. Kann negativ sein bis zum negativen Wert von <see cref="Dispo"/>.</value>
        /// <exception cref="InvalidOperationException">Wird ausgelöst, wenn ein Setzen des Saldos den erlaubten Disporahmen überschreitet.</exception>
        public double AktuellerSaldo
		{
			get { return _aktuellerSaldo; }
			private set 
            {
                _aktuellerSaldo = value;   
            }
        }


        protected double TransaktionAusfuehren(double betrag, Transaktionsart art, string text = "")
        {
            double value = 0;

            switch (art)
            {
                case Transaktionsart.Auszahlen:                   
                case Transaktionsart.Monatsabschluss:
                    betrag = -betrag;
                    break;
                default:
                    break;
            }

            value = AktuellerSaldo + betrag;

            if (value < -Dispo && art == Transaktionsart.Auszahlen)
            {
                throw new InvalidOperationException("Dispo überschritten.");
            }

            Buchungen.Add(new Buchung(betrag, art) { Buchungstext = text });

            var alterSaldo = AktuellerSaldo;
            AktuellerSaldo = value;
            
            AktuellerSaldoChanged?.Invoke(this, new KontoEventArgs() { AlterSaldo = alterSaldo, NeuerSaldo = value, Konto = this });
            
            if (value < 0 && art == Transaktionsart.Auszahlen)
            {
                KontoUeberzogen?.Invoke(this, new KontoEventArgs() { AlterSaldo = alterSaldo, NeuerSaldo = value, Konto = this });
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AktuellerSaldo)));
            return AktuellerSaldo;
        }

        /// <summary>
        /// Hebt einen Betrag auf das Konto ein (positiver Betrag wird zum Saldo hinzugefügt).
        /// </summary>
        /// <param name="betrag">Der einzuzahlende Betrag. Muss größer als 0 sein.</param>
        /// <returns>Der neue Kontostand nach erfolgreicher Einzahlung.</returns>
        /// <exception cref="ArgumentException">Wird ausgelöst, wenn <paramref name="betrag"/> kleiner oder gleich 0 ist.</exception>
        public virtual double Einzahlen(double betrag)
        {
            if (betrag <= 0)
            {
                throw new ArgumentException("Betrag muss positiv sein.");
            }
           
            return TransaktionAusfuehren(betrag, Transaktionsart.Einzahlen);
        }

        /// <summary>
        /// Zieht einen Betrag vom Konto ab (positiver Betrag reduziert den Saldo).
        /// </summary>
        /// <param name="betrag">Der auszuzahlende Betrag. Muss größer als 0 sein.</param>
        /// <returns>Der neue Kontostand nach erfolgreicher Auszahlung.</returns>
        /// <exception cref="ArgumentException">Wird ausgelöst, wenn <paramref name="betrag"/> kleiner oder gleich 0 ist.</exception>
        /// <exception cref="InvalidOperationException">Wird ausgelöst, wenn durch die Auszahlung der Dispo überschritten werden würde (diese Prüfung erfolgt über die Property-Setter-Logik).</exception>
        public virtual double Auszahlen(double betrag)
        {
            if (betrag <= 0)
            {
                throw new ArgumentException("Betrag muss positiv sein.");
            }
            return TransaktionAusfuehren(betrag, Transaktionsart.Auszahlen);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: Verwalteten Zustand (verwaltete Objekte) bereinigen
                }

                // TODO: Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer überschreiben
                // TODO: Große Felder auf NULL setzen
                disposedValue = true;
            }
        }

        // // TODO: Finalizer nur überschreiben, wenn "Dispose(bool disposing)" Code für die Freigabe nicht verwalteter Ressourcen enthält
        // ~Konto()
        // {
        //     // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Operatorüberladung für die Addition zweier Konten.
        /// </summary>
        /// <param name="k1"></param>
        /// <param name="k2"></param>
        /// <returns></returns>
        public static double operator +(Konto k1, Konto k2)
        {
            return k1.AktuellerSaldo + k2.AktuellerSaldo;
        }

        public static double operator -(Konto k1, Konto k2)
        {
            return k1.AktuellerSaldo - k2.AktuellerSaldo;
        }

        public abstract double Monatsabschluss();
    }
}
