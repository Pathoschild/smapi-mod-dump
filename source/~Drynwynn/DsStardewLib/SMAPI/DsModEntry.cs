using DsStardewLib.Utils;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DsStardewLib.SMAPI
{
  /// <summary>
  /// A class that is meant to be encapsulated in the Mod class.  Inheritence doesn't work here becuae SMAPI
  /// will currently complain if more than one class deriving from 'Mod' (even in a singular chain) is included.
  /// Therefore, include this class through composition and then call the Init method when ready.  It will set up
  /// logging, configuration, and listening for Button presses for config options following the basic syntax.
  /// </summary>
  /// <typeparam name="TConfig"></typeparam>
  public class DsModHelper<TConfig> where TConfig : class, new()
  {
    private Logger log;
    private TConfig config;
    public Logger Log { get => log; private set => log = value; }
    public TConfig Config { get => config; private set => config = value; }

    private List<PropertyInfo> configuredButtons = new List<PropertyInfo>();

    /// <summary>
    /// Initialize configuration, logging, and key presses to enable/disable config options.
    /// </summary>
    /// <param name="helper"></param>
    public void Init(IModHelper helper, IMonitor monitor)
    {
      Logger.Monitor = monitor;
      Log = Logger.GetLog();

      Log.Silly("Creating base class entry");

      Config = helper.ReadConfig<TConfig>();

      // Do this to speed up processing in handleKeyPress
      Log.Debug("Loading buttons that have configuration set");
      foreach (var fi in typeof(TConfig).GetProperties()) {
        if (fi.Name.EndsWith("Button")) {
          if ((SButton)fi.GetValue(Config) != SButton.None) {
            Log.Debug($"Adding button {fi.Name} to list");
            configuredButtons.Add(fi);
          }
        }
      }

      Log.Silly("Loading event handlers");
      InputEvents.ButtonPressed += new EventHandler<EventArgsInput>(HandleKeyPress);
      
      Log.Trace("Finished init, ready for operation");
    }

    /// <summary>
    /// Process if a user presses any of our keys.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HandleKeyPress(object sender, EventArgsInput e)
    {
      foreach (var fi in configuredButtons) {
        if ((SButton)fi.GetValue(Config) == e.Button) {
          var pName = fi.Name.Remove(fi.Name.Length - 6);
          var v = typeof(TConfig).GetProperty(pName);
          v?.SetValue(Config, !(bool)v.GetValue(Config));
          Log.Debug($"Switched value of {pName} to {(bool)v?.GetValue(Config)}");
        }
      }
    }
  }
}
