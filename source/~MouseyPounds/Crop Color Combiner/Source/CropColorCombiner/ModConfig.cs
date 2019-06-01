using StardewModdingAPI;

namespace CropColorCombiner
{
    class ModConfig
    {
        public SButton Unify_Color_Keybind { get; set; } = SButton.OemOpenBrackets;
        public bool Unify_Color_For_Flowers { get; set; } = true;
        public bool Unify_Color_For_Everything_Else { get; set; } = false;
        public SButton Reduce_Quality_Keybind { get; set; } = SButton.OemCloseBrackets;
        public bool Reduce_Quality_For_Flowers { get; set; } = true;
        public bool Reduce_Quality_For_Everything_Else { get; set; } = false;

    }
}
