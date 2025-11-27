namespace WebApiSample.Services;

public interface IPersistToDisc
{
    void SaveData(object data, string folderName);
}
