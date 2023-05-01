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
using Microsoft.Xna.Framework;
using StardewValley;

namespace StardewArchipelago.GameModifications.EntranceRandomizer
{
    public class OneWayEntrance
    {
        private const int MAX_DISTANCE = 2;

        public string OriginName { get; }
        public string DestinationName { get; }
        public Point OriginPosition { get; set; }
        public Point DestinationPosition { get; set; }
        public int FacingDirectionAfterWarp { get; }

        private OneWayEntrance _replacement;

        public OneWayEntrance(string originName, string destinationName, Point originPosition, Point destinationPosition, int facingDirectionAfterWarp)
        {
            OriginName = originName;
            DestinationName = destinationName;
            OriginPosition = originPosition;
            DestinationPosition = destinationPosition;
            FacingDirectionAfterWarp = facingDirectionAfterWarp;
        }

        public void ReplaceWith(OneWayEntrance replacementEntrance)
        {
            _replacement = replacementEntrance;
        }

        public bool GetModifiedWarp(WarpRequest originalWarp, out WarpRequest newWarp)
        {
            newWarp = originalWarp;
            if (_replacement == null)
            {
                return false;
            }

            var currentLocation = Game1.currentLocation.Name;
            if (string.Equals(currentLocation, OriginName, StringComparison.CurrentCultureIgnoreCase) && string.Equals(originalWarp.LocationRequest.Name, DestinationName, StringComparison.CurrentCultureIgnoreCase))
            {
                return ReplaceWarpRequest(originalWarp, out newWarp);
            }

            return false;
        }

        private bool ReplaceWarpRequest(WarpRequest originalWarp, out WarpRequest newWarp)
        {
            newWarp = originalWarp;
            var currentTile = new Point(Game1.player.getTileX(), Game1.player.getTileY());
            if (!IsCloseEnough(OriginPosition, currentTile))
            {
                return false;
            }

            var correctReplacement = EquivalentWarps.GetCorrectEquivalentWarp(_replacement);

            var locationRequest = originalWarp.LocationRequest;
            locationRequest.Name = correctReplacement.DestinationName;
            locationRequest.Location = Game1.getLocationFromName(correctReplacement.DestinationName, locationRequest.IsStructure);
            newWarp = new WarpRequest(locationRequest, correctReplacement.DestinationPosition.X, correctReplacement.DestinationPosition.Y, correctReplacement.FacingDirectionAfterWarp);
            return true;
        }

        private int GetTotalTileDistance(Point tile1, Point tile2)
        {
            return Math.Abs(tile1.X - tile2.X) + Math.Abs(tile1.Y - tile2.Y);
        }

        private bool IsCloseEnough(Point tile1, Point tile2)
        {
            return GetTotalTileDistance(tile1, tile2) <= MAX_DISTANCE;
        }
    }
}
