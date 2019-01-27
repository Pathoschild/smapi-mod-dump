using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using Harmony;
using System.Reflection;

namespace petChoicePerks
{
    public class ModEntry : Mod
    {
        Buff fishBuff;
        Buff forageBuff;
        int buffDuration;
        bool buffApplied;

        public static IMonitor ModMonitor;
        public static int PreviousDate = 0;

        /*********
        ** Public methods
        *********/
        public override void Entry(IModHelper helper)
        {
            ModMonitor = this.Monitor;                                

            Helper.Events.GameLoop.SaveLoaded += this.SaveLoaded;
            Helper.Events.GameLoop.DayStarted += this.TimeEvents_AfterDayStarted;
            Helper.Events.Input.ButtonReleased += this.ButtonReleased;
            Helper.Events.GameLoop.DayEnding += this.DayEnding;

            // Create a new Harmony instance for patching source code
            HarmonyInstance harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            // Get the method we want to patch
            MethodInfo targetMethod = AccessTools.Method(typeof(Farm), nameof(Farm.addCrows));

            // Get the patch that was created
            MethodInfo prefix = AccessTools.Method(typeof(ModEntry), nameof(ModEntry.Prefix));

            // Apply the patch
            harmony.Patch(targetMethod, prefix: new HarmonyMethod(prefix));
        }

        /*********
        ** Private methods
        *********/

        // Initialize buffs
        private void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {            
            fishBuff = new Buff(0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 30, "", "");
            fishBuff.description = "Petting your cat has given you a temporary fishing bonus.";
            fishBuff.sheetIndex = 1;

            forageBuff = new Buff(0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 30, "", "");
            forageBuff.description = "Petting your dog has given you a temporary foraging bonus.";
            forageBuff.sheetIndex = 5;
        }

        // Calculate buff duration based on player's daily luck
        private void TimeEvents_AfterDayStarted(object sender, DayStartedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            double luck = Game1.dailyLuck;
            //this.Monitor.Log("Player's luck: " + luck);

            // Reset daily buff
            buffApplied = false;

            // Base duration of the buff is eight hours
            buffDuration = 6*60;

            // TODO: Add/subtract a couple of hours based on daily luck
            double bonus = 0;

            // No bonus on a neutral day
            if (luck <= 0.02 && luck >= -0.02)
            {
                bonus = 0;
                //this.Monitor.Log("No bonus today.");
            }
            // No buff on a very bad luck day
            else if (luck < -0.07)
            {
                buffApplied = true;
                //this.Monitor.Log("No buff today.");
            }
            // Buff lasts the whole day on a very good luck day
            else if (luck > 0.07)
            {
                buffDuration = 24 * 60;
                //this.Monitor.Log("Buff will last the whole day!");
            }
            else
            {
                // Luck is between -0.07 and -0.03 and 0.03 and 0.07
                // So the variable bonus duration with this formula is between 1 and approx. 2.5 hours / -1 and -2.5 hours
                double luckDuration = luck * (100 / 3);
                //this.Monitor.Log("Bonus duration is " + luckDuration + " hours.");

                // Convert to minutes
                bonus = luckDuration * 60;

                // Positive bonus is double the fun, so the overall range for a bonus can be -1 - -2.5 hours and +2 - +5 hours
                // Total duration range is thus 3.5 hours - 11 hours (or 0 or 24 if you're especially lucky or unlucky)
                if (bonus > 0)
                    bonus *= 2;
               
                buffDuration += (int)bonus;
            }

            //this.Monitor.Log("Total buff duration is " + buffDuration / 60 + " hours");

            // Convert to game time minutes
            buffDuration = buffDuration / 10 * 7000;
        }

        // Apply animal happiness bonus if player has a dog
        private void DayEnding(object sender, DayEndingEventArgs e)
        {
            if (Game1.player.hasPet() && !Game1.player.catPerson)
            {
                List<FarmAnimal> animals = new List<FarmAnimal>();

                foreach (FarmAnimal a in Game1.getFarm().animals.Values)
                {
                    animals.Add(a);
                }

                foreach (StardewValley.Buildings.Building b in Game1.getFarm().buildings)
                {
                    if (b.indoors.Value != null && b.indoors.Value.GetType() == typeof(AnimalHouse))
                        animals.AddRange(((AnimalHouse)b.indoors.Value).animals.Values);
                }

                foreach (FarmAnimal a in animals)
                {
                    // Since max value is 255, make sure it doesn't overflow
                    if (a.happiness.Value <= 230)
                        a.happiness.Value += 25;
                    else
                        a.happiness.Value = 255;
                }
            }
        }

        // Check if the player just pet his ... pet and apply buff
        private void ButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if(Context.IsWorldReady && e.Button.IsActionButton() && !buffApplied)
            {
                //this.Monitor.Log(Game1.player.currentLocation.uniqueName);                

                // Find the player's pet
                if (Game1.player.hasPet())
                {
                    StardewValley.Characters.Pet thePet = (StardewValley.Characters.Pet)Game1.getCharacterFromName(Game1.player.getPetName());

                    // Was it pet today (ie. just now)? If so, apply the buff
                    if (this.Helper.Reflection.GetField<bool>(thePet, "wasPetToday").GetValue())
                    {
                        if (Game1.player.catPerson)
                        {
                            fishBuff.millisecondsDuration = buffDuration;
                            Game1.buffsDisplay.addOtherBuff(fishBuff);
                        }
                        else
                        {
                            forageBuff.millisecondsDuration = buffDuration;
                            Game1.buffsDisplay.addOtherBuff(forageBuff);
                        }

                        buffApplied = true;
                    }
                }
            }
        }
        
        // Override crow spawning if the player has a cat
        private static bool Prefix(ref Farm __instance)
        {
            // Check if player has a cat when day changes
            bool hasCat = false;

            if(ModEntry.PreviousDate != Game1.dayOfMonth)
            {
                ModEntry.PreviousDate = Game1.dayOfMonth;
                if (Game1.player.catPerson && Game1.player.hasPet())
                    hasCat = true;
                else if (!Game1.player.catPerson && Game1.player.hasPet())
                    hasCat = false;
            }            

            //if (hasCat)
                //ModEntry.ModMonitor.Log("Player has a cat. No crows will spawn.");
            //else
                //ModEntry.ModMonitor.Log("Player doesn't have a cat. Crows will spawn.");

            return !hasCat;
        }
    }
}
