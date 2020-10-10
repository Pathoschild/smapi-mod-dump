/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Jinxiewinxie/StardewValleyMods
**
*************************************************/

namespace WonderfulFarmLife
{
    /// <summary>The mod configuration model.</summary>
    public class ModConfig
    {
        /*********
        ** Accessors
        *********/
        public bool EditPath { get; set; } = true;

        public bool RemovePathAlltogether { get; set; }

        public bool ShowFarmStand { get; set; } = true;

        public bool RemoveShippingBin { get; set; } = true;

        public bool ShowPatio { get; set; } = true;

        public bool ShowBinClutter { get; set; } = true;

        public bool AddDogHouse { get; set; } = true;

        public bool AddGreenHouseArch { get; set; } = true;

        public bool AddTelescopeArea { get; set; } = true;

        public bool ShowTreeSwing { get; set; } = true;

        public bool ShowPicnicTable { get; set; } = true;

        public bool ShowPicnicBlanket { get; set; } = true;

        public bool AddStoneBridge { get; set; } = true;

        public bool ShowMemorialArea { get; set; } = true;

        public bool ShowMemorialAreaArch { get; set; } = true;

        public bool UsingTSE { get; set; } = true;
    }
}