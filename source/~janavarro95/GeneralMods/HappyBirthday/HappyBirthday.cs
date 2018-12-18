using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Omegasis.HappyBirthday.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;
using StardewValley.Monsters;
using SObject = StardewValley.Object;

namespace Omegasis.HappyBirthday
{
    /// <summary>The mod entry point.</summary>
    public class HappyBirthday : Mod, IAssetEditor
    {
        /*********
        ** Properties
        *********/
        /// <summary>The relative path for the current player's data file.</summary>
        private string DataFilePath;

        /// <summary>The absolute path for the current player's legacy data file.</summary>
        private string LegacyDataFilePath => Path.Combine(this.Helper.DirectoryPath, "Player_Birthdays", $"HappyBirthday_{Game1.player.Name}.txt");

        /// <summary>The mod configuration.</summary>
        public static ModConfig Config;

        /// <summary>The data for the current player.</summary>
        public static PlayerData PlayerBirthdayData;

        /// <summary>
        /// Wrapper for static field PlayerBirthdayData;
        /// </summary>
        public PlayerData PlayerData
        {
            get
            {
                return PlayerBirthdayData;
            }
            set
            {
                PlayerBirthdayData = value;
            }
        }

        /// <summary>Whether the player has chosen a birthday.</summary>
        private bool HasChosenBirthday => !string.IsNullOrEmpty(this.PlayerData.BirthdaySeason) && this.PlayerData.BirthdayDay != 0;

        /// <summary>The queue of villagers who haven't given a gift yet.</summary>
        private List<string> VillagerQueue;


        /// <summary>Whether we've already checked for and (if applicable) set up the player's birthday today.</summary>
        private bool CheckedForBirthday;
        //private Dictionary<string, Dialogue> Dialogue;
        //private bool SeenEvent;

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals(@"Data\mail");
        }

        public void Edit<T>(IAssetData asset)
        {
            asset
            .AsDictionary<string, string>()
            .Set("birthdayMom", "Dear @,^  Happy birthday sweetheart. It's been amazing watching you grow into the kind, hard working person that I've always dreamed that you would become. I hope you continue to make many more fond memories with the ones you love. ^  Love, Mom ^ P.S. Here's a little something that I made for you. %item object 221 1 %%");

            asset
            .AsDictionary<string, string>()
            .Set("birthdayDad", "Dear @,^  Happy birthday kiddo. It's been a little quiet around here on your birthday since you aren't around, but your mother and I know that you are making both your grandpa and us proud.  We both know that living on your own can be tough but we believe in you one hundred percent, just keep following your dreams.^  Love, Dad ^ P.S. Here's some spending money to help you out on the farm. Good luck! %item money 5000 5001 %%");
        }

        public static IModHelper ModHelper;

        public static IMonitor ModMonitor;

        /// <summary>
        /// Class to handle all birthday messages for this mod.
        /// </summary>
        public BirthdayMessages messages;

        /// <summary>
        /// Class to handle all birthday gifts for this mod.
        /// </summary>
        public GiftManager giftManager;

        /// <summary>
        /// Checks if the current billboard is the daily quest screen or not.
        /// </summary>
        bool isDailyQuestBoard;


        Dictionary<long,PlayerData> othersBirthdays;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            //helper.Content.AssetLoaders.Add(new PossibleGifts());
            Config = helper.ReadConfig<ModConfig>();

            TimeEvents.AfterDayStarted += this.TimeEvents_AfterDayStarted;
            GameEvents.UpdateTick += this.GameEvents_UpdateTick;
            SaveEvents.AfterLoad += this.SaveEvents_AfterLoad;
            SaveEvents.BeforeSave += this.SaveEvents_BeforeSave;
            ControlEvents.KeyPressed += this.ControlEvents_KeyPressed;
            MenuEvents.MenuChanged += MenuEvents_MenuChanged;
            MenuEvents.MenuClosed += MenuEvents_MenuClosed;

            

            GraphicsEvents.OnPostRenderGuiEvent += GraphicsEvents_OnPostRenderGuiEvent;
            StardewModdingAPI.Events.GraphicsEvents.OnPostRenderHudEvent += GraphicsEvents_OnPostRenderHudEvent;
            //MultiplayerSupport.initializeMultiplayerSupport();
            ModHelper = Helper;
            ModMonitor = Monitor;

            messages = new BirthdayMessages();
            giftManager = new GiftManager();
            isDailyQuestBoard = false;

            ModHelper.Events.Multiplayer.ModMessageReceived += Multiplayer_ModMessageReceived;

            ModHelper.Events.Multiplayer.PeerDisconnected += Multiplayer_PeerDisconnected;

            this.othersBirthdays = new Dictionary<long, PlayerData>();

        }

        /// <summary>
        /// Used to check for player disconnections.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Multiplayer_PeerDisconnected(object sender, PeerDisconnectedEventArgs e)
        {
            this.othersBirthdays.Remove(e.Peer.PlayerID);
        }

        private void Multiplayer_ModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == ModHelper.Multiplayer.ModID && e.Type==MultiplayerSupport.FSTRING_SendBirthdayMessageToOthers)
            {
                string message = e.ReadAs<string>();
                Game1.hudMessages.Add(new HUDMessage(message,1));
            }

            if (e.FromModID == ModHelper.Multiplayer.ModID && e.Type == MultiplayerSupport.FSTRING_SendBirthdayInfoToOthers)
            {
                KeyValuePair<long,PlayerData> message = e.ReadAs<KeyValuePair<long,PlayerData>>();
                if (!this.othersBirthdays.ContainsKey(message.Key))
                {
                    this.othersBirthdays.Add(message.Key,message.Value);
                    MultiplayerSupport.SendBirthdayInfoToConnectingPlayer(e.FromPlayerID);
                    Monitor.Log("Got other player's birthday data from: "+ Game1.getFarmer(e.FromPlayerID).name);
                }
                else
                {
                    //Brute force update birthday info if it has already been recevived but dont send birthday info again.
                    this.othersBirthdays.Remove(message.Key);
                    this.othersBirthdays.Add(message.Key, message.Value);
                    Monitor.Log("Got other player's birthday data from: " + Game1.getFarmer(e.FromPlayerID).name);
                }
                


            }

        }

        /// <summary>
        /// Used to check when a menu is closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuEvents_MenuClosed(object sender, EventArgsClickableMenuClosed e)
        {
            this.isDailyQuestBoard = false;
        }


        /// <summary>
        /// Used to properly display hovertext for all events happening on a calendar day.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GraphicsEvents_OnPostRenderHudEvent(object sender, EventArgs e)
        {
            if (Game1.activeClickableMenu == null) return;
            if (PlayerData == null) return;
            if (PlayerData.BirthdaySeason == null) return;
            if (PlayerData.BirthdaySeason.ToLower() != Game1.currentSeason.ToLower()) return;
            if (Game1.activeClickableMenu is Billboard)
            {
                if (isDailyQuestBoard) return;
                if ((Game1.activeClickableMenu as Billboard).calendarDays == null) return;

                //Game1.player.FarmerRenderer.drawMiniPortrat(Game1.spriteBatch, new Vector2(Game1.activeClickableMenu.xPositionOnScreen + 152 + (index - 1) % 7 * 32 * 4, Game1.activeClickableMenu.yPositionOnScreen + 230 + (index - 1) / 7 * 32 * 4), 1f, 4f, 2, Game1.player);

                string hoverText = "";
                List<string> texts = new List<string>();

                foreach (var clicky in (Game1.activeClickableMenu as Billboard).calendarDays)
                {
                    if (clicky.containsPoint(Game1.getMouseX(), Game1.getMouseY()))
                    {
                        if (!String.IsNullOrEmpty(clicky.hoverText))
                        {
                            texts.Add(clicky.hoverText); //catches npc birhday names.
                        }
                        else if (!String.IsNullOrEmpty(clicky.name))
                        {
                            texts.Add(clicky.name); //catches festival dates.
                        }
                    }
                }
                
                for(int i = 0; i< texts.Count; i++)
                {

                    hoverText += texts[i]; //Append text.
                    if (i == texts.Count - 1) continue;
                    else
                    {
                        hoverText += Environment.NewLine; //Append new line.
                    }
                }

                if (!String.IsNullOrEmpty(hoverText))
                {
                    var oldText = Helper.Reflection.GetField<string>(Game1.activeClickableMenu, "hoverText", true);
                    oldText.SetValue(hoverText);
                }

            }
        }

        /// <summary>
        /// Used to show the farmer's portrait on the billboard menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GraphicsEvents_OnPostRenderGuiEvent(object sender, EventArgs e)
        {
            if (Game1.activeClickableMenu == null) return;
            //Don't do anything if birthday has not been chosen yet.
            if (PlayerData == null) return;
            


            if (Game1.activeClickableMenu is Billboard)
            {
                if (isDailyQuestBoard) return;

                if (!String.IsNullOrEmpty(PlayerData.BirthdaySeason))
                {
                    if (PlayerData.BirthdaySeason.ToLower() == Game1.currentSeason.ToLower())
                    {
                        int index = PlayerData.BirthdayDay;
                        Game1.player.FarmerRenderer.drawMiniPortrat(Game1.spriteBatch, new Vector2(Game1.activeClickableMenu.xPositionOnScreen + 152 + (index - 1) % 7 * 32 * 4, Game1.activeClickableMenu.yPositionOnScreen + 230 + (index - 1) / 7 * 32 * 4), 0.5f, 4f, 2, Game1.player);
                    }
                }

                foreach(var pair in this.othersBirthdays)
                {
                    int index = pair.Value.BirthdayDay;
                    if (pair.Value.BirthdaySeason != Game1.currentSeason.ToLower()) continue; //Hide out of season birthdays.
                    index = pair.Value.BirthdayDay;
                    Game1.player.FarmerRenderer.drawMiniPortrat(Game1.spriteBatch, new Vector2(Game1.activeClickableMenu.xPositionOnScreen + 152 + (index - 1) % 7 * 32 * 4, Game1.activeClickableMenu.yPositionOnScreen + 230 + (index - 1) / 7 * 32 * 4), 0.5f, 4f, 2, Game1.getFarmer(pair.Key));
                }

            }
        }

        /// <summary>
        /// Functionality to display the player's birthday on the billboard.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MenuEvents_MenuChanged(object sender, EventArgsClickableMenuChanged e)
        {
            if (Game1.activeClickableMenu == null)
            {
                isDailyQuestBoard = false;
                return;
            }
            if(Game1.activeClickableMenu is Billboard)
            {
                isDailyQuestBoard = ModHelper.Reflection.GetField<bool>((Game1.activeClickableMenu as Billboard), "dailyQuestBoard", true).GetValue();
                if (isDailyQuestBoard) return;

                

                Texture2D text = new Texture2D(Game1.graphics.GraphicsDevice,1,1);
                Color[] col = new Color[1];
                col[0] = new Color(0, 0, 0, 1);
                text.SetData<Color>(col);
                //players birthdy position rect=new ....

                if (!String.IsNullOrEmpty(PlayerData.BirthdaySeason))
                {
                    if (PlayerData.BirthdaySeason.ToLower() == Game1.currentSeason.ToLower())
                    {
                        int index = PlayerData.BirthdayDay;
                        Rectangle birthdayRect = new Rectangle(Game1.activeClickableMenu.xPositionOnScreen + 152 + (index - 1) % 7 * 32 * 4, Game1.activeClickableMenu.yPositionOnScreen + 200 + (index - 1) / 7 * 32 * 4, 124, 124);
                        (Game1.activeClickableMenu as Billboard).calendarDays.Add(new ClickableTextureComponent("", birthdayRect, "", Game1.player.name + "'s Birthday", text, new Rectangle(0, 0, 124, 124), 1f, false));

                    }
                }


                foreach (var pair in this.othersBirthdays)
                {
                    if (pair.Value.BirthdaySeason != Game1.currentSeason.ToLower()) continue;
                    int index = pair.Value.BirthdayDay;
                    Rectangle otherBirthdayRect = new Rectangle(Game1.activeClickableMenu.xPositionOnScreen + 152 + (index - 1) % 7 * 32 * 4, Game1.activeClickableMenu.yPositionOnScreen + 200 + (index - 1) / 7 * 32 * 4, 124, 124);
                    (Game1.activeClickableMenu as Billboard).calendarDays.Add(new ClickableTextureComponent("", otherBirthdayRect, "", Game1.getFarmer(pair.Key).name + "'s Birthday", text, new Rectangle(0, 0, 124, 124), 1f, false));
                }

            }
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method invoked after a new day starts.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void TimeEvents_AfterDayStarted(object sender, EventArgs e)
        {
            this.CheckedForBirthday = false;
        }

        /// <summary>The method invoked when the presses a keyboard button.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void ControlEvents_KeyPressed(object sender, EventArgsKeyPressed e)
        {
            // show birthday selection menu
            if (Game1.activeClickableMenu != null) return;
            if (Context.IsPlayerFree && !this.HasChosenBirthday && e.KeyPressed.ToString() == Config.KeyBinding)
                Game1.activeClickableMenu = new BirthdayMenu(this.PlayerData.BirthdaySeason, this.PlayerData.BirthdayDay, this.SetBirthday);
        }

        /// <summary>The method invoked after the player loads a save.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            this.DataFilePath = Path.Combine("data", Game1.player.Name + "_" + Game1.player.UniqueMultiplayerID+".json");

            // reset state
            this.VillagerQueue = new List<string>();
            this.CheckedForBirthday = false;

            // load settings
            this.MigrateLegacyData();
            this.PlayerData = this.Helper.Data.ReadJsonFile<PlayerData>(this.DataFilePath) ?? new PlayerData();

            messages.createBirthdayGreetings();

            if (PlayerBirthdayData != null)
            {
                ModMonitor.Log("Send all birthday information from " + Game1.player.name);
                MultiplayerSupport.SendBirthdayInfoToOtherPlayers();
            }
            //this.SeenEvent = false;
            //this.Dialogue = new Dictionary<string, Dialogue>();
        }

        /// <summary>The method invoked just before the game updates the saves.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void SaveEvents_BeforeSave(object sender, EventArgs e)
        {
            if (this.HasChosenBirthday)
                this.Helper.Data.WriteJsonFile(this.DataFilePath, this.PlayerData);
        }

        /// <summary>The method invoked when the game updates (roughly 60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady || Game1.eventUp || Game1.isFestival())
                return;
            if (!this.HasChosenBirthday && Game1.activeClickableMenu == null && Game1.player.Name.ToLower()!="unnamed farmhand")
            {
                Game1.activeClickableMenu = new BirthdayMenu(this.PlayerData.BirthdaySeason, this.PlayerData.BirthdayDay, this.SetBirthday);
                this.CheckedForBirthday = false;
            }

            if (!this.CheckedForBirthday && Game1.activeClickableMenu==null)
            {
                this.CheckedForBirthday = true;

                // set up birthday
                if (this.IsBirthday())
                {
                    Messages.ShowStarMessage("It's your birthday today! Happy birthday!");
                    MultiplayerSupport.SendBirthdayMessageToOtherPlayers();




                    Game1.player.mailbox.Add("birthdayMom");
                    Game1.player.mailbox.Add("birthdayDad");

                    try
                    {
                        this.ResetVillagerQueue();
                    }
                    catch (Exception ex)
                    {
                        this.Monitor.Log(ex.ToString(), LogLevel.Error);
                    }
                    foreach (GameLocation location in Game1.locations)
                    {
                        foreach (NPC npc in location.characters)
                        {
                            if (npc is Child || npc is Horse || npc is Junimo || npc is Monster || npc is Pet)
                                continue;

                            //Add in birthday dialogues for npc.
                            try
                            {
                                if (Game1.player.getFriendshipHeartLevelForNPC(npc.Name) >= Config.minimumFriendshipLevelForBirthdayWish)
                                {
                                    bool spouseMessage = false; //Used to determine if there is a valid spouse message for the player. If false load in the generic birthday wish.
                                    //Check if npc name is spouse's name. If no spouse then add in generic dialogue.
                                    if (messages.spouseBirthdayWishes.ContainsKey(npc.Name) && Game1.player.isMarried())
                                    {
                                        Monitor.Log("Spouse Checks out");
                                        //Check to see if spouse message exists.
                                        if (!String.IsNullOrEmpty(messages.spouseBirthdayWishes[npc.Name]))
                                        {
                                            spouseMessage = true;
                                            Dialogue d = new Dialogue(messages.spouseBirthdayWishes[npc.Name], npc);
                                            npc.CurrentDialogue.Push(d);
                                            if (npc.CurrentDialogue.ElementAt(0) != d) npc.setNewDialogue(messages.spouseBirthdayWishes[npc.Name]);
                                        }
                                        else
                                        {
                                            Monitor.Log("No spouse message???", LogLevel.Warn);
                                        }
                                    }
                                    if (spouseMessage == false)
                                    {
                                        //Load in 
                                        Dialogue d = new Dialogue(messages.birthdayWishes[npc.Name], npc);
                                        npc.CurrentDialogue.Push(d);
                                        if (npc.CurrentDialogue.ElementAt(0) != d) npc.setNewDialogue(messages.birthdayWishes[npc.Name]);
                                    }
                                }
                            }
                            catch
                            {
                                if (Game1.player.getFriendshipHeartLevelForNPC(npc.Name) >= Config.minimumFriendshipLevelForBirthdayWish)
                                {
                                    Dialogue d = new Dialogue("Happy Birthday @!", npc);
                                    npc.CurrentDialogue.Push(d);
                                    if (npc.CurrentDialogue.ElementAt(0) != d)
                                        npc.setNewDialogue("Happy Birthday @!");
                                }
                            }
                        }
                    }
                }

                //Don't constantly set the birthday menu.
                if (Game1.activeClickableMenu != null)
                {
                    if (Game1.activeClickableMenu.GetType() == typeof(BirthdayMenu)) return;
                }
                // ask for birthday date
                if (!this.HasChosenBirthday && Game1.activeClickableMenu==null)
                {
                    Game1.activeClickableMenu = new BirthdayMenu(this.PlayerData.BirthdaySeason, this.PlayerData.BirthdayDay, this.SetBirthday);
                    this.CheckedForBirthday = false;
                }
            }

            // Set birthday gift for the player to recieve from the npc they are currently talking with.
            if (Game1.currentSpeaker != null)
            {
                string name = Game1.currentSpeaker.Name;
                if (this.IsBirthday() && this.VillagerQueue.Contains(name))
                {
                    try
                    {
                        giftManager.SetNextBirthdayGift(Game1.currentSpeaker.Name);
                        this.VillagerQueue.Remove(Game1.currentSpeaker.Name);
                    }
                    catch (Exception ex)
                    {
                        this.Monitor.Log(ex.ToString(), LogLevel.Error);
                    }
                }

                //Validate the gift and give it to the player.
                if (giftManager.BirthdayGiftToReceive != null)
                {
                    while (giftManager.BirthdayGiftToReceive.Name == "Error Item" || giftManager.BirthdayGiftToReceive.Name == "Rock" || giftManager.BirthdayGiftToReceive.Name == "???")
                        giftManager.SetNextBirthdayGift(Game1.currentSpeaker.Name);
                    Game1.player.addItemByMenuIfNecessaryElseHoldUp(giftManager.BirthdayGiftToReceive);
                    giftManager.BirthdayGiftToReceive = null;
                }
            }

        }

        /// <summary>Set the player's birthday/</summary>
        /// <param name="season">The birthday season.</param>
        /// <param name="day">The birthday day.</param>
        private void SetBirthday(string season, int day)
        {
            this.PlayerData.BirthdaySeason = season;
            this.PlayerData.BirthdayDay = day;
        }

        /// <summary>Reset the queue of villager names.</summary>
        private void ResetVillagerQueue()
        {
            this.VillagerQueue.Clear();

            foreach (GameLocation location in Game1.locations)
            {
                foreach (NPC npc in location.characters)
                {
                    if (npc is Child || npc is Horse || npc is Junimo || npc is Monster || npc is Pet)
                        continue;
                    if (this.VillagerQueue.Contains(npc.Name))
                        continue;
                    this.VillagerQueue.Add(npc.Name);
                }
            }
        }

        /// <summary>Get whether today is the player's birthday.</summary>
        private bool IsBirthday()
        {
            return
                this.PlayerData.BirthdayDay == Game1.dayOfMonth
                && this.PlayerData.BirthdaySeason == Game1.currentSeason;
        }

        /// <summary>Migrate the legacy settings for the current player.</summary>
        private void MigrateLegacyData()
        {
            // skip if no legacy data or new data already exists
            try
            {
                if (!File.Exists(this.LegacyDataFilePath) || File.Exists(this.DataFilePath))
                    if (this.PlayerData == null) this.PlayerData = new PlayerData();
                         return;
            }
            catch(Exception err)
            {
                err.ToString();
                // migrate to new file
                try
                {
                    string[] text = File.ReadAllLines(this.LegacyDataFilePath);
                    this.Helper.Data.WriteJsonFile(this.DataFilePath, new PlayerData
                    {
                        BirthdaySeason = text[3],
                        BirthdayDay = Convert.ToInt32(text[5])
                    });

                    FileInfo file = new FileInfo(this.LegacyDataFilePath);
                    file.Delete();
                    if (!file.Directory.EnumerateFiles().Any())
                        file.Directory.Delete();
                }
                catch (Exception ex)
                {
                    this.Monitor.Log($"Error migrating data from the legacy 'Player_Birthdays' folder for the current player. Technical details:\n {ex}", LogLevel.Error);
                }
            }
            
        }

        

    }
}
