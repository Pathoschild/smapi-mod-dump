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

namespace SlimeProduce
{
    public class ColorRange
    {
        public int[] Red { get; set; }
        public int[] Green { get; set; }
        public int[] Blue { get; set; }

        public ColorRange(int[] R, int[] G, int[] B)
        {
            Red = R;
            Green = G;
            Blue = B;
        }

        public bool Contains(Color c)
        {
            if (c.R >= Red[0] && c.R <= Red[1])
            {
                if (c.G >= Green[0] && c.G <= Green[1])
                {
                    if (c.B >= Blue[0] && c.B <= Blue[1])
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
