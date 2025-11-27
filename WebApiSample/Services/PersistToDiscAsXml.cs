using System.Xml.Serialization;

namespace WebApiSample.Services;

public class PersistToDiscAsXml : IPersistToDisc
{
    public void SaveData(object data, string folderName)
    {
        XmlSerializer xmlSerializer = new XmlSerializer(data.GetType());
        using (var writer = new StreamWriter(Path.Combine(folderName, "data.xml")))
        {
            xmlSerializer.Serialize(writer, data);
        }
    }
}
