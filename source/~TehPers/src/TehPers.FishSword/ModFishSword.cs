/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using TehPers.FishSword.Integrations.DynamicGameAssets;

namespace TehPers.FishSword
{
    public class ModFishSword : Mod
    {
        private const string swordSwipeSound = "swordswipe";
        private const string fishSwordDgaId = "TehPers.FishSword.DGA/FishSword";

        private static ModFishSword? instance;
        private static int queuedSwipes;
        private static int playedSwipes;

        private static readonly Lazy<IDynamicGameAssetsApi?> dgaApi = new(
            () => ModFishSword.instance?.Helper.ModRegistry.GetApi<IDynamicGameAssetsApi>(
                "spacechase0.DynamicGameAssets"
            )
        );

        public override void Entry(IModHelper helper)
        {
            if (ModFishSword.instance is null)
            {
                ModFishSword.instance = this;
            }
            else
            {
                this.Monitor.Log("Mod was somehow initialized multiple times.", LogLevel.Error);
                return;
            }

            // Apply patches
            var harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.Patch(
                AccessTools.Method(typeof(MeleeWeapon), nameof(MeleeWeapon.setFarmerAnimating)),
                prefix: new(
                    AccessTools.Method(
                        typeof(ModFishSword),
                        nameof(ModFishSword.MeleeWeapon_setFarmerAnimating_Prefix)
                    )
                ),
                postfix: new(
                    AccessTools.Method(
                        typeof(ModFishSword),
                        nameof(ModFishSword.MeleeWeapon_setFarmerAnimating_Postfix)
                    )
                )
            );
            harmony.Patch(
                AccessTools.Method(typeof(Game1), nameof(Game1.playSound)),
                prefix: new(
                    AccessTools.Method(
                        typeof(ModFishSword),
                        nameof(ModFishSword.Game1_playSound_Prefix)
                    )
                )
            );
        }

        // ReSharper disable once InconsistentNaming
        private static void MeleeWeapon_setFarmerAnimating_Prefix(MeleeWeapon __instance)
        {
            // Get DGA API
            if (ModFishSword.dgaApi.Value is not { } dgaApi)
            {
                return;
            }

            // Check if it's a fish sword
            if (dgaApi.GetDGAItemId(__instance) is not ModFishSword.fishSwordDgaId)
            {
                return;
            }

            ModFishSword.queuedSwipes += 1;
        }

        // ReSharper disable once InconsistentNaming
        private static void MeleeWeapon_setFarmerAnimating_Postfix(MeleeWeapon __instance)
        {
            // Get DGA API
            if (ModFishSword.dgaApi.Value is not { } dgaApi)
            {
                return;
            }

            // Check if it's a fish sword
            if (dgaApi.GetDGAItemId(__instance) is not ModFishSword.fishSwordDgaId)
            {
                return;
            }

            if (ModFishSword.playedSwipes > 0)
            {
                // Swipe was played
                ModFishSword.playedSwipes = Math.Max(ModFishSword.playedSwipes - 1, 0);
            }
            else
            {
                // Swipe was not played for some reason
                ModFishSword.queuedSwipes -= Math.Max(ModFishSword.queuedSwipes - 1, 0);
            }
        }

        private static void Game1_playSound_Prefix(ref string cueName)
        {
            // Check if it's the sword swipe sound
            if (!string.Equals(
                    cueName,
                    ModFishSword.swordSwipeSound,
                    StringComparison.OrdinalIgnoreCase
                ))
            {
                return;
            }

            // Check if it should be overridden
            if (ModFishSword.queuedSwipes <= 0)
            {
                return;
            }

            // Replace the sound
            cueName = "fishSlap";
            ModFishSword.queuedSwipes -= 1;
            ModFishSword.playedSwipes += 1;
        }
    }
}
