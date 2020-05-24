using System;
using System.Collections.Generic;
using StardewModdingAPI;

namespace Nice_Messages
{
    class NiceMessages {
        private Dictionary<int,string> newDayMessages;
        private IModHelper modelHelper;

     
        public NiceMessages(string season, IModHelper helper){
            this.modelHelper = helper;
            this.newDayMessages = (
                    modelHelper.Content.Load<Dictionary<int,string>>(getNewSeasonMessages(season), ContentSource.ModFolder)
            );
        }
       
        private string getNewSeasonMessages(string season){
            switch (season)
            {
                case "spring":
                    return "springMessages.json";

                case "summer":
                    return "summerMessages.json";

                case "fall":
                    return "fallMessages.json";

                case "winter":
                    return "winterMessages.json";
            }
            //if you get here, that means there are no seasons....
            //the implications of that are... troubling....
            return null;
        }

        public string randomMorningMessage() {
            Random RNG = new Random();
                return this.newDayMessages[RNG.Next(0, newDayMessages.Count-1)];
        }
    }//end of class
}//end of namespace
