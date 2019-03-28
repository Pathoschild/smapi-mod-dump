using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using SFarmer = StardewValley.Farmer;

namespace EnhancedRelationships
{
    internal class EnhancedRelationships : Mod, IAssetEditor
    {
        private ModConfig Config;        
        private List<NPC> BirthdayMessageQueue = new List<NPC>();
        private IDictionary<string, int> GaveNpcGift = new Dictionary<string, int>();
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals(@"Data\mail");
        }
        public void Edit<T>(IAssetData asset)
        {
            string[] npcz = { "Alex", "Elliott", "Harvey", "Sam", "Sebastian", "Shane", "Abigail", "Emily", "Haley", "Leah", "Maru", "Penny", "Caroline", "Clint", "Demetrius", "Dwarf", "Evelyn", "George", "Gus", "Jas", "Jodi", "Kent", "Krobus", "Lewis", "Linus", "Marnie", "Pam", "Pierre", "Robin", "Sandy", "Vincent", "Willy", "Wizard" };
            string[] npc_gifts = { "Complete Breakfast, Salmon Dinner",
            "Crab Cake, Duck Feather, Lobster, Pomegranate",
            "Coffee, Pickles, Super Meal, Truffle Oil, Wine",
            "Cactus Fruit, Maple Bar, Pizza, Tigerseye",
            "Frozen Tear, Obsidian, Pumpkin Soup, Sashimi, Void Egg",
            "Beer, Hot Pepper, Pepper Popper, Pizza",
            "Amethyst, Blackberry Cobbler, Chocolate Cake, Pufferfish, Pumpkin, Spicy Eel",
            "Amethyst, Aquamarine, Cloth, Emerald, Jade, Ruby, Survival Burger, Topaz, Wool",
            "Coconut, Fruit Salad, Pink Cake, Sunflower",
            "Goat Cheese, Poppyseed Muffin, Salad, Stir Fry, Truffle, Vegetable Medley, Wine",
            "Battery Pack, Cauliflower, Cheese Cauliflower, Diamond, Gold Bar, Iridium Bar, Miners Treat, Pepper Popper, Rhubarb Pie, Strawberry",
            "Diamond, Emerald, Melon, Poppy, Poppyseed Muffin, Red Plate, Roots Plate, Sandfish, Tom Kha Soup",
            "Fish Taco, Summer Spangle",
            "Amethyst, Aquamarine, Artichoke Risotto, Gold Bar, Iridium Bar, Jade, Omni Geode, Ruby, Topaz",
            "Bean Hotpot, Ice Cream, Rice Pudding, Strawberry",
            "Amethyst, Aquamarine, Emerald, Jade, Omni Geode, Ruby, Topaz",
            "Beet, Chocolate Cake, Diamond, Fairy Rose, Stuffing, Tulip",
            "Fried Mushroom, Leek",
            "Escargot, Fish Taco, Orange",
            "Fairy Rose, Plum Pudding",
            "Chocolate Cake, Crispy Bass, Diamond, Eggplant Parmesan, Fried Eel, Pancakes, Rubarb Pie, Vegetable Medley",
            "Fiddlehead Risotto, Roasted Hazelnuts",
            "Diamond, Iridium Bar, Pumpkin, Void Egg, Void Mayonnaise, Wild Horseradish",
            "Autumns Bounty, Glazed Yams, Hot Pepper, Vegetable Medley",
            "Blueberry Tart, Cactus Fruit, Coconut, Dish O' The Sea, Yam",
            "Diamond, Farmer's Lunch, Pink Cake, Pumpkin Pie",
            "Beer, Cactus Fruit, Glazed Yams, Mead, Pale Ale, Parsnip, Parsnip Soup",
            "Fried Calamari",
            "Goat Cheese, Peach, Spaghetti",
            "Crocus, Daffodil, Sweet Pea",
            "Cranberry Candy, Grape, Pink Cake",
            "Catfish, Diamond, Iridium Bar, Mead, Octopus, Pumpkin, Sea Cucumber, Sturgeon",
            "Purple Mushroom, Solar Essence, Super Cucumber, Void Essence"};
            var i18n = Helper.Translation;
            for (int i = 0; i < npcz.Count(); i++)
            {
                IDictionary<string, string> npc = asset.AsDictionary<string, string>().Data;
                npc["birthDayMail" + npcz[i]] = i18n.Get("npc_mail", new { npc_name = npcz[i], npc_gift = npc_gifts[i] });
                /*
                asset
                .AsDictionary<string, string>()
                .Set("birthDayMail" + npcz[i], i18n.Get("npc_mail", new { npc_name = npcz[i], npc_gift = npc_gifts[i]}));
                
                *///$"Dear @,^ Tomorrow is {npcz[i]}'s Birthday. You should give them a gift. They would love one of the following: ^^{npc_gifts[i]}."
            }
        }

        //EndMail Stuff


        


        public override void Entry(IModHelper helper)
        {
            this.Config = Helper.ReadConfig<ModConfig>();            
            SFarmer Player = Game1.player;
            Helper.Events.GameLoop.DayStarted += this.TimeEvents_AfterDayStarted;
            //TimeEvents.AfterDayStarted += this.TimeEvents_AfterDayStarted;
            Helper.Events.GameLoop.Saving += SaveEvents_BeforeSave;
            //SaveEvents.BeforeSave += SaveEvents_BeforeSave;
            //LocationEvents.CurrentLocationChanged += DoNpcGift;
            Helper.Events.Player.InventoryChanged += DoNpcGift;
            //PlayerEvents.InventoryChanged += DoNpcGift;
        }
        private void DoNpcGift(object sender, InventoryChangedEventArgs e)
        {
            SFarmer Player = Game1.player;
            var day = SDate.Now();
            foreach(GameLocation location in Game1.locations)
            {
                foreach(NPC npc in location.characters)
                {
                    if (!GaveNpcGift.ContainsKey(npc.Name))
                    {
                        if (Player.friendshipData.ContainsKey(npc.Name))
                        {
                            GaveNpcGift.Add(npc.Name, Player.friendshipData[npc.Name].GiftsToday);
                        }                        
                    }
                    else
                    {
                        if (npc.isBirthday(day.Season, day.Day))
                        {
                            GaveNpcGift[npc.Name] = Player.friendshipData[npc.Name].GiftsToday;
                        }
                    }                    
                }
            }
            /*
            foreach(KeyValuePair<string, int> pair in GaveNpcGift)
            {
                this.Monitor.Log($"Npc: {pair.Key} Gift Today: {pair.Value}");
            }*/
        }
        private void TimeEvents_AfterDayStarted(object sender, EventArgs e)
        {
            //Birthday tests was in AfterDay Started
            this.BirthdayMessageQueue.Clear();
            var today = SDate.Now();
                if(today.DaysSinceStart != 1)
            {
                var yesterday = SDate.Now().AddDays(-1);
                foreach (GameLocation location in Game1.locations)
                {
                    foreach (NPC characterz in location.characters)
                    {
                        this.DoLogic(characterz, yesterday.Day, yesterday.Season);
                        /*
                        if (!characterz.isBirthday(yesterday.Season, yesterday.Day))
                        {
                            if (GaveNpcGift.ContainsKey(characterz.name))
                            {
                                GaveNpcGift.Remove(characterz.name);
                            }
                        }*/
                    }                        
                }

                foreach (NPC birthdayMessage in this.BirthdayMessageQueue)
                    birthdayMessage.CurrentDialogue.Push(new Dialogue(PickRandomDialogue(), birthdayMessage));
            }
        }
       
        private void SaveEvents_BeforeSave(object sender, EventArgs e)
        {
            if (!this.Config.GetMail)
                return;
            var tomorrow = SDate.Now().AddDays(1);
            foreach (GameLocation location in Game1.locations)
            {
                foreach (NPC npc in location.characters)
                {
                    if (npc.isBirthday(tomorrow.Season, tomorrow.Day))
                    {
                        Game1.mailbox.Add($"birthDayMail{npc.Name}");
                    }
                }
            }            
        }
        private void DoLogic(NPC npc, int day, string season)
        {
            SFarmer Player = Game1.player;
            bool GiftGiven = false;
            if (!Player.friendshipData.ContainsKey(npc.Name) || Player.friendshipData[npc.Name].Points <= 0)
                return;
            int index = Player.getFriendshipHeartLevelForNPC(npc.Name);
            int basicAmount = this.Config.BasicAmount;
            index = index > 10 ? 10 : index;
            //Check to see if gift was given
            int giftInt;
            if (GaveNpcGift.ContainsKey(npc.Name))
            {
                giftInt = GaveNpcGift[npc.Name];
                GiftGiven = giftInt == 1 ? true : false;
            }
            //End check
            if (this.Config.EnableMissedBirthdays && npc.isBirthday(season, day) && GiftGiven == false)
            {
                int amount = !this.Config.EnableRounded ? (int)Math.Floor((double)basicAmount * (double)this.Config.BirthdayMultiplier * (double)this.Config.BirthdayHeartMultiplier[index] + (double)basicAmount * (double)this.Config.HeartMultiplier[index]) : (int)Math.Ceiling((double)basicAmount * (double)this.Config.BirthdayMultiplier * (double)this.Config.BirthdayHeartMultiplier[index] + (double)basicAmount * (double)this.Config.HeartMultiplier[index]);
                amount = amount * -1;
                Game1.player.changeFriendship(amount, npc);
                this.BirthdayMessageQueue.Add(npc);
                GaveNpcGift.Remove(npc.Name);
                //this.Monitor.Log($"Decreased Friendship {amount} With : {npc.Name}");
            }
            else
            {
                if (Player.friendshipData[npc.Name].GiftsThisWeek >= this.Config.AmtOfGiftsToKeepNpcHappy)
                    return;
                int amount = !this.Config.EnableRounded ? (int)Math.Floor((double)basicAmount * (double)this.Config.HeartMultiplier[index]) : (int)Math.Ceiling((double)basicAmount * (double)this.Config.HeartMultiplier[index]);
                Player.changeFriendship(amount, npc);
                //this.Monitor.Log($"Increased Friendship {amount} With : {npc.Name}");
            }
        }
        private string PickRandomDialogue()
        {
            var i18n = Helper.Translation;
            Random rnd = new Random();
            int outty = rnd.Next(0, 7);
            //Return the translated Text    
            return i18n.Get("npc_dialogue"+outty, new { player_name = Game1.player.Name });           
        }
        //Mail Updates
        

    }
}
