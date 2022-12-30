/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/andraemon/SlimeProduce
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlimeProduce
{
    public static class StringyStuff
    {
        public static string ToSlimeString(GreenSlime slime)
        {
            bool isTiger = slime.Name == "Tiger Slime";
            Color color = slime.color.Value;

            if (isTiger) color = new Color(255, 128, 0);

            return $"{color.PackedValue}/{isTiger}/{slime.firstGeneration.Value}/{slime.specialNumber.Value}";
        }

        public static bool TryParseSlimeString(string slimeData, out Color color, out bool isTiger, out bool firstGeneration, out int specialNumber)
        {
            color = Color.White;
            isTiger = false;
            firstGeneration = false;
            specialNumber = 0;

            string[] data;

            if (slimeData == null || (data = slimeData.Split('/')).Length != 4)
                return false;
            
            if (uint.TryParse(data[0], out uint packed)) color = new Color(packed);
            else return false;

            if (!bool.TryParse(data[1], out isTiger)) return false;

            if (!bool.TryParse(data[2], out firstGeneration)) return false;

            if (!int.TryParse(data[3], out specialNumber)) return false;

            return true;
        }

        public static bool TryGetSlimeColor(string slimeData, out Color color)
        {
            color = Color.White;
            string[] data;

            if (slimeData == null || (data = slimeData.Split('/')).Length != 4)
                return false;

            if (uint.TryParse(data[0], out uint packed))
            {
                color = new Color(packed);
                return true;
            }

            return false;
        }
    }
}
