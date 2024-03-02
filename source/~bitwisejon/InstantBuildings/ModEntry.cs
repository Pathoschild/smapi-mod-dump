/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bitwisejon/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
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
            BitwiseJonMods.Common.Utility.Log(string.Format("Config PerformInstantHouseUpgradeButton={0}", _config.PerformInstantHouseUpgradeButton));
            BitwiseJonMods.Common.Utility.Log(string.Format("Tractor Mod Found={0}", _tractorModFound));

            Helper.Events.Input.ButtonPressed += Input_ButtonPressed;
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if ((e.Button == _config.ToggleInstantBuildMenuButton || e.Button == _config.PerformInstantHouseUpgradeButton) && Game1.currentLocation is Farm)
            {
                if (e.Button == _config.ToggleInstantBuildMenuButton)
                {
                    BitwiseJonMods.Common.Utility.Log(string.Format("User clicked Instant Build key={0}", _config.ToggleInstantBuildMenuButton));
                    HandleInstantBuildButtonClick();
                }
                else if (e.Button == _config.PerformInstantHouseUpgradeButton)
                {
                    BitwiseJonMods.Common.Utility.Log(string.Format("User clicked Instant Upgrade key={0}", _config.PerformInstantHouseUpgradeButton));
                    HandleInstantUpgradeButtonClick();
                }

            }
        }

        private void HandleInstantBuildButtonClick()
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

        private void HandleInstantUpgradeButtonClick()
        {
            if (Context.IsPlayerFree && Game1.activeClickableMenu == null)
            {
                string msg = "";

                //These string replacements probably won't work in some languages due to the comma separator, but at least the rest of the message
                //  will be translated.  I will also look for the period separator just in case.
                switch (Game1.player.HouseUpgradeLevel)
                {
                    case 0:
                        msg = Game1.parseText(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_UpgradeHouse1"));
                        if (!_config.BuildUsesResources) msg = msg.Replace("10,000", "0").Replace("10.000", "0").Replace("450", "0");
                        this.createQuestionDialogue(msg);
                        break;
                    case 1:
                        msg = Game1.parseText(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_UpgradeHouse2"));
                        if (!_config.BuildUsesResources) msg = msg.Replace("50,000", "0").Replace("50.000", "0").Replace("150", "0");
                        this.createQuestionDialogue(msg);
                        break;
                    case 2:
                        msg = Game1.parseText(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_UpgradeHouse3"));
                        if (!_config.BuildUsesResources) msg = msg.Replace("100,000", "0").Replace("100.000", "0");
                        this.createQuestionDialogue(msg);
                        break;
                }
            }
        }

        private void createQuestionDialogue(string question)
        {
            Game1.currentLocation.createQuestionDialogue(question, Game1.currentLocation.createYesNoResponses(), (GameLocation.afterQuestionBehavior)((f, answer) =>
            {
                if (answer == "Yes")
                {
                    BitwiseJonMods.Common.Utility.Log(string.Format("User agreed to house upgrade from level {0} to level {1}.", Game1.player.HouseUpgradeLevel, Game1.player.HouseUpgradeLevel + 1));
                    houseUpgradeAccept();
                }
            }), (NPC)Game1.getCharacterFromName("Robin", true));
        }

        private Response[] createYesNoResponses()
        {
            return new Response[2]
            {
                new Response("Yes", Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_Yes")),
                new Response("No", Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No"))
            };
        }
        
        private void houseUpgradeAccept()
        {
            if (!_config.BuildUsesResources)
            {
                BitwiseJonMods.Common.Utility.Log("BuildUsesResources=false so completing house upgrade at no cost.");
                CompleteHouseUpgrade();
                return;
            }

            //See StardewValley.Locations.1GameLocation.houseUpgradeAccept()
            BitwiseJonMods.Common.Utility.Log("BuildUsesResources=true so checking if player has the resources to complete upgrade.");
            switch (Game1.player.HouseUpgradeLevel)
            {
                case 0:
                    if (Game1.player.Money >= 10000 && Game1.player.hasItemInInventory(388, 450, 0))
                    {
                        Game1.player.Money -= 10000;
                        Game1.player.removeItemsFromInventory(388, 450);
                        CompleteHouseUpgrade();
                        break;
                    }
                    if (Game1.player.Money < 10000)
                    {
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney3"));
                        break;
                    }
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_NotEnoughWood1"));
                    break;
                case 1:
                    if (Game1.player.Money >= 50000 && Game1.player.hasItemInInventory(709, 150, 0))
                    {
                        Game1.player.Money -= 50000;
                        Game1.player.removeItemsFromInventory(709, 150);
                        CompleteHouseUpgrade();
                        break;
                    }
                    if (Game1.player.Money < 50000)
                    {
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney3"));
                        break;
                    }
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_NotEnoughWood2"));
                    break;
                case 2:
                    if (Game1.player.Money >= 100000)
                    {
                        Game1.player.Money -= 100000;
                        CompleteHouseUpgrade();
                        break;
                    }
                    if (Game1.player.Money >= 100000)
                        break;
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney3"));
                    break;
            }
        }

        private void CompleteHouseUpgrade()
        {
            BitwiseJonMods.Common.Utility.Log("Performing instant house upgrade!");

            Game1.playSound("achievement");
            var homeOfFarmer = Utility.getHomeOfFarmer(Game1.player);
            homeOfFarmer.moveObjectsForHouseUpgrade(Game1.player.HouseUpgradeLevel + 1);

            Game1.player.daysUntilHouseUpgrade.Value = -1;
            ++Game1.player.HouseUpgradeLevel;
            homeOfFarmer.setMapForUpgradeLevel(Game1.player.HouseUpgradeLevel);

            //Cabins automatically change their appearance for all players, but the main house texture has to be changed manually.
            if (!(homeOfFarmer is Cabin))
            {
                //Use reflection to update house texture in otherwise private variable of Farm class.
                var houseSource = Helper.Reflection.GetField<NetRectangle>(Game1.getFarm(), "houseSource");
                var rect = new Microsoft.Xna.Framework.Rectangle(0, 144 * (Game1.player.HouseUpgradeLevel == 3 ? 2 : Game1.player.HouseUpgradeLevel), 160, 144);
                houseSource.GetValue().Value = rect;
            }

            Game1.stats.checkForBuildingUpgradeAchievements();
            BitwiseJonMods.Common.Utility.Log($"Upgrade complete! New upgrade level: {Game1.player.HouseUpgradeLevel}");
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
