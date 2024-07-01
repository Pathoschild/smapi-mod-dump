/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Constants;
using StardewArchipelago.Textures;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla.Walnutsanity
{
    public static class WalnutBushInjections
    {
        private static IMonitor _monitor;
        private static IModHelper _helper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;
        private static Texture2D _bushtexture;

        public static void Initialize(IMonitor monitor, IModHelper helper, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _helper = helper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            _bushtexture = ArchipelagoTextures.GetArchipelagoBush(monitor, helper);
            Utility.ForEachLocation((x) => SetupWalnutsanityBushes(x), true, true);
        }

        // public string GetShakeOffItem()
        public static bool GetShakeOffItem_ReplaceWalnutWithCheck_Prefix(Bush __instance, ref string __result)
        {
            try
            {
                if (__instance.size.Value != 4)
                {
                    return true; // run original logic
                }

                var bushId = $"Bush_{__instance.Location.Name}_{__instance.Tile.X}_{__instance.Tile.Y}";

                if (!_bushNameMap.ContainsKey(bushId))
                {
                    throw new Exception($"Bush '{bushId}' Could not be mapped to an Archipelago location!");
                }

                __result = IDProvider.CreateApLocationItemId(_bushNameMap[bushId]);
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetShakeOffItem_ReplaceWalnutWithCheck_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public void setUpSourceRect()
        public static bool SetUpSourceRect_UseArchipelagoTexture_Prefix(Bush __instance)
        {
            try
            {
                if (__instance.size.Value != 4)
                {
                    return true; // run original logic
                }

                SetUpSourceRectForWalnutsanityBush(__instance);
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(SetUpSourceRect_UseArchipelagoTexture_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public override void draw(SpriteBatch spriteBatch)
        public static bool Draw_UseArchipelagoTexture_Prefix(Bush __instance, SpriteBatch spriteBatch)
        {
            try
            {
                if (__instance.size.Value != 4)
                {
                    return true; // run original logic
                }

                var tile = __instance.Tile;
                var effects = __instance.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                if (__instance.drawShadow.Value)
                {
                    var shadowPosition = Game1.GlobalToLocal(Game1.viewport, new Vector2((float)((tile.X + 0.5) * 64.0 - 51.0), (float)(tile.Y * 64.0 - 16.0)));
                    spriteBatch.Draw(Game1.mouseCursors, shadowPosition, Bush.shadowSourceRect, Color.White, 0.0f, Vector2.Zero, 4f, effects, 1E-06f);
                }
                var globalPosition = new Vector2(tile.X * 64f + 64, (float)((tile.Y + 1.0) * 64.0));
                var position = Game1.GlobalToLocal(Game1.viewport, globalPosition);
                var sourceRectangle = new Rectangle?(__instance.sourceRect.Value);
                var layerDepth = (float)((__instance.getBoundingBox().Center.Y + 48) / 10000.0 - tile.X / 1000000.0);
                // private float shakeRotation;
                var shakeRotationField = _helper.Reflection.GetField<float>(__instance, "shakeRotation");
                var shakeRotation = shakeRotationField.GetValue();
                spriteBatch.Draw(_bushtexture, position, sourceRectangle, Color.White, shakeRotation, new Vector2(16, 32f), 4f, effects, layerDepth);

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(Draw_UseArchipelagoTexture_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static bool SetupWalnutsanityBushes(GameLocation gameLocation)
        {
            if (gameLocation is not IslandLocation)
            {
                return true;
            }

            foreach (var largeTerrainFeature in gameLocation.largeTerrainFeatures)
            {
                if (largeTerrainFeature is not Bush bush || bush.size.Value != 4)
                {
                    continue;
                }

                SetUpSourceRectForWalnutsanityBush(bush);
            }

            return true;
        }

        private static void SetUpSourceRectForWalnutsanityBush(Bush bush)
        {
            bush.sourceRect.Value = new Rectangle(bush.tileSheetOffset.Value * 32, 0, 32, 32);
        }

        private static readonly Dictionary<string, string> _bushNameMap = new()
        {
            { "Bush_IslandEast_17_37", "Jungle Bush" },
            { "Bush_IslandShrine_23_34", "Gem Birds Bush" },
            { "Bush_CaptainRoom_2_4", "Shipwreck Bush" },
            { "Bush_IslandWest_38_56", "Bush Behind Coconut Tree" },
            { "Bush_IslandWest_25_30", "Walnut Room Bush" },
            { "Bush_IslandWest_15_3", "Coast Bush" },
            { "Bush_IslandWest_31_24", "Bush Behind Mahogany Tree" },
            { "Bush_IslandWest_54_18", "Below Colored Crystals Cave Bush" },
            { "Bush_IslandWest_64_30", "Cliff Edge Bush" },
            { "Bush_IslandWest_104_3", "Farm Parrot Express Bush" },
            { "Bush_IslandWest_75_29", "Farmhouse Cliff Bush" },
            { "Bush_IslandNorth_9_84", "Grove Bush" },
            { "Bush_IslandNorth_4_42", "Above Dig Site Bush" },
            { "Bush_IslandNorth_45_38", "Above Field Office Bush 1" },
            { "Bush_IslandNorth_47_40", "Above Field Office Bush 2" },
            { "Bush_IslandNorth_56_27", "Bush Behind Volcano Tree" },
            { "Bush_IslandNorth_20_26", "Hidden Passage Bush" },
            { "Bush_IslandNorth_13_33", "Secret Beach Bush 1" },
            { "Bush_IslandNorth_5_30", "Secret Beach Bush 2" },
            { "Bush_Caldera_28_36", "Forge Entrance Bush" },
            { "Bush_Caldera_9_34", "Forge Exit Bush" },
            { "Bush_IslandSouth_31_5", "Cliff Over Island South Bush" },
        };
    }
}
