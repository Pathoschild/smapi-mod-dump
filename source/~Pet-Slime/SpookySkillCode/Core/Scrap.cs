/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using BirbCore.Attributes;
using HarmonyLib;
using Microsoft.Xna.Framework;
using MoonShared;
using SpaceCore;
using SpaceCore.Interface;
using SpookySkillCode.Objects;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using StardewValley.GameData.Objects;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using xTile.Dimensions;
using static SpaceCore.Skills;
using Log = BirbCore.Attributes.Log;

namespace SpookySkillCode.Core
{
    internal class Scrap
    {

        ///The methods in here are not used anymore, but I don't want to loose them as they might be useful in the future.
        ///also cause some of these took hours of work to figure out so I dont want to do that again if I change my mind.
        ///

        #region Light mechanics

        ///This region is about finding the light level the player is in
        ///Which I was able to do. The issue came from finding out WHEN I should try to find if the player was near a light source
        ///I couldn't find a good way to do it.
        ///Mainly since there is no link to like a general "light level"
        ///I kinda found a solution, but I dislike it which is why it's here in the scrap yard for now.

        public static int IsThePlayerInLight(Farmer who, int valueChange)
        {
            int startingBonus = 10;
            var location = who.currentLocation;

            // The mines is a dark place, calculate light levels there
            if (location is MineShaft mines && mines.isLightingDark.Value == true)
            {
                Log.Trace("Return true on Mines");
                startingBonus -= GetLightBonus(Game1.currentLightSources, who, startingBonus, -1);
                return valueChange += startingBonus;
            }

            // Calculate light in the volcano dungeon
            if (location is VolcanoDungeon)
            {
                Log.Trace("Return true on Volcano");
                startingBonus -= GetLightBonus(Game1.currentLightSources, who, startingBonus, -1);
                return valueChange += startingBonus;
            }

            // Try to calculate indoor places. if the place is meant to be bright, it will have a color of white, black, or the default
            if (RBGTest(location))
            {
                Log.Trace("Return true on RBGTest");
                startingBonus -= GetLightBonus(Game1.currentLightSources, who, startingBonus, -2);
                return valueChange += startingBonus;
            }

            // See if it's dark out
            if (Game1.isDarkOut(location))
            {
                Log.Trace("Return true on is dark");
                startingBonus -= GetLightBonus(Game1.currentLightSources, who, startingBonus, -2);
                return valueChange += startingBonus;
            }

            // If none of  above is true, assume the player is stealing during the day and give them -20 to their roll
            startingBonus -= 20;
            return valueChange += startingBonus;
        }

        public static int GetLightBonus(HashSet<LightSource> lightsources, Farmer who, int startingBonus, int hurtValue)
        {
            int bonus = startingBonus;
            foreach (var test in lightsources)
            {
                var lightVector = new Vector2(test.position.X, test.position.Y);
                bool inTheLight = IsInLight(lightVector, (int)(test.radius.Value * 64f * 4f), who);

                Log.Trace(" ");
                Log.Trace($"is the player in a light source: {inTheLight} ");
                Log.Trace(" ");
                bonus -= inTheLight ? hurtValue : 0;
            }
            return bonus;
        }

        public static bool RBGTest(GameLocation location)
        {
            if (location.IsOutdoors && Game1.ambientLight == Color.White)
                return false;

            return !(Game1.ambientLight == Color.Black || Game1.ambientLight == new Color(100, 120, 30));
        }


        public static bool IsInLight(Vector2 positionNonTile, int acceptableDistanceFromScreen, Farmer who)
        {
            positionNonTile.X -= who.StandingPixel.X;
            positionNonTile.Y -= who.StandingPixel.Y;
            if (positionNonTile.X > (float)(-acceptableDistanceFromScreen) && positionNonTile.X < (float)(acceptableDistanceFromScreen) && positionNonTile.Y > (float)(-acceptableDistanceFromScreen))
            {
                return positionNonTile.Y < acceptableDistanceFromScreen;
            }

            return false;
        }

        #endregion
    }
}
