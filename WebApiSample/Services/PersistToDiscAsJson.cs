using System.Text.Json;

namespace WebApiSample.Services;

public class PersistToDiscAsJson : IPersistToDisc
{
    public void SaveData(object data, string folderName)
    {
        var jsonString = JsonSerializer.Serialize(data);
        File.WriteAllText(Path.Combine(folderName, "data.json"), jsonString);
    }
}
