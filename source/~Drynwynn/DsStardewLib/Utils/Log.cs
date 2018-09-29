using StardewModdingAPI;
using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace DsStardewLib.Utils
{
  /// <summary>
  /// Handle all logging for the application.  Mainly here as a gap between spurious logging
  /// statements throughout the app and a full fledged logging framework.  Usually this class
  /// is a factory that generates a logger for each class, but for this simple mod that was all
  /// yanked out and now it's a singleton.
  /// </summary>
  public class Logger
  {
    /*******************
    ** PUBLIC PROPS/VARS
    *******************/

    /// <summary>
    /// Reference to the Monitor from SMAPI to actually do the logging.
    /// </summary>
    public static IMonitor Monitor { get; set; }

    /********************
    ** PRIVATE PROPS/VARS
    ********************/

    /// <summary>
    /// Singleton logger.
    /// </summary>
    private static Logger log;

    /// <summary>
    /// Header to add type and method information to the log line.  This information is not needed during a
    /// release build so this variable is here so the Conditional SetHeader method can be compiled out
    /// during Release build.
    /// </summary>
    private string header = "";

    /**************
    ** CONSTRUCTORS
    **************/

    /// <summary>
    /// Logger is a singleton so it cannot be instantiated directly.  Use GetLog() instead.
    /// </summary>
    private Logger() { }

    /****************
    ** PUBLIC METHODS
    ****************/

    /// <summary>
    /// Returns the instance of Logger that can be used for app logging.  Note that a Monitor
    /// MUST be set before this method will return a logger.
    /// </summary>
    /// <returns>The app Logger to use</returns>
    /// <exception cref="ArgumentException">If a Monitor has not been set before this method is called.</exception>
    public static Logger GetLog()
    {
      if (Logger.Monitor == null) {
        throw new ArgumentException("Cannot get log without first setting a monitor");
      }

      // A simple singleton
      return (log == null) ? log = new Logger() : log;
    }

    /// <summary>
    /// A generic logging function because I am lazy and don't want to type this.Monitor.log every time.
    /// (Yes Log.Log is only like nine keystrokes less.  Sue me.)
    /// </summary>
    /// <param name="message">The message to print to logs.  No processing is performed on this argument.</param>
    /// <param name="l">The log level to output the message as.  Defaults to LogLevel.Debug</param>
    public string Log(string msg = "", LogLevel l = LogLevel.Debug, [CallerMemberName] string callerName = "", [CallerFilePath] string callerPath = "")
    {
      SetHeader(callerName, callerPath);
      Logger.Monitor.Log($"{header}{msg}", l);
      return msg;
    }

    // Convenience functions so LogLevel doesn't have to be defined every time.
    // Can C# define methods programmatically in a loop?  Would make this nicer.
    public string Alert(string msg = "", [CallerMemberName] string callerName = "", [CallerFilePath] string callerPath = "")
    {
      return Log(msg, LogLevel.Alert, callerName, callerPath);
    }

    public string Debug(string msg = "", [CallerMemberName] string callerName = "", [CallerFilePath] string callerPath = "")
    {
      // Pathos advised that he likes debug to show by default in the console window and we should limit output to trace.
      // I don't personally agree with this; IMO log level in Release should be set to INFO, but it's not my API, so comply with
      // tool norms.
#if DEBUG
      return Log(msg, LogLevel.Debug, callerName, callerPath);
#else
      return Log(msg, LogLevel.Trace, callerName, callerPath);
#endif
    }

    public string Error(string msg = "", [CallerMemberName] string callerName = "", [CallerFilePath] string callerPath = "")
    {
      return Log(msg, LogLevel.Error, callerName, callerPath);
    }

    public string Info(string msg = "", [CallerMemberName] string callerName = "", [CallerFilePath] string callerPath = "")
    {
      return Log(msg, LogLevel.Info, callerName, callerPath);
    }

    public string Trace(string msg = "", [CallerMemberName] string callerName = "", [CallerFilePath] string callerPath = "")
    {
      return Log(msg, LogLevel.Trace, callerName, callerPath);
    }

    /// <summary>
    /// The lowest level of debug statements for the app.  This function is compiled out of Release builds.
    /// </summary>
    /// <seealso cref="Log"/>
    /// <remarks>
    /// This method is marked Conditional DEBUG so that during Release builds it is removed from code and
    /// any call to it becomes a noop.  I prefer this so that release code doesn't have the call to the function
    /// in it so that there is no performance impact for putting "silly" debug statements in Debug build (although such
    /// impacts would undoubtedly be very small, I still like to remove it completely).
    /// 
    /// It should go without saying that any logging statements that should be in a Release build that should be
    /// controlled via a LogLevel instead should not use this function.
    /// </remarks>
    [Conditional("DEBUG")]
    public void Silly(string msg = "", [CallerMemberName] string callerName = "", [CallerFilePath] string callerPath = "")
    {
      Trace(msg, callerName, callerPath);
    }

    public string Warn(string msg = "", [CallerMemberName] string callerName = "", [CallerFilePath] string callerPath = "")
    {
      return Log(msg, LogLevel.Warn, callerName, callerPath);
    }

    /*****************
    ** PRIVATE METHODS
    *****************/

    /// <summary>
    /// Set the header using compile time caller info.
    /// </summary>
    /// <param name="callerName">The name of the method</param>
    /// <param name="callerPath">The name of the file (used to infer class name MAYBE but close enough)</param>
    /// <remarks>An internal function that is compiled out in Release builds.</remarks>
    [Conditional("DEBUG")]
    private void SetHeader(string callerName, string callerPath)
    {
      header = $"{Path.GetFileNameWithoutExtension(callerPath)}#{callerName} - ";
    }
  }
}
