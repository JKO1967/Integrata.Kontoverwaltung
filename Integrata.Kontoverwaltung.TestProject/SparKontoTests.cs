using System;
using Xunit;
using Integrata.Kontoverwaltung.Businesslogik;

namespace Integrata.Kontoverwaltung.TestProjekt
{
    public class SparKontoTests
    {
        private class MockInhaber : IInhaber
        {
            public string Name { get; set; } = "Testinhaber";
            public string? Anschrift { get; set; } = "Testanschrift";
        }

        [Fact]
        public void SparKonto_Initialisierung_SetztStandardwerte()
        {
            // Arrange  
            var inhaber = new MockInhaber();

            // Act  
            var sparKonto = new SparKonto(inhaber);

            // Assert  
            Assert.Equal(0, sparKonto.Dispo);
            Assert.Equal(1.5, sparKonto.Zinssatz);
        }

        [Fact]
        public void Einzahlen_UnterGrenze_AktualisiertSaldo()
        {
            // Arrange  
            var sparKonto = new SparKonto(new MockInhaber());
            double betrag = 5000;
            var weltspartag = new DateTime(DateTime.Now.Year, 11, 27);
            // Act  
            var neuerSaldo = sparKonto.Einzahlen(betrag);

            if (weltspartag == DateTime.Now.Date)
            {
                betrag += 2;
            }
            // Assert  
            Assert.Equal(betrag, neuerSaldo);
        }

        [Fact]
        public void Einzahlen_UeberGrenze_WirftException()
        {
            // Arrange  
            var sparKonto = new SparKonto(new MockInhaber());
            double betrag = 15_000;

            // Act & Assert  
            Assert.Throws<InvalidOperationException>(() => sparKonto.Einzahlen(betrag));
        }

        [Fact]
        public void Einzahlen_AmWeltspartag_BonusHinzugefuegt()
        {
            // Arrange  
            var sparKonto = new SparKonto(new MockInhaber());
            double betrag = 1000;
            bool bonusBenachrichtigt = false;
            sparKonto.BonusNotification = (message) =>
            {
                Assert.Contains("Bonus von 2", message);
                bonusBenachrichtigt = true;
            };

            // Act  
            var weltspartag = new DateTime(DateTime.Now.Year, 11, 27);
            SystemTime.Set(weltspartag);
            var neuerSaldo = sparKonto.Einzahlen(betrag);
            SystemTime.Reset();
            // Assert  
            Assert.True(bonusBenachrichtigt);
            Assert.Equal(betrag + 2, neuerSaldo);
        }

        [Fact]
        public void Monatsabschluss_ZinsenHinzugefuegt()
        {
            var weltspartag = new DateTime(DateTime.Now.Year, 11, 27);
            // Arrange  
            var sparKonto = new SparKonto(new MockInhaber());
            double betrag = 8_000;
            sparKonto.Einzahlen(betrag);

            // Act  
            var neuerSaldo = sparKonto.Monatsabschluss();

            if (DateTime.Now.Date == weltspartag)
            {
                betrag += 2;
            }
            // Assert  
            double erwarteteZinsen = betrag * 1.5 / 1200;
            Assert.Equal(betrag + erwarteteZinsen, neuerSaldo, 2);
        }
    }

    public static class SystemTime
    {
        private static DateTime? _customDateTime;

        public static void Set(DateTime customDateTime) => _customDateTime = customDateTime;
        public static void Reset() => _customDateTime = null;
        public static DateTime Now => _customDateTime ?? DateTime.Now;
    }
}
