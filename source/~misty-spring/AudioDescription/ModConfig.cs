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
        public int CoolDown { get; set; } = 5;
        public bool PlayerSounds { get; set; } = false;
        public bool NPCs { get; set; } = true;
        public bool Environment { get; set; } = true;
        public bool ItemSounds { get; set; } = false;
        public bool FishingCatch { get; set; } = true;
    }
}