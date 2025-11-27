namespace Integrata.Kontoverwaltung.Businesslogik.Tests
{
    public class KontoTests
    {
        [Fact]
        public void Ctor_SetsDefaultsAndProperties()
        {
            // Arrange & Act
            var konto = new GiroKonto(new PersonenInhaber("Max Mustermann"));

            // Assert
            Assert.Equal("Max Mustermann", konto.Inhaber.Name);
            Assert.NotNull(konto.Kontonummer);
            Assert.Equal(10, konto.Kontonummer.Length);
            Assert.True(konto.Kontonummer.All(char.IsDigit), "Kontonummer muss nur Ziffern enthalten.");
            Assert.Equal(500.0, konto.Dispo);
            Assert.Equal(Waehrung.EUR, konto.Waehrung);
            Assert.Equal(0.0, konto.AktuellerSaldo);
        }

        [Fact]
        public void Einzahlen_PositiveAmount_UpdatesAndReturnsBalance()
        {
            // Arrange
            var konto = new GiroKonto(new PersonenInhaber("Test"));

            // Act
            var result = konto.Einzahlen(100.0);

            // Assert
            Assert.Equal(100.0, result);
            Assert.Equal(100.0, konto.AktuellerSaldo);
        }

        [Theory]
        [InlineData(0.0)]
        [InlineData(-50.0)]
        public void Einzahlen_NonPositive_ThrowsArgumentException(double betrag)
        {
            // Arrange
            var konto = new GiroKonto(new PersonenInhaber("Test"));

            // Act & Assert
            Assert.Throws<ArgumentException>(() => konto.Einzahlen(betrag));
        }

        [Fact]
        public void Auszahlen_PositiveWithinDispo_UpdatesAndReturnsBalance()
        {
            // Arrange
            var konto = new GiroKonto(new PersonenInhaber("Test"));
            konto.Einzahlen(200.0);

            // Act
            var result = konto.Auszahlen(150.0);

            // Assert
            Assert.Equal(50.0, result);
            Assert.Equal(50.0, konto.AktuellerSaldo);
        }

        [Theory]
        [InlineData(0.0)]
        [InlineData(-10.0)]
        public void Auszahlen_NonPositive_ThrowsArgumentException(double betrag)
        {
            // Arrange
            var konto = new GiroKonto(new PersonenInhaber("Test"));

            // Act & Assert
            Assert.Throws<ArgumentException>(() => konto.Auszahlen(betrag));
        }

        [Fact]
        public void Auszahlen_BeyondDispo_ThrowsInvalidOperationException()
        {
            // Arrange
            var konto = new GiroKonto(new PersonenInhaber("Test"));
            // Standard-Dispo ist 500.0

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => konto.Auszahlen(konto.Dispo + 1.0));
        }

        [Fact]
        public void Iban_IsComposedCorrectly()
        {
            // Arrange
            var konto = new GiroKonto(new PersonenInhaber("Test"));

            // Act
            var expected = $"DE89{Konto.Blz}{konto.Kontonummer}";

            // Assert
            Assert.Equal(expected, konto.Iban);
        }
    }
}