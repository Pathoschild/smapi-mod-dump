/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ncarigon/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.GameData;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using SObject = StardewValley.Object;

namespace GardenPotOptions {
    internal static class Patches {
        public static void Register() {
            if (ModEntry.Instance?.Helper is not null) {
                var harmony = new Harmony(ModEntry.Instance!.Helper.ModContent.ModID);
                harmony.Patch(
                    original: AccessTools.Method(typeof(Pickaxe), "DoFunction"),
                    prefix: new HarmonyMethod(typeof(Patches), nameof(Prefix_Pickaxe_DoFunction))
                );
                harmony.Patch(
                    original: AccessTools.Method(typeof(Axe), "DoFunction"),
                    prefix: new HarmonyMethod(typeof(Patches), nameof(Prefix_Axe_DoFunction))
                );
                harmony.Patch(
                    original: AccessTools.Method(typeof(Hoe), "DoFunction"),
                    prefix: new HarmonyMethod(typeof(Patches), nameof(Prefix_Hoe_DoFunction))
                );
                harmony.Patch(
                    original: AccessTools.Method(typeof(Utility), "tryToPlaceItem"),
                    prefix: new HarmonyMethod(typeof(Patches), nameof(Prefix_Utility_tryToPlaceItem))
                );
                harmony.Patch(
                    original: AccessTools.Method(typeof(Bush), "performUseAction"),
                    postfix: new HarmonyMethod(typeof(Patches), nameof(Postfix_Bush_performUseAction))
                );
                harmony.Patch(
                    original: AccessTools.Method(typeof(Tree), "performUseAction"),
                    postfix: new HarmonyMethod(typeof(Patches), nameof(Postfix_Tree_performUseAction))
                );
                harmony.Patch(
                    original: AccessTools.Method(typeof(FruitTree), "performUseAction"),
                    postfix: new HarmonyMethod(typeof(Patches), nameof(Postfix_FruitTree_performUseAction))
                );
                harmony.Patch(
                    original: AccessTools.Method(typeof(SObject), "placementAction"),
                    postfix: new HarmonyMethod(typeof(Patches), nameof(Postfix_Object_placementAction))
                );
                harmony.Patch(
                    original: AccessTools.Method(typeof(SObject), "loadDisplayName"),
                    postfix: new HarmonyMethod(typeof(Patches), nameof(Postfix_Object_loadDisplayName))
                );
                harmony.Patch(
                    original: AccessTools.Method(typeof(SObject), "maximumStackSize"),
                    postfix: new HarmonyMethod(typeof(Patches), nameof(Postfix_Object_maximumStackSize))
                );
                harmony.Patch(
                    original: AccessTools.Method(typeof(SObject), "ApplySprinkler"),
                    postfix: new HarmonyMethod(typeof(Patches), nameof(Postfix_Object_ApplySprinkler))
                );
                harmony.Patch(
                    original: AccessTools.Method(typeof(PlantableRule), "ShouldApplyWhen"),
                    prefix: new HarmonyMethod(typeof(Patches), nameof(Prefix_PlantableRule_ShouldApplyWhen))
                );
            }
        }

        private static bool IsGardenPot(Item? item) => item?.QualifiedItemId?.Equals("(BC)62") ?? false;

        private static void Prefix_Tool_DoFunction(GameLocation location, int x, int y, Farmer who) {
            try {
                if (ModEntry.Instance?.ModConfig?.KeepContents ?? false) {
                    var tile = new Vector2(x / 64, y / 64);
                    if ((location?.Objects?.TryGetValue(tile, out var obj) ?? false)
                        && obj is IndoorPot pot
                        && BetterIndoorPot.TryTransplant(pot, out var bip)
                        && who is not null
                    ) {
                        TryRemove<IndoorPot>(location, tile);
                        pot!.performRemoveAction();
                        if (bip.IsFilled()) {
                            location.debris?.Add(new Debris(bip, who.GetToolLocation(), who.GetBoundingBox().Center.ToVector2()));
                        } else {
                            Game1.createItemDebris(new SObject(tile, "62"), Game1.player.getStandingPosition(), Game1.player.FacingDirection);
                        }
                    }
                }
            } catch { }
        }

        private static void Prefix_Pickaxe_DoFunction(GameLocation location, int x, int y, Farmer who) {
            if ((ModEntry.Instance?.ModConfig?.KeepContents ?? false) && "Pickaxe" == ModEntry.Instance?.ModConfig?.SafeTool) {
                Prefix_Tool_DoFunction(location, x, y, who);
            }
        }

        private static void Prefix_Axe_DoFunction(GameLocation location, int x, int y, Farmer who) {
            if ((ModEntry.Instance?.ModConfig?.KeepContents ?? false) && "Axe" == ModEntry.Instance?.ModConfig?.SafeTool) {
                Prefix_Tool_DoFunction(location, x, y, who);
            }
        }

        private static void Prefix_Hoe_DoFunction(GameLocation location, int x, int y, Farmer who) {
            if ((ModEntry.Instance?.ModConfig?.KeepContents ?? false) && "Hoe" == ModEntry.Instance?.ModConfig?.SafeTool) {
                Prefix_Tool_DoFunction(location, x, y, who);
            }
        }

        private static void Postfix_Bush_performUseAction(
            Bush __instance
        ) {
            if ((ModEntry.Instance?.ModConfig?.AllowTransplant ?? false) // can transplant
                && (__instance?.size?.Value ?? 0) == 3 // must be a tea sapling (or modded?)
                && IsGardenPot(Game1.player?.ActiveItem) // player is holding a garden pot
                && Game1.player?.ActiveItem is not BetterIndoorPot // which has not been placed yet
            ) {
                var bip = new BetterIndoorPot(__instance);

                // remove empty garden pot from inventory
                Game1.player?.reduceActiveItemByOne();

                // drop full garden pot
                Game1.createItemDebris(bip, Game1.player?.getStandingPosition() ?? Vector2.Zero, Game1.player?.FacingDirection ?? 0);
            }
        }

        private static void Postfix_Tree_performUseAction(
            Tree __instance
        ) {
            if ((ModEntry.Instance?.ModConfig?.AllowTransplant ?? false) // can transplant
                && IsGardenPot(Game1.player?.ActiveItem) // player is holding a garden pot
                && Game1.player?.ActiveItem is not BetterIndoorPot // which has not been placed yet
                && __instance.growthStage.Value <= (ModEntry.Instance?.ModConfig?.TreeTransplantMax ?? -1)
            ) {
                var bip = new BetterIndoorPot(__instance);

                // remove empty garden pot from inventory
                Game1.player?.reduceActiveItemByOne();

                // drop full garden pot
                Game1.createItemDebris(bip, Game1.player?.getStandingPosition() ?? Vector2.Zero, Game1.player?.FacingDirection ?? 0);
            }
        }

        private static void Postfix_FruitTree_performUseAction(
            FruitTree __instance
        ) {
            if ((ModEntry.Instance?.ModConfig?.AllowTransplant ?? false) // can transplant
                && IsGardenPot(Game1.player?.ActiveItem) // player is holding a garden pot
                && Game1.player?.ActiveItem is not BetterIndoorPot // which has not been placed yet
                && __instance.growthStage.Value <= (ModEntry.Instance?.ModConfig?.FruitTreeTransplantMax ?? -1)
            ) {
                var bip = new BetterIndoorPot(__instance);

                // remove empty garden pot from inventory
                Game1.player?.reduceActiveItemByOne();

                // drop full garden pot
                Game1.createItemDebris(bip, Game1.player?.getStandingPosition() ?? Vector2.Zero, Game1.player?.FacingDirection ?? 0);
            }
        }

        private static string Test(this bool? b) => b.HasValue ? b.Value.ToString() : "null";

        private static void Prefix_Utility_tryToPlaceItem(
            GameLocation location, ref Item? item, int x, int y
        ) {
            if ((ModEntry.Instance?.ModConfig?.AllowTransplant ?? false) // can transpant
                && IsGardenPot(item)) { // using garden pot
                bool? isDirt, isBush, potEmpty, potCrop, potBush, dirtCrop, tileEmpty, isTree, potTree;

                var tile = new Vector2(x / 64, y / 64);
                HoeDirt? dirt = null;
                BetterIndoorPot? bip = item as BetterIndoorPot;
                potEmpty = bip is null;
                isDirt = (location?.terrainFeatures?.TryGetValue(tile, out var h) ?? false) && (dirt = h as HoeDirt) is not null;
                isBush = (location?.terrainFeatures?.TryGetValue(tile, out var b) ?? false) && b is Bush bush && bush is not null && (bush?.size?.Value ?? 0) == 3;
                isTree = (location?.terrainFeatures?.TryGetValue(tile, out var t) ?? false) && ((t is Tree tree && tree is not null) || (t is FruitTree fruit && fruit is not null));
                tileEmpty = location?.isTilePlaceable(tile) ?? false;
                dirtCrop = dirt?.crop is not null || dirt?.fertilizer?.Value is not null;
                potCrop = bip?.hoeDirt?.Value?.crop is not null || bip?.hoeDirt?.Value?.fertilizer?.Value is not null;
                potBush = bip?.bush?.Value is not null;
                potTree = bip?.tree is not null;

                string? deniedMsg = null;
                var disableCall = false;
                if ((isDirt ?? false) && (dirtCrop ?? false) && (tileEmpty ?? false) && (potCrop ?? false)) { // is dirt with fertilizer, potted crop
                    disableCall = true; // disable to prevent pot trampling dirt
                } else if ((isDirt ?? false) && !(dirtCrop ?? false) && (potTree ?? false)) {
                    disableCall = true;
                } else if ((isDirt ?? false) && !(dirtCrop ?? false) && (potCrop ?? false)) { // is dirt but no crop, potted crop
                    if (bip!.hoeDirt?.Value?.crop is null
                        || (location?.CanPlantSeedsHere(bip!.hoeDirt.Value.crop.netSeedIndex.Value, x, y, false, out deniedMsg) ?? false)
                    ) {
                        bip!.TransplantOut(dirt);

                        // remove from inventory
                        Game1.player.reduceActiveItemByOne();

                        // drop new garden pot
                        Game1.createItemDebris(new SObject(tile, "62"), Game1.player.getStandingPosition(), Game1.player.FacingDirection);
                    } else if (deniedMsg is not null) {
                        Game1.showRedMessage(deniedMsg);
                    }
                    disableCall = true;
                } else if ((isDirt ?? false) && (dirtCrop ?? false) && (potEmpty ?? false)) { // is dirt with crop, empty pot
                    if (dirt?.crop is null || (location?.CheckItemPlantRules(dirt.crop.netSeedIndex.Value, true, true, out deniedMsg) ?? false)) {
                        // create new pot
                        bip = new BetterIndoorPot(dirt);

                        // remove empty garden pot
                        Game1.player.reduceActiveItemByOne();

                        // drop full garden pot
                        Game1.createItemDebris(bip, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
                    } else if (deniedMsg is not null) {
                        Game1.showRedMessage(deniedMsg);
                    }
                    disableCall = true;
                } else if (((isBush ?? false) && (potEmpty ?? false)) // bush tile and empty pot
                    || ((isTree ?? false) && (potTree ?? false)) // tree tile and empty pot
                ) { 
                    // handled about in Bush/Tree/FruitTree.performUseAction
                } else if ((tileEmpty ?? false) && ((potBush ?? false) || (potTree ?? false))) { // empty tile and potted bush or tree
                    if (((potBush ?? false) && (new SObject("251", 1)?.placementAction(location, x, y) ?? false))
                        || ((potTree ?? false) && (new SObject(bip!.GetTreeSeedType, 1)?.placementAction(location, x, y) ?? false))
                    ) {
                        // remove temp object
                        location?.terrainFeatures.Remove(tile);

                        // place real object
                        bip!.TransplantOut(location, tile);

                        // remove from inventory
                        Game1.player.reduceActiveItemByOne();

                        // drop new pot
                        Game1.createItemDebris(new SObject(tile, "62"), Game1.player.getStandingPosition(), Game1.player.FacingDirection);
                    }
                    disableCall = true;
                }

                if (disableCall) {
                    // prevent original call
                    item = null;
                }
            }
        }

        private static void TryRemove<T>(GameLocation? location, Vector2 tile) {
            if (location is not null && location.Objects.TryGetValue(tile, out var t) && t is T) {
                location.Objects.Remove(tile);
            }
        }

        private static void Postfix_Object_placementAction(
            SObject __instance, ref bool __result,
            GameLocation location, int x, int y
        ) {
            var tile = new Vector2(x / 64, y / 64);
            if (__result &&
                (ModEntry.Instance?.ModConfig?.KeepContents ?? false)
                && __instance is BetterIndoorPot bip && bip.IsFilled()
                && (location?.Objects?.TryGetValue(tile, out var p) ?? false)
                && p is IndoorPot newPot && newPot is not null
            ) {
                bip.TryTransplantOut(newPot);
            }
        }

        private static void Postfix_Object_loadDisplayName(SObject __instance, ref string __result) {
            try {
                if ((ModEntry.Instance?.ModConfig?.KeepContents ?? false)
                    && __instance is BetterIndoorPot bip && bip.IsFilled()
                ) {
                    var suffix = bip.GetPottedType();
                    if (!string.IsNullOrWhiteSpace(suffix)) {
                        __result += $" ({suffix})";
                    }
                }
            } catch { }
        }

        private static void Postfix_Object_maximumStackSize(SObject __instance, ref int __result) {
            try {
                if ((ModEntry.Instance?.ModConfig?.KeepContents ?? false)
                    && __instance is BetterIndoorPot bip && bip.IsFilled()
                ) {
                    __result = 1;
                }
            } catch { }
        }

        private static void Postfix_Object_ApplySprinkler(SObject __instance, Vector2 tile) {
            try {
                if ((ModEntry.Instance?.ModConfig?.EnableSprinklers ?? false)
                    && (__instance?.Location?.Objects?.TryGetValue(tile, out var obj) ?? false) && obj is IndoorPot pot
                    && (pot?.hoeDirt?.Value?.state?.Value ?? 2) != 2
                ) {
                    pot?.Water();
                }
            } catch { }
        }

        private static void Prefix_PlantableRule_ShouldApplyWhen(PlantableRule __instance, ref bool isGardenPot) {
            try {
                if ((ModEntry.Instance?.ModConfig?.AllowAncientSeeds ?? false)
                    && (__instance?.Id ?? "") == "NoGardenPots" && isGardenPot
                    && (__instance!.DeniedMessage ?? "").Contains("AncientFruit")
                ) {
                    isGardenPot = false;
                }
            } catch { }
        }
    }
}
