using Integrata.Kontoverwaltung.Businesslogik;
using System.Windows;

namespace Integrata.Kontoverwaltung.WpfUi;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private Konto? konto;

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
                ((SparKonto)neuesKonto).BonusNotification += NotificationMessage_Received;
            }
            neuesKonto.AktuellerSaldoChanged += Konto_SaldoChanged;
            neuesKonto.KontoUeberzogen += Konto_Ueberzogen;
            int position = lstKonto.Items.Add(neuesKonto);
            lstKonto.SelectedIndex = position;
        }
        else
        {
            MessageBox.Show("Bitte geben Sie den Namen des Kontoinhabers an.");
            txtName.Focus();
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
                transaction?.Invoke(betrag).ToString("###,##0.00");
                //txtAktuellerSaldo.Text = konto.AktuellerSaldo.ToString("###,##0.00");
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Fehler bei der Transaktion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Transaktion nicht möglich, da Konto überzogen", MessageBoxButton.OK, MessageBoxImage.Error);
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
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show(ex.Message, "Buchung nicht möglich, da Konto überzogen", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}