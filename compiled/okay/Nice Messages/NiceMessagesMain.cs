using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace Nice_Messages
{
    public class NiceMessagesMain : Mod
    {
        private NiceMessages niceMessages;
        private IModHelper mainHelper;
        private String currSeason;
        
        //SM Api set up
        public override void Entry(IModHelper oneHelpyBoi){
            this.mainHelper = oneHelpyBoi;
            oneHelpyBoi.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            oneHelpyBoi.Events.GameLoop.DayStarted += GameLoop_DayStarted;
        }

        //listeners
        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e){
            this.currSeason = Game1.currentSeason;
            this.niceMessages = new NiceMessages(currSeason, mainHelper);
        }

        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e){
            changeSeason(currSeason);
            try { Game1.showGlobalMessage(niceMessages.randomMorningMessage()); }

            //Catch a bad formatting excption
            catch (System.Collections.Generic.KeyNotFoundException) {
                Monitor.Log("Invalid Key. Look in " + currSeason + "Messages.json and ensure it is in the format: int:\"message\"" +
                    "\n\t\tAlso ensure that keys are intigers, starting at 0 with no skipped numbers.", LogLevel.Error);
            }
        }

        //methods
        private void changeSeason(String checkSeason) {
            if (this.currSeason == Game1.currentSeason) { return; }
            this.currSeason = Game1.currentSeason;
            this.niceMessages = new NiceMessages(currSeason, mainHelper);
        }
    
    }
}
