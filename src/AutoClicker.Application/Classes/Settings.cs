using Newtonsoft.Json;
using NLog;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AutoClicker.Classes;
/// <summary>
/// This class configures the serialization and de-serialization of any settings defined within the class T.
/// Reading and writing are possible to and from any JSON file.
/// </summary>
public class Settings<T> where T : new()
{
    private readonly static ILogger _logger = LogManager.GetCurrentClassLogger();
    /// <summary>
    /// Our main reference to the individual settings
    /// </summary>
    public static T Main { get; set; }
    /// <summary>
    /// The file used to read / write to.
    /// </summary>
    internal static string SettingsFile { get; set; }
    /// <summary>
    /// Initiate a new settings object.
    /// </summary>
    public Settings() { }
    /// <summary>
    /// Read from the existing settings file, if it does not exist a new one with default values is created.
    /// </summary>
    /// <param name="settingsFile">The full file path for the JSON file with extension.</param>
    public async static Task<bool> Read(string settingsFile)
    {
        SettingsFile = settingsFile;
        Main = new();
        _logger.Info($"saving {typeof(T)} to the file {settingsFile}");
        if (File.Exists(SettingsFile))
        {
            // The file has already been created so 
            // there is no need to write defaults.
            return await Deserialize();
        }
        else
        {
            // No file exists so lets use the default values
            return await Serialize();
        }
    }
    /// <summary>
    /// Reads in the JSON file set at SettingsFile, de-serializes this into Main. Settings file will be recreated if unable to de serialize.
    /// </summary>
    private async static Task<bool> Deserialize()
    {
        try
        {
            if (string.IsNullOrEmpty(SettingsFile))
                return false;
            using var fileStream = new FileStream(SettingsFile, FileMode.Open,
                              FileAccess.Read, FileShare.ReadWrite, 4096, useAsync: true);
            using var sr = new StreamReader(fileStream, true);
            string contents = await sr.ReadToEndAsync();
            Main = JsonConvert.DeserializeObject<T>(contents, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented,
                Error = HandleDeserializationError
            });
            fileStream.Flush();
            fileStream.Close();
            return true;
        }
        catch (IOException)
        {
            _logger.Warn("Settings file could not be found, a new one will be created");
            return await Serialize();
        }
        catch (JsonSerializationException ex)
        {
            if (BackupFile(out string backupFile))
                _logger.Warn($"Unable to read settings file: {ex.Message} - A backup has been created at {backupFile}");
            else
                _logger.Warn($"Unable to read settings file: {ex.Message} - A backup could not be created.");
            return await Serialize();
        }
    }
    private static bool BackupFile(out string backupFile)
    {
        backupFile = "";
        if (string.IsNullOrEmpty(SettingsFile))
            return false;
        backupFile = string.Format("{0}-{1}",
            SettingsFile, DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"));
        File.Copy(SettingsFile, backupFile);
        return true;
    }
    /// <summary>
    /// Serializes Main and writes all into the file defined with SettingsFile.
    /// </summary>
    private async static Task<bool> Serialize()
    {
        string contents = JsonConvert.SerializeObject(Main, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented,
            Error = HandleSerializationError
        });
        return await WriteToFile(contents);
    }
    private async static Task<bool> WriteToFile(string contents)
    {
        if (string.IsNullOrEmpty(SettingsFile))
            return false;
        string directory = Path.GetDirectoryName(SettingsFile);
        if (string.IsNullOrEmpty(directory))
            return false;
        Directory.CreateDirectory(directory);
        FileMode mode = FileMode.Create;
        if (File.Exists(SettingsFile))
            mode = FileMode.Truncate;
        using var fileStream = new FileStream(SettingsFile, mode,
                               FileAccess.Write, FileShare.ReadWrite, 4096, useAsync: true);

        byte[] settingsBytes = Encoding.UTF8.GetBytes(contents);
        await fileStream.WriteAsync(settingsBytes);
        fileStream.Flush();
        fileStream.Close();
        return true;
    }
    private static void HandleDeserializationError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e)
    {
        if (e.CurrentObject != null)
            _logger.Error("Unable to deserialize an object located at: {0} - expected type: {1}",
                e.ErrorContext.Path, e.CurrentObject.GetType().FullName);
        else
        {
            if (BackupFile(out string backupFile))
                _logger.Warn($"Unable to read settings file: {SettingsFile} - A backup has been created at {backupFile}");
            else
                _logger.Warn($"Unable to read settings file: {SettingsFile} - A backup could not be created.");
        }
        e.ErrorContext.Handled = true;
    }
    private static void HandleSerializationError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e)
    {
        if (e.CurrentObject != null)
            _logger.Error("Unable to serialize an object located at: {0} - expected type: {1}",
                e.ErrorContext.Path, e.CurrentObject.GetType().FullName);
        else
        {
            if (BackupFile(out string backupFile))
                _logger.Warn($"Unable to read settings file: {SettingsFile} - A backup has been created at {backupFile}");
            else
                _logger.Warn($"Unable to read settings file: {SettingsFile} - A backup could not be created.");
        }
        e.ErrorContext.Handled = true;
    }
    /// <summary>
    /// Save the current settings to the file.
    /// </summary>
    public async static Task<bool> Save()
    {
        return await Serialize();
    }
}
