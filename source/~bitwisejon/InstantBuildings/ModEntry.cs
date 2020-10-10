/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bitwisejon/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitwiseJonMods
{
    public class ModEntry : Mod
    {
        private ModConfig _config;
        private bool _tractorModFound = false;

        public override void Entry(IModHelper helper)
        {
            BitwiseJonMods.Common.Utility.InitLogging(this.Monitor);

            _config = helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs args)
        {
            _tractorModFound = this.Helper.ModRegistry.IsLoaded("Pathoschild.TractorMod");

            BitwiseJonMods.Common.Utility.Log(string.Format("Config BuildUsesResources={0}", _config.BuildUsesResources));
            BitwiseJonMods.Common.Utility.Log(string.Format("Config ToggleInstantBuildMenuButton={0}", _config.ToggleInstantBuildMenuButton));
            BitwiseJonMods.Common.Utility.Log(string.Format("Tractor Mod Found={0}", _tractorModFound));

            Helper.Events.Input.ButtonPressed += Input_ButtonPressed;
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == _config.ToggleInstantBuildMenuButton && Game1.currentLocation is Farm)
            {
                if (Context.IsPlayerFree && Game1.activeClickableMenu == null)
                {
                    if (_tractorModFound)
                    {
                        //Get tractor blueprint from carpenter menu
                        var carpenterMenu = new CarpenterMenu();
                        Game1.activeClickableMenu = (IClickableMenu)carpenterMenu;
                        Game1.delayedActions.Add(new DelayedAction(100, new DelayedAction.delayedBehavior(this.getTractorBlueprintFromCarpenterMenu)));
                    }
                    else
                    {
                        activateInstantBuildMenu();
                    }
                }
                else if (Game1.activeClickableMenu is InstantBuildMenu)
                {
                    Game1.displayFarmer = true;
                    ((InstantBuildMenu)Game1.activeClickableMenu).exitThisMenu();
                }
            }
        }

        private void activateInstantBuildMenu(BluePrint tractorBlueprint = null)
        {
            Game1.activeClickableMenu = (IClickableMenu)new InstantBuildMenu(_config, tractorBlueprint);
        }

        private void getTractorBlueprintFromCarpenterMenu()
        {
            BluePrint tractorBlueprint = null;

            try
            {
                //jon, 11/27/19: For some reason, this is no longer showing the tractor garage image even though the blueprint is loading from the carpenter menu
                //  correctly.  It shows the default stable instead.
                IClickableMenu menu = Game1.activeClickableMenu is CarpenterMenu ? (CarpenterMenu)Game1.activeClickableMenu : null;

                if (menu != null)
                {
                    var blueprints = this.Helper.Reflection
                        .GetField<List<BluePrint>>(menu, "blueprints")
                        .GetValue();

                    var tractorModName = "Tractor Garage";
                    tractorBlueprint = blueprints.SingleOrDefault(b => b.displayName == tractorModName);
                    if (tractorBlueprint == null) BitwiseJonMods.Common.Utility.Log(string.Format("Could not load Tractor blueprint since it did not exist in Carpenter menu with display name '{0}'.", tractorModName));
                    menu.exitThisMenu();
                }
                else
                {
                    BitwiseJonMods.Common.Utility.Log("Unable to get Carpenter menu as active game menu. Might be another type.");
                }
            }
            catch (Exception ex)
            {
                BitwiseJonMods.Common.Utility.Log(string.Format("Exception trying to load Tractor blueprint, probably due to an incompatibility with another mod: {0}", ex.Message), LogLevel.Error);
            }

            activateInstantBuildMenu(tractorBlueprint);
        }
    }
}
