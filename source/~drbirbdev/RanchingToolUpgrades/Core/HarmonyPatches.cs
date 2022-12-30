/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using BirbShared;
using System.Reflection;
using System.Reflection.Emit;
using StardewValley.Tools;

namespace RanchingToolUpgrades
{
    [HarmonyPatch(typeof(Utility), nameof(Utility.getBlacksmithUpgradeStock))]
    class Utility_GetBlacksmithUpgradeStock
    {
        public static void Postfix(
            Dictionary<ISalable, int[]> __result,
            Farmer who)
        {
            try
            {
                UpgradeablePail.AddToShopStock(itemPriceAndStock: __result, who: who);
                UpgradeableShears.AddToShopStock(itemPriceAndStock: __result, who: who);
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
        }
    }

    [HarmonyPatch(typeof(Farmer), nameof(Farmer.showHoldingItem))]
    class Farmer_ShowHoldingItem
    {
        public static bool Prefix(
            Farmer who)
        {
            try
            {
                Item mrg = who.mostRecentlyGrabbedItem;
                if (mrg is UpgradeablePail || mrg is UpgradeableShears)
                {
                    Rectangle r;
                    if (mrg is UpgradeablePail)
                    {
                        r = UpgradeablePail.IconSourceRectangle((who.mostRecentlyGrabbedItem as Tool).UpgradeLevel);
                    }
                    else
                    {
                        r = UpgradeableShears.IconSourceRectangle((who.mostRecentlyGrabbedItem as Tool).UpgradeLevel);
                    }
                    Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(
                        textureName: ModEntry.Assets.SpritesPath,
                        sourceRect: r,
                        animationInterval: 2500f,
                        animationLength: 1,
                        numberOfLoops: 0,
                        position: who.Position + new Vector2(0f, -124f),
                        flicker: false,
                        flipped: false,
                        layerDepth: 1f,
                        alphaFade: 0f,
                        color: Color.White,
                        scale: 4f,
                        scaleChange: 0f,
                        rotation: 0f,
                        rotationChange: 0f)
                    {
                        motion = new Vector2(0f, -0.1f)
                    });
                    return false;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Utility), nameof(Utility.getAnimalShopStock))]
    class Utility_GetAnimalShopStock
    {
        public static void Postfix(
            Dictionary<ISalable, int[]> __result
            )
        {
            try
            {
                // Keying off of `new MilkPail()` or `new Shears()` doesn't work.
                // Iterate over items for sale, and remove any by the name "Milk Pail" or "Shears"
                foreach (ISalable key in __result.Keys)
                {
                    if (key.Name.Equals("Milk Pail") || key.Name.Equals("Shears"))
                    {
                        __result.Remove(key);
                    }
                }
                if (Game1.player.hasItemWithNameThatContains("Pail") == null)
                {
                    __result.Add(new UpgradeablePail(0), new int[2] { ModEntry.Config.PailBuyCost, 1 });
                }
                if (Game1.player.hasItemWithNameThatContains("Shears") == null)
                {
                    __result.Add(new UpgradeableShears(0), new int[2] { ModEntry.Config.ShearsBuyCost, 1 });
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
        }
    }

    // 3rd party
    // Allow sending tools to upgrade in the mail with Mail Services
    [HarmonyPatch("MailServicesMod.ToolUpgradeOverrides", "mailbox")]
    class MailServicesMod_ToolUpgradeOverrides_Mailbox
    {
        public static bool Prepare()
        {
            return ModEntry.Instance.Helper.ModRegistry.IsLoaded("Digus.MailServicesMod");
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var code = new List<CodeInstruction>(instructions);

            for (int i = 0; i < code.Count; i++)
            {
                if (code[i].Is(OpCodes.Isinst, typeof(Axe)))
                {
                    yield return new CodeInstruction(OpCodes.Isinst, typeof(UpgradeablePail));
                    yield return code[i + 1];
                    yield return code[i + 2];
                    yield return code[i + 3];
                    yield return new CodeInstruction(OpCodes.Isinst, typeof(UpgradeableShears));
                    yield return code[i + 1];
                    yield return code[i + 2];
                    yield return code[i + 3];
                    yield return code[i];
                }
                else
                {
                    yield return code[i];
                }
            }
        }
    }
}
