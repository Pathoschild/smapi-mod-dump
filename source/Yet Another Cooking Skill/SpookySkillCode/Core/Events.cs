/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using BirbCore.Attributes;
using HarmonyLib;
using Microsoft.Xna.Framework;
using MoonShared;
using SpaceCore;
using SpaceCore.Interface;
using SpookySkillCode.Objects;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using StardewValley.GameData.Objects;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using xTile.Dimensions;
using static SpaceCore.Skills;
using Log = BirbCore.Attributes.Log;

namespace SpookySkill.Core
{

    [SEvent]
    internal class Events
    {
        public const string Boo = "moonslime.Spooky.Cooldown";
        public static SpookyProfession Proffession5a =>  Spooky_Skill.Spooky5a;
        public static SpookyProfession Proffession5b =>  Spooky_Skill.Spooky5b;
        public static SpookyProfession Proffession10a1 =>  Spooky_Skill.Spooky10a1;
        public static SpookyProfession Proffession10a2 =>  Spooky_Skill.Spooky10a2;
        public static SpookyProfession Proffession10b1 =>  Spooky_Skill.Spooky10b1;
        public static SpookyProfession Proffession10b2 =>  Spooky_Skill.Spooky10b2;

        [SEvent.GameLaunchedLate]
        private static void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            BirbCore.Attributes.Log.Trace("Spooky: Trying to Register skill.");
            SpaceCore.Skills.RegisterSkill(new Spooky_Skill());
        }

        [SEvent.SaveLoaded]
        private static void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            string id = "moonslime.Spooky";
            int skillLevel = Game1.player.GetCustomSkillLevel(id);
            if (skillLevel == 0)
                return;

//           CheckSkillLevelUp(id, skillLevel, 5, Proffession5a, Proffession5b);
//           CheckSkillLevelUp(id, skillLevel, 10, Proffession10a1, Proffession10a2, Proffession10b1, Proffession10b2);

            AddCraftingRecipes(id, skillLevel);
            AddCookingRecipes(id, skillLevel);
        }

        private static void CheckSkillLevelUp(string id, int skillLevel, int targetLevel, params SpookyProfession[] professions)
        {
            if (skillLevel >= targetLevel && professions.Any(prof => !Game1.player.HasCustomProfession(prof)))
                Game1.endOfNightMenus.Push(new SkillLevelUpMenu(id, targetLevel));
        }

        private static void AddCraftingRecipes(string id, int skillLevel)
        {
            foreach (KeyValuePair<string, string> recipePair in DataLoader.CraftingRecipes(Game1.content))
            {
                string conditions = ArgUtility.Get(recipePair.Value.Split('/'), 4, "");
                if (!conditions.Contains(id) || conditions.Split(" ").Length < 2 || skillLevel < int.Parse(conditions.Split(" ")[1]))
                    continue;

                Game1.player.craftingRecipes.TryAdd(recipePair.Key, 0);
            }
        }

        private static void AddCookingRecipes(string id, int skillLevel)
        {
            foreach (KeyValuePair<string, string> recipePair in DataLoader.CookingRecipes(Game1.content))
            {
                string conditions = ArgUtility.Get(recipePair.Value.Split('/'), 3, "");
                if (!conditions.Contains(id) || conditions.Split(" ").Length < 2 || skillLevel < int.Parse(conditions.Split(" ")[1]))
                    continue;

                if (Game1.player.cookingRecipes.TryAdd(recipePair.Key, 0) && !Game1.player.hasOrWillReceiveMail("robinKitchenLetter"))
                    Game1.mailbox.Add("robinKitchenLetter");
            }
        }

        [SEvent.TimeChanged]
        private static void TimeChanged(object sender, TimeChangedEventArgs e)
        {
            if (!Game1.player.IsLocalPlayer)
                return;

            var farmer = Game1.getFarmer(Game1.player.UniqueMultiplayerID);
            if (!farmer.modDataForSerialization.TryGetValue(Boo, out string value))
                return;

            int storedCooldown = int.Parse(value);
            if (storedCooldown == 0)
                return;

            SetCooldown(farmer, storedCooldown - 1);

            if (storedCooldown - 1 == 0)
            {
                string line = ModEntry.Config.DeScary ? "moonslime.Spooky.Cooldown.Thieving.off_cooldown" : "moonslime.Spooky.Cooldown.Scaring.off_cooldown";
                Game1.addHUDMessage(HUDMessage.ForCornerTextbox(ModEntry.Instance.I18N.Get(line)));
            }
        }

        [SEvent.ButtonReleased]
        private static void ButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (e.Button != ModEntry.Config.Key_Cast || !Game1.player.IsLocalPlayer)
                return;

            Farmer player = Game1.player;
            player.modDataForSerialization.TryGetValue(Boo, out string value_1);
            int storedCooldown = 0;
            if (value_1 != null)
            {
                storedCooldown = int.Parse(value_1);
            }

            if (storedCooldown != 0)
            {
                player.performPlayerEmote("sad");
                string line = ModEntry.Config.DeScary ? "moonslime.Spooky.Cooldown.Thieving.on_cooldown" : "moonslime.Spooky.Cooldown.Scaring.on_cooldown";
                Game1.addHUDMessage(HUDMessage.ForCornerTextbox(ModEntry.Instance.I18N.Get(line)));
                return;
            }

            GameLocation location = player.currentLocation;
            Vector2 playerTile = player.Tile;
            List<NPC> npcsInRange = [];
            List<NPC> monstersInRange = [];


            foreach (var NPC in location.characters)
            {

                float Distance = Vector2.Distance(NPC.Tile, playerTile);
                float profession = (player.HasCustomProfession(Proffession10a1) ? 8 : 2);


                BirbCore.Attributes.Log.Trace("Scaring/Thieving: Button pressed, going to go through the list...");
                BirbCore.Attributes.Log.Trace("NPC name is: "+ NPC.Name);
                BirbCore.Attributes.Log.Trace("is NPC villager?: " + NPC.IsVillager.ToString());
                BirbCore.Attributes.Log.Trace("NPC Distance: "+ Distance.ToString());
                BirbCore.Attributes.Log.Trace("distance value: "+ profession.ToString());
                BirbCore.Attributes.Log.Trace("distance check: " + (Distance > profession).ToString());

                //Check to see if they are a villager
                if (NPC.IsVillager &&
                    //Check to see if they are in range of the player
                    //8 tiles if they have banshee, 2 if not
                    Distance <= profession &&
                    //Check to see if they are giftable
                    NPC.CanReceiveGifts() &&
                    //Make sure the player has friendship data with them
                    player.friendshipData.ContainsKey(NPC.Name) &&
                    //Check to make sure the player has not given them two gifts this week
                    player.friendshipData[NPC.Name].GiftsThisWeek < 2 &&
                    //Check to make sure the player has not given them a gift today
                    player.friendshipData[NPC.Name].GiftsToday < 1 &&
                    //And last, I don't want to give the elderly a heart attack, so leaving Evelyn and George out of this
                    //Sorry Cross-mod Elderfolk
                    NPC.Name != "Evelyn" && NPC.Name != "George")
                {
                    npcsInRange.Add(NPC);
                    continue;
                }

                // The monster side of things. we don't want to continue if the NPC is not a monster.
                if (NPC is not Monster)
                {
                    continue;
                }

                Monster monsterNPC = (Monster)NPC;

                if (monsterNPC.IsMonster &&
                    !monsterNPC.CanSocialize && // Just in case someone got the idea of making a friendly monster a monster class instead of an NPC class
                    !monsterNPC.IsInvisible && // If we cant see the monster, we can't scare or steal from it
                    !monsterNPC.isInvincible() && //If the monster is currently invincible, dont steal or scare from it
                    monsterNPC is not null && //If the NPC is a monster
                    Distance <= profession //and if the monster is in range.
                    )
                {
                    monstersInRange.Add(NPC);
                }
            }

            // We always steal from villagers first, if there are both villagers and monsters on the map
            if (npcsInRange.Any())
            {
                if (player.HasCustomProfession(Proffession10a1))
                {
                    foreach (var npc in npcsInRange)
                    {
                        // If the player has profession Proffession10a1, then all villagers on the list are affected
                        Villager_SPOOKY(npc, player);
                    }
                }
                else
                {
                    // Only the first villager on the list is affected
                    Villager_SPOOKY(npcsInRange[0], player);
                }
                return;
            }

            if (monstersInRange.Any())
            {
                if (player.HasCustomProfession(Proffession10a1))
                {
                    foreach (var monster in monstersInRange)
                    {
                        // If the player has profession Proffession10a1, then all monsters on the list are affected
                        Monster_SPOOKY(monster, player);
                    }
                }
                else
                {
                    // Only the first monster on the list is affected
                    Monster_SPOOKY(monstersInRange[0], player);
                }
                return;
            }

            // If there are no valid NPCs for the player to scare/ steal from ...
            // ... play an emote as feedback
            player.performPlayerEmote("sad");
        }

        public static void Monster_SPOOKY(NPC npc, Farmer player)
        {
            // If the NPC is not a monster, leave this method
            if (npc is not Monster)
            {
                return;
            }
            // the NPC is a monster, so we now cast the NPC as a monster for the rest of the method
            Monster monsterNPC = (Monster)npc;

            // Get the random dice roll from 0 to 99
            int diceRoll = Game1.random.Next(100);
            // Get how spooky the player is.
            // this is the player's spooky level * 2 + the dice roll.
            diceRoll += (Utilities.GetLevel(player) * 2);
            // Add 10 if the player has the ghoul profession
            if (player.HasCustomProfession(Proffession10b2))
            {
                diceRoll += 10;
            }

            // Set the spookyLevel string to 0 for now...
            string spookLevel = "level_0";

            // Set the sound that will play when we scare the monster
            string soundID = Game1.random.Choose("ghost", "explosion", "dog_bark", "thunder", "shadowpeep");
            if (ModEntry.Config.DeScary)
            {
                // If the player has the skill set to Thieving, then we...
                // ... Get the spooky level and calulate it here and
                // ... Set the sound ID to just a swish of the wind
                spookLevel = GetSpookLevel(diceRoll);
                soundID = "wind";
            } else
            {
                // If the player has the skill set to Scaring, then we ...
                // ... Add the fall adjustment to the dice roll
                diceRoll = FallAdjustment(diceRoll);
                // ... Get the spooky level now
                spookLevel = GetSpookLevel(diceRoll);

                // and if the spooky level is 3 or 4, we knock back the monster
                bool jump = spookLevel == "level_3" || spookLevel == "level_4";
                if (jump)
                {
                    Monster_Knockback(monsterNPC, 3f, player);
                }
            }

            // We play an emote and sound as feedback for the skill to the player that it is functioning
            player.performPlayerEmote("exclamation");
            player.currentLocation.playSound(soundID);

            // Create the loot the monster will drop
            Monster_CreateLoot(monsterNPC, player, spookLevel);

            // If the player has profession 10a2, then we heal the player 25% of their health and stamina.
            if (player.HasCustomProfession(Proffession10a2))
            {
                const double healingPercentage = 0.25;

                // Healing Health
                int healthIncrease = (int)(player.maxHealth * healingPercentage);
                player.health = Math.Min(player.health + healthIncrease, player.maxHealth);

                // Restoring Stamina
                int staminaIncrease = (int)(player.MaxStamina * healingPercentage);
                player.Stamina = Math.Min(player.Stamina + staminaIncrease, player.MaxStamina);
            }

            int exp = ((int)(CalculateExpSpookLevel(spookLevel) * 0.33));

            Utilities.AddEXP(player, exp);
        }



        public static void Villager_SPOOKY(NPC npc, Farmer player)
        {
            // Get the random dice roll from 0 to 99
            int diceRoll = Game1.random.Next(100);
            // Get how spooky the player is.
            // this is the player's spooky level * 2 + the dice roll.
            diceRoll += (Utilities.GetLevel(player) * 2);
            // Get the direction the player is facing vs the direction the NPC is facing.
            // Like if the player and NPC are facing each other, is the player facing the side, or the NPC's back
            string facingSide = GetFacingSide(npc, player);
            // Get the spook level adjustment based on if the player is facing the NPC's back, sides, or front
            diceRoll = CalculateSpookyDirectionChange(diceRoll, facingSide, player.HasCustomProfession(Proffession5a));

            // Add 10 if the player has the ghoul profession
            if (player.HasCustomProfession(Proffession10b2))
            {
                diceRoll += 10;
            }

            // Set the spook level string to the default value
            string spookLevel = "level_0";

            // Set the sound ID of what we will play
            string soundID = Game1.random.Choose("ghost", "explosion", "dog_bark", "thunder", "shadowpeep");

            // If the player has the zombie profession, they have a 50% chance of ignoring friendship lost
            bool zombieCharm = player.HasCustomProfession(Proffession5b) && Game1.random.NextDouble() < 0.5;

            // Calculate friendship lost based on facing direction
            int friendshipLost = CalculateFriendshipChangeDirectional(facingSide, zombieCharm);

            // Thieving skill only adjustments
            if (ModEntry.Config.DeScary)
            {
                soundID = "wind";
                spookLevel = GetSpookLevel(diceRoll);

                #region Player Skill Feedback

                // This section is to give feedback to the player

                // Make the villager jump if the player 
                // Make the NPC jump if they are scared enough
                bool jump = spookLevel == "level_0" || spookLevel == "level_1";
                if (jump) npc.Halt();
                if (jump) npc.jump();
                #endregion
            }
            // Scary skill only adjustments
            else
            {
                // Fall adjustment for the spooky roll
                diceRoll = FallAdjustment(diceRoll);

                spookLevel = GetSpookLevel(diceRoll);

                // People are moer in the mood to get scared during the fall
                friendshipLost = FallAdjustment(friendshipLost);

                #region Player Skill Feedback

                // Make the villager jump if the player 
                // Make the NPC jump if they are scared enough
                bool jump = spookLevel == "level_3" || spookLevel == "level_4";
                if (jump) npc.Halt();
                if (jump) npc.jump();
                #endregion
            }

            #region Player Skill Feedback

            // Play an emote above the player's head
            player.performPlayerEmote("exclamation");

            // Play a sound
            player.currentLocation.playSound(soundID);

            // Set the string to the current NPC's name and the spook level. So each NPC can have a custom string
            string type = ModEntry.Config.DeScary ? "Stolen" : "Scared";
            string spookString = ModEntry.Instance.I18N.Get($"moonslime.Spooky.{type}.{npc.Name}.{spookLevel}");
            if (spookString.Contains("no translation"))
            {
                // If no translation/custom string is found, set it to the default string for the spook level
                spookString = ModEntry.Instance.I18N.Get($"moonslime.Spooky.{type}.default.{spookLevel}");
            }

            // Show text above the NPC's head
            npc.showTextAboveHead(spookString);

            #endregion

            // Adjust friendship lost based on spook level
            friendshipLost = CalculateFriendshipSpookLevel(friendshipLost, spookLevel, zombieCharm);

            // Now add the friendship data to the NPC and make this count as a gift for the day and week
            Friendship friendship = player.friendshipData[npc.Name];
            friendship.GiftsToday++;
            friendship.GiftsThisWeek++;
            friendship.LastGiftDate = new WorldDate(Game1.Date);
            friendship.Points += friendshipLost;

            // Calculate if the villager is going to drop loot or not
            Villager_CreateLoot(npc, player, spookLevel);

            // If the player has profession 10a2, then we heal the player 25% of their health and stamina.
            if (player.HasCustomProfession(Proffession10a2))
            {
                const double healingPercentage = 0.25;

                // Healing Health
                int healthIncrease = (int)(player.maxHealth * healingPercentage);
                player.health = Math.Min(player.health + healthIncrease, player.maxHealth);

                // Restoring Stamina
                int staminaIncrease = (int)(player.MaxStamina * healingPercentage);
                player.Stamina = Math.Min(player.Stamina + staminaIncrease, player.MaxStamina);
            }

            // Add exp to the player based on how well spooked the villager is
            Utilities.AddEXP(player, CalculateExpSpookLevel(spookLevel));
        }

        private static int FallAdjustment(int value)
        {
            // People are more in the mood to get scared during the fall
            // You are also more scary in the fall
            if (Game1.currentSeason == "fall")
            {
                value += 5;
                // People are moer in the mood to get scared on Spirit's Eve
                // You are also more scary on Spirit's Eve
                if (Game1.dayOfMonth == 27)
                {
                    value += 5;
                }
            }
            return value;
        }

        public static void Monster_Knockback(Monster monster, float knockBackModifier, Farmer who)
        {
            Microsoft.Xna.Framework.Rectangle boundingBox = monster.GetBoundingBox();
            who.currentLocation.damageMonster(boundingBox, 0, 0, false, knockBackModifier, 100, 0f, 1f, triggerMonsterInvincibleTimer: false, who);
            monster.stunTime.Value += 200;

        }

        public static void Monster_CreateLoot(Monster npc, Farmer player, string spookLevel)
        {
            var lootList = new List<string>();

            // Define thresholds for different spook levels
            Dictionary<string, int> spookThresholds = new Dictionary<string, int>
            {
                { "level_0", 100 },
                { "level_1", 66 },
                { "level_2", 33 },
                { "level_3", 0 },
                { "level_4", 0 } 
            };

            //Get the player level
            int playerLevel = Utilities.GetLevel(player);

            // Loot generation based on player level and profession
            if (playerLevel >= 3)
            {
                AddLootFromList(lootList, Game1.NPCGiftTastes["Universal_Neutral"]);
            }

            if (playerLevel >= 7)
            {
                AddLootFromList(lootList, Game1.NPCGiftTastes["Universal_Like"]);
            }

            if (player.HasCustomProfession(Proffession10b1))
            {
                AddLootFromList(lootList, Game1.NPCGiftTastes["Universal_Love"]);
                lootList.AddRange(new List<string> { "516", "517", "518", "519", "520", "521", "522", "523", "524", "525", "526", "528", "529", "530", "531", "532", "533", "534" });
            }

            lootList.AddRange(npc.objectsToDrop);

            // Filter out invalid items and select a random valid item
            var validItems = lootList.Where(item => !string.IsNullOrEmpty(item) && !item.StartsWith('-') && ItemRegistry.GetData(item) != null).ToList();
            lootList.Clear();
            string selectedItem = validItems.RandomChoose(Game1.random, "766");


            // Log the selected item
            Log.Warn("Attempting to steal: " + ItemRegistry.GetData(selectedItem).DisplayName);

            // Calculate spooky roll
            int diceRoll = playerLevel + playerLevel + Game1.random.Next(100);

            // Apply profession bonus
            if (player.HasCustomProfession(Proffession10b2))
            {
                diceRoll += 10;
            }

            // Determine loot drop based on spook level
            if (diceRoll > spookThresholds[spookLevel])
            {
                if (spookLevel == "level_4")
                {
                    int howMany = diceRoll / 10;
                    Game1.createMultipleObjectDebris(selectedItem, npc.TilePoint.X, npc.TilePoint.Y, howMany, player.UniqueMultiplayerID, npc.currentLocation);
                }
                else
                {
                    Game1.createObjectDebris(selectedItem, npc.TilePoint.X, npc.TilePoint.Y, npc.currentLocation);
                }

                // Set cooldown based on spook level
                SetCooldown(player, 5 - Array.IndexOf(spookThresholds.Keys.ToArray(), spookLevel));
            }
        }


        public static void Villager_CreateLoot(NPC npc, Farmer player, string spookLevel)
        {
            // Generate a list of items based off the NPC's likes, neutrals, and loves
            var lootList = GetItemList(npc, player);

            // Make a new list
            var finalList = new List<string>();

            // For each item in the loot list, we discard any that might be...
            // a null spot, a negative item (which is a catagory), and if it has no Item Data (so an error item)
            foreach (string lootItem in lootList)
            {
                if (lootItem != null && !lootItem.StartsWith('-') && ItemRegistry.GetData(lootItem) != null)
                {
                    finalList.Add(lootItem);
                }
            }
            // Clear the old list as it's not needed anymore
            lootList.Clear();

            // Add Mayor Lewis' shorts to the list if the NPC is Lewis or Marnie
            if (npc.Name == "Lewis" || npc.Name == "Marnie")
            {
                finalList.Add("789");
            }

            // Shuffle the list to add in an additional layer of random to it after we finished adding all the items to it.
            finalList.Shuffle(Game1.random);

            // Select a random item from the final list
            // If there is somehow nothing on the final list, return bread
            string item = finalList.Count > 0 ? finalList[Game1.random.Next(finalList.Count)] : "216";

            // Make a dice roll
            int diceRoll = Game1.random.Next(100);

            // Add twice the player's level to the dice roll
            diceRoll += (Utilities.GetLevel(player) * 2);

            if (player.HasCustomProfession(Proffession10b2))
            {
                diceRoll += 10;
            }

            // Determine the cooldown duration based on spook level
            int cooldownDuration = 5 - (spookLevel switch
            {
                "level_1" => 1,
                "level_2" => 2,
                "level_3" => 3,
                "level_4" => 4,
                _ => 0 // Default case for "level_0" or unrecognized spook levels
            });

            // Create object debris based on spooky roll
            if (diceRoll > 100 - (33 * cooldownDuration))
            {
                if (spookLevel == "level_4")
                {
                    int howMany = diceRoll / 10;
                    Game1.createMultipleObjectDebris(item, npc.TilePoint.X, npc.TilePoint.Y, howMany, player.UniqueMultiplayerID, npc.currentLocation);
                }
                else
                {
                    Game1.createObjectDebris(item, npc.TilePoint.X, npc.TilePoint.Y, npc.currentLocation);
                }
            }
            // We set the cooldown here when we scare villages. It is based on the 
            SetCooldown(player, cooldownDuration);


            if (player.modDataForSerialization.TryGetValue(Boo, out string value))
            {
                Log.Warn("Spooky skill cooldown is now set to: " + value);
            }
        }


        // Method to generate a list of items from villager's likes, neutrals, and loves
        public static List<string> GetItemList(NPC npc, Farmer player)
        {
            List<string> lootList = [];

            if (!player.HasCustomProfession(Proffession10b1))
            {
                if (Utilities.GetLevel(player) >= 3)
                {
                    AddLootFromList(lootList, Game1.NPCGiftTastes["Universal_Neutral"]);
                }
                if (Utilities.GetLevel(player) >= 7)
                {
                    AddLootFromList(lootList, Game1.NPCGiftTastes["Universal_Like"]);
                }
            }
            if (Game1.year != 1 || player.HasCustomProfession(Proffession10b1))
            {
                AddLootFromList(lootList, Game1.NPCGiftTastes["Universal_Love"]);
            }

            Game1.NPCGiftTastes.TryGetValue(npc.Name, out string value2);
            string[] array5 = value2.Split('/');
            List<string[]> npcTastesList = new List<string[]>();
            for (int i = 0; i < 10; i += 2)
            {
                string[] array6 = ArgUtility.SplitBySpace(array5[i + 1]);
                string[] array7 = new string[array6.Length];
                for (int j = 0; j < array6.Length; j++)
                {
                    if (array6[j].Length > 0)
                    {
                        array7[j] = array6[j];
                    }
                }

                npcTastesList.Add(array7);
            }


            // Assume list2 is defined somewhere and populated properly as per earlier description.
            if (player.HasCustomProfession(Proffession10b1))
            {
                // Player has vampire profession, only add loved items
                lootList.AddRange(npcTastesList[0].Where(item => !lootList.Contains(item)));
            }
            else
            {
                // Player does not have vampire profession, add Loved, Like, and Neutral items
                for (int i = 0; i < 5; i++)
                {
                    if (i == 0 || i == 1 || i == 4) // Assuming indices 0, 1, 4 are Loved, Like, Neutral
                    {
                        foreach (string item in npcTastesList[i])
                        {
                            if (!lootList.Contains(item))
                            {
                                lootList.Add(item);
                            }
                        }
                    }
                    else
                    {
                        // Remove items not Loved, Liked, or Neutral (if they were mistakenly added)
                        foreach (string item in npcTastesList[i])
                        {
                            lootList.Remove(item);
                        }
                    }
                }
            }

            return lootList.Distinct().ToList();
        }


        // Method to add items to loot list from a given list of items
        private static void AddLootFromList(List<string> lootList, string itemList)
        {
            lootList.AddRange(ArgUtility.SplitBySpace(itemList));
            lootList.Shuffle(Game1.random);
        }

        // Method to calculate the spook level
        private static string GetSpookLevel(int spookyLevel)
        {
            if (spookyLevel <= 24) return "level_0";
            if (spookyLevel <= 55) return "level_1";
            if (spookyLevel <= 90) return "level_2";
            if (spookyLevel <= 100) return "level_3";
            return "level_4";
        }

        // Method to calculate the change of friendship based on the direction the NPC is facing
        private static int CalculateFriendshipChangeDirectional(string facingSide, bool zombieCharm)
        {
            int friendshipLost = 0;
            if (facingSide == "side") friendshipLost += zombieCharm ? 0 : -4;
            if (facingSide == "front") friendshipLost += zombieCharm ? 0 : -8;
            if (facingSide == "back") friendshipLost += zombieCharm ? 0 : -2; ///the player feels guilty about it so they still loose friendship even if hidden from the NPC
            return friendshipLost;
        }

        // Method to calculate the change of friendship based the spook level
        private static int CalculateFriendshipSpookLevel(int friendshipLost, string facingSide, bool zombieCharm)
        {
            if (facingSide == "level_0") friendshipLost += zombieCharm ? 0 : -8;
            if (facingSide == "level_1") friendshipLost += zombieCharm ? 0 : -4;
            if (facingSide == "level_2") friendshipLost += zombieCharm ? 0 : -2;
            return friendshipLost;
        }

        // Method to calculate addition or subtraction to the player's spookyRoll based on what side of the NPC they are scaring from
        private static int CalculateSpookyDirectionChange(int spookyRoll, string facingSide, bool spooky5a)
        {

            if (facingSide == "side") spookyRoll += spooky5a ? 0 : 0;
            if (facingSide == "front") spookyRoll += spooky5a ? 0 : -10;
            if (facingSide == "back") spookyRoll += spooky5a ? 0 : +10;
            return spookyRoll;
        }

        // Method to calculate if the player is facing the NPC's front, sides, or back when scaring
        private static string GetFacingSide(NPC npc, Farmer player)
        {
            int npcDirection = npc.FacingDirection;
            int playerDirection = player.FacingDirection;
            bool isSide = (playerDirection == 0 && npcDirection == 1) ||
                          (playerDirection == 0 && npcDirection == 3) ||
                          (playerDirection == 1 && npcDirection == 0) ||
                          (playerDirection == 1 && npcDirection == 2) ||
                          (playerDirection == 2 && npcDirection == 1) ||
                          (playerDirection == 2 && npcDirection == 3) ||
                          (playerDirection == 3 && npcDirection == 0) ||
                          (playerDirection == 3 && npcDirection == 2);
            bool isBack = (playerDirection == 0 && npcDirection == 2) ||
                          (playerDirection == 1 && npcDirection == 3) ||
                          (playerDirection == 2 && npcDirection == 0) ||
                          (playerDirection == 3 && npcDirection == 1);
            if (isSide) return "side";
            if (isBack) return "back";
            return "front";
        }


        // Method to figure out how much exp the player should get from scaring/stealing based on how "well" they did
        private static int CalculateExpSpookLevel(string spookLevel)
        {
            float exp = 0;
            if (spookLevel == "level_0") exp += ModEntry.Config.ExpMod * ModEntry.Config.ExpFromFail;
            if (spookLevel == "level_1") exp += ModEntry.Config.ExpMod * ModEntry.Config.ExpLevel1;
            if (spookLevel == "level_2") exp += ModEntry.Config.ExpMod * ModEntry.Config.ExpLevel2;
            if (spookLevel == "level_3") exp += ModEntry.Config.ExpMod * ModEntry.Config.ExpLevel3;
            if (spookLevel == "level_4") exp += ModEntry.Config.ExpMod * ModEntry.Config.ExpLevel4;
            return (int)Math.Floor(exp);
        }



        // Method to set the cooldown timer for Scaring/Stealing
        private static void SetCooldown(Farmer who, int cooldown)
        {
            //Make sure the player is not null and they are the local player
            if (who != null && who.IsLocalPlayer)
            {
                //Set the variables up
                //Make sure to get the unique multiplayer to make sure we are setting the cooldown of the right player
                const string scaryMessageKey = "moonslime.Spooky.Cooldown.Scaring.apply";
                const string stealingMessageKey = "moonslime.Spooky.Cooldown.Thieving.apply";
                string coolDownAsString = cooldown.ToString();
                Farmer player = Game1.getFarmer(who.UniqueMultiplayerID);

                //Check to see if they have the mod Data already and get that value
                if (player.modDataForSerialization.TryGetValue(Boo, out string storedCooldownString))
                {
                    //Change the value into an int
                    int storedCooldown = int.Parse(storedCooldownString);
                    //If the value is 0, we are going to be setting the cooldown, so send the player a message about it
                    if (storedCooldown == 0)
                    {
                        string messageKey = ModEntry.Config.DeScary ? stealingMessageKey : scaryMessageKey;
                        Game1.addHUDMessage(HUDMessage.ForCornerTextbox(ModEntry.Instance.I18N.Get(messageKey)));
                    }
                    //If the cooldown we want to set does not equal the stored cooldown...
                    if (cooldown != storedCooldown)
                    {
                        //.... we set the modData to the cooldown we want to set
                        player.modDataForSerialization[Boo] = coolDownAsString;
                    }
                }
                //If there is no modData value for our key...
                else
                {
                    //We set the modData value to what we want the cooldown to be
                    player.modDataForSerialization.TryAdd(Boo, coolDownAsString);
                    //And we send the player a message saying Scaring/Thieving is now on cooldown
                    string messageKey = ModEntry.Config.DeScary ? stealingMessageKey : scaryMessageKey;
                    Game1.addHUDMessage(HUDMessage.ForCornerTextbox(ModEntry.Instance.I18N.Get(messageKey)));
                }
            }
        }
    }
}
