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
