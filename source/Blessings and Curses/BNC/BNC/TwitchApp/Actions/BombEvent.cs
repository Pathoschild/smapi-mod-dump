/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GenDeathrow/SDV_BlessingsAndCurses
**
*************************************************/

using System;
using System.Collections.Concurrent;
using BNC.TwitchApp;
using BNC.TwitchApp.Actions;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using StardewValley;
using static BNC.BuffManager;

namespace BNC.Actions
{

    public class BombEvent : BaseAction, ITickable
    {
        public static System.Collections.Concurrent.ConcurrentQueue<Item> updateQueue = new ConcurrentQueue<Item>();

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "bombtype")]
        public String bombtype;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "cnt")]
        public int cnt;

        private int currentCnt = 0;


        public override ActionResponse Handle()
        {
            int index;
            switch (bombtype)
            {
                case "bomb":
                    index = 287;
                    break;
                case "mega_bomb":
                    index = 288;
                    break;
                default:
                    index = 286;
                    break;
            }

            Farmer who = Game1.player;
            for (int i = 0; i < cnt; i++)
            {
                Item bomb = Utility.getItemFromStandardTextDescription($"O {index} 1", who);

                BombEvent.updateQueue.Enqueue(bomb);
            }
            BNC_Core.Logger.Log($"list: {BombEvent.updateQueue.Count}", StardewModdingAPI.LogLevel.Debug);
            return ActionResponse.Done;
        }

        public static ActionResponse UpdateTick()
        {

            if (!BombEvent.updateQueue.TryDequeue(out Item bomb))
                return ActionResponse.Retry;

            Farmer who = Game1.player;

            int x = (int)(Game1.random.Next(1) - 1);
            int y = (int)(Game1.random.Next(1) - 1);
            if (x == 0 & y == 0) x = (int)who.position.X + 1;
            else x = (int)who.position.X + x;
            y = (int)who.position.Y + y;

            Vector2 tileLocation = new Vector2(x / 64, y / 64);
            if (((StardewValley.Object)bomb).placementAction(who.currentLocation, x, y, Game1.player))
            {
                return ActionResponse.Done;
            }
            else
            {
                BombEvent.updateQueue.Enqueue(bomb);
                return ActionResponse.Retry;
            }

        }

        private static Vector2 getRangeFromPlayer(int range, int minRange = 3)
        {
            int xStart = Game1.player.getTileX() - range;
            int yStart = Game1.player.getTileY() - range;

            int randX = Game1.random.Next(range * 2 + 2);
            int randY = Game1.random.Next(range * 2 + 2);


            Vector2 vector = new Vector2(xStart + randX, yStart + randY);
            while (Vector2.Distance(vector, Game1.player.getTileLocation()) < minRange)
            {
                vector.X = xStart + Game1.random.Next(range * 2 + 2);
                vector.Y = yStart + Game1.random.Next(range * 2 + 2);
            }
            return vector;
        }

        void ITickable.Update()
        {
            throw new NotImplementedException();
        }
    }
}
