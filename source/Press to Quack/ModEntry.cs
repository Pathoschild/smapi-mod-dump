/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DraconisLeonidas/PressToQuack
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using GenericModConfigMenu;

//I just wanna make the farmer quack on command, because it's funny.

namespace PressToQuack
{
    public class ModEntry : Mod
    {
        // Add config
        private ModConfig config;

        public override void Entry(IModHelper helper)
        {
            // Read in config file and create if needed
            this.config = this.Helper.ReadConfig<ModConfig>();

            // Set up GMCM config when game is launched
            helper.Events.GameLoop.GameLaunched += SetUpConfig;

            // Set up button pressing doing thing
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;

            /* Old Config stuff
            this.config = this.Helper.ReadConfig<ModConfig>(); */
        }

        private void SetUpConfig(object sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's AUP (if installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.config)
                );

            // add config option
            configMenu.AddKeybind(
              mod: this.ModManifest,
              name: () => "Quack Button",
              tooltip: () => "The button that makes you quack.",
              getValue: () => this.config.QuackButton,
              setValue: value => this.config.QuackButton = value
              );
        }

        //When button is pressed while player is in game and not in a menu, Quack.
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            //Only allow Quack if player is not in menu
            if (Context.IsPlayerFree)
            {
                //Press button
                if (e.Button == this.config.QuackButton)
                {
                    //Quack (Localized on Player, so audible in Multiplayer! I'm so sorry)
                    Game1.currentLocation.playSound("Duck");

                }

            }
        }

    }
}
