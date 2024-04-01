/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/StephHoel/ModsStardewValley
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace AddMoney;

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    private ModConfig _config;

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        I18n.Init(helper.Translation);
        _config = helper.ReadConfig<ModConfig>();

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.Input.ButtonPressed += OnButtonPressed;
    }

    /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
    {
        // ignore if player hasn't loaded a save yet
        if (!Context.IsWorldReady)
            return;

        // print button presses to the console window
        //this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);

        if (e.Button == _config.ButtonToAddMoney)
        {
            var gold = _config.GoldToAdd;

            Game1.player.Money += gold;
            Game1.addHUDMessage(new($"{gold}{I18n.Message()}", 2));
            Monitor.Log($"{Game1.player.Name} added {gold}G.", LogLevel.Debug);
        }
        //else
        //    Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);
    }

    private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
    {
        // get Generic Mod Config Menu's API (if it's installed)
        var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is null)
            return;

        // register mod
        configMenu.Register(
            mod: this.ModManifest,
            reset: () => _config = new ModConfig(),
            save: () => this.Helper.WriteConfig(_config)
        );

        configMenu.AddSectionTitle(
            mod: ModManifest,
            text: I18n.ConfigTitleGeneralOptions
        );

        // ButtonToAddMoney
        configMenu.AddKeybind(
            mod: ModManifest,
            name: I18n.ConfigButtonAddToMoneyKeyName,
            getValue: () => _config.ButtonToAddMoney,
            setValue: value => _config.ButtonToAddMoney = value
        );

        // GoldToAdd
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: I18n.ConfigGoldAddName,
            getValue: () => _config.GoldToAdd,
            setValue: val => _config.GoldToAdd = val
        );

        
    }
}