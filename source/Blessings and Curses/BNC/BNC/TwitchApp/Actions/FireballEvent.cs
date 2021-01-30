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
using StardewValley.Projectiles;

namespace BNC.Actions
{
    class FireballEvent : BaseAction, ITickable
    {
        public static System.Collections.Concurrent.ConcurrentQueue<BasicProjectile> updateQueue = new ConcurrentQueue<BasicProjectile>();
        public static int fireballsLeft = 0;
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
                    case "easy":
                        cnt = 25;
                        break;
                    case "normal":
                        cnt = 50;
                        break;
                    case "hard":
                        cnt = 100;
                        break;
                    case "insane":
                        cnt = 200;
                        break;
                    default:
                        cnt = 35;
                        break;
                }


                BNC_Core.Logger.Log($"Spawning Fireball Event:{from} from Actions", LogLevel.Error);


                fireballsLeft += cnt;

                return ActionResponse.Done;
            }
            catch (ArgumentNullException)
            {
                BNC_Core.Logger.Log($"Error trying to trigger FireBall from Actions", LogLevel.Error);
                return ActionResponse.Done;
            }

        }
        public static int tick = 0;

        public static ActionResponse UpdateTick()
        {

            if (tick++ > 2)
                return ActionResponse.Retry;
            else tick = 0;

            BNC_Core.Logger.Log($" fireballs {fireballsLeft} ", LogLevel.Debug);

            for (int i = 0; i < Game1.random.Next(20); i++)
            {
                if (FireballEvent.fireballsLeft-- <= 0)
                {
                    FireballEvent.fireballsLeft = 0;
                    return ActionResponse.Retry;
                }

                BNC_Core.Logger.Log($" Try to spawn{fireballsLeft} ", LogLevel.Debug);

                Vector2 startPosition = getStartingLocation();
                Vector2 trajectory2 = Utility.getVelocityTowardPoint(startPosition, new Vector2(Game1.player.GetBoundingBox().X, Game1.player.GetBoundingBox().Y) + new Vector2(Game1.random.Next(-128, 128)), 8f);
                BasicProjectile fireball = new BasicProjectile(15, 10, 8, 4, 0f, trajectory2.X, trajectory2.Y, startPosition, "", "", true, false, Game1.player.currentLocation, Game1.player);
                fireball.height.Value = 48f;

                    Game1.player.currentLocation.projectiles.Add(fireball);
                    Game1.player.currentLocation?.playSound("fireball");

            }

            BNC_Core.Logger.Log($" Done {fireballsLeft} ", LogLevel.Debug);
            return ActionResponse.Done;
        }


        private static Vector2 getStartingLocation()
        {
            //return Vector2.Zero;
            //return Game1.player.position;
            int x = Game1.viewport.X;
            int y = Game1.viewport.Y;

            switch (Game1.random.Next(3))
            {
                case 0:
                    x = Game1.viewport.X;
                    y = Game1.viewport.Y + Game1.random.Next(Game1.viewport.Height-1);
                    break;
                case 1:
                    x = Game1.viewport.X + Game1.random.Next(Game1.viewport.Width - 1);
                    y = Game1.viewport.Y;
                    break;
                case 2:
                    x = Game1.viewport.X - Game1.random.Next(Game1.viewport.Width - 1);
                    y = Game1.viewport.Height;
                    break;
                case 3:
                    x = Game1.viewport.Width;
                    y = Game1.viewport.Y + Game1.random.Next(Game1.viewport.Height - 1);
                    break;
            }

            return new Vector2(x,y);
        }

        private static Vector2 getRangeFromViewPort(int range, int minRange = 3)
        {
            int xStart = (int)Game1.viewport.X;
            int yStart = (int)Game1.viewport.Y;

            int randX = -Game1.random.Next(range * 2 + 2);
            int randY = -Game1.random.Next(range * 2 + 2);

            Vector2 vector = new Vector2(xStart + randX, yStart + randY);
            return vector;
        }

        public void Update()
        {
            
        }
    }
}

