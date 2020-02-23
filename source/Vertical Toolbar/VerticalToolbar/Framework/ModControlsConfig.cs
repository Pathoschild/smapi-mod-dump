using StardewModdingAPI;

namespace SB_VerticalToolMenu
{
    /// <summary>The mod configuration model for input bindings.</summary>
    internal class ModControlsConfig
    {
        /// <summary>The key which must be held to activate the toolbar hotkeys.</summary>
        public SButton HoldToActivateSlotKeys { get; set; } = SButton.LeftControl;

        /// <summary>The key to select the first toolbar slot.</summary>
        public SButton ChooseSlot1 { get; set; } = SButton.NumPad1;

        /// <summary>The key to select the second toolbar slot.</summary>
        public SButton ChooseSlot2 { get; set; } = SButton.NumPad2;

        /// <summary>The key to select the third toolbar slot.</summary>
        public SButton ChooseSlot3 { get; set; } = SButton.NumPad3;

        /// <summary>The key to select the fourth toolbar slot.</summary>
        public SButton ChooseSlot4 { get; set; } = SButton.NumPad4;

        /// <summary>The key to select the fifth toolbar slot.</summary>
        public SButton ChooseSlot5 { get; set; } = SButton.NumPad5;

        /// <summary>The key to move the slot selection to the left.</summary>
        public SButton ScrollLeft { get; set; } = SButton.LeftTrigger;

        /// <summary>The key to move the slot selection to the right.</summary>
        public SButton ScrollRight { get; set; } = SButton.LeftTrigger;

        /// <summary> Sets the orientation of the vertical toolbar</summary>
        public SB_VerticalToolMenu.Framework.Orientation Orientation = Framework.Orientation.LeftOfToolbar;
        //
    }
}
