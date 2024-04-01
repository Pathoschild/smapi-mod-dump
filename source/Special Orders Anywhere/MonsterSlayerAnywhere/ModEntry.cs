/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AcidicNic/StardewValleyMods
**
*************************************************/

using System;
using System.Xml.Linq;
using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace AcdicNic.Stardew.MonsterSlayerAnywhere {

    internal sealed class ModEntry : Mod {

        private ModConfig Config;

        public override void Entry(IModHelper helper) {
            this.Config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e) {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );

            // keybind title
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "KeyBind Settings"
            );
            // activate key config
            configMenu.AddKeybindList(
                mod: this.ModManifest,
                name: () => "Toggle Monster List",
                tooltip: () => "Opens the Monster Eradication Goals menu. Default: F7",
                getValue: () => this.Config.ToggleMonsterList,
                setValue: value => this.Config.ToggleMonsterList = value
            );
        }

        private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e) {
            if (!Context.IsWorldReady || !this.Config.ToggleMonsterList.JustPressed())
                return;

            // Open the Monster Eradication Goals list
            if (Context.IsPlayerFree) {
                ((AdventureGuild)Game1.getLocationFromName("AdventureGuild")).showMonsterKillList();
                return;
            }

            // Close the Monster Eradication Goals list
            if (Game1.activeClickableMenu is LetterViewerMenu letterMenu) {
                if (!letterMenu.isMail && !letterMenu.isFromCollection)
                    letterMenu.exitThisMenu();
            }
        }

    }
}
