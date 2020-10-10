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
using StardewValley;

namespace FishingAutomaton.Lib.HarmonyHacks
{
  /// <summary>
  /// Harmony class to patch the 'didPlayerJustClickAtAll' function
  /// </summary>
  [HarmonyPatch(typeof(Game1), "didPlayerJustClickAtAll")]
  class ClickAtAllHack : HarmonyHack
  {
    // Variables to do the business, and also to meet the Hack interface.
    private static ModConfig config = null;
    private static Logger log = null;
    public Logger Log { get => ClickAtAllHack.log; set => ClickAtAllHack.log = value; }
    public HarmonyConfig Config { get => ClickAtAllHack.config; set => ClickAtAllHack.config = value as ModConfig; }

    /// <summary>
    /// Whether the function should simulate the user clicking.
    /// </summary>
    public static bool simulateClick = false;
 
    /// <summary>
    /// A Harmony Postfix function that forces the didPlayerJustClickAtAll function
    /// to return true if the member variable <code>simulateclick</code> is set
    /// to true.
    /// </summary>
    /// <param name="__result">Harmony will inject this reference to the original return value</param>
    [HarmonyPostfix]
    static void YesPlayerDidClick(ref bool __result)
    {
      if (simulateClick) {
        log?.Silly("Forcing a click");
        __result = true;
      }
    }
  }

  /// <summary>
  /// Harmony class to patch the 'isOneOfTheseKeysDown' function.  This is specifically needed
  /// to simulate the bobber bar in the fishing minigame being controlled by an actual user.
  /// The check for the useTool button is to prevent other things (like that chat box) from
  /// also being activated at the same time as the fishing game.
  /// </summary>
  [HarmonyPatch(typeof(Game1), "isOneOfTheseKeysDown")]
  class IsButtonDownHack : HarmonyHack
  {
    // Variables to do the business, and also to meet the Hack interface.
    private static ModConfig config = null;
    private static Logger log = null;
    public Logger Log { get => IsButtonDownHack.log; set => IsButtonDownHack.log = value; }
    public HarmonyConfig Config { get => IsButtonDownHack.config; set => IsButtonDownHack.config = value as ModConfig; }

    /// <summary>
    /// Whether the function should simulate the user holding a button down.
    /// </summary>
    public static bool simulateDown = false;

    /// <summary>
    /// A Harmony Postfix function that forces the isOneOfTheseKeysDown function
    /// to return true if the member variable <code>simulateDown</code> is set
    /// to true and the key being checked is the use tool button.
    /// </summary>
    /// <param name="__result">Harmony will inject this reference to the original return value</param>
    [HarmonyPostfix]
    static void YesButtonIsDown(ref bool __result, InputButton[] keys)
    {
      if (simulateDown) {
        foreach (InputButton key in keys) {
          foreach (InputButton key2 in Game1.options.useToolButton) {
            if (key.key == key2.key) {
              log?.Silly("Forcing use tool button down");
              __result = true;
              return;
            }
          }
        }
      }
    }
  }
}
