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

namespace BitwiseJonMods
{
    public class ModEntry : Mod
    {
        private ModConfig _config;

        public override void Entry(IModHelper helper)
        {
            BitwiseJonMods.Common.Utility.InitLogging(this.Monitor);
            _config = helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs args)
        {
            BitwiseJonMods.Common.Utility.Log(string.Format("Config BuildUsesResources={0}", _config.BuildUsesResources));
            BitwiseJonMods.Common.Utility.Log(string.Format("Config ToggleInstantBuildMenuButton={0}", _config.ToggleInstantBuildMenuButton));
            BitwiseJonMods.Common.Utility.Log(string.Format("Config PerformInstantHouseUpgradeButton={0}", _config.PerformInstantHouseUpgradeButton));

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
                activateInstantBuildMenu();
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

            //See StardewValley.Locations.GameLocation.houseUpgradeAccept()
            BitwiseJonMods.Common.Utility.Log("BuildUsesResources=true so checking if player has the resources to complete upgrade.");
            switch (Game1.player.HouseUpgradeLevel)
            {
                case 0:
                    if (Game1.player.Money >= 10000 && Game1.player.Items.ContainsId("(O)388", 450))
                    {
                        Game1.player.Money -= 10000;
                        Game1.player.Items.ReduceId("(O)388", 450);
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
                    if (Game1.player.Money >= 65000 && Game1.player.Items.ContainsId("(O)709", 100))
                    {
                        Game1.player.Money -= 65000;
                        Game1.player.Items.ReduceId("(O)709", 100);
                        CompleteHouseUpgrade();
                        break;
                    }
                    if (Game1.player.Money < 65000)
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

            Game1.stats.checkForBuildingUpgradeAchievements();
            BitwiseJonMods.Common.Utility.Log($"Upgrade complete! New upgrade level: {Game1.player.HouseUpgradeLevel}");
        }

        private void activateInstantBuildMenu()
        {
            Game1.activeClickableMenu = (IClickableMenu)new InstantBuildMenu(_config);
        }
    }
}
