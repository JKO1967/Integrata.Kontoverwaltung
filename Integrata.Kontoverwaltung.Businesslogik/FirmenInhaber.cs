namespace Integrata.Kontoverwaltung.Businesslogik;
public class FirmenInhaber : IInhaber
{
    public FirmenInhaber(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
    public string? Anschrift { get; set; }

    public string? Handelsregisterauszug { get; set; }
}
