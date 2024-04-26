/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hunter-Chambers/StardewValleyMods
**
*************************************************/

using EasyFishin.helpers;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;

namespace EasyFishin
{
    /// <summary>The main entry point.</summary>
    public class EasyFishin : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration.</summary>
        #pragma warning disable CS8618 // Non-nullable field must contain a non-null value
        private EasyFishinConfig Config;
        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            I18n.Init(helper.Translation);
            Config = helper.ReadConfig<EasyFishinConfig>();

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.Display.MenuChanged += OnMenuChanged;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is BobberBar bobberBar)
            {
                FishingHelper.ApplyFishingMiniGameAdjustments(Config, Context.ScreenId, bobberBar);
            }
            else if (e.OldMenu is BobberBar)
            {
                FishingHelper.ClearBobberBar(Context.ScreenId);
            }
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }

            FishingHelper.ApplyInfiniteBaitAndTackle(Config);

            // trigger auto hook, if possible
            if (Config.AutoHook && Game1.player.CurrentTool is FishingRod fishingRod)
            {
                FishingHelper.AutoHook(fishingRod, Config.DisableVibration);
            }

            if (Config.AlwaysPerfect && Game1.activeClickableMenu is BobberBar)
            {
                // setting the bobber to perfect during the updatetick event means that the catch will still be perfect even if
                // the player actually participated in the fishing minigame and would have traditionally failed the perfect catch
                FishingHelper.SetBobberToPerfect(Context.ScreenId);
            }
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            IGenericModConfigMenuApi? configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
            {
                return;
            }

            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new EasyFishinConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config_AlwaysPerfect(),
                getValue: () => Config.AlwaysPerfect,
                setValue: value => Config.AlwaysPerfect = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config_AlwaysFindTreasure(),
                getValue: () => Config.AlwaysFindTreasure,
                setValue: value => Config.AlwaysFindTreasure = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config_InstantCatchFish(),
                getValue: () => Config.InstantCatchFish,
                setValue: value => Config.InstantCatchFish = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config_InstantCatchTreasure(),
                getValue: () => Config.InstantCatchTreasure,
                setValue: value => Config.InstantCatchTreasure = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config_InfiniteBait(),
                getValue: () => Config.InfiniteBait,
                setValue: value => Config.InfiniteBait = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config_InfiniteTackle(),
                getValue: () => Config.InfiniteTackle,
                setValue: value => Config.InfiniteTackle = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config_AutoHook(),
                getValue: () => Config.AutoHook,
                setValue: value => Config.AutoHook = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config_DisableControllerVibration(),
                getValue: () => Config.DisableVibration,
                setValue: value => Config.DisableVibration = value
            );
        }
    }
}
