/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Linq;
using HarmonyLib;
using StardewModdingAPI;

namespace Snake
{
    class SnakeMachine
    {
        public SnakeMachine()
        {
        }

        public static void start(IModHelper helper)
        {
            Game1.currentMinigame = new SnakeMinigame(helper);
        }

        public static StardewValley.Object GetNew(StardewValley.Object alt)
        {
            if (Game1.bigCraftablesInformation.Values.Any(v => v.Contains("Snake Arcade Machine"))){
                var obj = new StardewValley.Object(Vector2.Zero, Game1.bigCraftablesInformation.FirstOrDefault(b => b.Value.Contains("Snake Arcade Machine")).Key, false);
                return obj;
            }

            return alt;
        }
    }
}
