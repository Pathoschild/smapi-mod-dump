/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/isaacs-dev/Minidew-Mods
**
*************************************************/

/*
 * MIT License
 *
 * Copyright (c) 2017-2019 Isaac S.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace FriendsForever
{
    public class Config {
        /// <summary>Whether spouses should be prevented from having friendship decay.</summary>
        public bool AffectSpouses = false;
        /// <summary>Whether dates should be prevented from having friendship decay. Does nothing if married.</summary>
        public bool AffectDates = true;
        /// <summary>Whether everyone else should be prevented from having friendship decay. Effects everyone
        /// but the spouse if they are married.</summary>
        public bool AffectEveryoneElse = true;
        /// <summary>Whether animals should be prevented from having friendship decay. Due to SMAPI limitations you have
        /// to sleep in the bed for this to work.</summary>
        public bool AffectAnimals = true;
    }

    public class ModEntry : Mod {
        internal Config Config;
        internal Dictionary<FarmAnimal, int> AnimalFriendships = new Dictionary<FarmAnimal, int>();

        /// <summary>Mod entry point. Reads the config and adds the listeners.</summary>
        /// <param name="helper">Helper object for various mod functions (such as loading config files).</param>
        public override void Entry(IModHelper helper) {//is Menus.SaveGameMenu
            Config = helper.ReadConfig<Config>();
            helper.Events.GameLoop.DayEnding += EndDay;
            //helper.Events.GameLoop.DayStarted += DebugDay;

            if (!(Config.AffectDates || Config.AffectDates || Config.AffectEveryoneElse || Config.AffectAnimals)) {
                Monitor.Log("This mod can be removed, all features currently disabled.", LogLevel.Warn);
            }
        }
        
        /// <summary>This function is generally used (uncomment code in Entry) to make sure the mod works.</summary>
        private void DebugDay(object sender, DayStartedEventArgs e) {
            var farmers = Game1.getAllFarmers();
            var npcs = Utility.getAllCharacters();

            var farmerArray = farmers as Farmer[] ?? farmers.ToArray();
            foreach (NPC character in npcs) {
                if (!character.isVillager() && !character.IsMonster)
                    continue;

                foreach (Farmer farmer in farmerArray) {
                    if (!farmer.friendshipData.ContainsKey((character.Name)))
                        continue;

                    var friendship = farmer.friendshipData[character.Name];
                    this.Monitor.Log(character.Name + ": " + friendship.Points);
                }
            }

            if (Config.AffectAnimals) {
                var animals = Game1.getFarm().getAllFarmAnimals().Distinct();
                foreach (var animal in animals) {
                    this.Monitor.Log(animal.Name + ": " + animal.friendshipTowardFarmer.Get());
                }
            }
        }
        
        /// <summary>Before the day is done, we need to set all the talked-to flags.</summary>
        private void EndDay(object sender, DayEndingEventArgs e) {
            //This is a host-only mod:
            if (!Context.IsMainPlayer)
                return;

            var farmers = Game1.getAllFarmers();
            var npcs = Utility.getAllCharacters();

            var farmerArray = farmers as Farmer[] ?? farmers.ToArray();
            foreach (NPC character in npcs) {
                if (!character.isVillager() && !character.IsMonster)
                    continue;

                foreach (Farmer farmer in farmerArray) {
                    if (!farmer.friendshipData.ContainsKey(character.Name))
                        continue;

                    var friendship = farmer.friendshipData[character.Name];
                    
                    if (farmer.spouse == character.Name && !Config.AffectSpouses) {
                        continue;
                    //If the they are 'dating' and we're not to affect dates, skip them. The exception to this is if
                    //the farmer has a spouse, in which case we want to treat them like everyone else:
                    } else if (friendship.Status == FriendshipStatus.Dating && !Config.AffectDates
                            && farmer.spouse == null) {
                        continue;
                    } else if (!Config.AffectEveryoneElse) {
                        continue;
                    }

                    //Set the flag for having talked to that character, but don't add any points.
                    //The player can talk to the person themselves and still get the 20 points.
                    friendship.TalkedToToday = true;
                }
            }

            if (Config.AffectAnimals) {
                var animals = Game1.getFarm().getAllFarmAnimals().Distinct();
                foreach (var animal in animals) {
                    animal.wasPet.Set(true);
                }
            }
        }
    }
}
