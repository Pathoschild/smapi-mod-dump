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
using StardewArchipelago.Archipelago;
using StardewArchipelago.GameModifications.EntranceRandomizer;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData;

namespace StardewArchipelago.GameModifications.CodeInjections
{
    public class EntranceInjections
    {
        private static IMonitor _monitor;
        private static ArchipelagoClient _archipelago;
        private static EntranceManager _entranceManager;

        public static void Initialize(IMonitor monitor, ArchipelagoClient archipelago, EntranceManager entranceManager)
        {
            _monitor = monitor;
            _archipelago = archipelago;
            _entranceManager = entranceManager;
        }

        public static bool PerformWarpFarmer_EntranceRandomization_Prefix(ref LocationRequest locationRequest, ref int tileX,
            ref int tileY, ref int facingDirectionAfterWarp)
        {
            try
            {
                if (Game1.currentLocation.Name.ToLower() == locationRequest.Name.ToLower() || Game1.player.passedOut || Game1.player.FarmerSprite.isPassingOut() || Game1.player.isInBed)
                {
                    return true; // run original logic
                }

                var targetPosition = new Point(tileX, tileY);
                var entranceIsReplaced = _entranceManager.TryGetEntranceReplacement(Game1.currentLocation.Name, locationRequest.Name, targetPosition, out var replacedWarp);
                if (!entranceIsReplaced)
                {
                    return true; // run original logic
                }

                locationRequest.Name = replacedWarp.LocationRequest.Name;


                foreach (var activePassiveFestival in Game1.netWorldState.Value.ActivePassiveFestivals)
                {
                    if (Utility.TryGetPassiveFestivalData(activePassiveFestival, out var data) &&
                        Game1.dayOfMonth >= data.StartDay && Game1.dayOfMonth <= data.EndDay && data.Season == Game1.season &&
                        data.MapReplacements != null && data.MapReplacements.TryGetValue(locationRequest.Name, out var name))
                    {
                        locationRequest.Name = name;
                    }
                }

                locationRequest.Location = replacedWarp.LocationRequest.Location;
                locationRequest.IsStructure = replacedWarp.LocationRequest.IsStructure;
                tileX = replacedWarp.TileX;
                tileY = replacedWarp.TileY;
                facingDirectionAfterWarp = (int)replacedWarp.FacingDirectionAfterWarp;

                SetCorrectSwimsuitState(locationRequest, tileX, tileY);

                return true; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(PerformWarpFarmer_EntranceRandomization_Prefix)} going from {Game1.currentLocation.Name} to {locationRequest.Name}:{Environment.NewLine}\t{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static void SetCorrectSwimsuitState(LocationRequest locationRequest, int tileX, int tileY)
        {
            var shouldBeInSwimsuit = GetCorrectSwimsuitState(locationRequest, tileX, tileY);
            if (shouldBeInSwimsuit)
            {
                Game1.player.changeIntoSwimsuit();
            }
            else
            {
                Game1.player.changeOutOfSwimSuit();
            }
        }

        private static bool GetCorrectSwimsuitState(LocationRequest locationRequest, int tileX, int tileY)
        {
            if (locationRequest.Location.Name.Equals("BathHouse_Pool"))
            {
                return true;
            }

            if (!locationRequest.Location.Name.StartsWith("BathHouse_", StringComparison.OrdinalIgnoreCase) ||
                !locationRequest.Location.Name.EndsWith("Locker", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (locationRequest.Name.Contains("Women", StringComparison.OrdinalIgnoreCase))
            {
                return tileX < 5;
            }
            else
            {
                return tileX > 12;
            }
        }
    }
}
