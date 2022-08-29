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
        private ModConfig _config;        
        private readonly List<NPC> _birthdayMessageQueue = new();
        private readonly IDictionary<string, int> _gaveNpcGift = new Dictionary<string, int>();
        private IDictionary<string, string> _npcGifts = new Dictionary<string, string>();
        private bool _debugging;

        public override void Entry(IModHelper helper)
        {
            _config = Helper.ReadConfig<ModConfig>();            
            Helper.Events.GameLoop.DayStarted += TimeEvents_AfterDayStarted;
            Helper.Events.GameLoop.Saving += SaveEvents_BeforeSave;
            Helper.Events.Player.InventoryChanged += DoNpcGift;

            //Lets add in the new content events
            Helper.Events.Content.AssetRequested += ContentEvents_AssetRequested;
        }

        private void DoNpcGift(object sender, InventoryChangedEventArgs e)
        {
            var player = Game1.player;
            var day = SDate.Now();
            foreach(var location in Game1.locations)
            {
                foreach(var npc in location.characters)
                {
                    if (!_gaveNpcGift.ContainsKey(npc.Name))
                    {
                        if (player.friendshipData.ContainsKey(npc.Name))
                        {
                            _gaveNpcGift.Add(npc.Name, player.friendshipData[npc.Name].GiftsToday);
                        }                        
                    }
                    else
                    {
                        if (npc.isBirthday(day.Season, day.Day))
                        {
                            _gaveNpcGift[npc.Name] = player.friendshipData[npc.Name].GiftsToday;
                        }
                    }                    
                }
            }
        }
        private void TimeEvents_AfterDayStarted(object sender, EventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;
            if (Game1.player.Name.ToLower().Contains("debug"))
            {
                _debugging = true;
            }
            try
            {
                //Birthday tests was in AfterDay Started
                _birthdayMessageQueue.Clear();
                var today = SDate.Now();
                if (today.DaysSinceStart != 1)
                {
                    var yesterday = SDate.Now().AddDays(-1);
                    foreach (var location in Game1.locations)
                    {
                        foreach (var characters in location.characters)
                            DoLogic(characters, yesterday.Day, yesterday.Season);
                    }

                    foreach (var birthdayMessage in _birthdayMessageQueue)
                        birthdayMessage.CurrentDialogue.Push(new Dialogue(PickRandomDialogue(), birthdayMessage));
                }
            }
            catch (Exception ex)
            {
                Monitor.Log(ex.ToString());
            }
           
        }
        private void SaveEvents_BeforeSave(object sender, EventArgs e)
        {
            if (!_config.GetMail)
                return;
            try
            {
                var tomorrow = SDate.Now().AddDays(1);
                foreach (var location in Game1.locations)
                {
                    foreach (var npc in location.characters)
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
                Monitor.Log(ex.ToString());
            }
                      
        }
        private void DoLogic(NPC npc, int day, string season)
        {
            var player = Game1.player;
            var giftGiven = false;
            if (!player.friendshipData.ContainsKey(npc.Name) || player.friendshipData[npc.Name].Points <= 0)
                return;
            var index = player.getFriendshipHeartLevelForNPC(npc.Name);
            var basicAmount = _config.BasicAmount;
            index = index > 10 ? 10 : index;
            
            
            //Check to see if gift was given
            if (_gaveNpcGift.ContainsKey(npc.Name))
            {
                var giftInt = _gaveNpcGift[npc.Name];
                giftGiven = giftInt == 1;
            }
            //End check
            if (_config.EnableMissedBirthdays && npc.isBirthday(season, day) && giftGiven == false)
            {
                var amount = !_config.EnableRounded ? (int)Math.Floor(basicAmount * (double)_config.BirthdayMultiplier * _config.BirthdayHeartMultiplier[index] + basicAmount * (double)_config.HeartMultiplier[index]) : (int)Math.Ceiling(basicAmount * (double)_config.BirthdayMultiplier * _config.BirthdayHeartMultiplier[index] + basicAmount * (double)_config.HeartMultiplier[index]);
                amount *= -1;
                Game1.player.changeFriendship(amount, npc);
                _birthdayMessageQueue.Add(npc);
                if (_debugging)
                {
                    Monitor.Log($"Message should have been added for {npc.Name}");
                }

                _gaveNpcGift.Remove(npc.Name);
                Monitor.Log($"Decreased Friendship {amount} With : {npc.Name}");
            }
            else
            {
                if (player.friendshipData[npc.Name].GiftsThisWeek >= _config.AmtOfGiftsToKeepNpcHappy)
                    return;
                var amount = !_config.EnableRounded ? (int)Math.Floor(basicAmount * (double)_config.HeartMultiplier[index]) : (int)Math.Ceiling(basicAmount * (double)_config.HeartMultiplier[index]);
                player.changeFriendship(amount, npc);
                if (_debugging)
                {
                    Monitor.Log($"Increased Friendship {amount} With : {npc.Name} Index: {index}");
                }
                
            }
        }

        private void ContentEvents_AssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/mail"))
            {
                e.Edit(asset =>
                {
                    _npcGifts = GetNpcGifts();
                    var i18N = Helper.Translation;
                    foreach (var d in _npcGifts)
                    {
                        var npc = asset.AsDictionary<string, string>().Data;
                        npc["birthDayMail" + d.Key] =
                            i18N.Get("npc_mail", new { npc_name = d.Key, npc_gift = d.Value });
                    }
                });
            }
        }
        
        private string PickRandomDialogue()
        {
            var i18N = Helper.Translation;
            var rnd = new Random();
            var outer = rnd.Next(0, 7);
            //Return the translated Text    
            return i18N.Get("npc_dialogue"+outer, new { player_name = Game1.player.Name });           
        }
        //Mail Updates
        
        //Grab NPC Gifts Loves
        private IDictionary<string, string> GetNpcGifts(bool loved = true)
        {
            if (loved)
            {
                if (_debugging)
                    Monitor.Log($"Loved was true");
            }
            
            var outer = Game1.NPCGiftTastes;
            IDictionary<string, string> results = new Dictionary<string, string>();
            var giftNames = "";
            foreach (var o in outer)
            {
                if (o.Value.Contains('/'))
                {
                    giftNames = (from n in o.Value.Split('/')[1].Split(' ') where n.Length > 0 select new StardewValley.Object(Convert.ToInt32(n), 1) into obj where obj.DisplayName != "Error Item" select obj).Aggregate(giftNames, (current, obj) => current + $"{obj.DisplayName}, ");
                    if(giftNames.Contains(", "))
                        results.Add(o.Key, giftNames.Substring(0, giftNames.Length - 2));
                    giftNames = "";
                }
            }
            return results;
        }
    }
}
