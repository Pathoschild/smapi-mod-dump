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

namespace StardewArchipelago.Locations.CodeInjections.Vanilla.Walnutsanity
{
    public static class WalnutDigSpotsInjections
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
        }

        // public override string checkForBuriedItem(int xLocation, int yLocation, bool explosion, bool detectOnly, Farmer who)
        public static bool CheckForBuriedItem_ReplaceWalnutWithCheck_Prefix(IslandLocation __instance, int xLocation, int yLocation, bool explosion, bool detectOnly, Farmer who, ref string __result)
        {
            try
            {
                if (!__instance.IsBuriedNutLocation(new Point(xLocation, yLocation)))
                {
                    return true; // run original logic
                }

                var digSpotId = $"Buried_{__instance.Name}_{xLocation}_{yLocation}";
                if (!_digSpotNameMap.ContainsKey(digSpotId))
                {
                    throw new Exception($"Dig Spot '{digSpotId}' Could not be mapped to an Archipelago location!");
                }

                if (!Game1.netWorldState.Value.FoundBuriedNuts.Add(digSpotId))
                {
                    return false; // don't run original logic
                }

                Game1.player.team.MarkCollectedNut(digSpotId);
                var itemId = IDProvider.CreateApLocationItemId(_digSpotNameMap[digSpotId]);
                var item = ItemRegistry.Create(itemId);
                Game1.createItemDebris(item, new Vector2(xLocation, yLocation) * 64f, -1, __instance);

                __result = "";
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CheckForBuriedItem_ReplaceWalnutWithCheck_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static readonly Dictionary<string, string> _digSpotNameMap = new()
        {
            { "Buried_IslandWest_62_76", "Starfish Triangle" },
            { "Buried_IslandWest_43_74", "Starfish Diamond" },
            { "Buried_IslandWest_30_75", "X in the sand" },
            { "Buried_IslandWest_21_81", "Diamond Of Indents" },
            { "Buried_IslandWest_39_24", "Diamond Of Pebbles" },
            { "Buried_IslandWest_88_14", "Circle Of Grass" },
            { "Buried_IslandNorth_26_81", "Big Circle Of Stones" },
            { "Buried_IslandNorth_42_77", "Diamond Of Grass" },
            { "Buried_IslandNorth_57_79", "Small Circle Of Stones" },
            { "Buried_IslandNorth_62_54", "Patch Of Sand" },
            { "Buried_IslandNorth_19_39", "Crooked Circle Of Stones" },
            { "Buried_IslandNorth_54_21", "Arc Of Stones" },
            { "Buried_IslandNorth_19_13", "Northmost Point Circle Of Stones" },
            { "Buried_IslandSouthEast_25_17", "Diamond Of Yellow Starfish" },
            { "Buried_IslandSouthEastCave_36_26", "Pirate Cove Patch Of Sand" },
        }; // Around the tiger slimes is "Buried_IslandWest_39_24"
    }
}
