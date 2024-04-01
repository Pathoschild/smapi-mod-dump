/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/


using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace OmniTools
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public bool SmartSwitch { get; set; } = true;
        public bool FromWeapon { get; set; } = true;
        public SButton ModButton { get; set; } = SButton.LeftAlt;
        public SButton CycleButton { get; set; } = SButton.X;
        public SButton RemoveButton { get; set; } = SButton.Z;
        public SButton ToggleButton { get; set; } = SButton.None;
        public bool ShowNumber { get; set; } = true;
        public Color NumberColor { get; set; } = Color.Pink;
        public bool SwitchForObjects { get; set; } = true;
        public bool SwitchForTrees { get; set; } = true;
        public bool SwitchForResourceClumps { get; set; } = true;
        public bool SwitchForGrass { get; set; } = true;
        public bool SwitchForCrops { get; set; } = true;
        public bool HarvestWithScythe { get; set; } = false;
        public bool SwitchForPan { get; set; } = true;
        public bool SwitchForWateringCan { get; set; } = true;
        public bool SwitchForFishing { get; set; } = true;
        public bool SwitchForWatering { get; set; } = true;
        public bool SwitchForTilling { get; set; } = true;
        public bool SwitchForAnimals { get; set; } = true;
        public bool SwitchForMonsters { get; set; } = true;
        public float MonsterMaxDistance { get; set; } = 256;
    }
}
