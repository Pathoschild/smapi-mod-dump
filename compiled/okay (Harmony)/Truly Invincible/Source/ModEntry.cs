using System;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace TrulyInvincible
{
  /// <summary>The mod entry point.</summary>
  public class ModEntry : Mod
  {
    /*********
    ** Public methods
    *********/
    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
      var harmony = new Harmony(this.ModManifest.UniqueID);

      harmony.Patch(
         original: AccessTools.Method(typeof(StardewValley.Farmer), nameof(StardewValley.Farmer.takeDamage)),
         prefix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.TakeDamage_Prefix))
      );

      harmony.Patch(
        original: AccessTools.Method(typeof(StardewValley.Locations.MineShaft), nameof(StardewValley.Locations.MineShaft.enterMineShaft)),
        prefix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.enterMineShaft_Prefix)),
        postfix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.enterMineShaft_Postfix))
      );
    }
  }
}