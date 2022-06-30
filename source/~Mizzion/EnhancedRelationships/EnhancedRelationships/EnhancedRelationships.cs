/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

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
    internal class EnhancedRelationships : Mod//, IAssetEditor
    {
        private ModConfig Config;        
        private List<NPC> BirthdayMessageQueue = new List<NPC>();
        private IDictionary<string, int> GaveNpcGift = new Dictionary<string, int>();
        private IDictionary<string, string> NpcGifts = new Dictionary<string, string>();
        private bool debugging = true;

        /*
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals(@"Data\mail");
        }
        public void Edit<T>(IAssetData asset)
        {
            NpcGifts = GetNpcGifts();
            var i18n = Helper.Translation;
            foreach (var d in NpcGifts)
            {
                IDictionary<string, string> npc = asset.AsDictionary<string, string>().Data;
                npc["birthDayMail" + d.Key] = i18n.Get("npc_mail", new { npc_name = d.Key, npc_gift = d.Value });
            }
        }

        //EndMail Stuff
        */

        


        public override void Entry(IModHelper helper)
        {
            this.Config = Helper.ReadConfig<ModConfig>();            
            SFarmer Player = Game1.player;
            Helper.Events.GameLoop.DayStarted += this.TimeEvents_AfterDayStarted;
            Helper.Events.GameLoop.Saving += SaveEvents_BeforeSave;
            Helper.Events.Player.InventoryChanged += DoNpcGift;

            //Lets add in the new content events
            Helper.Events.Content.AssetRequested += this.ContentEvents_AssetRequested;
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
            try
            {
                //Birthday tests was in AfterDay Started
                this.BirthdayMessageQueue.Clear();
                var today = SDate.Now();
                if (today.DaysSinceStart != 1)
                {
                    var yesterday = SDate.Now().AddDays(-1);
                    foreach (GameLocation location in Game1.locations)
                    {
                        foreach (NPC characterz in location.characters)
                            DoLogic(characterz, yesterday.Day, yesterday.Season);
                    }

                    foreach (NPC birthdayMessage in this.BirthdayMessageQueue)
                        birthdayMessage.CurrentDialogue.Push(new Dialogue(PickRandomDialogue(), birthdayMessage));
                }
            }
            catch (Exception ex)
            {
                this.Monitor.Log(ex.ToString());
            }
           
        }
        private void SaveEvents_BeforeSave(object sender, EventArgs e)
        {
            if (!this.Config.GetMail)
                return;
            try
            {
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
            catch (Exception ex)
            {
                this.Monitor.Log(ex.ToString());
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
                amount *= -1;
                Game1.player.changeFriendship(amount, npc);
                this.BirthdayMessageQueue.Add(npc);
                if (debugging)
                {
                    this.Monitor.Log($"Message should have been added for {npc.Name}");
                }

                GaveNpcGift.Remove(npc.Name);
                this.Monitor.Log($"Decreased Friendship {amount} With : {npc.Name}");
            }
            else
            {
                if (Player.friendshipData[npc.Name].GiftsThisWeek >= this.Config.AmtOfGiftsToKeepNpcHappy)
                    return;
                int amount = !this.Config.EnableRounded ? (int)Math.Floor((double)basicAmount * (double)this.Config.HeartMultiplier[index]) : (int)Math.Ceiling((double)basicAmount * (double)this.Config.HeartMultiplier[index]);
                Player.changeFriendship(amount, npc);
                if (debugging)
                {
                    this.Monitor.Log($"Increased Friendship {amount} With : {npc.Name}");
                }
                
            }
        }

        private void ContentEvents_AssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/mail"))
            {
                e.Edit(asset =>
                {
                    NpcGifts = GetNpcGifts();
                    var i18n = Helper.Translation;
                    foreach (var d in NpcGifts)
                    {
                        IDictionary<string, string> npc = asset.AsDictionary<string, string>().Data;
                        npc["birthDayMail" + d.Key] =
                            i18n.Get("npc_mail", new { npc_name = d.Key, npc_gift = d.Value });
                    }
                });
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
        
        //Grab NPC Gifts Loves
        private IDictionary<string, string> GetNpcGifts(bool loved = true)
        {
            var outter = Game1.NPCGiftTastes;
            IDictionary<string, string> results = new Dictionary<string, string>();
            string giftNames = "";
            foreach (var o in outter)
            {
                if (o.Value.Contains('/'))
                {
                   foreach (var n in o.Value.Split('/')[1].Split(' '))
                   {
                       StardewValley.Object obj = new StardewValley.Object(Convert.ToInt32(n), 1, false, -1, 0);
                       if(obj.DisplayName != "Error Item")
                           giftNames += $"{obj.DisplayName}, ";
                   }
                   results.Add(o.Key, giftNames.Substring(0, giftNames.Length - 2));
                   giftNames = "";
                }
            }
            return results;
        }
    }
}
