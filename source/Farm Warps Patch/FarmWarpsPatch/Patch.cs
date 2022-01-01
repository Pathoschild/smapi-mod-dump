/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/FarmWarpsPatch
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;
using xTile.Dimensions;

namespace FarmWarpsPatch
{
    public class Patch
    {
        private static IMonitor Monitor;

        public static void initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

        public static bool island_performAction_prefix(string action, Farmer who, Location tileLocation, bool __result)
        {
            //only override obelisk action
            if(action == "FarmObelisk")
            {
                //display stuff from vanilla
                //not sure what exactly all of it does
                for (int index = 0; index < 12; ++index)
                    who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(354, (float)Game1.random.Next(25, 75), 6, 1, new Vector2((float)Game1.random.Next((int)who.Position.X - 256, (int)who.Position.X + 192), (float)Game1.random.Next((int)who.Position.Y - 256, (int)who.Position.Y + 192)), false, Game1.random.NextDouble() < 0.5));
                who.currentLocation.playSound("wand", NetAudio.SoundContext.Default);
                Game1.displayFarmer = false;
                Game1.player.temporarilyInvincible = true;
                Game1.player.temporaryInvincibilityTimer = -2000;
                Game1.player.freezePause = 1000;
                Game1.flashAlpha = 1f;

                //the IMPORTANT BIT! actually warp the farmer and show fade-in
                DelayedAction.fadeAfterDelay((Game1.afterFadeFunction)(() =>{ warpToFarm(who); }), 1000);

                //more stuff from vanilla
                //not really sure what it does
                new Microsoft.Xna.Framework.Rectangle(who.GetBoundingBox().X, who.GetBoundingBox().Y, 64, 64).Inflate(192, 192);
                int num = 0;
                for (int index = who.getTileX() + 8; index >= who.getTileX() - 8; --index)
                {
                    who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2((float)index, (float)who.getTileY()) * 64f, Color.White, 8, false, 50f, 0, -1, -1f, -1, 0)
                    {
                        layerDepth = 1f,
                        delayBeforeAnimationStart = num * 25,
                        motion = new Vector2(-0.25f, 0.0f)
                    });
                    ++num;
                }

                //set the return value to true and skip the original code
                //prevents duplicate warping and other shenanigans
                __result = true;
                return false;
            }

            //some other action, ignore and continue to the original code
            return true;
        }

        private static void warpToFarm(Farmer who)
        {
            //default coords for default, forest, hilltop, riverlands, & wilderness
            int default_x = 48;
            int default_y = 7;
            switch (Game1.whichFarm)
            {
                //four corners
                case 5:
                    default_x = 48;
                    default_y = 39;
                    break;
                //beach
                case 6:
                    default_x = 82;
                    default_y = 29;
                    break;
            }

            //get the warp totem entry and default to hardcoded coords
            Point propertyPosition = Game1.getFarm().GetMapPropertyPosition("WarpTotemEntry", default_x, default_y);

            //warp
            Game1.warpFarmer("Farm", propertyPosition.X, propertyPosition.Y, false);

            //display stuff from vanilla
            Game1.fadeToBlackAlpha = 0.99f;
            Game1.screenGlow = false;
            Game1.player.temporarilyInvincible = false;
            Game1.player.temporaryInvincibilityTimer = 0;
            Game1.displayFarmer = true;
        }
    }
}
