/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/


using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace UtilityGrid
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;

        public SButton ToggleGrid { get; set; } = SButton.Home;
        public SButton SwitchGrid { get; set; } = SButton.Delete;
        public SButton SwitchTile { get; set; } = SButton.PageUp;
        public SButton RotateTile { get; set; } = SButton.PageDown;
        public SButton PlaceTile { get; set; } = SButton.MouseLeft;
        public SButton DestroyTile { get; set; } = SButton.MouseRight;
        public Color WaterColor { get; set; } = Color.Aqua;
        public Color UnpoweredGridColor { get; set; } = Color.White;
        public Color ElectricityColor { get; set; } = Color.Yellow;
        public Color InsufficientColor { get; set; } = Color.Red;
        public Color ShadowColor { get; set; } = Color.Black;
        public int PipeCostGold { get; set; } = 100;
        public int PipeDestroyGold { get; set; } = 50;
        public string PipeCostItems { get; set; } = "378:2";
        public string PipeDestroyItems { get; set; } = "378:1";
        public string PipeSound { get; set; } = "dirtyHit";
        public string DestroySound { get; set; } = "axe";
    }
}
