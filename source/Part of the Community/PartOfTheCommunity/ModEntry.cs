using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Events;
using StardewValley.Locations;
using StardewValley.Menus;

namespace SB_PotC
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        private readonly string[] StoreOwners = { "Pierre", "Gus", "Clint", "Marnie", "Robin", "Sandy", "Willy" };

        private const int HasTalked = 0;
        private const int ReceivedGift = 1;
        private const int RelationsGifted = 2;

        private SerializableDictionary<string, SerializableDictionary<string, string>> CharacterRelationships;
        private SerializableDictionary<string, int[]> WitnessCount;
        private SerializableDictionary<string, bool> HasShoppedInStore;
        private bool HasEnteredEvent;
        private int CurrentNumberOfCompletedBundles;
        private uint CurrentNumberOfCompletedDailyQuests;
        private bool HasRecentlyCompletedQuest;
        private int DaysAfterCompletingLastDailyQuest;
        private int CurrentUniqueItemsShipped;
        private bool IsReady;
        private ModConfig Config;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {

            if (this.CharacterRelationships == null || this.WitnessCount == null)
            {
                if (this.CharacterRelationships == null)
                    this.CharacterRelationships = new SerializableDictionary<string, SerializableDictionary<string, string>>();
                if (this.WitnessCount == null)
                    this.WitnessCount = new SerializableDictionary<string, int[]>();
            }
            GameEvents.UpdateTick += this.ModUpdate;
            SaveEvents.AfterLoad += this.SetVariables;
            SaveEvents.AfterReturnToTitle += this.Reset;
            SaveEvents.BeforeSave += this.EndOfDayUpdate;
            SaveEvents.AfterSave += this.SaveConfigFile;
            this.HasShoppedInStore = new SerializableDictionary<string, bool>();
            this.HasRecentlyCompletedQuest = false;
            this.DaysAfterCompletingLastDailyQuest = -1;
            this.IsReady = false;
        }


        /*********
        ** Private methods
        *********/
        private void SaveConfigFile(object sender, EventArgs e)
        {
            this.Helper.WriteJsonFile($"{Constants.SaveFolderName}/config.json", this.Config);
        }

        private void SetVariables(object sender, EventArgs e)
        {
            this.Config = this.Helper.ReadJsonFile<ModConfig>($"{Constants.SaveFolderName}/config.json") ?? new ModConfig();
            foreach (string name in Game1.player.friendships.Keys)
            {
                this.CheckRelationshipData(name);
                this.WitnessCount.Add(name, new int[4]);
            }
            this.CurrentNumberOfCompletedBundles = ((CommunityCenter)Game1.getLocationFromName("CommunityCenter")).numberOfCompleteBundles();
            if (!this.Config.HasGottenInitialUjimaBonus)
            {
                foreach (string storeOwner in this.StoreOwners)
                {
                    if (Game1.getCharacterFromName(storeOwner) == null) continue;
                    Game1.player.changeFriendship((this.Config.UjimaBonus * this.CurrentNumberOfCompletedBundles), Game1.getCharacterFromName(storeOwner));
                }

                this.Monitor.Log($"You have gained {20 * this.CurrentNumberOfCompletedBundles} friendship from all store owners for completing {this.CurrentNumberOfCompletedBundles} {(this.CurrentNumberOfCompletedBundles > 1 ? "Bundles" : "Bundle")}", LogLevel.Info);
                this.Config.HasGottenInitialUjimaBonus = true;
            }
            this.CurrentUniqueItemsShipped = Game1.player.basicShipped.Count;
            if (!this.Config.HasGottenInitialKuumbaBonus)
            {
                int friendshipPoints = this.Config.KuumbaBonus * this.CurrentUniqueItemsShipped;
                Utility.improveFriendshipWithEveryoneInRegion(Game1.player, friendshipPoints, 2);
                this.Monitor.Log($"Gained {friendshipPoints} friendship for shipping {this.CurrentUniqueItemsShipped} unique {(this.CurrentUniqueItemsShipped != 1 ? "items" : "item")}", LogLevel.Info);
                this.Config.HasGottenInitialKuumbaBonus = true;
            }
            this.CurrentNumberOfCompletedDailyQuests = Game1.stats.questsCompleted;
            this.IsReady = true;
        }

        private void Reset(object sender, EventArgs e)
        {
            this.IsReady = false;
            this.Config = null;
            this.CharacterRelationships.Clear();
            this.WitnessCount.Clear();
            this.HasShoppedInStore.Clear();
            this.HasEnteredEvent = false;
            this.HasRecentlyCompletedQuest = false;
            this.CurrentNumberOfCompletedBundles = 0;
            this.CurrentNumberOfCompletedDailyQuests = 0;
            this.CurrentUniqueItemsShipped = 0;
            this.DaysAfterCompletingLastDailyQuest = 0;
        }

        private static List<Character> AreThereCharactersWithinDistance(Vector2 tile, int tilesAway, GameLocation location)
        {
            List<Character> charactersWithinDistance = new List<Character>();
            foreach (NPC character in location.characters)
            {
                if (character != null && Vector2.Distance(character.getTileLocation(), tile) <= tilesAway)
                    charactersWithinDistance.Add(character);
            }
            return charactersWithinDistance;
        }

        private void ModUpdate(object sender, EventArgs e)
        {
            if (!Game1.hasLoadedGame || SaveGame.IsProcessing || !this.IsReady)
                return;

            foreach (string name in Game1.player.friendships.Keys.ToArray())
            {
                if (Game1.getCharacterFromName(name) == null)
                    continue;

                // if the NPC was divorced by the player, nothing occurs
                if (Game1.getCharacterFromName(name).divorcedFromFarmer)
                    continue;

                //check if Player gave NPC a gift
                if (Game1.player.friendships[name][3] == 1)
                {
                    if (!this.WitnessCount.ContainsKey(name))
                        this.WitnessCount.Add(name, new int[4]);
                    if (this.WitnessCount[name][ModEntry.ReceivedGift] < 1)
                    {
                        this.CheckRelationshipData(name);
                        // if the gift made the reciever decrease their friendship, do nothing, else
                        foreach (string relation in this.CharacterRelationships[name].Keys.ToArray())
                        {
                            if (string.IsNullOrEmpty(relation) || Game1.getCharacterFromName(relation) == null)
                                continue;
                            if (!this.WitnessCount.ContainsKey(relation))
                                this.WitnessCount.Add(relation, new int[4]);
                            if (Game1.getCharacterFromName(relation).divorcedFromFarmer)
                                continue;
                            CheckRelationshipData(relation);
                            if (this.WitnessCount[relation][ModEntry.RelationsGifted] < this.CharacterRelationships[relation].Count)
                                this.WitnessCount[relation][ModEntry.RelationsGifted]++;
                        }
                        this.WitnessCount[name][ModEntry.ReceivedGift] = 1;
                    }
                }

                //check if player is talking to a NPC
                if (Game1.player.hasTalkedToFriendToday(name))
                {
                    if (!this.WitnessCount.ContainsKey(name))
                        this.WitnessCount.Add(name, new int[4]);
                    if (this.WitnessCount[name][ModEntry.HasTalked] < 1)
                    {
                        CheckRelationshipData(name);
                        List<Character> charactersWithinDistance = ModEntry.AreThereCharactersWithinDistance(Game1.player.getTileLocation(), 20, Game1.player.currentLocation);
                        foreach (Character characterWithinDistance in charactersWithinDistance)
                        {
                            if (characterWithinDistance == null || characterWithinDistance.name == name)
                                continue;
                            if (Game1.getCharacterFromName(characterWithinDistance.name).divorcedFromFarmer)
                                characterWithinDistance.doEmote(12);
                            else
                            {
                                if (!this.WitnessCount.ContainsKey(characterWithinDistance.name))
                                    this.WitnessCount.Add(characterWithinDistance.name, new int[4]);
                                this.WitnessCount[characterWithinDistance.name][3]++;
                                if (this.WitnessCount[characterWithinDistance.name][3] != 0 && (this.WitnessCount[characterWithinDistance.name][3] & (this.WitnessCount[characterWithinDistance.name][3] - 1)) == 0)
                                {
                                    characterWithinDistance.doEmote(32);
                                    Game1.player.changeFriendship(this.Config.WitnessBonus, (characterWithinDistance as NPC));
                                    this.Monitor.Log($"{characterWithinDistance.name} saw you taking to a {name}. +{this.Config.WitnessBonus} Friendship: {characterWithinDistance.name}", LogLevel.Info);
                                }
                            }
                        }
                        this.WitnessCount[name][ModEntry.HasTalked] = 1;
                    }
                }
            }

            //Check if the player is actively shopping
            //TODO: Add the Bus/Pam
            if (Game1.activeClickableMenu is ShopMenu shopMenu)
            {
                Item heldItem = this.Helper.Reflection.GetPrivateValue<Item>(shopMenu, "heldItem");
                if (heldItem != null)
                {
                    string shopOwner = "";
                    switch (Game1.currentLocation.name)
                    {
                        case "SeedShop":
                            shopOwner = "Pierre";
                            break;
                        case "AnimalShop":
                            shopOwner = "Marnie";
                            break;
                        case "Blacksmith":
                            shopOwner = "Clint";
                            break;
                        case "FishShop":
                            shopOwner = "Willy";
                            break;
                        case "ScienceHouse":
                            shopOwner = "Robin";
                            break;
                        case "Saloon":
                            shopOwner = "Gus";
                            break;
                        case "Mine":
                            shopOwner = "Dwarf";
                            break;
                        case "SandyHouse":
                            shopOwner = "Sandy";
                            break;
                        case "Sewer":
                            shopOwner = "Krobus";
                            break;
                    }
                    if (!string.IsNullOrEmpty(shopOwner))
                    {
                        if (!this.HasShoppedInStore.ContainsKey(shopOwner))
                            this.HasShoppedInStore.Add(shopOwner, false);
                        if (Game1.getCharacterFromName(shopOwner) != null && this.HasShoppedInStore[shopOwner] == false)
                        {
                            Game1.player.changeFriendship(this.Config.UjamaaBonus, Game1.getCharacterFromName(shopOwner));
                            this.Monitor.Log($"{shopOwner}: Pleasure doing business with you!", LogLevel.Info);
                            this.HasShoppedInStore[shopOwner] = true;
                        }
                    }
                }
            }

            // Check if the Player has entered a festival
            if (this.HasEnteredEvent == false)
            {
                if (Game1.currentLocation != null && Game1.currentLocation.currentEvent != null && Game1.player.currentLocation.currentEvent.isFestival)
                {
                    Utility.improveFriendshipWithEveryoneInRegion(Game1.player, this.Config.UmojaBonusFestival, 2);
                    foreach (string name in Game1.player.friendships.Keys.ToArray())
                    {
                        NPC character = Game1.getCharacterFromName(name);
                        if (character != null && character.currentLocation == Game1.currentLocation)
                        {
                            if (character.divorcedFromFarmer)
                                character.doEmote(12);
                            else
                                character.doEmote(32);
                        }
                    }
                    this.Monitor.Log("The Villagers Are glad you came!", LogLevel.Info);
                    this.HasEnteredEvent = true;
                }
            }

            // Check if the Player is getting married or having a baby
            if ((Game1.weddingToday || Game1.farmEvent is BirthingEvent) && this.HasEnteredEvent == false)
            {
                CheckRelationshipData(Game1.player.spouse);
                SerializableDictionary<string, string> relationships = this.CharacterRelationships[Game1.player.spouse];
                foreach (string relation in relationships.Keys.ToArray())
                {
                    if (Game1.getCharacterFromName(relation) == null) continue;
                    if (!relationships[relation].ToLower().Contains("friend"))
                    {
                        Game1.player.changeFriendship(this.Config.UmojaBonusMarry, Game1.getCharacterFromName(relation));
                        this.Monitor.Log($"{relation}: Married into the Family, recieved +{this.Config.UmojaBonusMarry} friendship", LogLevel.Info);
                    }
                    else
                    {
                        Game1.player.changeFriendship(this.Config.UmojaBonusMarry / 2, Game1.getCharacterFromName(relation));
                        this.Monitor.Log($"{relation}: Married a friend, recieved +{this.Config.UmojaBonusMarry / 2} friendship", LogLevel.Info);
                    }
                }
                this.HasEnteredEvent = true;
            }

            //Check if the Player recently completed a daily quest
            if (Game1.stats.questsCompleted > this.CurrentNumberOfCompletedDailyQuests)
            {
                this.DaysAfterCompletingLastDailyQuest = 0;
                this.CurrentNumberOfCompletedDailyQuests = Game1.stats.questsCompleted;
                this.HasRecentlyCompletedQuest = true;
            }
        }

        private void EndOfDayUpdate(object sender, EventArgs e)
        {
            //Gifting, Talking
            foreach (string name in this.WitnessCount.Keys.ToArray())
            {
                if (Game1.getCharacterFromName(name) == null)
                    continue;
                if (this.WitnessCount[name][1] > 0)
                {
                    foreach (string relation in this.CharacterRelationships[name].Keys.ToArray())
                    {
                        if (string.IsNullOrEmpty(relation) || Game1.getCharacterFromName(relation) == null)
                            continue;
                        if (this.WitnessCount[relation][2] > 0)
                        {
                            Game1.player.changeFriendship(this.Config.StorytellerBonus * this.WitnessCount[relation][2], Game1.getCharacterFromName(relation));
                            Monitor.Log($"{relation}: Friendship raised {this.Config.StorytellerBonus * this.WitnessCount[relation][2]} for Gifting to someone {relation} loves:", LogLevel.Info);
                            this.WitnessCount[relation][2] = 0;
                        }
                    }
                    this.WitnessCount[name][1] = 0;
                    this.WitnessCount[name][3] = 0;
                }
                if (((Game1.player.isMarried() && Game1.player.spouse == name) || (Game1.getCharacterFromName(name) is Child child && child.isChildOf(Game1.player))) && this.WitnessCount[name][0] == 1)
                {
                    foreach (string relation in this.CharacterRelationships[name].Keys.ToArray())
                    {
                        if (string.IsNullOrEmpty(relation) || Game1.getCharacterFromName(relation) == null)
                            continue;
                        if (this.CharacterRelationships[name][relation] != "Friend" && this.CharacterRelationships[name][relation] != "Wartorn")
                        {
                            Game1.player.changeFriendship(this.Config.UmojaBonus, Game1.getCharacterFromName(relation));
                            Monitor.Log($"{relation}: Friendship raised {this.Config.UmojaBonus} for loving your family:", LogLevel.Info);

                        }
                    }
                }
                this.WitnessCount[name][0] = 0;
            }

            //Check if New Bundles were completed today
            CommunityCenter communityCenter = (CommunityCenter)Game1.getLocationFromName("CommunityCenter");
            if (this.CurrentNumberOfCompletedBundles < communityCenter.numberOfCompleteBundles())
            {
                int newNumberOfCompletedBundles = communityCenter.numberOfCompleteBundles();
                foreach (string storeOwner in this.StoreOwners)
                {
                    if (Game1.getCharacterFromName(storeOwner) != null)
                        Game1.player.changeFriendship(this.Config.UjimaBonus * newNumberOfCompletedBundles, Game1.getCharacterFromName(storeOwner));
                }
                this.Monitor.Log($"You have gained {20 * (newNumberOfCompletedBundles - this.CurrentNumberOfCompletedBundles)} friendship from all store owners for completing {newNumberOfCompletedBundles - this.CurrentNumberOfCompletedBundles} Bundle today", LogLevel.Info);
                this.CurrentNumberOfCompletedBundles = newNumberOfCompletedBundles;
            }

            //Update the Daily Quest Counters
            if (this.DaysAfterCompletingLastDailyQuest < 3 && this.HasRecentlyCompletedQuest)
            {
                int friendshipPoints = this.Config.UjimaBonus / (int)Math.Pow(2, this.DaysAfterCompletingLastDailyQuest);
                Utility.improveFriendshipWithEveryoneInRegion(Game1.player, friendshipPoints, 2);
                this.Monitor.Log($"Gained {friendshipPoints} friendship for recent daily quest completion", LogLevel.Info);
            }
            else
            {
                if (this.DaysAfterCompletingLastDailyQuest >= 3)
                    this.HasRecentlyCompletedQuest = false;
            }
            if (!Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.currentSeason))
                this.DaysAfterCompletingLastDailyQuest++;

            //Check if any new items were shipped
            if (Game1.player.basicShipped.Count > this.CurrentUniqueItemsShipped)
            {
                int friendshipPoints = this.Config.KuumbaBonus * (Game1.player.basicShipped.Count - this.CurrentUniqueItemsShipped);
                Utility.improveFriendshipWithEveryoneInRegion(Game1.player, friendshipPoints, 2);
                this.Monitor.Log($"Gained {friendshipPoints} friendship for shipping items", LogLevel.Info);
                this.CurrentUniqueItemsShipped = Game1.player.basicShipped.Count;
            }

            //Resetting Miscellaneous Flags
            foreach (string name in this.HasShoppedInStore.Keys.ToArray())
                this.HasShoppedInStore[name] = false;
            if (this.HasEnteredEvent) this.HasEnteredEvent = false;
        }

        private void CheckRelationshipData(string name)
        {
            if (!this.CharacterRelationships.ContainsKey(name))
            {
                this.CharacterRelationships.Add(name, new SerializableDictionary<string, string>());
                this.Monitor.Log($"New Entry: {name}", LogLevel.Info);
                switch (name)
                {
                    case "Vincent":
                    case "Sam":
                        this.CharacterRelationships[name].Add("Jodi", "Mother");
                        this.CharacterRelationships[name].Add("Kent", "Father");
                        this.CharacterRelationships[name].Add(name == "Vincent" ? "Sam" : "Vincent", "Brother");
                        if (name.Equals("Vincent"))
                            this.CharacterRelationships[name].Add("Jas", "Friend");
                        else
                        {
                            this.CharacterRelationships[name].Add("Abigail", "Friend");
                            this.CharacterRelationships[name].Add("Sebastian", "Friend");
                            this.CharacterRelationships[name].Add("Penny", "Friend");
                        }
                        return;
                    case "Maru":
                    case "Sebastian":
                        this.CharacterRelationships[name].Add("Robin", "Mother");
                        this.CharacterRelationships[name].Add("Demetrius", name == "Maru" ? "Father" : "Step-Father");
                        this.CharacterRelationships[name].Add(name == "Maru" ? "Sebastian" : "Maru", "Half-" + name == "Maru" ? "Brother" : "Sister");
                        if (name.Equals("Sebastian"))
                        {
                            this.CharacterRelationships[name].Add("Abigail", "Friend");
                            this.CharacterRelationships[name].Add("Sam", "Friend");
                        }
                        else
                            this.CharacterRelationships[name].Add("Penny", "Friend");
                        return;
                    case "Dwarf":
                    case "Krobus":
                        this.CharacterRelationships[name].Add(name == "Dwarf" ? "Krobus" : "Dwarf", "Wartorn");
                        return;
                    case "Jodi":
                    case "Kent":
                        this.CharacterRelationships[name].Add(name == "Jodi" ? "Kent" : "Jodi", name == "Jodi" ? "Husband" : "Wife");
                        this.CharacterRelationships[name].Add("Sam", "Son");
                        this.CharacterRelationships[name].Add("Vincent", "Son");
                        this.CharacterRelationships[name].Add("Caroline", "Friend");
                        return;
                    case "Emily":
                        this.CharacterRelationships[name].Add("Haley", "Sister");
                        this.CharacterRelationships[name].Add("Sandy", "Friend");
                        this.CharacterRelationships[name].Add("Gus", "Friend");
                        this.CharacterRelationships[name].Add("Clint", "Friend");
                        this.CharacterRelationships[name].Add("Shane", "Friend");
                        return;
                    case "Abigail":
                        this.CharacterRelationships[name].Add("Pierre", "Father");
                        this.CharacterRelationships[name].Add("Caroline", "Mother");
                        this.CharacterRelationships[name].Add("Sebastian", "Friend");
                        this.CharacterRelationships[name].Add("Sam", "Friend");
                        return;
                    case "Caroline":
                        this.CharacterRelationships[name].Add("Pierre", "Husband");
                        this.CharacterRelationships[name].Add("Abigail", "Daughter");
                        this.CharacterRelationships[name].Add("Jodi", "Friend");
                        this.CharacterRelationships[name].Add("Kent", "Friend");
                        return;
                    case "Alex":
                        this.CharacterRelationships[name].Add("George", "Grandfather");
                        this.CharacterRelationships[name].Add("Evelyn", "Grandmother");
                        this.CharacterRelationships[name].Add("Haley", "Friend");
                        return;
                    case "Demetrius":
                        this.CharacterRelationships[name].Add("Robin", "Wife");
                        this.CharacterRelationships[name].Add("Maru", "Daughter");
                        this.CharacterRelationships[name].Add("Sebastian", "Step-Son");
                        return;
                    case "Jas":
                        this.CharacterRelationships[name].Add("Marnie", "Aunt");
                        this.CharacterRelationships[name].Add("Shane", "Godfather");
                        this.CharacterRelationships[name].Add("Vincent", "Friend");
                        return;
                    case "Marnie":
                        this.CharacterRelationships[name].Add("Shane", "Nephew");
                        this.CharacterRelationships[name].Add("Jas", "Neice");
                        this.CharacterRelationships[name].Add("Lewis", "Frind");
                        return;
                    case "Penny":
                        this.CharacterRelationships[name].Add("Pam", "Mother");
                        this.CharacterRelationships[name].Add("Sam", "Friend");
                        this.CharacterRelationships[name].Add("Maru", "Friend");
                        return;
                    case "Robin":
                        this.CharacterRelationships[name].Add("Demetrius", "Husband");
                        this.CharacterRelationships[name].Add("Maru", "Daughter");
                        this.CharacterRelationships[name].Add("Sebastian", "Son");
                        return;
                    case "Shane":
                        this.CharacterRelationships[name].Add("Marnie", "Aunt");
                        this.CharacterRelationships[name].Add("Jas", "Goddaughter");
                        this.CharacterRelationships[name].Add("Emily", "Friend");
                        return;
                    case "Elliott":
                        this.CharacterRelationships[name].Add("Willy", "Friend");
                        this.CharacterRelationships[name].Add("Leah", "Friend");
                        return;
                    case "Evelyn":
                        this.CharacterRelationships[name].Add("George", "Husband");
                        this.CharacterRelationships[name].Add("Alex", "Son");
                        return;
                    case "George":
                        this.CharacterRelationships[name].Add("Evelyn", "Husband");
                        this.CharacterRelationships[name].Add("Alex", "Son");
                        return;
                    case "Gus":
                        this.CharacterRelationships[name].Add("Pam", "Friend");
                        this.CharacterRelationships[name].Add("Emily", "Friend");
                        return;
                    case "Haley":
                        this.CharacterRelationships[name].Add("Emily", "Sister");
                        this.CharacterRelationships[name].Add("Alex", "Friend");
                        return;
                    case "Pam":
                        this.CharacterRelationships[name].Add("Penny", "Daughter");
                        this.CharacterRelationships[name].Add("Gus", "Friend");
                        return;
                    case "Pierre":
                        this.CharacterRelationships[name].Add("Caroline", "Wife");
                        this.CharacterRelationships[name].Add("Abigail", "Daughter");
                        return;
                    case "Clint":
                        this.CharacterRelationships[name].Add("Emily", "Admire");
                        return;
                    case "Leah":
                        this.CharacterRelationships[name].Add("Elliott", "Friend");
                        return;
                    case "Lewis":
                        this.CharacterRelationships[name].Add("Marnie", "Frind");
                        return;
                    case "Sandy":
                        this.CharacterRelationships[name].Add("Emily", "Friend");
                        return;
                    case "Willy":
                        this.CharacterRelationships[name].Add("Elliott", "Friend");
                        return;
                }

                //Check for Relationships of your children
                if (Game1.getCharacterFromName(name) is Child child && child.isChildOf(Game1.player))
                {
                    CheckRelationshipData(Game1.player.spouse);
                    this.CharacterRelationships[name].Add(Game1.player.spouse, Utility.isMale(Game1.player.spouse) ? "Father" : "Mother");
                    this.CharacterRelationships[Game1.player.spouse].Add(name, Utility.isMale(name) ? "Son" : "Daughter");
                    foreach (string relation in this.CharacterRelationships[Game1.player.spouse].Keys.ToArray())
                    {
                        if (relation == name)
                            continue;
                        this.CheckRelationshipData(relation);
                        switch (this.CharacterRelationships[Game1.player.spouse][relation])
                        {
                            case "Grandfather":
                                this.CharacterRelationships[name].Add(relation, "Great-Grandfather");
                                this.CharacterRelationships[relation].Add(name, Utility.isMale(name) ? "Great-Grandson" : "Great-Granddaughter");
                                break;
                            case "Grandmother":
                                this.CharacterRelationships[name].Add(relation, "Great-Grandmother");
                                this.CharacterRelationships[relation].Add(name, Utility.isMale(name) ? "Great-Grandson" : "Great-Granddaughter");
                                break;
                            case "Father":
                            case "Step-Father":
                                this.CharacterRelationships[name].Add(relation, "Grandfather");
                                this.CharacterRelationships[relation].Add(name, Utility.isMale(name) ? "Grandson" : "Granddaughter");
                                break;
                            case "Mother":
                                this.CharacterRelationships[name].Add(relation, "Grandmother");
                                this.CharacterRelationships[relation].Add(name, Utility.isMale(name) ? "Grandson" : "Granddaughter");
                                break;
                            case "Brother":
                            case "Step-Brother":
                            case "Half-Brother":
                                this.CharacterRelationships[name].Add(relation, "Uncle");
                                this.CharacterRelationships[relation].Add(name, Utility.isMale(name) ? "Nephew" : "Niece");
                                break;
                            case "Sister":
                            case "Step-Sister":
                            case "Half-Sister":
                                this.CharacterRelationships[name].Add(relation, "Aunt");
                                this.CharacterRelationships[relation].Add(name, Utility.isMale(name) ? "Nephew" : "Niece");
                                break;
                            case "Niece":
                            case "Nephew":
                                this.CharacterRelationships[name].Add(relation, "Cousin");
                                this.CharacterRelationships[relation].Add(name, "Cousin");
                                break;
                            case "Son":
                            case "Daughter":
                                this.CharacterRelationships[name].Add(relation, Utility.isMale(relation) ? "Brother" : "Sister");
                                this.CharacterRelationships[relation].Add(name, Utility.isMale(name) ? "Brother" : "Sister");
                                break;
                        }
                    }

                    // Check for the other children. They might be from another marriage
                    foreach (Child getChildren in Game1.player.getChildren())
                    {
                        if (getChildren.name != name && !this.CharacterRelationships[name].ContainsKey(getChildren.name))
                        {
                            this.CharacterRelationships[name].Add(getChildren.name, "Half-" + (Utility.isMale(getChildren.name) ? "Brother" : "Sister"));
                            this.CharacterRelationships[getChildren.name].Add(name, "Half-" + (Utility.isMale(getChildren.name) ? "Brother" : "Sister"));
                        }
                    }
                }
            }
        }
    }
}