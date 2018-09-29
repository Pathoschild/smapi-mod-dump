using DsStardewLib.Config;
using DsStardewLib.Utils;
using Harmony;
using StardewValley;
using StardewValley.Locations;
using System;

namespace FishingAutomaton.Lib.HarmonyHacks
{
  /// <summary>
  /// The mine shaft has its own trash check for lava levels.  However it's a short enough function (unlike gamelocation getfish)
  /// that we can just loop over it until it gives us that desired lava eel.
  /// </summary>
  [HarmonyPatch(typeof(MineShaft), "getFish", new Type[] { typeof(float), typeof(int), typeof(int), typeof(Farmer), typeof(double) })]
  class NoTrashLavaMineHack : HarmonyHack
  {
    // Variables to do the business, and also to meet the Hack interface.
    private static ModConfig config = null;
    private static Logger log = null;
    public Logger Log { get => NoTrashLavaMineHack.log; set => NoTrashLavaMineHack.log = value; }
    public HarmonyConfig Config { get => NoTrashLavaMineHack.config; set => NoTrashLavaMineHack.config = value as ModConfig; }

    /// <summary>
    /// If trash is returned from here, we're in the lava mines (else the skip trash in gamelocation getfish would have triggered
    /// already), so just run the function again until we get the fish.  We could just load the fish itself, but this removes
    /// the need to load or hardcode the fish ID.
    /// </summary>
    /// <param name="__result">Harmony var - the result of the original call as a reference so we can set it.</param>
    /// <param name="__instance">Harmony var - the instance of the original MineShaft, so we can call getFish</param>
    /// <param name="millisecondsAfterNibble">Harmony var - original arg</param>
    /// <param name="bait">Harmony var - original arg</param>
    /// <param name="waterDepth">Harmony var - original arg</param>
    /// <param name="who">Harmony var - original arg</param>
    /// <param name="baitPotency">Harmony var - original arg</param>
    [HarmonyPostfix]
    static void CheckFish(ref StardewValley.Object __result, MineShaft __instance,
                          float millisecondsAfterNibble, int bait, int waterDepth, Farmer who, double baitPotency)
    {
      if ((bool)config?.noTrash) {
        while (__instance.getMineArea(-1) == 80 && (__result.ParentSheetIndex >= 167 && __result.ParentSheetIndex < 173)) {
          log.Silly("Lava trash returned, looping for fish");
          __result = __instance.getFish(millisecondsAfterNibble, bait, waterDepth, who, baitPotency);
        }
      }
    }
  }
}
