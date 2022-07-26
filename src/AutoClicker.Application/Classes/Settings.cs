using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace AutoClicker.Classes;
public class AutoClickerSettings
{
    #region ExceptionLogging
    internal bool _exceptionLogging = true;
    public bool ExceptionLogging
    {
        get { return _exceptionLogging; }
        set { _exceptionLogging = value; }
    }
    #endregion
    #region ExceptionLogLocation
    internal string _exceptionLogLocation = Environment.ExpandEnvironmentVariables(@"%APPDATA%\" +
            ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyCompanyAttribute), false)).Company + @"\" +
            Assembly.GetExecutingAssembly().GetName().Name + @"\Logs\");
    public string ExceptionLogLocation
    {
        get { return _exceptionLogLocation; }
        set { _exceptionLogLocation = value; }
    }
    #endregion
    #region ExceptionLogDays
    internal int _exceptionLogDays = 5;
    public int ExceptionLogDays
    {
        get { return _exceptionLogDays; }
        set { _exceptionLogDays = value; }
    }
    #endregion
    #region MillisecondStartDelay
    internal int _millisecondStartDelay = 5000;
    public int MillisecondStartDelay
    {
        get { return _millisecondStartDelay; }
        set { _millisecondStartDelay = value; }
    }
    #endregion
    #region CheckForUpdates
    internal bool _checkForUpdates = true;
    public bool CheckForUpdates
    {
        get { return _checkForUpdates; }
        set { _checkForUpdates = value; }
    }
    #endregion
    #region PauseHotkeys
    //https://docs.microsoft.com/en-gb/windows/win32/inputdev/virtual-key-codes?redirectedfrom=MSDN
    internal int[] _pauseHotkeys = new int[2] { 162, 69 };
    public int[] PauseHotkeys
    {
        get { return _pauseHotkeys; }
        set { _pauseHotkeys = value; }
    }
    #endregion
    #region PlayHotkeys
    //https://docs.microsoft.com/en-gb/windows/win32/inputdev/virtual-key-codes?redirectedfrom=MSDN
    internal int[] _playHotkeys = Array.Empty<int>();
    public int[] PlayHotkeys
    {
        get { return _playHotkeys; }
        set { _playHotkeys = value; }
    }
#endregion
}
public class Settings : IDisposable
{
    /// <summary>
    /// Our main reference to the individual settings
    /// </summary>
    public static AutoClickerSettings Main { get; set; }
    private XmlSerializer Serializer;
    private readonly XmlWriterSettings XmlWriterSettings;
    /// <summary>
    /// The file used to read / write to.
    /// </summary>
    internal string SettingsFile { get; set; }
    /// <summary>
    /// Initiate a new settings object, to read and write user settings to.
    /// </summary>
    /// <param name="settingsFile">The file path for the xml file</param>
    public Settings(string settingsFile)
    {
        SettingsFile = settingsFile;
        Main = new();
        XmlWriterSettings = new() { Indent = true };
        Serializer = new(typeof(AutoClickerSettings));
        if (File.Exists(SettingsFile))
        {
            // The file has already been created so 
            // there is no need to write defaults.
            Read();
        }
        else
        {
            // No file exists so lets use the default values
            Write();
        }
    }
    private void Read()
    {
        using FileStream fs = new(SettingsFile, FileMode.Open);
        try
        {
            Main = (AutoClickerSettings)Serializer.Deserialize(fs);
            System.Diagnostics.Debug.WriteLine(Main.PauseHotkeys[0]);
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show("Current settings file is not compatible with this version of the application, " +
                "this file will be removed and replaced. The application may require starting again.\r\nDetails: " + ex.Message,
                "Settings File Error",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            fs.Close();
            Write();
        }
    }
    private void Write()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(SettingsFile));
        using XmlWriter write = XmlWriter.Create(SettingsFile, XmlWriterSettings);
        Serializer.Serialize(write, Main);
        write.Close();
    }
    public void Reset(AutoClickerSettings settings)
    {
        using XmlWriter write = XmlWriter.Create(SettingsFile, XmlWriterSettings);
        Serializer.Serialize(write, settings);
        write.Close();
    }
    public void Reset()
    {
        Main = new();
        using XmlWriter write = XmlWriter.Create(SettingsFile, XmlWriterSettings);
        Serializer.Serialize(write, Main);
        write.Close();
    }
    public void Save()
    {
        Write();
    }
    public void Dispose()
    {
        Write();
        Serializer = null;
        Main = null;
        GC.SuppressFinalize(this);
    }
}