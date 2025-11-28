

using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace Integrata.Kontoverwaltung.WpfUi;
internal static class Utilities
{
    public static void LogMessage(string message,[CallerMemberName] string memberName = "", [CallerFilePath] string file = "", [CallerLineNumber] int linenumber = 0 )
    {
        string logname = ConfigurationManager.AppSettings["LogFilename"] ?? "KontoLog.txt";
        Environment.SetEnvironmentVariable("MeineVariable", "MeinWert", EnvironmentVariableTarget.Process);

        string? tempPath = Environment.GetEnvironmentVariable("temp");
        
        var variables = Environment.GetEnvironmentVariables();
        // Implement logging logic here, e.g., write to a file or console
        string fileName = @$"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\{logname}";
        // Möglichkeit 1 mit statischer FileKlasse
        File.AppendAllText(fileName, $"{DateTime.Now}: {message} in {memberName} Zeile {linenumber} in Datei {file}{Environment.NewLine}");


        // Möglichkeit 2 mit FileInfo
        FileInfo fileInfo = new FileInfo(fileName);        
        using (StreamWriter writer = fileInfo.AppendText())
        {
            writer.WriteLine($"{DateTime.Now}: {message} in {memberName} Zeile {linenumber} in Datei {file}");
        }
    }
}
