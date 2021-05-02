/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomTV
{
    public class TVChannel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Intro { get; set; }

        public TemporaryAnimatedSprite IntroScreen { get; set; }

        public TemporaryAnimatedSprite IntroOverlay { get; set; }

        public int[] IntroScreenOffset { get; set; } = new[] { 0, 0 };

        public int[] IntroOverlayOffset { get; set; } = new[] { 0, 0 };
        public string[] Seasons { get; set; } = new string[4] { "spring", "summer", "fall", "winter" };
        public string[] Days { get; set; } = new string[7] { "mon", "tue", "wed", "thu", "fri", "sat", "sun" };

        public bool Random { get; set; } = false;

        public string IntroMusic { get; set; } = "none";
    }
}
