/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewArchipelago;

namespace StardewArchipelago.Integrations.GenericModConfigMenu
{
    /// <summary>The API which lets other mods add a config UI through Generic Mod Config Menu.</summary>
    public interface IGenericModConfigMenuApi
    {
        /*********
        ** Methods
        *********/
        /// <summary>Register a mod whose config can be edited through the UI.</summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="reset">Reset the mod's config to its default values.</param>
        /// <param name="save">Save the mod's current config to the <c>config.json</c> file.</param>
        /// <param name="titleScreenOnly">Whether the options can only be edited from the title screen.</param>
        /// <remarks>Each mod can only be registered once, unless it's deleted via <see cref="Unregister"/> before calling this again.</remarks>
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

        /****
        ** Basic options
        ****/
        /// <summary>Add a section title at the current position in the form.</summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="text">The title text shown in the form.</param>
        /// <param name="tooltip">The tooltip text shown when the cursor hovers on the title, or <c>null</c> to disable the tooltip.</param>
        void AddSectionTitle(IManifest mod, Func<string> text, Func<string> tooltip = null);

        /// <summary>Add a paragraph of text at the current position in the form.</summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="text">The paragraph text to display.</param>
        void AddParagraph(IManifest mod, Func<string> text);

        /// <summary>Add a boolean option at the current position in the form.</summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="getValue">Get the current value from the mod config.</param>
        /// <param name="setValue">Set a new value in the mod config.</param>
        /// <param name="name">The label text to show in the form.</param>
        /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
        /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged"/>, or <c>null</c> to auto-generate a randomized ID.</param>
        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);

        /// <summary>Set whether the options registered after this point can only be edited from the title screen.</summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="titleScreenOnly">Whether the options can only be edited from the title screen.</param>
        /// <remarks>This lets you have different values per-field. Most mods should just set it once in <see cref="Register"/>.</remarks>
        void SetTitleScreenOnlyForNextOptions(IManifest mod, bool titleScreenOnly);

        /// <summary>Register a method to notify when any option registered by this mod is edited through the config UI.</summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="onChange">The method to call with the option's unique field ID and new value.</param>
        /// <remarks>Options use a randomized ID by default; you'll likely want to specify the <c>fieldId</c> argument when adding options if you use this.</remarks>
        void OnFieldChanged(IManifest mod, Action<string, object> onChange);

        /// <summary>Remove a mod from the config UI and delete all its options and pages.</summary>
        /// <param name="mod">The mod's manifest.</param>
        void Unregister(IManifest mod);
    }

    class GenericModConfig
    {
        private IModHelper Helper;
        private IManifest ModManifest;
        private ModConfig Config;

        public GenericModConfig(ModEntry mod)
        {
            Helper = mod.Helper;
            ModManifest = mod.ModManifest;
            Config = mod.Config;
        }

        public void RegisterConfig()
        {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu == null)
            {
                return;
            }

            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.SetTitleScreenOnlyForNextOptions(ModManifest, true);

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Enable Seed Shop Overhaul",
                tooltip: () => "",
                getValue: () => Config.EnableSeedShopOverhaul,
                setValue: (value) => Config.EnableSeedShopOverhaul = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Hide Empty Archipelago Letters",
                tooltip: () => "",
                getValue: () => Config.HideEmptyArchipelagoLetters,
                setValue: (value) => Config.HideEmptyArchipelagoLetters = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Use Custom Archipelago Icons",
                tooltip: () => "",
                getValue: () => Config.UseCustomArchipelagoIcons,
                setValue: (value) => Config.UseCustomArchipelagoIcons = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Skip Hold Up Animations",
                tooltip: () => "",
                getValue: () => Config.SkipHoldUpAnimations,
                setValue: (value) => Config.SkipHoldUpAnimations = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Disable Friendship Decay",
                tooltip: () => "",
                getValue: () => Config.DisableFriendshipDecay,
                setValue: (value) => Config.DisableFriendshipDecay = value
            );
        }
    }
}