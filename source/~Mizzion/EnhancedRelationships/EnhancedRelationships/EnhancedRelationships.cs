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
using Mizzion.Stardew.Common.Integrations.GenericModConfigMenu;
using Object = StardewValley.Object;

namespace EnhancedRelationships
{
    internal class EnhancedRelationships : Mod//, IAssetEditor
    {
        private ModConfig _config;    
        private ITranslationHelper _i18N;
        
        private readonly List<NPC> _birthdayMessageQueue = new();
        
        //NPC stuff
        private readonly IDictionary<string, int> _gaveNpcGift = new Dictionary<string, int>();
        
        
        //Debugging
        private bool _debugging = false;
        
        //Command Variables
        private string _commandGifts = "";
        
        //Generic Mod Config Menu
        private IGenericModConfigMenuApi _cfgMenu;

        public override void Entry(IModHelper helper)
        {
            _config = Helper.ReadConfig<ModConfig>();
            _i18N = Helper.Translation;
           
            //GameLoop Events
            Helper.Events.GameLoop.DayStarted += DayStarted;
            Helper.Events.GameLoop.Saving += Saving;
            Helper.Events.GameLoop.GameLaunched += GameLaunched;

            //Let's add in the new content events
            Helper.Events.Content.AssetRequested += AssetRequested;
            
            //Commands
            Helper.ConsoleCommands.Add("er_gifts", "Lists all npc gifts and likes.", CommandGifts);
        }

        

        //Event Methods
        
        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            #region Generic Moc Config Menu
            
            _cfgMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (_cfgMenu is null) return;

            //Register mod
            _cfgMenu.Register(
                mod: ModManifest,
                reset: () => _config = new ModConfig(),
                save: () => Helper.WriteConfig(_config)
                );

            _cfgMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => _i18N.Get("setting_mod_name"),
                tooltip: null
                );

            _cfgMenu.AddBoolOption(
                mod: ModManifest,
                name: () => _i18N.Get("setting_getmail_text"),
                tooltip: () => _i18N.Get("setting_getmail_description"),
                getValue: () => _config.GetMail,
                setValue: value => _config.GetMail = value
            );
            
            _cfgMenu.AddBoolOption(
                mod: ModManifest,
                name: () => _i18N.Get("setting_getmail_text"),
                tooltip: () => _i18N.Get("setting_getmail_description"),
                getValue: () => _config.GetMail,
                setValue: value => _config.GetMail = value
            );
            
            _cfgMenu.AddBoolOption(
                mod: ModManifest,
                name: () => _i18N.Get("setting_enablemissedbirthdays_text"),
                tooltip: () => _i18N.Get("setting_enablemissedbirthdays_description"),
                getValue: () => _config.EnableMissedBirthdays,
                setValue: value => _config.EnableMissedBirthdays = value
            );
            
            _cfgMenu.AddBoolOption(
                mod: ModManifest,
                name: () => _i18N.Get("setting_enablerounded_text"),
                tooltip: () => _i18N.Get("setting_enablerounded_description"),
                getValue: () => _config.EnableRounded,
                setValue: value => _config.EnableRounded = value
            );
            
            _cfgMenu.AddNumberOption(
                mod: ModManifest,
                name: () => _i18N.Get("setting_giftstokeepnpchappy_text"),
                tooltip: () => _i18N.Get("setting_giftstokeepnpchappy_description"),
                getValue: () => _config.AmtOfGiftsToKeepNpcHappy,
                setValue: value => _config.AmtOfGiftsToKeepNpcHappy = value
            );
            
            _cfgMenu.AddSectionTitle(
                mod: ModManifest,
                text: ()=> _i18N.Get("setting_multipliers")
                );
            
            
            _cfgMenu.AddNumberOption(
                mod: ModManifest,
                name: () => _i18N.Get("setting_basicamount_text"),
                tooltip: () => _i18N.Get("setting_basicamount_description"),
                getValue: () => _config.BasicAmount,
                setValue: value => _config.BasicAmount = value
            );
            
            _cfgMenu.AddNumberOption(
                mod: ModManifest,
                name: () => _i18N.Get("setting_birthdaymultiplier_text"),
                tooltip: () => _i18N.Get("setting_birthdaymultiplier_description"),
                getValue: () => _config.HeartMultipliers.BirthdayMultiplier,
                setValue: value => _config.HeartMultipliers.BirthdayMultiplier = value
            );
            
            _cfgMenu.AddParagraph(
                mod: ModManifest,
                text: () => _i18N.Get("setting_multiplier_paragraph")
            );

            

           
            #endregion
        }


        private void Saving(object sender, SavingEventArgs e)
        {
            if (!_config.GetMail) return;

            try
            {
                var tomorrow = SDate.Now().AddDays(1);
                
                Utility.ForEachVillager(delegate(NPC villager)
                {
                    if(villager.Birthday_Day == tomorrow.Day && villager.Birthday_Season == tomorrow.SeasonKey)
                        Game1.mailbox.Add($"birthDayMail{villager.Name}");
                    return true;
                });
            }
            catch (Exception ex)
            {
                Monitor.Log(ex.ToString());
            }
        }

        
        private void AssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/mail"))
            {
                e.Edit(asset =>
                {
                    

                    //Lets go through villagers and Create the mail for them.
                    Utility.ForEachVillager(delegate(NPC villager)
                    {
                        var npcLovedGifts = GetNpcGifts(true).Where(npc => npc.Key == villager.Name);
                        var lovedGifts = "";
                        foreach (var gift in npcLovedGifts) lovedGifts += gift.Value.Length >= 3 ? gift.Value[..^2] : gift.Value;

                        var  npcLikedGifts = GetNpcGifts().Where(npc => npc.Key == villager.Name);
                        var likedGifts = "";
                        foreach (var gift in npcLikedGifts) likedGifts += gift.Value.Length >= 3 ? gift.Value[..^2] : gift.Value;

                        var npc = asset.AsDictionary<string, string>().Data;

                        npc["birthDayMail" + villager.Name] = _i18N.Get("npc_mail", new { npc_name = villager.Name, npc_loved_gift = lovedGifts,  npc_liked_gift = likedGifts});
                        
                        return true;
                    });
                });
            }
        }
        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            if (!Context.IsWorldReady) return;

            _debugging = Game1.player.Name.Contains("ER_");
            
            try
            {
                _birthdayMessageQueue.Clear();
                var today = SDate.Now();

                if (today.DaysSinceStart <= 1) return;
                
                var yesterday = today.AddDays(-1);
                    
                Utility.ForEachVillager(delegate(NPC villager)
                {
                    if(villager != null)
                        DoLogic(villager, yesterday.Day, yesterday.SeasonKey);
                    return true;
                });
                
                foreach(var birthdayMessage in _birthdayMessageQueue)
                    birthdayMessage.CurrentDialogue.Push(new Dialogue(birthdayMessage, null, PickRandomDialogue()));
            }
            catch(Exception ex)
            {
                Monitor.Log(ex.ToString());
            }
        }
        
        
        //Custom Methods
        private string GetSeason(Season season)
        {
            string seasonName = "";
            
            switch(season)
            {
                case Season.Spring:
                    seasonName = "Spring";
                break;
                
                case Season.Summer:
                    seasonName = "Summer";
                    break;
                case Season.Fall:
                    seasonName = "Fall";
                    break;
                case Season.Winter:
                    seasonName = "Winter";
                    break;
            }

            return seasonName;
        }

        private void DoLogic(NPC npc, int day, string season)
        {
            if (npc is null) return; 
            
            var player = Game1.player;
            
            //Make sure the player has talk to the npc
            if (!player.friendshipData.ContainsKey(npc.Name) || player.friendshipData[npc.Name].Points <= 0) return;

            
            
            var lastGiftDate = player.friendshipData[npc.Name].LastGiftDate != null ? player.friendshipData[npc.Name].LastGiftDate.DayOfMonth : 0;
            var lastGiftSeason = player.friendshipData[npc.Name].LastGiftDate != null ? player.friendshipData[npc.Name].LastGiftDate.SeasonKey : "";
            
            
            var giftGiven =  /*_gaveNpcGift.ContainsKey(npc.Name) && _gaveNpcGift[npc.Name] >= 1 || */lastGiftDate == day && lastGiftSeason == season;
            var missedBirthday = _config.EnableMissedBirthdays && npc.Birthday_Day == day &&
                                 npc.Birthday_Season == season && !giftGiven;
            var isBirthday = npc.Birthday_Day == day && npc.Birthday_Season == season;
            var wasBirthday = npc.Birthday_Day == day && npc.Birthday_Season == season &&
                              lastGiftDate != day && lastGiftSeason == season;
            

            var gaveEnoughGifts = player.friendshipData[npc.Name].GiftsThisWeek >= _config.AmtOfGiftsToKeepNpcHappy;
            
            
            
            var heartLevel = player.getFriendshipHeartLevelForNPC(npc.Name) > 10 ? 10 : player.getFriendshipHeartLevelForNPC(npc.Name);
            var basicAmount = _config.BasicAmount;
            var amount = 0;
            
            var birthdayAmount = !_config.EnableRounded
                ? (int)Math.Floor(
                    basicAmount * (double)_config.HeartMultipliers.BirthdayMultiplier * _config.HeartMultipliers.BirthdayHeartMultiplier[heartLevel] +
                    basicAmount * (double)_config.HeartMultipliers.HeartMultiplier[heartLevel])
                : (int)Math.Ceiling(
                    basicAmount * (double)_config.HeartMultipliers.BirthdayMultiplier * _config.HeartMultipliers.BirthdayHeartMultiplier[heartLevel] +
                    basicAmount * (double)_config.HeartMultipliers.HeartMultiplier[heartLevel]); 
            
            var normalAmount = !_config.EnableRounded
                ? (int)Math.Floor(basicAmount * (double)_config.HeartMultipliers.HeartMultiplier[heartLevel])
                : (int)Math.Ceiling(basicAmount * (double)_config.HeartMultipliers.HeartMultiplier[heartLevel]);
            
            //Time to do the magic(Edits)

            if (isBirthday && !giftGiven && missedBirthday && lastGiftDate != day && lastGiftSeason != season)
            {
                birthdayAmount *= missedBirthday ? -1 : 1;
                _birthdayMessageQueue.Add(npc);
                DoLog($"wasBirthday and !giftGiven & missedBirthday. Amount: {birthdayAmount}");
                DoLog($"Message should be added for NPC: {npc.Name}");
                ChangeFriendship(npc, birthdayAmount, true);
            }
            else if (isBirthday && giftGiven && lastGiftDate == day && lastGiftSeason == season)
            {
                DoLog($"wasBirthday && giftGiven. Amount: {birthdayAmount}");
                ChangeFriendship(npc, birthdayAmount);
            }
            else
            {
                DoLog($"Went to else. Amount: {normalAmount}");
                ChangeFriendship(npc, normalAmount);
            }
            
            //Debugging log for NPC's
            DoLog($"npc: {npc.Name} day: {day} season: {season} birthdayAmount: {birthdayAmount} normalAmt: {normalAmount}. isBirthday: {isBirthday} wasBirthday: {wasBirthday} giftGiven: {giftGiven} missedBirthday: {missedBirthday}.\r\n NPC Debug: NAME: {npc.Name}, lastGiftDate: {lastGiftDate} lastGiftSeason: {lastGiftSeason} birthDay: {npc.Birthday_Day} birthSeason: {npc.Birthday_Season} Current FriendPoint: {player.friendshipData[npc.Name].Points} GiftsToday: {player.friendshipData[npc.Name].GiftsToday} GiftsThisWeek: {player.friendshipData[npc.Name].GiftsThisWeek}");
            //Need to figure out the removal of the _gaveNpcGift
            //_gaveNpcGift.Remove(npc.Name);
        }

        private void ChangeFriendship(NPC npc, int amt, bool decrease = false)
        {
            var incDec = decrease ? "Decreased" : "Increased";
            Game1.player.changeFriendship(amt, npc);
            DoLog($"{incDec} friendship for NPC: {npc.Name} by Amount: {amt}.", true);
        }
        


        private string PickRandomDialogue()
        {
            return _i18N.Get($"npc_dialogue{Utility.CreateDaySaveRandom().Next(0, 7)}",
                new { player_name = Game1.player.Name });
        }

        private IDictionary<string, string> GetNpcGifts(bool loved = false)
        {
            var results = new Dictionary<string, string>();
            
            Utility.ForEachVillager(delegate(NPC villager)
            {
                var gifts = DataLoader.NpcGiftTastes(Game1.content).Where(npc => npc.Key == villager.Name);

                foreach (var item in gifts)
                {
                    string[] giftItems;
                    if(loved)
                        giftItems = item.Value.Contains('/') ? item.Value.Split('/')[1].Split(' ') : item.Value.Split(' ');
                    else
                        giftItems = item.Value.Contains('/') ? item.Value.Split('/')[3].Split(' ') : item.Value.Split(' ');

                    //Skip the universals
                    if (giftItems.Contains("Universal_")) continue;

                    var giftNames = "";

                    foreach (var items in giftItems)
                    {
                        
                        var i = ItemRegistry.GetMetadata($"(O){items}");

                        if (!i.Exists() || i.GetParsedOrErrorData().DisplayName.Contains("Error")) continue;

                        giftNames += $"{i.GetParsedOrErrorData().DisplayName}, ";
                    }

                    results.TryAdd(villager.Name, giftNames);

                }
                return true;
            });
            return results;
        }
        
        //Debug Methods

        private void DoLog(string message, bool forPLayer = false)
        {
            if(_debugging || forPLayer)
                Monitor.Log(message,  !forPLayer ? LogLevel.Error : LogLevel.Info);
        }
        
        //Command Methods

        private void CommandGifts(string command, string[] args)
        {
            var likedGifts = "";
            var lovedGifts = "";
            
            if(args.Length < 1) return;
            var arg = args[0];

            switch (arg)
            {
                case"debug":
                    if (args[1] == "") return;
                    
                    var villagerName = args[1];
                    var loveGifts = GetNpcGifts(true).Where(n => n.Key == villagerName).ToArray();
                    var likeGifts = GetNpcGifts().Where(n => n.Key == villagerName).ToArray();

                    if (!loveGifts.Any()|| !likeGifts.Any())
                    {
                        Monitor.Log($"Couldn't find a NPC by the name of: {villagerName}", LogLevel.Error);
                        return;
                    }

                    lovedGifts = "";
                    likedGifts = "";
                
                    foreach (var gift in loveGifts) lovedGifts += gift.Value.Length >= 3 ? gift.Value[..^2] : gift.Value;

                    foreach (var gift in likeGifts) likedGifts += gift.Value.Length >= 3 ? gift.Value[..^2] : gift.Value;

                    Monitor.Log($"NPC: {villagerName}\r\n Loved: {lovedGifts} \r\n Liked: {likedGifts}");
                    
                    break;
                default:
                    Utility.ForEachVillager(delegate(NPC villager)
                    {
                        var loveGifts = GetNpcGifts(true).Where(n => n.Key == villager.displayName).ToArray();
                        var likeGifts = GetNpcGifts().Where(n => n.Key == villager.displayName).ToArray();

                        lovedGifts = "";
                        likedGifts = "";
                
                        foreach (var gift in loveGifts) lovedGifts += gift.Value.Length >= 3 ? gift.Value[..^2] : gift.Value;

                        foreach (var gift in likeGifts) likedGifts += gift.Value.Length >= 3 ? gift.Value[..^2] : gift.Value;

                        Monitor.Log($"NPC: {villager.displayName} Loved: {lovedGifts} && Liked: {likedGifts}", LogLevel.Info);
                        return true;
                    });
                    break;
            }
            
        }
    }
}
