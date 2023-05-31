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
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewArchipelago.Archipelago;
using StardewValley;

namespace StardewArchipelago.Locations.CodeInjections.Modded
{
    public static class MagicModInjections
    {
        private const string ANALYZE_BLINK_AP_LOCATION = "Analyze All Toil School Locations";
        private const string ANALYZE_SPIRIT_AP_LOCATION = "Analyze All Eldritch School Locations";
        private const string ANALYZE_CLEARDEBRIS_AP_LOCATION = "Analyze: Clear Debris";
        private const string ANALYZE_TILL_AP_LOCATION = "Analyze: Till";
        private const string ANALYZE_WATER_AP_LOCATION = "Analyze: Water";
        private const string ANALYZE_EVAC_AP_LOCATION = "Analyze: Evac";
        private const string ANALYZE_HASTE_AP_LOCATION = "Analyze: Haste";
        private const string ANALYZE_HEAL_AP_LOCATION = "Analyze: Heal";
        private const string ANALYZE_BUFF_AP_LOCATION = "Analyze All Life School Locations";
        private const string ANALYZE_SHOCKWAVE_AP_LOCATION = "Analyze: Shockwave";
        private const string ANALYZE_FIREBALL_AP_LOCATION = "Analyze: Fireball";
        private const string ANALYZE_FROSTBITE_AP_LOCATION = "Analyze: Frostbite";
        private const string ANALYZE_TELEPORT_AP_LOCATION = "Analyze All Elemental School Locations";
        private const string ANALYZE_LANTERN_AP_LOCATION = "Analyze: Lantern";
        private const string ANALYZE_TENDRILS_AP_LOCATION = "Analyze: Tendrils";
        private const string ANALYZE_PHOTOSYNTHESIS_AP_LOCATION = "Analyze All Nature School Locations";
        private const string ANALYZE_DESCEND_AP_LOCATION = "Analyze: Descend";
        private const string ANALYZE_METEOR_AP_LOCATION = "Analyze: Meteor";
        private const string ANALYZE_LUCKSTEAL_AP_LOCATION = "Analyze: Lucksteal";
        private const string ANALYZE_BLOODMANA_AP_LOCATION = "Analyze: Bloodmana";
        private const string ANALYZE_REWIND_AP_LOCATION = "Analyze Every Magic School Location";

        private const int COFFEE = 395;
        private const int LIFE_ELIXIR = 773;
        private const int EARTH_CRYSTAL = 86;
        private const int FIRE_QUARTZ = 82;
        private const int ICE_PIP = 161;
        private const int MINE_LADDER = 173;
        private const int CROP_TILE = 622;

        private static IMonitor _monitor;
        private static IModHelper _helper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;


        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _helper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
        }

        // internal class AnalyzeSpell : Spell
        // public override IActiveEffect OnCast(Farmer player, int level, int targetX, int targetY)
        public static bool OnCast_AnalyzeGivesLocations_Prefix(object __instance, Farmer player, int level, int targetX, int targetY, object __result)
        {
            try
            {
                __result = null;
                if (player != Game1.player)
                    return false;
                var spellsLearned = new List<string>();
                if (player.CurrentItem != null)
                {
                    CheckItemAnalyzeLocations(player, spellsLearned);
                }
                foreach (var lightSource in player.currentLocation.sharedLights.Values)
                {
                    if (Utility.distance(targetX, lightSource.position.X, targetY, lightSource.position.Y) < lightSource.radius.Value * Game1.tileSize)
                    {
                        spellsLearned.Add(ANALYZE_LANTERN_AP_LOCATION);
                        break;
                    }
                }
                CheckTileAnalyzeLocations(player, spellsLearned, targetX, targetY);
                if (spellsLearned.Any(spell => _locationChecker.IsLocationMissingAndExists(spell)))
                {
                    Game1.playSound("secret1");
                }
                foreach (var spell in spellsLearned)
                {
                    _locationChecker.AddCheckedLocation(spell);
                }
                CheckTotalCheckLocations();
                return false; //Don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(OnCast_AnalyzeGivesLocations_Prefix)}:\n{ex}", LogLevel.Error);
                return true; //Run original logic
            }
            
        }

        private static void CheckItemAnalyzeLocations(Farmer player, List<string> spellsLearned)
        {
            if (player.CurrentTool != null)
            {
                if (player.CurrentTool is StardewValley.Tools.Axe || player.CurrentTool is StardewValley.Tools.Pickaxe)
                    spellsLearned.Add(ANALYZE_CLEARDEBRIS_AP_LOCATION);
                else if (player.CurrentTool is StardewValley.Tools.Hoe)
                    spellsLearned.Add(ANALYZE_TILL_AP_LOCATION);
                else if (player.CurrentTool is StardewValley.Tools.WateringCan)
                    spellsLearned.Add(ANALYZE_WATER_AP_LOCATION);
            }
            else if (player.CurrentItem is StardewValley.Objects.Boots)
            {
                spellsLearned.Add(ANALYZE_EVAC_AP_LOCATION);
            }
            else if (player.ActiveObject != null)
            {
                if (!player.ActiveObject.bigCraftable.Value)
                {
                    int index = player.ActiveObject.ParentSheetIndex;
                    if (index == COFFEE)
                        spellsLearned.Add(ANALYZE_HASTE_AP_LOCATION);
                    else if (index == LIFE_ELIXIR)
                        spellsLearned.Add(ANALYZE_HEAL_AP_LOCATION);
                    else if (index == EARTH_CRYSTAL)
                        spellsLearned.Add(ANALYZE_SHOCKWAVE_AP_LOCATION);
                    else if (index == FIRE_QUARTZ)
                        spellsLearned.Add(ANALYZE_FIREBALL_AP_LOCATION);
                    else if (index == ICE_PIP)
                        spellsLearned.Add(ANALYZE_FROSTBITE_AP_LOCATION);
                }
            }
        }

        private static void CheckTileAnalyzeLocations(Farmer player, List<string> spellsLearned, int targetX, int targetY)
        {
            var tilePos = new Vector2(targetX / Game1.tileSize, targetY / Game1.tileSize);
            if (player.currentLocation.terrainFeatures.ContainsKey(tilePos) && player.currentLocation.terrainFeatures[tilePos] is StardewValley.TerrainFeatures.HoeDirt hoeDirt)
            {
                if (hoeDirt.crop != null)
                    spellsLearned.Add(ANALYZE_TENDRILS_AP_LOCATION);
            }
            var tile = player.currentLocation.map.GetLayer("Buildings").Tiles[(int)tilePos.X, (int)tilePos.Y];
            if (tile != null && tile.TileIndex == MINE_LADDER)
                spellsLearned.Add(ANALYZE_DESCEND_AP_LOCATION);
            if (player.currentLocation is Farm farm)
            {
                foreach (var clump in farm.resourceClumps)
                {
                    if (clump.parentSheetIndex.Value == CROP_TILE && new Rectangle((int)clump.tile.Value.X, (int)clump.tile.Value.Y, clump.width.Value, clump.height.Value).Contains((int)tilePos.X, (int)tilePos.Y))
                        spellsLearned.Add(ANALYZE_METEOR_AP_LOCATION);
                }
            }
            if (player.currentLocation.doesTileHaveProperty((int)tilePos.X, (int)tilePos.Y, "Action", "Buildings") == "EvilShrineLeft")
                spellsLearned.Add(ANALYZE_LUCKSTEAL_AP_LOCATION);
            if (player.currentLocation is StardewValley.Locations.MineShaft mineShaft && mineShaft.mineLevel == 100 && mineShaft.waterTiles[(int)tilePos.X, (int)tilePos.Y])
                spellsLearned.Add(ANALYZE_BLOODMANA_AP_LOCATION);

        }

        private static void CheckTotalCheckLocations()
        {
            if (KnowsAllToilSpells() && _locationChecker.IsLocationNotChecked(ANALYZE_BLINK_AP_LOCATION))
            {
                _locationChecker.AddCheckedLocation(ANALYZE_BLINK_AP_LOCATION);
            }
            if (KnowsAllEldrichSpells() && _locationChecker.IsLocationNotChecked(ANALYZE_SPIRIT_AP_LOCATION))
            {
                _locationChecker.AddCheckedLocation(ANALYZE_SPIRIT_AP_LOCATION);
            }
            if (KnowsAllElementalSpells() && _locationChecker.IsLocationNotChecked(ANALYZE_TELEPORT_AP_LOCATION))
            {
                _locationChecker.AddCheckedLocation(ANALYZE_TELEPORT_AP_LOCATION);
            }
            if (KnowsAllLifeSpells() && _locationChecker.IsLocationNotChecked(ANALYZE_BUFF_AP_LOCATION))
            {
                _locationChecker.AddCheckedLocation(ANALYZE_BUFF_AP_LOCATION);
            }
            if (KnowsAllNatureSpells() && _locationChecker.IsLocationNotChecked(ANALYZE_PHOTOSYNTHESIS_AP_LOCATION))
            {
                _locationChecker.AddCheckedLocation(ANALYZE_PHOTOSYNTHESIS_AP_LOCATION);
            }
            if (KnowsAllSpellsButRewind() && _locationChecker.IsLocationNotChecked(ANALYZE_REWIND_AP_LOCATION))
            {
                _locationChecker.AddCheckedLocation(ANALYZE_REWIND_AP_LOCATION);
            }
        }

        private static bool KnowsAllToilSpells()
        {
            if (_locationChecker.IsLocationChecked(ANALYZE_TILL_AP_LOCATION) 
            && _locationChecker.IsLocationChecked(ANALYZE_CLEARDEBRIS_AP_LOCATION)
            && _locationChecker.IsLocationChecked(ANALYZE_WATER_AP_LOCATION))
            {
                return true;
            }
            return false;
        }
        private static bool KnowsAllEldrichSpells()
        {
            if (_locationChecker.IsLocationChecked(ANALYZE_METEOR_AP_LOCATION) 
            && _locationChecker.IsLocationChecked(ANALYZE_BLOODMANA_AP_LOCATION)
            && _locationChecker.IsLocationChecked(ANALYZE_LUCKSTEAL_AP_LOCATION))
            {
                return true;
            }
            return false;
        }
        private static bool KnowsAllElementalSpells()
        {
            if (_locationChecker.IsLocationChecked(ANALYZE_FIREBALL_AP_LOCATION)
            && _locationChecker.IsLocationChecked(ANALYZE_FROSTBITE_AP_LOCATION)
            && _locationChecker.IsLocationChecked(ANALYZE_DESCEND_AP_LOCATION))
            {
                return true;
            }
            return false;
        }
        private static bool KnowsAllLifeSpells()
        {
            if (_locationChecker.IsLocationChecked(ANALYZE_EVAC_AP_LOCATION) 
            && _locationChecker.IsLocationChecked(ANALYZE_HEAL_AP_LOCATION)
            && _locationChecker.IsLocationChecked(ANALYZE_HASTE_AP_LOCATION))
            {
                return true;
            }
            return false;
        }
        private static bool KnowsAllNatureSpells()
        {
            if (_locationChecker.IsLocationChecked(ANALYZE_TENDRILS_AP_LOCATION)
            && _locationChecker.IsLocationChecked(ANALYZE_SHOCKWAVE_AP_LOCATION)
            && _locationChecker.IsLocationChecked(ANALYZE_LANTERN_AP_LOCATION))
            {
                return true;
            }
            return false;
        }
        private static bool KnowsAllSpellsButRewind()
        {
            if (KnowsAllEldrichSpells() && KnowsAllElementalSpells() && KnowsAllLifeSpells() && KnowsAllNatureSpells()
            && KnowsAllToilSpells())
            {
                return true;
            }
            return false;
        }
    }
}