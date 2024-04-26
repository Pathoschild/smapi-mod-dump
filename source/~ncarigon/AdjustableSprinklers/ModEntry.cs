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
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using SObject = StardewValley.Object;

namespace AdjustableSprinklers {
    internal sealed class ModEntry : Mod {
        private static Config? ModConfig;
        private static IMonitor? ModMonitor;

        public override void Entry(IModHelper helper) {
            ModConfig = Config.Register(helper);
            ModMonitor = this.Monitor;
            var harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), "GetBaseRadiusForSprinkler"),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(Postfix_Object_GetBaseRadiusForSprinkler))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), "GetModifiedRadiusForSprinkler"),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(Postfix_Object_GetModifiedRadiusForSprinkler))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), "GetSprinklerTiles"),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(Postfix_Object_GetSprinklerTiles))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), "drawPlacementBounds"),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(Postfix_Object_drawPlacementBounds))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), "ApplySprinkler"),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(Postfix_Object_ApplySprinkler))
            );
            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
        }

        [HarmonyPriority(Priority.VeryLow)]
        private static void Postfix_Object_GetBaseRadiusForSprinkler(
            SObject __instance, ref int __result
        ) {
            if (__instance is not null && __result != -1) {
                var radius = ModConfig?.BaseRadius ?? 0;
                var increase = ModConfig?.TierIncrease ?? 1;
                __result = __instance.QualifiedItemId switch {
                    "(O)599" => radius,
                    "(O)621" => radius + increase,
                    "(O)645" => radius + (increase * 2),
                    "(O)1113" => radius + (increase * 3),
                    _ => -1
                };
                if (__result == -1) {
                    __result = __instance.Name switch {
                        "Sprinkler" => radius,
                        "Quality Sprinkler" => radius + increase,
                        "Iridium Sprinkler" => radius + (increase * 2),
                        "Prismatic Sprinkler" => radius + (increase * 3),
                        _ => -1
                    };
                }
            }
        }

        [HarmonyPriority(Priority.VeryLow)]
        private static void Postfix_Object_GetModifiedRadiusForSprinkler(
            SObject __instance, ref int __result
        ) {
            if (__instance is not null && __result != -1) {
                if (__instance.heldObject.Value != null && __instance.heldObject.Value.QualifiedItemId == "(O)915") {
                    // need to -1 here to remove vanilla game logic and adjust to the configured value
                    __result += (ModConfig?.TierIncrease ?? 1) - 1;
                }
            }
        }

        [HarmonyPriority(Priority.VeryLow)]
        private static void Postfix_Object_GetSprinklerTiles(
            SObject __instance, ref List<Vector2> __result
        ) {
            if ((ModConfig?.CircularArea ?? false) && (__instance?.IsSprinkler() ?? false)) {
                __result = GetTiles(__instance.TileLocation, __instance.GetModifiedRadiusForSprinkler(), true);
            }
        }

        private static void Postfix_Object_drawPlacementBounds(
            SObject __instance, SpriteBatch spriteBatch
        ) {
            if (__instance is not null && spriteBatch is not null
                && ((ModConfig?.ShowSprinklerArea ?? false) || (ModConfig?.ShowScarecrowArea ?? false))
            ) {
                List<Vector2>? tiles = null;
                if ((ModConfig?.ShowSprinklerArea ?? false) && __instance.IsSprinkler()) {
                    tiles = __instance.GetSprinklerTiles();
                } else if ((ModConfig?.ShowScarecrowArea ?? false) && __instance.IsScarecrow()) {
                    tiles = GetTiles(__instance.TileLocation, __instance.GetRadiusForScarecrow(), false);
                }
                if (tiles is not null) {
                    foreach (Vector2 v in tiles) {
                        spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(new Vector2((int)v.X * 64, (int)v.Y * 64)), new Rectangle(194, 388, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.01f);
                    }
                }
            }
        }

        private static List<Vector2> GetTiles(Vector2 center, int radius, bool round) {
            var tiles = new List<Vector2>();
            for (var x = -radius; x <= radius; x++) {
                for (var y = -radius; y <= radius; y++) {
                    var tile = center + new Vector2(x, y);
                    if (Vector2.Distance(center, tile) < radius + (round ? 0.5 : 0.0)) {
                        tiles.Add(tile);
                    }
                }
            }
            return tiles;
        }

        private void Input_ButtonPressed(object? sender, StardewModdingAPI.Events.ButtonPressedEventArgs e) {
            if ((ModConfig?.ActivateWhenClicked ?? false)
                && (e?.Button.IsActionButton() ?? false)
                && Game1.player?.currentLocation is not null
            ) {
                var obj = Game1.player.currentLocation.getObjectAtTile((int)e.Cursor.GrabTile.X, (int)e.Cursor.GrabTile.Y);
                if (obj?.IsSprinkler() ?? false) {
                    obj.ApplySprinklerAnimation();
                    Task.Delay(2000).ContinueWith(_ => { // INFO: adding a delay here for a more natural look
                        foreach (var tile in obj.GetSprinklerTiles()) {
                            obj.ApplySprinkler(tile);
                        }
                    });
                }
            }
        }

        private static void Postfix_Object_ApplySprinkler(
            SObject __instance, Vector2 tile
        ) {
            if ((ModConfig?.WaterGardenPots ?? false)
                && (__instance?.Location?.Objects?.TryGetValue(tile, out var t) ?? false)
                && t is IndoorPot pot && pot is not null
            ) {
                pot?.Water();
            }
        }
    }
}