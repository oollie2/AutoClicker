using System;
using System.Reflection;

namespace AutoClicker.Classes;

public class AutoClickerSettings
{
    #region ExceptionLogging
    internal bool _exceptionLogging = true;
    /// <summary>
    /// The application will automatically catch exceptions that happen and log them to a file. Each exception is appended to the file throughout a day, and a new file will be created the next day. If no file exists, no exceptions have been caught.
    /// </summary>
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
    /// <summary>
    /// The directory that the exception logs are written to.
    /// </summary>
    public string ExceptionLogLocation
    {
        get { return _exceptionLogLocation; }
        set { _exceptionLogLocation = value; }
    }
    #endregion
    #region ExceptionLogDays
    internal int _exceptionLogDays = 5;
    /// <summary>
    /// This is how long the application will store logs for, if a log is longer than the defined amount the file is deleted. If the value is 0 logs are not deleted.
    /// </summary>
    public int ExceptionLogDays
    {
        get { return _exceptionLogDays; }
        set { _exceptionLogDays = value; }
    }
    #endregion
    #region MillisecondStartDelay
    internal int _millisecondStartDelay = 5000;
    /// <summary>
    /// After pressing the start button, the application will wait this long in milliseconds before beginning.
    /// </summary>
    public int MillisecondStartDelay
    {
        get { return _millisecondStartDelay; }
        set { _millisecondStartDelay = value; }
    }
    #endregion
    #region CheckForUpdates
    internal bool _checkForUpdates = true;
    /// <summary>
    /// If the value is set as true, the application will automatically check for an update every time it is started.
    /// </summary>
    public bool CheckForUpdates
    {
        get { return _checkForUpdates; }
        set { _checkForUpdates = value; }
    }
    #endregion
    #region PauseHotkeys
    //https://docs.microsoft.com/en-gb/windows/win32/inputdev/virtual-key-codes?redirectedfrom=MSDN
    internal int[] _pauseHotkeys = new int[2] { 162, 69 };
    /// <summary>
    /// A list of integers, matching a virtual key code. https://docs.microsoft.com/en-gb/windows/win32/inputdev/virtual-key-codes?redirectedfrom=MSDN. Key presses are not ordered, and must be added to the array with starting and enclosing int tags.
    /// </summary>
    public int[] PauseHotkeys
    {
        get { return _pauseHotkeys; }
        set { _pauseHotkeys = value; }
    }
    #endregion
    #region PlayHotkeys
    //https://docs.microsoft.com/en-gb/windows/win32/inputdev/virtual-key-codes?redirectedfrom=MSDN
    internal int[] _playHotkeys = Array.Empty<int>();
    /// <summary>
    /// A list of integers, matching a virtual key code. https://docs.microsoft.com/en-gb/windows/win32/inputdev/virtual-key-codes?redirectedfrom=MSDN. Key presses are not ordered, and must be added to the array with starting and enclosing int tags.
    /// </summary>
    public int[] PlayHotkeys
    {
        get { return _playHotkeys; }
        set { _playHotkeys = value; }
    }
#endregion
}
