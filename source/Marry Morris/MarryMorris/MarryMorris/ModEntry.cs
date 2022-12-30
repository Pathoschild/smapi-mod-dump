/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/yoshimax2/MarryMorris
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.IO;

namespace MarryMorris
{
    public class ModEntry : Mod
    {
        //Checks if two Morrises found in save data (when married/post CC game may say there's two Morrises even when there's one, so this controls for that)
        public bool checkSave()
        {
            string save = Game1.GetSaveGameName() + "_" + (object)Game1.uniqueIDForThisGame;
            string savedata;
            string search = "<NPC><name>MorrisTod</name>";
            string path = new string(Path.Combine(Path.Combine(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StardewValley"), "Saves"), save), save));

            try
            {
                StreamReader s = new StreamReader(path);
                savedata = s.ReadToEnd();
                s.Close();

            }
            catch (Exception ex)
            {
                this.Monitor.Log("Unable to open save file. Won't delete any duplicate Morris NPCs.", LogLevel.Debug);
                return false;
            }

            if (savedata.IndexOf(search) != savedata.LastIndexOf(search))
            {
                this.Monitor.Log("Morris found more than once in save", LogLevel.Trace);
                return true;
            }
            else if (savedata.IndexOf(search) == savedata.LastIndexOf(search))
            {
                this.Monitor.Log("Morris found once in save", LogLevel.Trace);
                return false;
            }
            else
            {
                this.Monitor.Log("Morris not found in save", LogLevel.Trace);
                return false;
            }
        }

        public int countMorrises()
        {
            IList<GameLocation> locations = Game1.locations;
            int morrises = 0;

            foreach (GameLocation location in locations)
            {
                {
                    Netcode.NetCollection<NPC> newNpcs = location.getCharacters();

                    foreach (NPC npc in newNpcs)
                    {
                        if (npc.Name == "MorrisTod")
                        {
                            morrises += 1;

                        }
                    }
                }
            }

            return morrises;
        }

        public void removeDuplicate (string locate)
        {
            bool foundMorris = false;
            bool hasDuplicate = false;
            GameLocation loc = Game1.getLocationFromName(locate);
            NPC morrisToRemove = null;

            foreach (NPC character in loc.characters)
            {
                if (!foundMorris && character.Name == "MorrisTod")
                {
                    foundMorris = true;
                }
                else if (foundMorris && character.Name == "MorrisTod")
                {
                    morrisToRemove = character;
                    hasDuplicate = true;
                }
            }


            if (hasDuplicate && checkSave())
            {
                this.Monitor.Log("Multiple Morris NPCs detected in this save", LogLevel.Debug);
                this.Monitor.Log("Removing extra Morris from " + loc.Name, LogLevel.Debug);
                loc.characters.Remove(morrisToRemove);
            }
            

        }

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            //Game1.getCharacterFromName("MorrisTod").Schedule = getMorrisSchedule();

            bool isLoaded = this.Helper.ModRegistry.IsLoaded("FlashShifter.SVECode");

            //Adds Morris back into the game after SVE removes him

            if (isLoaded && (Game1.MasterPlayer.mailReceived.Contains("ccIsComplete") || Game1.MasterPlayer.hasCompletedCommunityCenter()))
            {
                
                Game1.getCharacterFromName("MorrisTod").currentLocation.addCharacter(Game1.getCharacterFromName("MorrisTod"));
                this.Monitor.Log("Adding Morris back to " + Game1.getCharacterFromName("MorrisTod").currentLocation.Name, LogLevel.Trace);
              
            }

            //Checks for extra Morrises
            int morrises = countMorrises();

            //Removes extra Morrises
            if (morrises > 1)
            {

                //Remove Morris duplicates if there are two of him in same location

                removeDuplicate("Custom_Yoshimax_MorrisHome");

                removeDuplicate("FarmHouse");

                removeDuplicate("Town");

                removeDuplicate("Custom_MorrisHouse");


            }

            morrises = countMorrises();

            if (morrises > 1)
            {

                //Removes Morris from Town on initial install

                for (int index1 = 0; index1 < Game1.locations.Count; ++index1)
                {
                    for (int index2 = 0; index2 < Game1.locations[index1].getCharacters().Count; ++index2)
                    {
                        if (Game1.locations[index1].getCharacters()[index2].Name.Equals("MorrisTod") && Game1.locations[index1].Name == "Town")
                        {
                            this.Monitor.Log("Multiple Morris NPCs detected in this save (" + morrises + " Morrises)", LogLevel.Debug);
                            this.Monitor.Log("Removing extra Morris from " + Game1.locations[index1].Name, LogLevel.Debug);
                            Game1.locations[index1].characters.RemoveAt(index2);
                        }
                    }
                }
            }




        }

    }
}
