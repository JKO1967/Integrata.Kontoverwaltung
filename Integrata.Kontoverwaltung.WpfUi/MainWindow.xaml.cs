using Integrata.Kontoverwaltung.Businesslogik;
using System.Collections.ObjectModel;
using System.Windows;

namespace Integrata.Kontoverwaltung.WpfUi;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private Konto? konto;
    private ObservableCollection<Buchung>? Buchungen;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void btnNeu_Click(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(txtName.Text))
        {
            Konto neuesKonto;
            if (rbGirokonto.IsChecked.HasValue && rbGirokonto.IsChecked.Value)
            {
                neuesKonto = new GiroKonto(new FirmenInhaber(txtName.Text));
            }
            else
            {
                neuesKonto = new SparKonto(new PersonenInhaber(txtName.Text));
                //((SparKonto)neuesKonto).BonusNotification += NotificationMessage_Received;
            }
            GenerateBuchungen(neuesKonto);
            neuesKonto.AktuellerSaldoChanged += Konto_SaldoChanged;
            //neuesKonto.KontoUeberzogen += Konto_Ueberzogen;
            neuesKonto.KontoUeberzogen += (sender, e) =>
            {
                MessageBox.Show($"Achtung: Das Konto ist überzogen! Aktueller Saldo: {e.NeuerSaldo.ToString("###,##0.00")}", "Konto überzogen", MessageBoxButton.OK, MessageBoxImage.Warning);
            };
            int position = lstKonto.Items.Add(neuesKonto);
            lstKonto.SelectedIndex = position;
        }
        else
        {
            MessageBox.Show("Bitte geben Sie den Namen des Kontoinhabers an.");
            txtName.Focus();
        }
    }

    private void GenerateBuchungen(Konto neuesKonto)
    {
        for (int i = 1; i < 40; i++)
        {
            neuesKonto.Einzahlen(100 * (i + 1));
            neuesKonto.Auszahlen(50 * i);
            if (i % 5 == 0)
            {
                neuesKonto.Monatsabschluss();
            }
        }
    }

    private void Konto_Ueberzogen(object sender, KontoEventArgs e)
    {
        MessageBox.Show($"Achtung: Das Konto ist überzogen! Aktueller Saldo: {e.NeuerSaldo.ToString("###,##0.00")}", "Konto überzogen", MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    private void Konto_SaldoChanged(object sender, KontoEventArgs e)
    {
        txtAktuellerSaldo.Text = e.NeuerSaldo.ToString("###,##0.00");
    }

    private void NotificationMessage_Received(string message)
    {
        MessageBox.Show($"Herzlichen Glückwunsch! {message} erhalten.", "Bonus gutgeschrieben", MessageBoxButton.OK, MessageBoxImage.Information);

    }

    private void lstKonto_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        konto = lstKonto.SelectedItem as Konto;
        if (konto != null)
        {
            txtKontoName.Text = $"{konto.Inhaber.Name} ({konto.Inhaber.GetType().Name})";
            txtIban.Text = konto.Iban;
            txtKontotyp.Text = konto.GetType().Name;
            txtAktuellerSaldo.Text = konto.AktuellerSaldo.ToString("###,##0.00");
            txtWaehrung.Text = konto.Waehrung.ToString();
            Buchungen = new ObservableCollection<Buchung>(konto.Buchungen);
            dgKontoDetails.ItemsSource = Buchungen;

            txtCurrentSaldo.SetBinding(System.Windows.Controls.TextBox.TextProperty, new System.Windows.Data.Binding("AktuellerSaldo")
            {
                Source = konto,
                StringFormat = "###,##0.00",
                Mode = System.Windows.Data.BindingMode.OneWay
            });

        }
    }

    private void btnEinzahlen_Click(object sender, RoutedEventArgs e)
    {
        TransaktionAusfuehren(konto != null ? konto.Einzahlen : null);
    }

    private void btnAuszahlen_Click(object sender, RoutedEventArgs e)
    {
        TransaktionAusfuehren(konto != null ? konto.Auszahlen : null);
    }

    private void TransaktionAusfuehren(Func<double, double>? transaction)
    {
        if (konto == null)
        {
            MessageBox.Show("Bitte wählen Sie ein Konto aus.");
            return;
        }

        if (double.TryParse(txtBetrag.Text, out double betrag))
        {
            try
            {
                transaction?.Invoke(betrag);
                //txtAktuellerSaldo.Text = konto.AktuellerSaldo.ToString("###,##0.00");
                Buchungen?.Add(konto.Buchungen.Last());
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Fehler bei der Transaktion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Transaktion nicht möglich, da Konto überzogen", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Unerwarteter Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                // Ich logge jetzt diesen Fehler
                throw; // Wirf den Fehler erneut, damit er weiter oben behandelt werden kann
            }
        }
        else
        {
            MessageBox.Show("Bitte geben Sie einen gültigen Betrag ein.");
        }
        txtBetrag.SelectAll();  // oder txtBetrag.Text = "";
        txtBetrag.Focus();

    }

    private void btnMonatsabschluss_Click(object sender, RoutedEventArgs e)
    {
        if (konto == null)
        {
            MessageBox.Show("Bitte wählen Sie ein Konto aus.");
            return;
        }

        try
        {
            konto.Monatsabschluss().ToString("###,##0.00");
            Buchungen?.Add(konto.Buchungen.Last());
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show(ex.Message, "Buchung nicht möglich, da Konto überzogen", MessageBoxButton.OK, MessageBoxImage.Error);
        }

    }

    private void btnSortieren_Click(object sender, RoutedEventArgs e)
    {
        var result = Buchungen?.OrderBy(b => b.Buchungsdatum.Date).ThenBy(b => b.Betrag).ToList();
        dgKontoDetails.ItemsSource = result;
    }

    private void btnFilter_Click(object sender, RoutedEventArgs e)
    {
        // LINQ Methodsyntax
        //dgKontoDetails.ItemsSource = Buchungen?
        //    //.Where(b => b.Betrag >= 500 && b.Transaktionsart == Businesslogik.Enums.Transaktionsart.Einzahlen)
        //    //.Where(DoFiltern)
        //    .Where(b => DoFiltern(b))
        //    .OrderByDescending(b => b.Betrag)
        //    .ToList();

        // LINQ Querysyntax
        dgKontoDetails.ItemsSource = (from b in Buchungen!
                                      where b.Betrag >= 500 && b.Transaktionsart == Businesslogik.Enums.Transaktionsart.Einzahlen
                                      orderby b.Betrag descending
                                      select b).ToList();
    }

    private bool DoFiltern(Buchung b)
    {
        return b.Betrag >= 500 && b.Transaktionsart == Businesslogik.Enums.Transaktionsart.Einzahlen;
    }

    private void btnTransformation_Click(object sender, RoutedEventArgs e)
    {
        dgKontoDetails.ItemsSource = Buchungen?.Select(b => new { Datum = b.Buchungsdatum.ToLongDateString(), b.Betrag, Art = b.Transaktionsart }).ToList();
    }

    private void btnReset_Click(object sender, RoutedEventArgs e)
    {
        dgKontoDetails.ItemsSource = Buchungen;
    }
}