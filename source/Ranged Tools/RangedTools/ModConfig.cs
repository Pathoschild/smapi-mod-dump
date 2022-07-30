/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/vgperson/RangedTools
**
*************************************************/

namespace RangedTools
{
    class ModConfig
    {
        public int AxeRange { get; set; } = 1;
        public int PickaxeRange { get; set; } = 1;
        public int HoeRange { get; set; } = 1;
        public int WateringCanRange { get; set; } = 1;
        public int ScytheRange { get; set; } = 1;
        public int WeaponRange { get; set; } = 1;
        public int SeedRange { get; set; } = 1;
        public int ObjectPlaceRange { get; set; } = 1;
        
        public bool CenterScytheOnCursor { get; set; } = false;
        public bool CenterWeaponOnCursor { get; set; } = false;
        
        public bool AxeUsableOnPlayerTile { get; set; } = false;
        public bool PickaxeUsableOnPlayerTile { get; set; } = true;
        public bool HoeUsableOnPlayerTile { get; set; } = true;
        
        public bool ToolAlwaysFaceClick { get; set; } = true;
        public bool WeaponAlwaysFaceClick { get; set; } = true;
        
        public int ToolHitLocationDisplay { get; set; } = 1;
        
        public bool UseHalfTilePositions { get; set; } = true;
        public bool AllowRangedChargeEffects { get; set; } = false;
        public bool AttacksIgnoreObstacles { get; set; } = false;
        public bool CustomRangeOnClickOnly { get; set; } = true;
    }
}
