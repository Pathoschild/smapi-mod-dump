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
using System.Reflection;
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

        private static bool IsFilledPot(SObject obj, out IndoorPot? pot) =>
            (pot = obj as IndoorPot) is not null
            && (pot.hoeDirt?.Value?.crop is not null
            || pot.bush?.Value is not null
            || pot.hoeDirt?.Value?.fertilizer?.Value is not null
            || pot.heldObject?.Value is not null);

        private static void Prefix_Tool_DoFunction(GameLocation location, int x, int y, Farmer who) {
            try {
                if (ModEntry.Instance?.ModConfig?.KeepContents ?? false) {
                    var tile = new Vector2(x / 64, y / 64);
                    if ((location?.Objects?.TryGetValue(tile, out var obj) ?? false)
                         && IsFilledPot(obj, out var pot)
                         && who is not null
                    ) {
                        location.debris?.Add(new Debris(pot, who.GetToolLocation(), who.GetBoundingBox().Center.ToVector2()));
                        pot!.performRemoveAction();
                        location.Objects.Remove(tile);
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
            Bush __instance, Vector2 tileLocation
        ) {
            IndoorPot? pot;
            if ((ModEntry.Instance?.ModConfig?.AllowTransplant ?? false) // can transplant
                && (__instance?.size?.Value ?? 0) == 3 // must be a tea sapling (or modded?)
                && (Game1.player?.ActiveItem?.QualifiedItemId?.Equals("(BC)62") ?? false) // player is holding a garden pot
                && ((pot = Game1.player.ActiveItem as IndoorPot) is null // which has not been placed yet...
                    || !IsFilledPot(pot, out _)) // or is still empty
            ) {
                // put existing bush in a new garden pot
                pot = new IndoorPot(tileLocation);
                try { pot.bush.Value = __instance; } catch { }

                // clear existing bush from tile
                __instance!.Location.terrainFeatures.Remove(tileLocation);

                // remove empty garden pot from inventory
                Game1.player.reduceActiveItemByOne();
                // drop full garden pot
                Game1.createItemDebris(pot, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
            }
        }

        private static readonly FieldInfo? BushYDrawOffset = typeof(Bush).GetField("yDrawOffset", BindingFlags.NonPublic | BindingFlags.Instance);

        private static void Prefix_Utility_tryToPlaceItem(
            GameLocation location, ref Item? item, int x, int y
        ) {
            if ((ModEntry.Instance?.ModConfig?.AllowTransplant ?? false) // can transpant
                && (item?.QualifiedItemId?.Equals("(BC)62") ?? false)) { // using garden pot
                bool? isDirt, isBush, potEmpty, potCrop, potBush, dirtCrop, tileEmpty;

                var tile = new Vector2(x / 64, y / 64);
                HoeDirt? dirt = null;
                IndoorPot? pot = item as IndoorPot;
                potEmpty = pot is null || !IsFilledPot(pot, out _);
                isDirt = (location?.terrainFeatures?.TryGetValue(tile, out var h) ?? false) && (dirt = h as HoeDirt) is not null;
                Bush? bush;
                isBush = (location?.terrainFeatures?.TryGetValue(tile, out var b) ?? false) && (bush = b as Bush) is not null && (bush?.size?.Value ?? 0) == 3;
                tileEmpty = location?.isTilePlaceable(tile) ?? false;
                dirtCrop = dirt?.crop is not null || dirt?.fertilizer?.Value is not null;
                potCrop = pot?.hoeDirt?.Value?.crop is not null || pot?.hoeDirt?.Value?.fertilizer?.Value is not null;
                potBush = pot?.bush?.Value is not null;

                var disableCall = false;
                if ((isDirt ?? false) && !(dirtCrop ?? false) && (potCrop ?? false)) { // is dirt but no crop, potted crop
                    // transplant garden pot content to hoed dirt
                    try { dirt!.crop = pot!.hoeDirt.Value.crop; } catch { }
                    try { dirt!.fertilizer.Value = pot!.hoeDirt.Value.fertilizer.Value; } catch { }
                    try { dirt!.state.Value = pot!.hoeDirt.Value.state.Value; } catch { }

                    // clear garden pot
                    try { pot!.hoeDirt.Value = null; } catch { }

                    // remove from inventory
                    Game1.player.reduceActiveItemByOne();

                    // drop new garden pot
                    Game1.createItemDebris(new SObject(tile, "62"), Game1.player.getStandingPosition(), Game1.player.FacingDirection);

                    disableCall = true;
                } else if ((isDirt ?? false) && (dirtCrop ?? false) && (potEmpty ?? false) // is dirt with crop, empty pot
                    //&& (!(dirt!.crop.indexOfHarvest.Value?.Equals("454") ?? false) || (ModEntry.Instance?.ModConfig?.AllowAncientSeeds ?? false))
                ) { 
                    if (location.CheckItemPlantRules(dirt.crop.netSeedIndex.Value, true, true, out var denisedMsg)) {
                        // create new pot
                        pot = new IndoorPot(tile);

                        // transplant hoed dirt content to garden pot
                        try { pot.hoeDirt.Value = new HoeDirt(dirt!.state.Value, dirt.crop) { Location = dirt.Location }; } catch { }
                        try { pot.hoeDirt.Value.fertilizer.Value = dirt!.fertilizer.Value; } catch { }

                        // clear hoed dirt
                        try { dirt!.crop = null; } catch { }
                        try { dirt!.fertilizer.Value = null; } catch { }
                        try { dirt!.state.Value = 0; } catch { }

                        // remove empty garden pot
                        Game1.player.reduceActiveItemByOne();

                        // drop full garden pot
                        Game1.createItemDebris(pot, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
                    } else {
                        Game1.showRedMessage(denisedMsg);
                    }
                    disableCall = true;
                } else if ((isBush ?? false) && (potEmpty ?? false)) { // bush tile and empty pot
                    // handled about in Bush.performUseAction
                } else if ((tileEmpty ?? false) && (potBush ?? false)) { // empty tile and potted bush
                    // add bush to tile
                    location?.terrainFeatures.Add(tile, pot!.bush.Value);
                    // reset draw offset so bush is centered
                    BushYDrawOffset?.SetValue(pot!.bush.Value, 0);
                    // remove bush from pot
                    pot!.bush.Value = null;

                    // remove from inventory
                    Game1.player.reduceActiveItemByOne();

                    // drop new pot
                    Game1.createItemDebris(new SObject(tile, "62"), Game1.player.getStandingPosition(), Game1.player.FacingDirection);

                    disableCall = true;
                }

                if (disableCall) {
                    // prevent original call
                    item = null;
                }
            }
        }

        private static void Postfix_Object_placementAction(
            SObject __instance, ref bool __result,
            GameLocation location, int x, int y
        ) {
            var tile = new Vector2(x / 64, y / 64);
            if (__result &&
                (ModEntry.Instance?.ModConfig?.KeepContents ?? false)
                && IsFilledPot(__instance, out var pot)
                && (location?.Objects?.TryGetValue(tile, out var p) ?? false)
                && p is IndoorPot newPot && newPot is not null
            ) {
                if (Game1.IsMasterGame) {
                    // directly swapping does not seem to work after the first placement
                    //location.objects.Remove(tile);
                    //location.objects.Add(tile, __instance);

                    // so we transplant to the new pot instead
                    try { newPot.bush.Value = pot!.bush.Value; } catch { }
                    try { newPot.hoeDirt.Value = pot!.hoeDirt.Value; } catch { }
                    try { newPot.heldObject.Value = pot!.heldObject.Value; } catch { }
                    try { newPot.showNextIndex.Value = pot!.showNextIndex.Value; } catch { }

                    // disassociate with the original pot
                    try { pot!.bush.Value = null; } catch { }
                    try { pot!.hoeDirt.Value = null; } catch { }
                    try { pot!.heldObject.Value = null; } catch { }

                    // update locations
                    try { newPot.bush.Value.Location = location; } catch { }
                    try { newPot.hoeDirt.Value.Location = location; } catch { }
                    try { newPot.hoeDirt.Value.crop.currentLocation = location; } catch { }
                    try { newPot.heldObject.Value.Location = location; } catch { }

                    // update tiles
                    try { newPot.bush.Value.Tile = tile; } catch { }
                    try { newPot.hoeDirt.Value.Tile = tile; } catch { }
                    try { newPot.hoeDirt.Value.crop.tilePosition = tile; } catch { }
                    try { newPot.heldObject.Value.TileLocation = tile; } catch { }
                } else {
                    // don't remove item
                    __result = false;
                    // remove the new empty pot
                    location.objects.Remove(tile);
                    // notify user
                    Game1.showRedMessage(ModEntry.Instance?.Helper.Translation.Get("NCarigon.GardenPotOptions/farmhand_placement_error") ?? "null");
                }
            }
        }

        private static void Postfix_Object_loadDisplayName(SObject __instance, ref string __result) {
            try {
                if ((ModEntry.Instance?.ModConfig?.KeepContents ?? false) && IsFilledPot(__instance, out var pot)) {
                    if (pot?.hoeDirt?.Value?.crop?.indexOfHarvest?.Value is not null) {
                        __result += $" ({new SObject(pot.hoeDirt.Value.crop.indexOfHarvest.Value, 1).DisplayName})";
                    } else if (pot?.bush?.Value is not null) {
                        __result += $" ({(pot.bush.Value.size.Value == 3 ? new SObject("251", 1).DisplayName : "Bush")})";
                    } else if (pot?.hoeDirt?.Value?.fertilizer?.Value is not null) {
                        __result += $" ({new SObject(pot.hoeDirt.Value.fertilizer.Value.Replace("(O)", ""), 1).DisplayName})";
                    } else if (pot?.heldObject?.Value?.DisplayName is not null) {
                        __result += " (" + pot.heldObject.Value.DisplayName + ")";
                    }
                }
            } catch { }
        }

        private static void Postfix_Object_maximumStackSize(SObject __instance, ref int __result) {
            try {
                if ((ModEntry.Instance?.ModConfig?.KeepContents ?? false)
                    && IsFilledPot(__instance, out _)
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
