/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using System;
using System.Diagnostics.CodeAnalysis;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoreEnchantments.Enchantments;
using Spacechase.Shared.Harmony;
using SpaceShared;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Tools;

namespace MoreEnchantments.Patches
{
    /// <summary>Applies Harmony patches to <see cref="FishingRod"/>.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = DiagnosticMessages.NamedForHarmony)]
    internal class FishingRodPatcher : BasePatcher
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public override void Apply(HarmonyInstance harmony, IMonitor monitor)
        {
            harmony.Patch(
                original: this.RequireMethod<FishingRod>(nameof(FishingRod.attachmentSlots)),
                postfix: this.GetHarmonyMethod(nameof(After_AttachmentSlots))
            );

            harmony.Patch(
                original: this.RequireMethod<FishingRod>("calculateTimeUntilFishingBite"),
                prefix: this.GetHarmonyMethod(nameof(Before_CalculateTimeUntilFishingBite))
            );

            harmony.Patch(
                original: this.RequireMethod<FishingRod>(nameof(FishingRod.attach)),
                prefix: this.GetHarmonyMethod(nameof(Before_Attach))
            );

            harmony.Patch(
                original: this.RequireMethod<FishingRod>(nameof(FishingRod.drawAttachments)),
                postfix: this.GetHarmonyMethod(nameof(After_DrawAttachments))
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method to call after <see cref="FishingRod.attachmentSlots"/>.</summary>
        private static void After_AttachmentSlots(FishingRod __instance, ref int __result)
        {
            if (__instance.hasEnchantmentOfType<MoreLuresEnchantment>())
                ++__result;
        }

        /// <summary>The method to call before <see cref="FishingRod.calculateTimeUntilFishingBite"/>.</summary>
        private static bool Before_CalculateTimeUntilFishingBite(FishingRod __instance, Vector2 bobberTile, bool isFirstCast, Farmer who, ref float __result)
        {
            if (Game1.currentLocation.isTileBuildingFishable((int)bobberTile.X, (int)bobberTile.Y) && Game1.currentLocation is BuildableGameLocation)
            {
                Building bldg = (Game1.currentLocation as BuildableGameLocation).getBuildingAt(bobberTile);
                if (bldg is FishPond pond && pond.currentOccupants.Value > 0)
                {
                    __result = FishPond.FISHING_MILLISECONDS;
                    return false;
                }
            }
            int b = FishingRod.maxFishingBiteTime - 250 * who.FishingLevel;
            int c = 0;
            if (__instance.attachments[1] != null && __instance.attachments[1].ParentSheetIndex == 686 ||
                 __instance.hasEnchantmentOfType<MoreLuresEnchantment>() &&
                 __instance.attachments[2] != null && __instance.attachments[2].ParentSheetIndex == 686)
            {
                c += 5000;
            }
            if (__instance.attachments[1] != null && __instance.attachments[1].ParentSheetIndex == 687 ||
                      __instance.hasEnchantmentOfType<MoreLuresEnchantment>() &&
                      __instance.attachments[2] != null && __instance.attachments[2].ParentSheetIndex == 687)
            {
                c += 10000;
            }
            b -= c;

            float time = Game1.random.Next(FishingRod.minFishingBiteTime, b);
            if (Mod.Instance.Helper.Reflection.GetField<bool>(__instance, "isFirstCast").GetValue())
            {
                time *= 0.75f;
            }
            if (__instance.attachments[0] != null)
            {
                time *= 0.5f;
                if (__instance.attachments[0].ParentSheetIndex == 774)
                {
                    time *= 0.75f;
                }
            }
            __result = Math.Max(500f, time);
            return false;
        }

        /// <summary>The method to call before <see cref="FishingRod.attach"/>.</summary>
        private static bool Before_Attach(FishingRod __instance, StardewValley.Object o, ref StardewValley.Object __result)
        {
            if (o != null && o.Category == -21 && __instance.UpgradeLevel > 1)
            {
                StardewValley.Object tmp = __instance.attachments[0];
                if (tmp != null && tmp.canStackWith(o))
                {
                    tmp.Stack = o.addToStack(tmp);
                    if (tmp.Stack <= 0)
                    {
                        tmp = null;
                    }
                }
                __instance.attachments[0] = o;
                Game1.playSound("button1");
                __result = tmp;
                return false;
            }
            if (o != null && o.Category == -22 && __instance.UpgradeLevel > 2)
            {
                // Rewrote this portion
                bool hasEnchant = __instance.hasEnchantmentOfType<MoreLuresEnchantment>();
                if (__instance.attachments[1] == null)
                {
                    __instance.attachments[1] = o;
                }
                else if (__instance.attachments[2] == null && hasEnchant)
                {
                    __instance.attachments[2] = o;
                }
                else if (__instance.attachments[2] != null && hasEnchant)
                {
                    __result = __instance.attachments[2];
                    __instance.attachments[2] = o;
                }
                else if (__instance.attachments[1] != null)
                {
                    __result = __instance.attachments[1];
                    __instance.attachments[1] = o;
                }
                Game1.playSound("button1");
                return false;
            }
            if (o == null)
            {
                if (__instance.attachments[0] != null)
                {
                    StardewValley.Object result2 = __instance.attachments[0];
                    __instance.attachments[0] = null;
                    Game1.playSound("dwop");
                    __result = result2;
                    return false;
                }
                if (__instance.attachments[2] != null)
                {
                    StardewValley.Object result3 = __instance.attachments[2];
                    __instance.attachments[2] = null;
                    Game1.playSound("dwop");
                    __result = result3;
                    return false;
                }
                if (__instance.attachments[1] != null)
                {
                    StardewValley.Object result3 = __instance.attachments[1];
                    __instance.attachments[1] = null;
                    Game1.playSound("dwop");
                    __result = result3;
                    return false;
                }
            }
            __result = null;
            return false;
        }

        /// <summary>The method to call before <see cref="FishingRod.drawAttachments"/>.</summary>
        private static void After_DrawAttachments(FishingRod __instance, SpriteBatch b, int x, int y)
        {
            if (__instance.UpgradeLevel > 2 && __instance.hasEnchantmentOfType<MoreLuresEnchantment>())
            {
                if (__instance.attachments[2] == null)
                {
                    b.Draw(Game1.menuTexture, new Vector2(x, y + 64 + 4 + 64 + 4), Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 37), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.86f);
                    return;
                }
                b.Draw(Game1.menuTexture, new Vector2(x, y + 64 + 4 + 64 + 4), Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.86f);
                __instance.attachments[2].drawInMenu(b, new Vector2(x, y + 64 + 4 + 64 + 4), 1f);
            }
        }
    }
}
