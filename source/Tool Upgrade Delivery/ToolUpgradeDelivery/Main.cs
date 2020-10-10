/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sorrylate/Stardew-Valley-Mod
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;

namespace ToolUpgradeDelivery
{
    public class ModEntry : Mod
    {
        private ModConfig Config;
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.DayStarted += this.onDayStarted;
            helper.Events.Input.ButtonPressed += this.onButtonPressed;
        }

        public void onDayStarted(object sender, DayStartedEventArgs e)
        {
            this.ReceiveUpgrade();
        }

        public void onButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;

            if (e.Button == Config.ReceiveUpgradeButton)
            {
                //if player have tool upgrade
                if (Game1.player.toolBeingUpgraded.Value != null)
                {
                    //if days left for tool upgrade more than 1 day then show Instant Upgrade question dialogue
                    if (Game1.player.daysLeftForToolUpgrade >= 1)
                    {
                        //if config enabled / true, default is false
                        if (Config.InstantUpgrade)
                        {
                            //question dialogue
                            GameLocation location = Game1.currentLocation;
                            Response[] responses =
                                {
                            new Response ("Yes", "Yes"),
                            new Response ("No", "No")
                                };
                            location.createQuestionDialogue($"Do you want to instant upgrade your tool? Price : {Config.CostInstantUpgrade}G", responses, delegate (Farmer _, string answer)
                            {
                                switch (answer)
                                {
                                    case "Yes":
                                        //checking player inventory space
                                        if (Game1.player.freeSpotsInInventory() > 0)
                                        {
                                            //if player have money more than Config.CostInstantUpgrade
                                            if (Game1.player.Money >= Config.CostInstantUpgrade)
                                            {
                                                //spend player money then do Instant Upgrade method
                                                Game1.player.Money -= Config.CostInstantUpgrade;
                                                this.InstantUpgrade();
                                            }
                                            //if player money less than Config.CostInstantUpgrade
                                            else Game1.drawObjectDialogue($"You don't have enough money");
                                        }
                                        //if player inventory full dialogue
                                        else Game1.drawObjectDialogue("Your inventory is full");
                                        break;

                                    case "No":
                                        //do nothing
                                        break;
                                }
                            });
                        }
                    }
                    //if day left for tool upgrade is 0 / finish then do Receive Upgrade method
                    else if (Game1.player.daysLeftForToolUpgrade <= 0)
                    {
                        this.ReceiveUpgrade();
                    }
                }
                //dialogue when player doesn't have tool upgrade
                else Game1.drawObjectDialogue($"You don't have tool upgrade in Clint");
            }
        }

        private void ReceiveUpgrade()
        {
            if (Game1.player.toolBeingUpgraded.Value != null)
            {
                //When your upgrade is done then do something
                if (Game1.player.daysLeftForToolUpgrade <= 0)
                {
                    //Checking your inventory space
                    if (Game1.player.freeSpotsInInventory() > 0)
                    {
                        //Avoid trash can added to your inventory
                        if (Game1.player.toolBeingUpgraded.Value.DisplayName == "Copper Trash Can")
                        {
                            Game1.drawObjectDialogue($"You receive {Game1.player.toolBeingUpgraded.Value.DisplayName}");
                            Game1.player.trashCanLevel += 1;
                            Game1.player.toolBeingUpgraded.Value = null;
                            return;
                        }
                        else if (Game1.player.toolBeingUpgraded.Value.DisplayName == "Steel Trash Can")
                        {
                            Game1.drawObjectDialogue($"You receive {Game1.player.toolBeingUpgraded.Value.DisplayName}");
                            Game1.player.trashCanLevel += 1;
                            Game1.player.toolBeingUpgraded.Value = null;
                            return;
                        }
                        else if (Game1.player.toolBeingUpgraded.Value.DisplayName == "Gold Trash Can")
                        {
                            Game1.drawObjectDialogue($"You receive {Game1.player.toolBeingUpgraded.Value.DisplayName}");
                            Game1.player.trashCanLevel += 1;
                            Game1.player.toolBeingUpgraded.Value = null;
                            return;
                        }
                        else if (Game1.player.toolBeingUpgraded.Value.DisplayName == "Iridium Trash Can")
                        {
                            Game1.drawObjectDialogue($"You receive {Game1.player.toolBeingUpgraded.Value.DisplayName}");
                            Game1.player.trashCanLevel += 1;
                            Game1.player.toolBeingUpgraded.Value = null;
                            return;
                        }

                        //Add Tool upgrade to player inventory
                        Game1.drawObjectDialogue($"You receive {Game1.player.toolBeingUpgraded.Value.DisplayName}");
                        Game1.player.addItemToInventory(Game1.player.toolBeingUpgraded.Value);

                        //Remove tool in Clint, to avoid duplicate tool
                        Game1.player.toolBeingUpgraded.Value = null;
                        return;
                    }

                    //if your inventory full dialogue
                    else Game1.drawObjectDialogue("Your inventory is full");

                }
            }

        }

        private void InstantUpgrade()
        {
            //Avoid trash can added to your inventory
            if (Game1.player.toolBeingUpgraded.Value.DisplayName == "Copper Trash Can")
            {
                Game1.drawObjectDialogue($"You receive {Game1.player.toolBeingUpgraded.Value.DisplayName}");
                Game1.player.trashCanLevel += 1;
                Game1.player.toolBeingUpgraded.Value = null;
                return;
            }
            else if (Game1.player.toolBeingUpgraded.Value.DisplayName == "Steel Trash Can")
            {
                Game1.drawObjectDialogue($"You receive {Game1.player.toolBeingUpgraded.Value.DisplayName}");
                Game1.player.trashCanLevel += 1;
                Game1.player.toolBeingUpgraded.Value = null;
                return;
            }
            else if (Game1.player.toolBeingUpgraded.Value.DisplayName == "Gold Trash Can")
            {
                Game1.drawObjectDialogue($"You receive {Game1.player.toolBeingUpgraded.Value.DisplayName}");
                Game1.player.trashCanLevel += 1;
                Game1.player.toolBeingUpgraded.Value = null;
                return;
            }
            else if (Game1.player.toolBeingUpgraded.Value.DisplayName == "Iridium Trash Can")
            {
                Game1.drawObjectDialogue($"You receive {Game1.player.toolBeingUpgraded.Value.DisplayName}");
                Game1.player.trashCanLevel += 1;
                Game1.player.toolBeingUpgraded.Value = null;
                return;
            }

            //Add Tool upgrade to player inventory
            Game1.drawObjectDialogue($"You receive {Game1.player.toolBeingUpgraded.Value.DisplayName}");
            Game1.player.addItemToInventory(Game1.player.toolBeingUpgraded.Value);

            //Remove tool in Clint, to avoid duplicate tool
            Game1.player.toolBeingUpgraded.Value = null;
            return;

        }
    }
}
