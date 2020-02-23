using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;

namespace ToolDeliveryMail
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
                this.ReceiveUpgrade();
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

                        //Add Tool upgrade to player inventory
                        Game1.drawObjectDialogue($"You receive {Game1.player.toolBeingUpgraded.Value.DisplayName}");
                        Game1.player.addItemToInventory(Game1.player.toolBeingUpgraded.Value);

                        //Remove tool in Clint, to avoid duplicate tool
                        Game1.player.toolBeingUpgraded.Value = null;
                        return;
                    }
                    //if your inventory full dialogue
                    else Game1.drawObjectDialogue("Your inventory is full");

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

                }
            }
        }

    }
}
