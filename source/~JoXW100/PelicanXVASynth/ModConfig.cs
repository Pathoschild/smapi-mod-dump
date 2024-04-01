/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

namespace PelicanXVASynth
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public string NPCGameVoices { get; set; } = "";
        public int MaxSecondsWait { get; set; } = 60;
        public int MillisecondsPrepare { get; set; } = 500;
        public int MaxLettersToPrepare { get; set; } = 20;
    }
}
