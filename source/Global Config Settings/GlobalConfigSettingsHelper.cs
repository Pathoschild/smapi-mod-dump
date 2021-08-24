/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Gaphodil/GlobalConfigSettings
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewValley;

namespace GlobalConfigSettings
{
    public class GlobalConfigSettingsHelper
    {
        public enum OptionChangeTypes
        {
            CheckBox,
            DropDown,
            InputListener,
            Slider
        }

        public Dictionary<string, int> OptionToId;
        public Dictionary<string, int> KeyToId;

        public GlobalConfigSettingsHelper()
        {
            OptionToId = new Dictionary<string, int>();
            KeyToId = new Dictionary<string, int>();
            PopulateDictionaries();
        }

        private void PopulateDictionaries()
        {
            OptionToId["AutoRun"] = Options.toggleAutoRun;
            OptionToId["MusicVolume"] = Options.musicVolume;
            OptionToId["SoundVolume"] = Options.soundVolume;
            OptionToId["DialogueTypingSound"] = Options.toggleDialogueTypingSounds;
            //OptionToId[] = Options.toggleFullscreen;
            //OptionToId[] = Options.toggleWindowedOrTrueFullscreen;
            //OptionToId[] = Options.screenResolution;
            OptionToId["ShowPortraits"] = Options.showPortraitsToggle;
            OptionToId["ShowMerchantPortraits"] = Options.showMerchantPortraitsToggle;
            OptionToId["MenuBackgrounds"] = Options.menuBG;
            //OptionToId[] = Options.toggleFootsteps; //unused
            OptionToId["AlwaysShowToolLocation"] = Options.alwaysShowToolHitLocationToggle;
            OptionToId["HideToolHitLocationWhenMoving"] = Options.hideToolHitLocationWhenInMotionToggle;
            //OptionToId[] = Options.windowMode;
            OptionToId["PauseWhenGameWindowIsInactive"] = Options.pauseWhenUnfocused;
            OptionToId["LockToolbar"] = Options.pinToolbar;
            OptionToId["ControllerRumble"] = Options.toggleRumble;
            //OptionToId[] = Options.ambientOnly; //unused
            OptionToId["ZoomLevel"] = Options.zoom;
            OptionToId["ZoomButtons"] = Options.zoomButtonsToggle;
            OptionToId["AmbientVolume"] = Options.ambientVolume;
            OptionToId["FootstepVolume"] = Options.footstepVolume;
            OptionToId["InvertToolbarScrollDirection"] = Options.invertScrollDirectionToggle;
            OptionToId["SnowTransparency"] = Options.snowTransparencyToggle;
            OptionToId["ShowFlashEffects"] = Options.screenFlashToggle;
            OptionToId["LightingQuality"] = Options.lightingQualityToggle;
            OptionToId["UseHardwareCursor"] = Options.toggleHardwareCursor;
            OptionToId["ControllerPlacementTileIndicator"] = Options.toggleShowPlacementTileGamepad;
            OptionToId["ItemStowing"] = Options.stowingModeSelect;
            OptionToId["UseControllerStyleMenus"] = Options.toggleSnappyMenus;
            //OptionToId[] = Options.toggleIPConnections;
            //OptionToId[] = Options.serverMode;
            //OptionToId[] = Options.toggleFarmhandCreation;
            OptionToId["ShowAdvancedCraftingInformation"] = Options.toggleShowAdvancedCraftingInformation;
            //OptionToId[] = Options.toggleMPReadyStatus;
            //OptionToId[] = Options.mapScreenshot;
            OptionToId["VSync"] = Options.toggleVsync;
            OptionToId["GamepadMode"] = Options.gamepadModeSelect;
            OptionToId["UiScale"] = Options.uiScaleSlider;
            //OptionToId[] = Options.moveBuildingPermissions;
            OptionToId["SlingshotFireMode"] = Options.slingshotModeSelect;
            OptionToId["FishingBiteSound"] = Options.biteChime;
            OptionToId["MuteAnimalSounds"] = Options.toggleMuteAnimalSounds;

            KeyToId["CheckDoAction"] = Options.input_actionButton;
            //KeyToId[] = Options.input_toolSwapButton; // wait this exists and it's bound to Z?
            //KeyToId[] = Options.input_cancelButton; // wait this exists and it's bound to V? and it's explicitly unrebindable???
            KeyToId["UseTool"] = Options.input_useToolButton;
            KeyToId["MoveUp"] = Options.input_moveUpButton;
            KeyToId["MoveRight"] = Options.input_moveRightButton;
            KeyToId["MoveDown"] = Options.input_moveDownButton;
            KeyToId["MoveLeft"] = Options.input_moveLeftButton;
            KeyToId["AccessMenu"] = Options.input_menuButton;
            KeyToId["Run"] = Options.input_runButton;
            KeyToId["ChatBox"] = Options.input_chatButton;
            KeyToId["AccessJournal"] = Options.input_journalButton;
            KeyToId["AccessMap"] = Options.input_mapButton;
            KeyToId["InventorySlot1"] = Options.input_slot1;
            KeyToId["InventorySlot2"] = Options.input_slot2;
            KeyToId["InventorySlot3"] = Options.input_slot3;
            KeyToId["InventorySlot4"] = Options.input_slot4;
            KeyToId["InventorySlot5"] = Options.input_slot5;
            KeyToId["InventorySlot6"] = Options.input_slot6;
            KeyToId["InventorySlot7"] = Options.input_slot7;
            KeyToId["InventorySlot8"] = Options.input_slot8;
            KeyToId["InventorySlot9"] = Options.input_slot9;
            KeyToId["InventorySlot10"] = Options.input_slot10;
            KeyToId["InventorySlot11"] = Options.input_slot11;
            KeyToId["InventorySlot12"] = Options.input_slot12;
            KeyToId["ShiftToolbar"] = Options.input_toolbarSwap;
            KeyToId["EmoteMenu"] = Options.input_emoteButton;
        }


        public OptionChangeTypes GetChangeType(string type, string key)
        {
            if (type == "System.Boolean")
                return OptionChangeTypes.CheckBox;
            if (type == "StardewModdingAPI.SButton")
                return OptionChangeTypes.InputListener;
            switch (OptionToId[key])
            {
                case Options.musicVolume:
                case Options.soundVolume:
                case Options.ambientVolume:
                case Options.footstepVolume:
                case Options.snowTransparencyToggle:
                    return OptionChangeTypes.Slider;
                case Options.lightingQualityToggle:
                case Options.uiScaleSlider:
                case Options.zoom:
                //case Options.screenResolution:
                //case Options.windowMode:
                case Options.serverMode:
                case Options.stowingModeSelect:
                case Options.gamepadModeSelect:
                //case Options.moveBuildingPermissions:
                case Options.slingshotModeSelect:
                case Options.biteChime:
                    return OptionChangeTypes.DropDown;
                default:
                    throw new Exception("bad config key");
            }
        }
    }
}