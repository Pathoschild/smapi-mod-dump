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
using System.Collections.Generic;

namespace PyTKLite
{
    public class ColorManipulation
    {
        public float saturation;
        public float light;
        public List<Color> palette;

        public ColorManipulation(List<Color> palette, float saturation = 100, float light = 100)
        {
            this.saturation = saturation;
            this.light = light;
            this.palette = palette;
        }

        public ColorManipulation(float saturation = 100, float light = 100)
        {
            this.saturation = saturation;
            this.light = light;
            palette = new List<Color>();
        }
    }
}
