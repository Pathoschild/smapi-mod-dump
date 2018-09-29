namespace ExtendedFridge
{
    internal class FridgeModConfig
    {
        public string fridgePrevPageKey {get; set;}
        public string fridgeNextPageKey {get; set;}
        public bool autoSwitchPageOnGrab {get; set;}

        public FridgeModConfig()
        {
            fridgeNextPageKey = "Right";
            fridgePrevPageKey = "Left";
            autoSwitchPageOnGrab = true;
        }
    }
}