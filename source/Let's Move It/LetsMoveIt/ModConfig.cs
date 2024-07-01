/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Exblosis/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;

namespace LetsMoveIt
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public bool CopyMode { get; set; } = false;
        public bool MultiSelect { get; set; } = false;

        //Switch Components
        public bool MoveCropWithoutTile { get; set; } = true;
        public bool MoveCropWithoutIndoorPot { get; set; } = false;

        //Enable|Disable Components
        public bool EnableMoveBuilding { get; set; } = true;
        public bool EnableMoveEntity { get; set; } = true;
        public bool EnableMoveCrop { get; set; } = true;
        //Objects
        public bool EnableMoveObject { get; set; } = true;
        public bool EnableMovePlaceableObject { get; set; } = true;
        public bool EnableMoveCollectibleObject { get; set; } = true;
        public bool EnableMoveGeneratedObject { get; set; } = true;
        //Resource Clumps
        public bool EnableMoveResourceClump { get; set; } = true;
        public bool EnableMoveGiantCrop { get; set; } = true;
        public bool EnableMoveStump { get; set; } = true;
        public bool EnableMoveHollowLog { get; set; } = true;
        public bool EnableMoveBoulder { get; set; } = true;
        public bool EnableMoveMeteorite { get; set; } = true;
        //Terrain Features
        public bool EnableMoveTerrainFeature { get; set; } = true;
        public bool EnableMoveFlooring { get; set; } = true;
        public bool EnableMoveTree { get; set; } = true;
        public bool EnableMoveFruitTree { get; set; } = true;
        public bool EnableMoveGrass { get; set; } = true;
        public bool EnableMoveFarmland { get; set; } = true;
        public bool EnableMoveBush { get; set; } = true;

        //Sound
        public string Sound { get; set; } = "shwip";

        //Keybinding
        public SButton ModKey { get; set; } = SButton.LeftAlt;
        public SButton MoveKey { get; set; } = SButton.MouseLeft;
        public SButton OverwriteKey { get; set; } = SButton.LeftControl;
        public SButton CancelKey { get; set; } = SButton.Escape;
        public SButton RemoveKey { get; set; } = SButton.Delete;
        public SButton ToggleCopyModeKey { get; set; } = SButton.None;
        public SButton ToggleMultiSelectKey { get; set; } = SButton.None;
    }
}
