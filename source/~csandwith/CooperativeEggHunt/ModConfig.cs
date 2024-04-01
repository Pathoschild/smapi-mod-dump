/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace CooperativeEggHunt
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public int NPCMinEggs { get; set; } = 1;
        public int NPCMaxEggs { get; set; } = 8;
        public int EggsToWin { get; set; } = 35;
        public int EggsPerTalk { get; set; } = 1;
        public int PointsPerEgg { get; set; } = 1000;
        public string NPCHunters { get; set; } = "Maru,Abigail,Jas,Sam,Vincent,Leo";
    }
}
