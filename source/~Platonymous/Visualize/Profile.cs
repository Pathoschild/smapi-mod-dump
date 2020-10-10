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

namespace Visualize
{
    public class Profile
    {
        public string name { get; set; } = "Visualize Profile";
        public string author { get; set; } = "none";
        public string version { get; set; } = "1.0.0";
        public string id { get; set; } = "auto";
        public float saturation { get; set; } = 100;
        public int[] tint { get; set; } = new int[] { 255, 255, 255, 255 };
        public string palette { get; set; } = "none";
    }
}
