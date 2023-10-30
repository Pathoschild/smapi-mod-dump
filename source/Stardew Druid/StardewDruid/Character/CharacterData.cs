/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewDruid.Character
{
    public static class CharacterData
    {

        public static StardewValley.AnimatedSprite CharacterSprite(string characterName)
        {

            StardewValley.AnimatedSprite characterSprite = new("Characters\\Krobus");

            return characterSprite;
        
        }

        public static Texture2D CharacterPortrait(string characterName)
        {

            Texture2D characterPortrait = Game1.content.Load<Texture2D>("Characters\\Krobus");

            return characterPortrait;

        }

        public static Dictionary<int, int[]> CharacterSchedule(string characterName)
        {

            return new Dictionary<int, int[]>();

        }

        public static CharacterDisposition CharacterDisposition(string characterName)
        {

            return new CharacterDisposition()
            {
                Age = 1,
                Manners = 2,
                SocialAnxiety = 1,
                Optimism = 0,
                Gender = 0,
                datable = false,
                Birthday_Season = "fall",
                Birthday_Day = 27,
                id = 18465001,
                speed = 1,

            };

        }

        public static string CharacterDefaultMap(string characterName)
        {

            return "FarmCave";

        }

        public static Vector2 CharacterPosition(string characterName)
        {

            return new Vector2(6, 6) * 64;
        }


    }
}
