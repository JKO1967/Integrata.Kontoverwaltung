namespace Integrata.Kontoverwaltung.Businesslogik;
public class PersonenInhaber : IInhaber
{
    public PersonenInhaber(string name)
    {
        Name = name;
    }

    public string Name { get; set ; }
    public string? Anschrift { get; set; }

    public DateOnly Geburtsdatum { get; set; }
}
