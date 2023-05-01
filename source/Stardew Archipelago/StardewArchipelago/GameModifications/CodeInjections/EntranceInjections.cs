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

        public static void Initialize(IMonitor monitor, ArchipelagoClient archipelago)
        {
            _monitor = monitor;
            _archipelago = archipelago;
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

                var entranceExists = Entrances.TryGetEntrance(Game1.currentLocation.Name, locationRequest.Name,
                    out var desiredEntrance);
                if (!entranceExists)
                {
#if DEBUG
                    RecordNewEntrance(locationRequest, tileX, tileY, facingDirectionAfterWarp);
#endif
                    return true; // run original logic
                }

                var warpRequest = new WarpRequest(locationRequest, tileX, tileY, facingDirectionAfterWarp);
                var warpIsModified = desiredEntrance.GetModifiedWarp(warpRequest, out var newWarp);
                if (!warpIsModified)
                {
                    return true; // run original logic
                }

                locationRequest.Name = newWarp.LocationRequest.Name;
                locationRequest.Location = newWarp.LocationRequest.Location;
                locationRequest.IsStructure = newWarp.LocationRequest.IsStructure;
                tileX = newWarp.TileX;
                tileY = newWarp.TileY;
                facingDirectionAfterWarp = newWarp.FacingDirectionAfterWarp;
                return true; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(PerformWarpFarmer_EntranceRandomization_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

#if DEBUG

        private static OneWayEntrance _temporaryOneWayEntrance;
        private static List<(OneWayEntrance, OneWayEntrance)> _newEntrances = new();
        private static void RecordNewEntrance(LocationRequest locationRequest, int tileX, int tileY, int facingDirectionAfterWarp)
        {
            var currentPosition = new Point(Game1.player.getTileX(), Game1.player.getTileY());
            var warpPosition = new Point(tileX, tileY);
            var newEntrance = new OneWayEntrance(Game1.currentLocation.Name, locationRequest.Name, currentPosition,
                warpPosition, facingDirectionAfterWarp);

            if (_temporaryOneWayEntrance == null ||
                _temporaryOneWayEntrance.OriginName.ToLower() != newEntrance.DestinationName.ToLower() ||
                _temporaryOneWayEntrance.DestinationName.ToLower() != newEntrance.OriginName.ToLower())
            {
                _temporaryOneWayEntrance = newEntrance;
                _monitor.Log($"Found a new Entrance from {newEntrance.OriginName} to {newEntrance.DestinationName}", LogLevel.Warn);
                return;
            }
            
            var entranceSide1 = new OneWayEntrance(_temporaryOneWayEntrance.OriginName, newEntrance.OriginName,
                newEntrance.DestinationPosition, _temporaryOneWayEntrance.DestinationPosition, _temporaryOneWayEntrance.FacingDirectionAfterWarp);
            var entranceSide2 = new OneWayEntrance(newEntrance.OriginName, _temporaryOneWayEntrance.OriginName,
                _temporaryOneWayEntrance.DestinationPosition, newEntrance.DestinationPosition, newEntrance.FacingDirectionAfterWarp);
            _newEntrances.Add((entranceSide1, entranceSide2));
            _monitor.Log($"Completed the entrance from {entranceSide1.OriginName} to {entranceSide1.DestinationName}", LogLevel.Warn);
            _temporaryOneWayEntrance = null;
        }

        public static void SaveNewEntrancesToFile()
        {
            using var streamWriter =
                new StreamWriter(
                    "D:\\Programs\\Steam\\steamapps\\common\\Stardew Valley\\Mods\\StardewArchipelago\\NewEntrances.txt");
            foreach (var (entrance1, entrance2) in _newEntrances)
            {
                streamWriter.WriteLine($"public static readonly (OneWayEntrance, OneWayEntrance) {entrance1.OriginName}To{entrance1.DestinationName} = AddEntrance(\"{entrance1.OriginName}\", \"{entrance1.DestinationName}\", {entrance1.OriginPosition.X}, {entrance1.OriginPosition.Y}, {entrance1.DestinationPosition.X}, {entrance1.DestinationPosition.Y}, {entrance2.FacingDirectionAfterWarp}, {entrance1.FacingDirectionAfterWarp});");
            }

            _newEntrances = new();
        }

#endif
    }
}
