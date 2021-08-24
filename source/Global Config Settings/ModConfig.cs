/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Gaphodil/GlobalConfigSettings
**
*************************************************/

using StardewModdingAPI;

namespace GlobalConfigSettings
{
    class ModConfig
    {
        /// <summary>
        /// Reassigns global settings on every save load (and joining multiplayer) instead of farm creation.
        /// </summary>
        public bool ChangeOnEveryLoad { get; set; } = false;

        // In-game settings

        // General
        public bool AutoRun { get; set; } = true;
        public bool ShowPortraits { get; set; } = true;
        public bool ShowMerchantPortraits { get; set; } = true;
        public bool AlwaysShowToolLocation { get; set; } = false;
        public bool HideToolHitLocationWhenMoving { get; set; } = true;

        // options: auto, force_on, force_off
        public string GamepadMode { get; set; } = "auto";
        // options: off, gamepad, both
        public string ItemStowing { get; set; } = "off";
        // options: legacy, [anything else]
        public string SlingshotFireMode { get; set; } = "hold";

        public bool ControllerPlacementTileIndicator { get; set; } = true;
        public bool PauseWhenGameWindowIsInactive { get; set; } = true;
        public bool UseControllerStyleMenus { get; set; } = true;
        public bool ShowAdvancedCraftingInformation { get; set; } = false;

        // Multiplayer

        // Sound
        // options: int in range [0, 100]
        public int MusicVolume { get; set; } = 75;
        public int SoundVolume { get; set; } = 100;
        public int AmbientVolume { get; set; } = 75;
        public int FootstepVolume { get; set; } = 90;

        // options: string in range [-1, 3]
        public string FishingBiteSound = "-1";
        public bool DialogueTypingSound { get; set; } = true;
        public bool MuteAnimalSounds { get; set; } = false;

        // Graphics
        // options: Windowed, Fullscreen, Windowed Borderless
        //public string WindowMode { get; set; } = "off";
        // options: based on xna graphics adapter??? whatever
        //public string Resolution { get; set; } = "off";
        public bool VSync { get; set; } = true;

        // options: string in range [75%, 150%] by multiple of 5
        public string UiScale { get; set; } = "100%";
        public bool MenuBackgrounds { get; set; } = false;
        public bool LockToolbar { get; set; } = false;
        // options: string in range [75%, 200%] by multiple of 5
        public string ZoomLevel { get; set; } = "100%";
        public bool ZoomButtons { get; set; } = false;
        // options: Low, Med., High (Lowest and Ultra in code)
        public string LightingQuality { get; set; } = "Med.";
        // options: int in range [0, 100]; inverse direction of in-game menu
        public int SnowTransparency { get; set; } = 100;
        public bool ShowFlashEffects { get; set; } = true;
        public bool UseHardwareCursor { get; set; } = false;

        // Controls
        public bool ControllerRumble { get; set; } = true;
        public bool InvertToolbarScrollDirection { get; set; } = false;

        public SButton CheckDoAction { get; set; } = SButton.X;
        public SButton UseTool { get; set; } = SButton.C;
        public SButton AccessMenu { get; set; } = SButton.E;
        public SButton AccessJournal { get; set; } = SButton.F;
        public SButton AccessMap { get; set; } = SButton.M;
        public SButton MoveUp { get; set; } = SButton.W;
        public SButton MoveLeft { get; set; } = SButton.A;
        public SButton MoveDown { get; set; } = SButton.S;
        public SButton MoveRight { get; set; } = SButton.D;
        public SButton ChatBox { get; set; } = SButton.T;
        public SButton EmoteMenu { get; set; } = SButton.Y;
        public SButton Run { get; set; } = SButton.LeftShift;
        public SButton ShiftToolbar { get; set; } = SButton.Tab;
        public SButton InventorySlot1 { get; set; } = SButton.D1;
        public SButton InventorySlot2 { get; set; } = SButton.D2;
        public SButton InventorySlot3 { get; set; } = SButton.D3;
        public SButton InventorySlot4 { get; set; } = SButton.D4;
        public SButton InventorySlot5 { get; set; } = SButton.D5;
        public SButton InventorySlot6 { get; set; } = SButton.D6;
        public SButton InventorySlot7 { get; set; } = SButton.D7;
        public SButton InventorySlot8 { get; set; } = SButton.D8;
        public SButton InventorySlot9 { get; set; } = SButton.D9;
        public SButton InventorySlot10 { get; set; } = SButton.D0;
        public SButton InventorySlot11 { get; set; } = SButton.OemMinus;
        public SButton InventorySlot12 { get; set; } = SButton.OemPlus;

    }
}
