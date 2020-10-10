/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bogie5464/SiloReport
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Menus;
using StardewValley.Buildings;
using StardewModdingAPI.Framework;

namespace SiloReport
{
    public class ModEntry : Mod
    {
        //Declare config object
        private ModConfig config;

        public override void Entry(IModHelper helper)
        {
            //Read in config
            config = helper.ReadConfig<ModConfig>();
            
            //Subscribe to event
            helper.Events.GameLoop.DayStarted += dayStartedEvents;
        }


        //Event handler method
        private void dayStartedEvents(object sender, DayStartedEventArgs e)
        {
            bool hasSilo = Game1.getFarm().buildings.Any(b => b.buildingType.Value == "Silo");

            //Chat Mode
            if (config.notificationMode == notificationModes.ChatMode)
            {
                //If multiple people have the mod installed on a server the chat would get a per player message. This checks to see if you're the master player and if not returns.
                //Can be overridden by config
                if (config.checkMasterPlayer)
                {
                    if (Game1.player != Game1.MasterPlayer)
                        return;
                }

                //Send chat logic
                if (hasSilo)
                {
                    //Scrapped, SMAPI overrides Game1.multiplayer in SGame.CS line 171
                    //Multiplayer multiplayer = this.Helper.Reflection.GetField<Multiplayer>(Game1.game1, "Multiplayer").GetValue();
                    //multiplayer.sendChatMessage(LocalizedContentManager.LanguageCode.en, $"Hay left: {Game1.getFarm().piecesOfHay}");

                    //Thank you Pathoschild for the workaround
                    Game1.chatBox.activate();
                    Game1.chatBox.setText($"Hay left: {Game1.getFarm().piecesOfHay}");
                    Game1.chatBox.chatBox.RecieveCommandInput('\r');
                }

                //Called if Farmer doesn't have silo
                else
                    this.Monitor.VerboseLog("You do not have a Silo, build Silo to get report.");
                
            }
            
            //UI Mode
            else if(config.notificationMode == notificationModes.UIMode)
            {
                //UI Message logic
                if (hasSilo)
                {
                    HUDMessage report = new HUDMessage($"Hay left: {Game1.getFarm().piecesOfHay}", (String) null);
                    Game1.addHUDMessage(report);
                }

                //Called if Farmer doesn't have silo
                else
                {
                    HUDMessage report = new HUDMessage("You do not have a silo built, build one to receive messages", 3);
                    Game1.addHUDMessage(report);
                }
            }
        }
    }
}
