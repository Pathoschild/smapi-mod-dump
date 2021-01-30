/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GenDeathrow/SDV_BlessingsAndCurses
**
*************************************************/

using System.Collections.Generic;
using BNC.TwitchApp;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;
using xTile.Dimensions;
using System;
namespace BNC.Managers.Augments
{

    public class Teleport : BaseAugment
    {
        private int ticks = 0;
        private bool teleporting = false;
        private IEnumerator<Point> teleportationPath;

        public Teleport()
        {
            this.DisplayName = "Phasing Out!";
            this.desc = "Player randomly moves thru dimensions.";
        }

        public override void Init() { }

        public override ActionResponse MonsterTickUpdate(Monster m)
        {
            return ActionResponse.Done;
        }

        public override ActionResponse PlayerTickUpdate()
        {
            Farmer farmer = Game1.player;

            if (Game1.random.NextDouble() >= .05)
            {
                BNC_Core.Logger.Log("Will Teleport", StardewModdingAPI.LogLevel.Debug);
                int tries = 0;
                Vector2 possiblePoint = new Vector2(farmer.getTileLocation().X + (float)((Game1.random.NextDouble() < 0.5) ? Game1.random.Next(-5, -1) : Game1.random.Next(2, 6)), farmer.getTileLocation().Y + (float)((Game1.random.NextDouble() < 0.5) ? Game1.random.Next(-5, -1) : Game1.random.Next(2, 6)));
                for (; tries < 6; tries++)
                {
                    if (farmer.currentLocation.isTileOnMap(possiblePoint) && farmer.currentLocation.isTileLocationOpen(new Location((int)possiblePoint.X, (int)possiblePoint.Y)) && !farmer.currentLocation.isTileOccupiedForPlacement(possiblePoint))
                    {
                        break;
                    }
                    possiblePoint = new Vector2(farmer.getTileLocation().X + (float)((Game1.random.NextDouble() < 0.5) ? Game1.random.Next(-5, -1) : Game1.random.Next(2, 6)), farmer.getTileLocation().Y + (float)((Game1.random.NextDouble() < 0.5) ? Game1.random.Next(-5, -1) : Game1.random.Next(2, 6)));
                }
                if (tries < 6)
                {
                    BNC_Core.Logger.Log("Found Spot", StardewModdingAPI.LogLevel.Debug);
                    this.teleporting = true;
                    this.teleportationPath = Utility.GetPointsOnLine((int)farmer.getTileLocation().X, (int)farmer.getTileLocation().Y, (int)possiblePoint.X, (int)possiblePoint.Y, ignoreSwap: true).GetEnumerator();
                }
            }

            BNC_Core.Logger.Log("Run Teleport", StardewModdingAPI.LogLevel.Debug);
            if (this.teleporting)
            {
                BNC_Core.Logger.Log("is Teleporting", StardewModdingAPI.LogLevel.Debug);
                if (this.teleportationPath.MoveNext())
                {
                    BNC_Core.Logger.Log("Move", StardewModdingAPI.LogLevel.Debug);
                    farmer.Position = new Vector2(this.teleportationPath.Current.X * 64 + 4, this.teleportationPath.Current.Y * 64 - 32 - 4);
                }
                else
                {
                    BNC_Core.Logger.Log("Stop Teleport", StardewModdingAPI.LogLevel.Debug);
                    this.teleporting = false;
                }
            }


            return ActionResponse.Done;
        }

        public override ActionResponse UpdateMonster(WarpedEventArgs e, Monster npc) { return ActionResponse.Done; }

        public override ActionResponse WarpLocation(WarpedEventArgs e) { return ActionResponse.Done; }
    }
}
