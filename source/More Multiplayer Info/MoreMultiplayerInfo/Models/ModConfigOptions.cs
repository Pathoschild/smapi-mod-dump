namespace MoreMultiplayerInfo
{
    public class ModConfigOptions
    {

        [OptionDisplay("Show Inventory")]
        public bool ShowInventory { get; set; } = true;


        [OptionDisplay("Show Info in Text Box")]
        public bool ShowReadyInfoInChatBox { get; set; } = true;


        [OptionDisplay("Last Player Alert")]
        public bool ShowLastPlayerReadyInfoInChatBox { get; set; } = true;

        [OptionDisplay("Hide in Single Player")]
        public bool HideInSinglePlayer { get; set; } = false;

        [OptionDisplay("Show Cutscene Alerts")]
        public bool ShowCutsceneInfoInChatBox { get; set; }
    }
}