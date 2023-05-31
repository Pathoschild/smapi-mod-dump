/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
namespace MultiplayerMod.Framework.Patch.Mobile
{
    internal class UtilityPatch
    {
        public static int To4(int v)
        {
            int num = v % 4;
            int num2 = v - num;
            if (num > 2)
            {
                num2 += 4;
            }
            return num2;
        }

        public static Vector2 To4(Vector2 v)
        {
            float num = (float)UtilityPatch.To4((int)v.X);
            int num2 = UtilityPatch.To4((int)v.Y);
            return new Vector2(num, (float)num2);
        }

        public static Rectangle To4(Rectangle v)
        {
            int x = UtilityPatch.To4(v.X);
            int y = UtilityPatch.To4(v.Y);
            int width = UtilityPatch.To4(v.Width);
            int height = UtilityPatch.To4(v.Height);
            return new Rectangle(x, y, width, height);
        }
        public static int CompareGameVersions(string version, string other_version, bool ignore_platform_specific = false, bool major_only = false)
        {
            string[] array = version.Split('.', StringSplitOptions.None);
            string[] array2 = other_version.Split('.', StringSplitOptions.None);
            if (major_only && array[0] == array2[0] && array[1] == array2[1])
            {
                return 0;
            }
            for (int i = 0; i < Math.Max(array.Length, array2.Length); i++)
            {
                float num = 0f;
                float num2 = 0f;
                if (i < array.Length)
                {
                    float.TryParse(array[i], out num);
                }
                if (i < array2.Length)
                {
                    float.TryParse(array2[i], out num2);
                }
                if (num != num2 || (i == 2 && ignore_platform_specific))
                {
                    return num.CompareTo(num2);
                }
            }
            return 0;
        }
    }
}
