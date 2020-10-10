/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Drynwynn/StardewValleyMods
**
*************************************************/

using DsStardewLib.Config;
using DsStardewLib.Utils;
using Harmony;
using StardewValley.Locations;

namespace NoAddedFlyingMineMonsters.Lib.HarmonyHacks
{
  /// <summary>
  /// Simple Harmony patch that removes the call to spawn a random flying monster.  This will prevent monsters from
  /// spawning while walking around a mine level, and also when the 'fog' rolls in.
  /// </summary>
  [HarmonyPatch(typeof(MineShaft), "spawnFlyingMonsterOffScreen")]
  class NoRandomMonsters : HarmonyHack
  {
    // Variables to do the business, and also to meet the Hack interface.
    private static ModConfig config = null;
    private static Logger log = null;
    public Logger Log { get => NoRandomMonsters.log; set => NoRandomMonsters.log = value; }
    public HarmonyConfig Config { get => NoRandomMonsters.config; set => NoRandomMonsters.config = value as ModConfig; }

    /// <summary>
    /// If the config is set to deny any random monsters, then return false so Harmony will just prevent the call
    /// from running in the first place.  Note that this will not prevent any flying enemies that are placed on the map
    /// during load.
    /// </summary>
    /// <returns>True if random flying monsters should spawn, false if not.</returns>
    [HarmonyPrefix]
    static bool AllowMonsters()
    {
      if (config?.NoRandomMonsters ?? false) {
        log?.Silly("Denying random flying monster spawn");
        return false;
      }
      return true;
    }
  }
}
