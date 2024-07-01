/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlowBuff
{
    public static class ColorParser
    {
        public static Color Read(string s)
        {
            string[] parts = s.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (parts.Length == 3)
                return new Color(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]));
            if (parts.Length == 4)
                return new Color(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));

            ModEntry.Instance.Monitor.Log($"'{s}' is not a valid color format, using a white color as a default", LogLevel.Error);
            return Color.White;
        }

        public static string Write(Color c)
        {
            StringBuilder sb = new();
            sb.Append(c.R).Append(c.G).Append(c.B);

            if (c.A != 255)
                sb.Append(c.A);

            return sb.ToString();
        }
    }
}
