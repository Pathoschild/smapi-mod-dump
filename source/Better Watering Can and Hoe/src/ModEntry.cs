/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Aflojack/BetterWateringCanAndHoe
**
*************************************************/

using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;

namespace BetterWateringCanAndHoe{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod{
    
    /*********
    ** Fields
    *********/
    /// <summary>The mod configuration from the player.</summary>
    private ModConfig Config;
    /// <summary>The mod data from the player.</summary>
    private ModData Data;
    /// <summary>Manager class for Better Hoe mod.</summary>
    private GardenToolManager BetterHoeManager;
    /// <summary>Manager class for Better Watering Can mod.</summary>
    private GardenToolManager BetterWateringCanManager;
    
        /**********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper){
            Config = Helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.Input.ButtonsChanged += OnButtonsChanged;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        /**********
        ** Private methods
        *********/
        /// <summary>Raised when the save file loaded.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e){
            ModDataLoad();
            BetterHoeManager = new GardenToolManager(
                Config.BetterHoeModEnabled, 
                Config.HoeAlwaysHighestOption, 
                Config.HoeSelectTemporary, 
                new GardenTool("dialogbox.hoeQuestion", Data.HoeSelectedOption), 
                Config.HoeTimerStart);
            BetterWateringCanManager = new GardenToolManager(
                Config.BetterWateringCanModEnabled, 
                Config.WateringCanAlwaysHighestOption, 
                Config.WateringCanSelectTemporary, 
                new GardenTool("dialogbox.wateringCanQuestion", Data.WateringCanSelectedOption), 
                Config.WateringCanTimerStart);
        }
        
        /// <summary>Raised when the game launched. Function to build config with GenericModConfigMenu.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e){
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.AddKeybind(
                mod: ModManifest,
                name: () => Helper.Translation.Get("configMenu.selectionOpenKey.name"),
                tooltip: () => Helper.Translation.Get("configMenu.selectionOpenKey.tooltip"),
                getValue: () => Config.SelectionOpenKey,
                setValue: value => Config.SelectionOpenKey = value
            );

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("configMenu.wateringCan.title")
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("configMenu.enabled.name"),
                tooltip: () => Helper.Translation.Get("configMenu.enabled.tooltip"),
                getValue: () => Config.BetterWateringCanModEnabled,
                setValue: value => Config.BetterWateringCanModEnabled = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("configMenu.alwaysHighestOption.name"),
                tooltip: () => Helper.Translation.Get("configMenu.alwaysHighestOption.tooltip"),
                getValue: () => Config.WateringCanAlwaysHighestOption,
                setValue: value => Config.WateringCanAlwaysHighestOption = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("configMenu.selectTemporary.name"),
                tooltip: () => Helper.Translation.Get("configMenu.selectTemporary.tooltip"),
                getValue: () => Config.WateringCanSelectTemporary,
                setValue: value => Config.WateringCanSelectTemporary = value
            );
            
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("configMenu.selectTemporaryTimer.name"),
                tooltip: () => Helper.Translation.Get("configMenu.selectTemporaryTimer.tooltip"),
                getValue: () => Config.WateringCanTimerStart/60,
                setValue: value => Config.WateringCanTimerStart = value*60,
                min: 10,
                max: 90,
                interval: 1
            );

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("configMenu.hoe.title")
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("configMenu.enabled.name"),
                tooltip: () => Helper.Translation.Get("configMenu.enabled.tooltip"),
                getValue: () => Config.BetterHoeModEnabled,
                setValue: value => Config.BetterHoeModEnabled = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("configMenu.alwaysHighestOption.name"),
                tooltip: () => Helper.Translation.Get("configMenu.alwaysHighestOption.tooltip"),
                getValue: () => Config.HoeAlwaysHighestOption,
                setValue: value => Config.HoeAlwaysHighestOption = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("configMenu.selectTemporary.name"),
                tooltip: () => Helper.Translation.Get("configMenu.selectTemporary.tooltip"),
                getValue: () => Config.HoeSelectTemporary,
                setValue: value => Config.HoeSelectTemporary = value
            );
            
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("configMenu.selectTemporaryTimer.name"),
                tooltip: () => Helper.Translation.Get("configMenu.selectTemporaryTimer.tooltip"),
                getValue: () => Config.HoeTimerStart/60,
                setValue: value => Config.HoeTimerStart = value*60,
                min: 10,
                max: 90,
                interval: 1
            );
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e){
            if (!Context.IsWorldReady)
                return;

            BetterWateringCanManager.TimerTick();
            BetterHoeManager.TimerTick();
            
            switch (Game1.player.CurrentTool){
                case WateringCan:
                    BetterWateringCanManager.Tick();
                    break;
                case Hoe:
                    BetterHoeManager.Tick();
                    break;
            }
            
            ModDataWrite();
        }

        /// <summary>Raised after the player pressed/released any buttons on the keyboard, mouse, or controller.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e){
            if (!Context.IsWorldReady)
                return;

            if (Game1.player.IsBusyDoingSomething())
                return;
            
            if (Helper.Input.GetState(Config.SelectionOpenKey) != SButtonState.Released)
                return;

            switch (Game1.player.CurrentTool){
                case WateringCan:
                    BetterWateringCanManager.ButtonAction(Helper);
                    break;
                case Hoe:
                    BetterHoeManager.ButtonAction(Helper);
                    break;
            }
            
            ModDataWrite();
        }

        /// <summary>Load current player mod data json from data folder.</summary>
        private void ModDataLoad(){
            Data = Helper.Data.ReadJsonFile<ModData>($"data/{Constants.SaveFolderName}.json") ?? new ModData();
        }

        /// <summary>Write current player mod data json to data folder if that necessary.</summary>
        private void ModDataWrite(){
            if (!BetterWateringCanManager.DataChange && !BetterHoeManager.DataChange)
                return;

            Data.WateringCanSelectedOption = BetterWateringCanManager.SelectedOption;
            Data.HoeSelectedOption = BetterHoeManager.SelectedOption;
            
            Helper.Data.WriteJsonFile($"data/{Constants.SaveFolderName}.json", Data);

            BetterWateringCanManager.DataChange = false;
            BetterHoeManager.DataChange = false;
        }
    }
}