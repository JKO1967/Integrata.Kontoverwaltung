using System;
using Xunit;
using Integrata.Kontoverwaltung.Businesslogik;
using Moq;

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
            var date = SystemTime.Now;
            var neuerSaldo = sparKonto.Einzahlen(betrag);
            SystemTime.Reset();
            // Assert  
            Assert.True(bonusBenachrichtigt);
            Assert.Equal(betrag + 2, neuerSaldo);
        }

        [Fact]
        public void Monatsabschluss_ZinsenHinzugefuegt_MitMock()
        {
            // Arrange
            var mockSparKonto = new Mock<SparKonto>(new MockInhaber()) { CallBase = true };
            double betrag = 8_000;
            //mockSparKonto.Setup(k => k.Einzahlen(It.IsAny<double>())).Returns((double b) => b);
            var saldo =mockSparKonto.Object.Einzahlen(betrag);
            saldo = mockSparKonto.Object.Einzahlen(betrag);
            mockSparKonto.Object.Auszahlen(2000); // Aktueller Saldo 14.000

            //mockSparKonto.Setup(k => k.Monatsabschluss()).Returns(() =>
            //{
            //    double zinsen = betrag * 1.5 / 1200;
            //    return betrag + zinsen;
            //});
            var zins = 1.5;
            var aktuellerSaldo = mockSparKonto.Object.AktuellerSaldo;
            mockSparKonto.Setup(k => k.GetZinsFromDatabase()).Returns(zins);

            // Act
            var neuerSaldo = mockSparKonto.Object.Monatsabschluss();

            // Assert
            double erwarteteZinsen = aktuellerSaldo * zins / 1200;
            Assert.Equal(aktuellerSaldo + erwarteteZinsen, neuerSaldo, 2);
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
