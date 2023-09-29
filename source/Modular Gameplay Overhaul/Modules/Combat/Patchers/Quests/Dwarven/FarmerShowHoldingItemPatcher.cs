/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Patchers.Quests.Dwarven;

#region using directives

using System.Reflection;
using DaLion.Overhaul.Modules.Combat.Integrations;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerShowHoldingItemPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerShowHoldingItemPatcher"/> class.</summary>
    internal FarmerShowHoldingItemPatcher()
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.showHoldingItem));
    }

    #region harmony patches

    /// <summary>Show Dwarvish Blueprint on obtain.</summary>
    [HarmonyPrefix]
    private static bool FarmerShowHoldingItemPrefix(Farmer who)
    {
        try
        {
            if (!JsonAssetsIntegration.DwarvishBlueprintIndex.HasValue ||
                who.mostRecentlyGrabbedItem?.ParentSheetIndex != JsonAssetsIntegration.DwarvishBlueprintIndex.Value)
            {
                return true; // run original logic
            }

            Game1.currentLocation.temporarySprites.Add(
                new TemporaryAnimatedSprite(
                    "LooseSprites\\Cursors",
                    new Rectangle(420, 489, 25, 18),
                    2500f,
                    1,
                    0,
                    who.Position + new Vector2(-20f, -152f),
                    flicker: false,
                    flipped: false) { motion = new Vector2(0f, -0.1f), scale = 4f, layerDepth = 1f, });
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
