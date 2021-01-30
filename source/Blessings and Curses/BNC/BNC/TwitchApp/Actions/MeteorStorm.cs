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
using Newtonsoft.Json;
using StardewModdingAPI;
using BNC.TwitchApp;
using StardewValley;
using Microsoft.Xna.Framework;
using System.Collections.Concurrent;
using BNC.TwitchApp.Actions;

namespace BNC.Actions
{
    class MeteorStorm : BaseAction, ITickable
    {
        public static System.Collections.Concurrent.ConcurrentQueue<Meteor> updateQueue = new ConcurrentQueue<Meteor>();

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "stormtype")]
        public String stormtype;

        public static void Init() { }

        public override ActionResponse Handle()
        {
            try
            {
                int cnt = 0;
                switch (stormtype)
                {
                    case "small":
                        cnt = 15;
                        break;
                    case "medium":
                        cnt = 35;
                        break;
                    case "large":
                        cnt = 60;
                        break;
                    case "apocalyptic":
                        cnt = 100;
                        break;
                    default:
                        cnt = 35;
                        break;
                }


                BNC_Core.Logger.Log($"Spawning Meteor Storm:{from} from Actions", LogLevel.Error);

                for (int i = 0; i < cnt; i++)
                {
                    Meteor projectile = new Meteor(getRangeFromViewPort(800, 400), Game1.random.Next(100, 600), Game1.player.currentLocation);

                    projectile.height.Value = 24f;
                    projectile.ignoreMeleeAttacks.Value = true;
                    projectile.hostTimeUntilAttackable = 0.1f;

                    MeteorStorm.updateQueue.Enqueue(projectile);
                }

                return ActionResponse.Done;
            }
            catch (ArgumentNullException)
            {
                BNC_Core.Logger.Log($"Error trying to Meteor Storm from Actions", LogLevel.Error);
                return ActionResponse.Done;
            }

        }
        public static int tick = 0;

        public static int lastPlayed = 0;
        public static ActionResponse UpdateTick()
        {

            if (tick++ > 2)
                return ActionResponse.Retry;
            else tick = 0;


            for (int i = 0; i < Game1.random.Next(6)+1; i++)
            {
                if (!MeteorStorm.updateQueue.TryDequeue(out Meteor meteor))
                    return ActionResponse.Retry;

                meteor.SetPosition(getRangeFromViewPort(800, 400));
                Game1.player.currentLocation.projectiles.Add(meteor);

                if (lastPlayed-- <= 0)
                {
                    Game1.player.currentLocation?.playSound("fireball");
                    lastPlayed = 100;
                }
            }
            


            return ActionResponse.Done;
        }

        private static Vector2 getRangeFromViewPort(int rangeY, int rangeX, int minRange = 3)
        {
            int xStart = (int)Game1.viewport.X;
            int yStart = (int)Game1.viewport.Y;

            int randX = -Game1.random.Next(rangeX * 2 + 2);
            int randY = -Game1.random.Next(rangeY * 2 + 2);

            Vector2 vector = new Vector2(xStart + randX, yStart + randY);
            return vector;
        }

        public void Update()
        {
            
        }
    }
}

