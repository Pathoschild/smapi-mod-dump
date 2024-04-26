/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hunter-Chambers/StardewValleyMods
**
*************************************************/

using GenericModConfigMenu;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace AutoGrabTruffles
{
    /// <summary>The mod entry point.</summary>
    internal sealed class AutoGrabTruffles : Mod
    {
        #pragma warning disable CS8618 // Non-nullable field must contain a non-null value
        private AutoGrabTrufflesConfig Config;
        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            I18n.Init(helper.Translation);
            Config = helper.ReadConfig<AutoGrabTrufflesConfig>();

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        }

        /*********
        ** Private methods
        *********/
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            CreateMenu();
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            if (Context.IsOnHostComputer && Context.IsMainPlayer)
            {
                DigUpProducePatcher.Initialize(Monitor, Config);
                Harmony harmony = new (ModManifest.UniqueID);
                DigUpProducePatcher.ApplyPatch(harmony);
            }
            else
            {
                DisableMenuForNonHost();
            }
        }

        private void CreateMenu()
        {
            IGenericModConfigMenuApi? configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
            {
                return;
            }

            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new AutoGrabTrufflesConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config_ApplyProfessionLabel(professionName: I18n.Config_Gatherer()),
                getValue: () => Config.ApplyGathererProfession,
                setValue: value => Config.ApplyGathererProfession = value
            );
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => I18n.Config_WhoseToApplyLabel(professionName: I18n.Config_Gatherer()),
                getValue: () => Config.WhoseGathererProfessionToUse,
                setValue: value => Config.WhoseGathererProfessionToUse = value,
                allowedValues: new string[] { "Owner", "Anyone" }
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config_ApplyProfessionLabel(professionName: I18n.Config_Botanist()),
                getValue: () => Config.ApplyBotanistProfession,
                setValue: value => Config.ApplyBotanistProfession = value
            );
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => I18n.Config_WhoseToApplyLabel(professionName: I18n.Config_Botanist()),
                getValue: () => Config.WhoseBotanistProfessionToUse,
                setValue: value => Config.WhoseBotanistProfessionToUse = value,
                allowedValues: new string[] { "Owner", "Anyone" }
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config_GainXpLabel(),
                getValue: () => Config.GainExperience,
                setValue: value => Config.GainExperience = value
            );
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => I18n.Config_WhoGainsXpLabel(),
                getValue: () => Config.WhoGainsExperience,
                setValue: value => Config.WhoGainsExperience = value,
                allowedValues: new string[] { "Owner", "Everyone" }
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config_UpdateGameStats(),
                getValue: () => Config.UpdateGameStats,
                setValue: value => Config.UpdateGameStats = value
            );
        }

        private void DisableMenuForNonHost()
        {
            IGenericModConfigMenuApi? configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
            {
                return;
            }

            configMenu.Unregister(ModManifest);

            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new AutoGrabTrufflesConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.AddParagraph(
                mod: ModManifest,
                text: () => I18n.Config_DisabledMessage()
            );
        }
    }
}