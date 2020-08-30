using StardewModdingAPI;


namespace ExpandedFridge
{
    /// <summary>
    /// Stores options for the Manager.
    /// </summary>
    public class ModConfig
    {
        public bool HideMiniFridges { get; set; } = true;
        public SButton NextFridgeTabButton { get; set; } = SButton.RightTrigger;
        public SButton LastFridgeTabButton { get; set; } = SButton.LeftTrigger;
    }
}
