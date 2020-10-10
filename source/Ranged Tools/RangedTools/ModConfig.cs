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
        public int SeedRange { get; set; } = 1;
        public int ObjectPlaceRange { get; set; } = 1;
        
        public bool AxeUsableOnPlayerTile { get; set; } = false;
        public bool PickaxeUsableOnPlayerTile { get; set; } = true;
        public bool HoeUsableOnPlayerTile { get; set; } = true;
        
        public bool ToolAlwaysFaceClick { get; set; } = true;
        public bool WeaponAlwaysFaceClick { get; set; } = true;
        
        public bool CustomRangeOnClickOnly { get; set; } = true;
    }
}
