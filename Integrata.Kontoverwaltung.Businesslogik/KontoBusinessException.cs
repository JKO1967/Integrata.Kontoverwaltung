namespace Integrata.Kontoverwaltung.Businesslogik;
public class KontoBusinessException : Exception
{
    public KontoBusinessException()
    {
    }

    public KontoBusinessException(string? message) : base(message)
    {
    }

    public KontoBusinessException(string? message, Exception? innerException) : base(message, innerException)
    {
    }    
}
