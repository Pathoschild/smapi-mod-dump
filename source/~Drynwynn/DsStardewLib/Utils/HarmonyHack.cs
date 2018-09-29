using DsStardewLib.Config;

namespace DsStardewLib.Utils
{
  /// <summary>
  /// Classes inheriting this are guaranteeing they can be loaded through the HarmonyWrapper system.  Note that
  /// any of these values can be null or set to null as determined by the mod.  The contract just states that it
  /// can be set, not what the value is.
  /// </summary>
  interface HarmonyHack
  {
    /// <summary>
    /// Provide the ability to set a log object.
    /// </summary>
    Logger Log { set; }

    /// <summary>
    /// Provide the ability to set a config object.
    /// </summary>
    HarmonyConfig Config { set; }
  }
}
