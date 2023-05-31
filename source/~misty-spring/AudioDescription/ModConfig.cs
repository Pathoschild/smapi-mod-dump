/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

namespace AudioDescription
{
    internal class ModConfig
    {
        public string Type { get; set; } = "HUDMessage";
        public string Blacklist { get; set; } 
        public int CoolDown { get; set; } = 5;
		public int YOffset { get; set; } = 0;
		public int XOffset { get; set; } = 0;
        public bool PlayerSounds { get; set; } = false;
        public bool NPCs { get; set; } = true;
        public bool Environment { get; set; } = true;
        public bool ItemSounds { get; set; } = false;
        public bool FishingCatch { get; set; } = true;
		public bool Minigames { get; set; } = false;
    }
}