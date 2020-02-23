using System;
using StardewValley;
using StardewValley.Events;
using StardewValley.Locations;

namespace FamilyPlanning.Patches
{
    /* Utility.pickPersonalFarmEvent():
     * This patch mostly reproduces the original method's logic.
     * I've added messages to the player & the ability to change the random chance.
     */ 
    class PickPersonalFarmEventPatch
    {
        public static bool Prefix(ref FarmEvent __result)
        {
            //Skip if there's a wedding
            if (Game1.weddingToday)
                return true;

            //Skip if there's a birth
            //(This is very close to the original code)
            if (Game1.player.isMarried() && Game1.player.GetSpouseFriendship().DaysUntilBirthing <= 0 && Game1.player.GetSpouseFriendship().NextBirthingDate != null)
            {
                if (Game1.player.spouse != null)
                {
                    __result = new BirthingEvent();
                    return false;
                }

                long key = Game1.player.team.GetSpouse(Game1.player.UniqueMultiplayerID).Value;
                if (Game1.otherFarmers.ContainsKey(key))
                {
                    __result = new PlayerCoupleBirthingEvent();
                    return false;
                }
            }

            Random random = new Random((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2 ^ 470124797 + (int)Game1.player.UniqueMultiplayerID);

            //Checking for a baby QuestionEvent
            if (Game1.player.isMarried())
            {
                //(It helps the flow of if statements to get local variables here, even if unnecessary.)
                bool message = ModEntry.MessagesConfig();
                string npcSpouse = Game1.player.spouse;
                long? playerSpouse = Game1.player.team.GetSpouse(Game1.player.UniqueMultiplayerID);

                //QuestionEvent for an NPC spouse
                if (npcSpouse != null && Game1.getCharacterFromName(npcSpouse, true).canGetPregnant() && Game1.player.currentLocation == Game1.getLocationFromName(Game1.player.homeLocation.Value))
                {
                    //QuestionEvent(1) is possible
                    bool isGaySpouse = Game1.getCharacterFromName(npcSpouse, true).isGaySpouse();
                   
                    if (message)
                    {
                        if (isGaySpouse)
                            ModEntry.monitor.Log(npcSpouse + " may ask you about adopting a baby tonight.", StardewModdingAPI.LogLevel.Info);
                        else
                            ModEntry.monitor.Log(npcSpouse + " may ask you about having a baby tonight.", StardewModdingAPI.LogLevel.Info);
                    }

                    //Random check
                    //(ModEntry.GetFamilyData().BabyQuestionChance defaults to 5 -> the default 0.05 value)
                    int questionPercent = ModEntry.GetFamilyData().BabyQuestionChance;
                    if(random.NextDouble() < (questionPercent / 100.0))
                    {
                        if (message)
                        {
                            if (isGaySpouse)
                                ModEntry.monitor.Log(npcSpouse + " will ask you about adopting a baby tonight.", StardewModdingAPI.LogLevel.Info);
                            else
                                ModEntry.monitor.Log(npcSpouse + " will ask you about having a baby tonight.", StardewModdingAPI.LogLevel.Info);
                        }
                        __result = new QuestionEvent(1);
                        return false;
                    }

                    if (message)
                    {
                        if (isGaySpouse)
                            ModEntry.monitor.Log(npcSpouse + " could have asked about adopting a baby tonight, but luck was not on your side. (" + questionPercent + "% chance.)", StardewModdingAPI.LogLevel.Info);
                        else
                            ModEntry.monitor.Log(npcSpouse + " could have asked about having a baby tonight, but luck was not on your side. (" + questionPercent + "% chance.)", StardewModdingAPI.LogLevel.Info);
                    }
                }
                //QuestionEvent for a player spouse
                else if(playerSpouse.HasValue && Game1.player.GetSpouseFriendship().NextBirthingDate == null)
                {
                    Game1.otherFarmers.TryGetValue(playerSpouse.Value, out Farmer otherFarmer);
                    if (otherFarmer != null && otherFarmer.currentLocation == Game1.player.currentLocation && (otherFarmer.currentLocation == Game1.getLocationFromName(otherFarmer.homeLocation.Value) || otherFarmer.currentLocation == Game1.getLocationFromName(Game1.player.homeLocation.Value)))
                    {
                        //(I needed to use reflection because Utility.playersCanGetPregnantHere is a private method.)
                        if(ModEntry.helper.Reflection.GetMethod(typeof(Utility), "playersCanGetPregnantHere", true).Invoke<bool>(new object[] { otherFarmer.currentLocation as FarmHouse }))
                        {
                            //QuestionEvent(3) is possible
                            if (message)
                                ModEntry.monitor.Log(otherFarmer.Name + " may ask about having a baby tonight.", StardewModdingAPI.LogLevel.Info);

                            //Random check
                            //(ModEntry.GetFamilyData().BabyQuestionChance defaults to 5 -> the default 0.05 value)
                            int questionPercent = ModEntry.GetFamilyData().BabyQuestionChance;
                            if(random.NextDouble() < (questionPercent / 100.0))
                            {
                                if (message)
                                    ModEntry.monitor.Log(otherFarmer.Name + " will ask about having a baby tonight.", StardewModdingAPI.LogLevel.Info);
                                __result = new QuestionEvent(3);
                                return false;
                            }
                            
                            if(message)
                                ModEntry.monitor.Log(otherFarmer.Name + " could have asked about having a baby tonight, but luck was not on your side. (There was a " + questionPercent + "% chance.)", StardewModdingAPI.LogLevel.Info);
                        }
                    }
                }
                else if(message)
                {
                    ModEntry.monitor.Log("Your spouse cannot ask about having a baby tonight.");
                }
            }
            
            //If no other event happened, then check for animal events
            __result = random.NextDouble() < 0.5 ? (FarmEvent)new QuestionEvent(2) : (FarmEvent)new SoundInTheNightEvent(2);
            return false;
        }
    }
}