using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace Nice_Messages
{
    class NiceMessages {
        private Dictionary< string, string[] > unifiedMessages;
        private IModHelper modelHelper;

        public NiceMessages(IModHelper helper)
        {
            this.modelHelper = helper;
            this.unifiedMessages = modelHelper.Content.Load<Dictionary<string,string[]>>("unifiedMessages.json", ContentSource.ModFolder);
        }

        /*
         * Takes the indentified weather as a key and accesses the correct table, returning a random message
         * based on the number of messages in the message table array.
         * result of the random number is subtracted by 1 to correct for index starting at 0
         */

        public string getMorningMessage(string currSeason, int weatherIcon)
        {
            string weatherKey = identifyWeather(currSeason, weatherIcon);
            string[] msgTable = unifiedMessages[weatherKey];
            return msgTable[new Random().Next(msgTable.Length - 1)];
        }


        //Creates a key to be used by final dictonary.
        //Keys are strings in "<season>/<weather>" format except for festivals, since they are fixed seasons
        //Keys will be used to select the correct table of lines from the master dictonary

        /* 
         * VALD KEYS ********************************************************************
         * spring/sunny         summer/sunny        fall/sunny          winter/sunny    *
         * spring/windy                             fall/windy                          *
         * spring/rain          summer/rain         fall/rain                           *
         * spring/lightning     summer/lightning    fall/lightning                      *
         *                                                              winter/snow     *
         * ******************************************************************************
         */
        private string identifyWeather(string currSeason, int currWeather) 
        {   
            switch (currWeather) 
            {
                case 0:     return currSeason+"/sunny";
                case 1:     return currSeason+"/sunny";
                case 2:     return currSeason+"/sunny";
                case 3:     return "spring/windy";
                case 4:     return currSeason+"/rain";
                case 5:     return currSeason+"/lightning";
                case 6:     return "fall/windy";
                case 7:     return "winter/snow";
            }
            return null;
        }

    }//end of class
}//end of namespace
