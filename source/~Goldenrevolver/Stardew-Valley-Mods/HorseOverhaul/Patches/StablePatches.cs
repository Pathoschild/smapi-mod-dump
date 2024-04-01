/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace HorseOverhaul.Patches
{
    using HarmonyLib;
    using Microsoft.Xna.Framework.Graphics;
    using StardewModdingAPI;
    using StardewValley;
    using StardewValley.Buildings;
    using StardewValley.Objects;
    using StardewValley.Tools;
    using System;
    using System.Linq;

    internal class StableAndSaddleBagPatches
    {
        private static HorseOverhaul mod;

        internal static void ApplyPatches(HorseOverhaul horseOverhaul, Harmony harmony)
        {
            mod = horseOverhaul;

            harmony.Patch(
               original: AccessTools.Method(typeof(Chest), nameof(Chest.draw), new Type[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }),
               prefix: new HarmonyMethod(typeof(StableAndSaddleBagPatches), nameof(DoNothingIfSaddleBag)));

            harmony.Patch(
               original: AccessTools.Method(typeof(Chest), nameof(Chest.draw), new Type[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float), typeof(bool) }),
               prefix: new HarmonyMethod(typeof(StableAndSaddleBagPatches), nameof(DoNothingIfSaddleBag)));

            harmony.Patch(
               original: AccessTools.Method(typeof(Chest), nameof(Chest.performToolAction)),
               prefix: new HarmonyMethod(typeof(StableAndSaddleBagPatches), nameof(DoNothingIfSaddleBag)));

            harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performToolAction)),
               postfix: new HarmonyMethod(typeof(StableAndSaddleBagPatches), nameof(CheckForWaterHit)));

            harmony.Patch(
               original: AccessTools.Method(typeof(Stable), nameof(Stable.performActionOnDemolition)),
               prefix: new HarmonyMethod(typeof(StableAndSaddleBagPatches), nameof(SaveItemsFromDemolition)));

            harmony.Patch(
               original: AccessTools.Method(typeof(Utility), nameof(Utility.iterateChestsAndStorage)),
               prefix: new HarmonyMethod(typeof(StableAndSaddleBagPatches), nameof(IterateOverSaddles)));

            harmony.Patch(
               original: AccessTools.Method(typeof(Building), nameof(Building.resetTexture)),
               postfix: new HarmonyMethod(typeof(StableAndSaddleBagPatches), nameof(ResetStableTexture)));
        }

        public static bool DoNothingIfSaddleBag(Chest __instance)
        {
            return !__instance?.modData?.ContainsKey($"{mod.ModManifest.UniqueID}/isSaddleBag") == true;
        }

        public static void ResetStableTexture(Building __instance)
        {
            if (__instance is not Stable stable || stable.IsTractorGarage() || !mod.Config.Water || mod.Config.DisableStableSpriteChanges || mod.UsingIncompatibleTextures)
            {
                return;
            }

            __instance.texture = new Lazy<Texture2D>(CreateUpdatedStableTexture(stable));
        }

        internal static Texture2D CreateUpdatedStableTexture(Stable stable)
        {
            if (stable.paintedTexture != null)
            {
                stable.paintedTexture.Dispose();
                stable.paintedTexture = null;
            }

            string text = stable.textureName();
            Texture2D texture2D;

            try
            {
                texture2D = Game1.content.Load<Texture2D>(text);
            }
            catch
            {
                return Game1.content.Load<Texture2D>("Buildings\\Error");
            }

            bool isTroughFull = stable?.modData?.ContainsKey($"{mod.ModManifest.UniqueID}/gotWater") == true;

            if (isTroughFull)
            {
                texture2D = mod.FilledTroughTexture;
            }
            else
            {
                texture2D = mod.EmptyTroughTexture;
            }

            stable.paintedTexture = BuildingPainter.Apply(texture2D, text + "_PaintMask", stable.netBuildingPaintColor.Value);

            if (stable.paintedTexture != null)
            {
                texture2D = stable.paintedTexture;
            }

            return texture2D;
        }

        public static void IterateOverSaddles(Action<Item> action)
        {
            var farmItems = Game1.getFarm().Objects.Values;

            // do this even if saddle bags are disabled
            foreach (var horse in mod.Horses)
            {
                // check if it is placed on the farm, then it was checked already from the overridden method
                if (horse != null && horse.SaddleBag != null && !farmItems.Contains(horse.SaddleBag))
                {
                    foreach (Item item in horse.SaddleBag.Items)
                    {
                        if (item != null)
                        {
                            action(item);
                        }
                    }
                }
            }
        }

        public static void CheckForWaterHit(GameLocation __instance, Tool t, int tileX, int tileY)
        {
            if (__instance is not Farm)
            {
                return;
            }

            if (!Context.IsWorldReady || !mod.Config.Water)
            {
                return;
            }

            if (t is not WateringCan can || can.WaterLeft <= 0)
            {
                return;
            }

            foreach (Building building in __instance.buildings)
            {
                if (building is Stable stable && !stable.IsTractorGarage())
                {
                    bool doesXHit = stable.tileX.Value + 1 == tileX || stable.tileX.Value + 2 == tileX;

                    if (doesXHit && stable.tileY.Value == tileY)
                    {
                        mod.Horses.Where(h => h?.Stable?.HorseId == stable.HorseId).Do(h => h.JustGotWater());
                    }
                }
            }
        }

        public static void SaveItemsFromDemolition(Stable __instance)
        {
            if (__instance.IsTractorGarage() || !Context.IsMainPlayer)
            {
                return;
            }

            var horseW = mod.Horses.Where(h => h?.Stable?.HorseId == __instance.HorseId).FirstOrDefault();

            if (horseW == null || horseW.SaddleBag == null)
            {
                return;
            }

            if (horseW.SaddleBag.Items.Count > 0)
            {
                foreach (var item in horseW.SaddleBag.Items)
                {
                    Game1.player.team.returnedDonations.Add(item);
                    Game1.player.team.newLostAndFoundItems.Value = true;
                }

                horseW.SaddleBag.Items.Clear();
            }

            Game1.getFarm().Objects.Remove(horseW.SaddleBag.TileLocation);

            if (__instance.modData.ContainsKey($"{mod.ModManifest.UniqueID}/stableID"))
            {
                __instance.modData.Remove($"{mod.ModManifest.UniqueID}/stableID");
            }
        }
    }
}