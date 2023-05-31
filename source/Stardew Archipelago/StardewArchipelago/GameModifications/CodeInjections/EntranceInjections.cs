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
using System.IO;
using Microsoft.Xna.Framework;
using StardewArchipelago.Archipelago;
using StardewArchipelago.GameModifications.EntranceRandomizer;
using StardewModdingAPI;
using StardewValley;

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
                if (Game1.currentLocation.Name.ToLower() == locationRequest.Name.ToLower())
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
                locationRequest.Location = replacedWarp.LocationRequest.Location;
                locationRequest.IsStructure = replacedWarp.LocationRequest.IsStructure;
                tileX = replacedWarp.TileX;
                tileY = replacedWarp.TileY;
                facingDirectionAfterWarp = (int)replacedWarp.FacingDirectionAfterWarp;
                return true; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(PerformWarpFarmer_EntranceRandomization_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
