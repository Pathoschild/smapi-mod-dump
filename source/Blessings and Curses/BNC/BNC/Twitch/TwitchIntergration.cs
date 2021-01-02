/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GenDeathrow/SDV_BlessingsAndCurses
**
*************************************************/

using BNC.Configs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Timers;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;
using static BNC.Spawner;

namespace BNC
{

    class TwitchIntergration
    {
        private static TwitchClient client;
        private static ConnectionCredentials credentials;
        private static String TwitchUsername;
        private static String TwitchToken;
        private static String TwitchChannel;

        public static bool hasPollStarted;
        public static Timer timer = new Timer();

        public static List<String> usersVoted = new List<string>();

        public static bool hasMinePollStarted;



        public static Dictionary<BuffManager.BuffOption, int> votes;
        public static Dictionary<MineBuffManager.AugmentOption, int> augvotes;

        
        private static int currentTick = 30;

        private static List<String> currentDisplayText = new List<string>();

        private static Spawner spawner = new Spawner();

        private static readonly string fileName = "twitch_secret.json";


        public static void LoadConfig(IModHelper helper)
        {
            TwitchSecret twitchSettings = new TwitchSecret();

            if (helper.Data.ReadJsonFile<TwitchSecret>(fileName) == null)
                helper.Data.WriteJsonFile<TwitchSecret>(fileName, twitchSettings);
            else
                twitchSettings = helper.Data.ReadJsonFile<TwitchSecret>(fileName);

            TwitchUsername = twitchSettings.Twitch_User_Name;
            TwitchToken = twitchSettings.OAuth_Token;
            TwitchChannel = twitchSettings.Twitch_Channel_Name;
            TwitchInit(helper);
        }


        public static void TwitchInit(IModHelper helper)
        {
            if (TwitchUsername == null || TwitchToken == null || TwitchChannel == null)
            {
                BNC_Core.Logger.Log($"Tried to initialize twitch integration but failed. Is Null? username:{TwitchUsername == null} || token:{TwitchToken == null} || channel:{TwitchChannel == null}", LogLevel.Warn);
                return;
            }
            else
            {
                BNC_Core.Logger.Log($"Twitch Loading... username:{TwitchUsername} || channel:{TwitchChannel}", LogLevel.Info);
            }
            credentials = new ConnectionCredentials(TwitchUsername, TwitchToken);
            client = new TwitchClient();
            client.Initialize(credentials, TwitchChannel);
            client.OnConnected += OnConnected;
            client.OnDisconnected += OnDisconnect;
            client.OnMessageReceived += OnMessageReceived;
            client.OnNewSubscriber += OnNewSubscriber;
            client.OnConnectionError += OnConnectionError;
            client.OnError += OnError;
            client.OnGiftedSubscription += OnGifted;
            client.OnReSubscriber += OnReSubscriber;
            BNC_Core.helper.Events.Display.RenderedHud += GraphicsEvents_OnPostRenderHudEvent;
            timer.Interval = 1000;
            timer.Elapsed += onTimertick;
            Connet();
        }


        private static void OnError(object sender, OnErrorEventArgs e)
        {
            BNC_Core.Logger.Log($"Twitch Integration Error {e.Exception.Message}", LogLevel.Error);
        }

        private static void OnConnectionError(object sender, OnConnectionErrorArgs e)
        {
            BNC_Core.Logger.Log($"Twitch Integration Connection Error {e.Error.Message}", LogLevel.Error);
        }

        private static void onTimertick(object sender, ElapsedEventArgs e)
        {
            if (currentTick-- <= 0)
                if (hasPollStarted)
                    EndBuffPoll();
                else if (hasMinePollStarted)
                    EndMinePoll();
        }

        private static void UpdateVotingText()
        {
            currentDisplayText.Clear();

            if (hasPollStarted)
            {
                int maximum = 0;
                foreach (int count in votes.Values)
                    maximum += count;

                foreach (KeyValuePair<BuffManager.BuffOption, int> vote in votes)
                {
                    int perc = (vote.Value == 0 || maximum == 0) ? 0 : Convert.ToInt32(((double)vote.Value / maximum) * 100);
                    currentDisplayText.Add($"{vote.Key.displayName} [{vote.Key.id}] ({perc}%) -- {vote.Key.shortdesc}");
                }
            }else if (hasMinePollStarted)
            {
                int maximum = 0;
                foreach (int count in augvotes.Values)
                    maximum += count;
                foreach (KeyValuePair<MineBuffManager.AugmentOption, int> vote in augvotes)
                {
                    int perc = (vote.Value == 0 || maximum == 0) ? 0 : Convert.ToInt32(((double)vote.Value / maximum) * 100);
                    currentDisplayText.Add($"{vote.Key.DisplayName} [{vote.Key.id}] ({perc}%) -- {vote.Key.desc}");
                }
            }
        }

        public static bool isConnected()
        {
            if (client == null)
                return false;

            return client.IsConnected;
        }

        static int reconnectTrys = 0;
        public static void attemptReconnect(string msg = null)
        {
            if (reconnectTrys >= 4)
                return;
            BNC_Core.Logger.Log($"Attempting to reconnect to the Channel:{TwitchChannel}...", LogLevel.Info);

            if (reconnectTrys++ < 4)
            {
                if (client != null)
                {
                    Connet();

                    if (msg != null) sendMessage(msg);
                }
                else
                    BNC_Core.Logger.Log($"Attempted to connect to Channel:{TwitchChannel}, But failed", LogLevel.Error);
            }
        }

        private static bool sendMessage(string msg)
        {

            try
            {
                client.SendMessage(TwitchChannel, msg);
                if (Config.ShowDebug())
                    BNC_Core.Logger.Log($"Sending Message to {TwitchChannel}! >>> {msg}", LogLevel.Info);
                return true;
            }
            catch(Exception e)
            {
                attemptReconnect();
                BNC_Core.Logger.Log($"Faild to send message to Twitch Channel {TwitchChannel}! >>> {msg} Error:"+ e, LogLevel.Error);
                return false;
            }
        }

        public static void StartBuffPoll(BuffManager.BuffOption[] buffs)
        {

            if(Config.ShowDebug())
                BNC_Core.Logger.Log($"Poll has started...", LogLevel.Info);

           if (!sendMessage($"---> TIME TO CHOOSE A BLESSING/CURSE!"))
               return;

            votes = new Dictionary<BuffManager.BuffOption, int>();
            string buildMsg =  "[";
            foreach (BuffManager.BuffOption buff in buffs)
            {
                buildMsg += $"{buff.id} {(buffs.GetValue(buffs.Length-1) == buff ? "" : " / ")}";
                votes.Add(buff, 0);
            }
            buildMsg += "]";

            sendMessage(buildMsg);

            timer.Start();
            hasPollStarted = true;
            usersVoted.Clear();
            UpdateVotingText();
        }

        public static void EndBuffPoll()
        {
            if (Config.ShowDebug())
                BNC_Core.Logger.Log($"Poll has ended!", LogLevel.Info);
            currentTick = Config.getVotingTime();
            timer.Stop();
            hasPollStarted = false;

            if (Config.ShowDebug())
                BNC_Core.Logger.Log($"Time to tally the votes!", LogLevel.Info);

            BuffManager.BuffOption selectedId = null;
            int votecount = -1;
            foreach(KeyValuePair<BuffManager.BuffOption, int> vote in votes)
            {
                if(vote.Value > votecount)
                {
                   votecount = vote.Value;
                   selectedId = vote.Key;
                }
            }

            if (selectedId != null)
            {
                if (Config.ShowDebug())
                    BNC_Core.Logger.Log($"Chat has selected {selectedId.displayName} : vote# {votecount}", LogLevel.Info);

                BuffManager.AddBuffToQueue(selectedId);

                sendMessage($"Chat has spoken! Selected {selectedId.displayName}!");

                if(selectedId.hudMessage != null)
                    Game1.addHUDMessage(new HUDMessage(selectedId.hudMessage, null));
            }
            else if (Config.ShowDebug())
                    BNC_Core.Logger.Log($"Error: {selectedId} Buff was null", LogLevel.Error);

            votes.Clear();
        }

        public static void StartMinePoll(MineBuffManager.AugmentOption[] augments)
        {
            if (Config.ShowDebug())
                BNC_Core.Logger.Log($"Poll has started...", LogLevel.Info);

            sendMessage($"---> TIME TO CHOOSE A COMBAT AUGMENT!");

            augvotes = new Dictionary<MineBuffManager.AugmentOption, int>();
            string buildMsg = "[";
            foreach (MineBuffManager.AugmentOption aug in augments)
            {
                buildMsg += $"{aug.id} {(augments[augments.Length - 1] == aug ? "" : " / ")}";
                augvotes.Add(aug, 0);
            }
            buildMsg += "]";

            sendMessage(buildMsg);

            timer.Start();
            hasMinePollStarted = true;
            usersVoted.Clear();
            UpdateVotingText();
        }

        public static void EndMinePoll()
        {
            if (Config.ShowDebug())
                BNC_Core.Logger.Log($"Poll has ended!", LogLevel.Info);
            currentTick = Config.getVotingTime();
            timer.Stop();
            hasMinePollStarted = false;

            if (Config.ShowDebug())
                BNC_Core.Logger.Log($"Time to tally the votes!", LogLevel.Info);

            MineBuffManager.AugmentOption selectedId = null;
            int votecount = -1;
            foreach (KeyValuePair<MineBuffManager.AugmentOption, int> vote in augvotes)
            {
                if (vote.Value > votecount)
                {
                    votecount = vote.Value;
                    selectedId = vote.Key;
                }
            }

            if (selectedId != null)
            {
                if (Config.ShowDebug())
                    BNC_Core.Logger.Log($"Chat has selected {selectedId.DisplayName} : vote# {votecount}", LogLevel.Info);

                MineBuffManager.CurrentAugment = selectedId.id;

                sendMessage($"Chat has spoken! Selected {selectedId.DisplayName}!");

                if (selectedId.DisplayName != null)
                    Game1.addHUDMessage(new HUDMessage(selectedId.DisplayName, null));
            }
            else if (Config.ShowDebug())
                BNC_Core.Logger.Log($"Error: {selectedId} Buff was null", LogLevel.Info);

            augvotes.Clear();
        }


        public static void Connet()
        {
            BNC_Core.Logger.Log($"{TwitchUsername} has connected to Channel:{TwitchChannel}", LogLevel.Info);
            try
            {
                client.Connect();
            }
            catch (Exception e) { BNC_Core.Logger.Log($"Attempted to connect to {TwitchChannel}, But failed {e}", LogLevel.Warn); }
        }

        public static void Disconnect()
        {
            BNC_Core.Logger.Log($"{TwitchUsername} has disconnected from Channel:{TwitchChannel}", LogLevel.Warn);
            client.Disconnect();
        }

        private static void OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            Spawner.SpawnTwitchJunimo(e.Subscriber.DisplayName);
            if (Config.ShowDebug() && BNC_Core.config.Spawn_Subscriber_Junimo)
                BNC_Core.Logger.Log($"Attemting to spawn Junimo:{e.Subscriber.DisplayName} from resub", LogLevel.Info);
        }


        private static void OnGifted(object sender, OnGiftedSubscriptionArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if(BNC_Core.config.Spawn_GiftSub_Subscriber_Mobs)
                Spawner.AddMonsterToSpawnFromType(TwitchMobType.Slime, e.GiftedSubscription.MsgParamRecipientDisplayName, true);

            if (Config.ShowDebug())
                BNC_Core.Logger.Log($"Adding GiftSub Slime:{e.GiftedSubscription.MsgParamRecipientDisplayName} to queue...", LogLevel.Info);
        }


        private static void OnReSubscriber(object sender, OnReSubscriberArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            Spawner.SpawnTwitchJunimo(e.ReSubscriber.DisplayName);
            if (Config.ShowDebug() && BNC_Core.config.Spawn_Subscriber_Junimo)
                BNC_Core.Logger.Log($"Adding Junimo to queue from resub: {e.ReSubscriber.DisplayName}", LogLevel.Info);
        }

        private static int BitCount = 0;
        private static List<String> mobNames = new List<String>();

        private static DateTime lastHelp = DateTime.Now;

        private static void DebugCommands(string msg, string sender)
        {
            if (msg.ToLower().Contains("$bits"))
            {

                MatchCollection matches = Regex.Matches(msg.ToLower(), "\\$bits([0-9]+)");

                if (Config.ShowDebug())
                    BNC_Core.Logger.Log($"DEBUGMODE: {matches.Count} cheer found from {sender}", LogLevel.Debug);

                if (matches.Count > 0)
                {
                    ArrayList bitList = new ArrayList();
                    int[] list = new int[matches.Count];

                    int i = 0;
                    int totalBitsCounted = 0;
                    foreach (Match match in matches)
                    {
                        int bitamt = 0;
                        try
                        {
                            bitamt = int.Parse(match.Groups[1].ToString());
                            list[i++] = bitamt;
                            totalBitsCounted += bitamt;
                        }
                        catch { BNC_Core.Logger.Log($"{match.Groups[1].ToString()} is not a valid number from {sender}", LogLevel.Warn); }
                    }

                    foreach (int bitamt in list)
                    {
                        TwitchMobType? type = (TwitchMobType)Spawner.GetMonsterFromBits(bitamt);

                        if (type != null)
                        {
                            if (Config.ShowDebug())
                                BNC_Core.Logger.Log($"DEBUGMODE: Adding Monster:{type.ToString()}:{sender} to queue with {bitamt}bits", LogLevel.Debug);
                            Spawner.AddMonsterToSpawnFromType((TwitchMobType)type, sender);
                        }
                        else if (Config.ShowDebug())
                            BNC_Core.Logger.Log($"DEBUGMODE: Failed to add monster to queue with {bitamt}bits from {sender}", LogLevel.Debug);
                    }
                }
                /*
                BNC_Core.Logger.Log($"split {split.Length} length");
                if (split.Length >= 2)
                {
                    int bitamt = 0;

                    try
                    {
                        bitamt = int.Parse(split[1]);
                    }
                    catch{ BNC_Core.Logger.Log($"{split[1]} is not a valid number from {sender}"); }

                    if(bitamt > 0)
                    {
                        TwitchMobType? type = (TwitchMobType?)Spawner.GetMonsterFromBits(bitamt);

                        if (type != null)
                        {
                            if (Config.ShowDebug())
                                BNC_Core.Logger.Log($"DEBUGMODE: Adding Monster:{type.ToString()}:{sender} to queue with {bitamt}bits");
                            Spawner.AddMonsterToSpawnFromType((TwitchMobType)type, sender);
                        }
                        else if (Config.ShowDebug())
                            BNC_Core.Logger.Log($"DEBUGMODE: Failed to add monster to queue with {bitamt}bits from {sender}");
                    }

                }
                */
            }
            else if (msg.StartsWith("$sub"))
            {
                if (BNC_Core.config.Spawn_Subscriber_Junimo)
                    Spawner.SpawnTwitchJunimo(sender);
                else
                    BNC_Core.Logger.Log($"DEBUGMODE: 'Spawn_Subscriber_Junimo' Config option is turned off for subscribers");

                if (Config.ShowDebug() && BNC_Core.config.Spawn_Subscriber_Junimo)
                    BNC_Core.Logger.Log($"DEBUGMODE: Adding Junimo:{sender} to queue from a new sub");
            }
            else if (msg.StartsWith("$gift"))
            {
                if (BNC_Core.config.Spawn_GiftSub_Subscriber_Mobs)
                    Spawner.AddMonsterToSpawnFromType(TwitchMobType.Slime, sender, true);
                else
                    BNC_Core.Logger.Log($"DEBUGMODE: 'Spawn_GiftSub_Subscriber_Mobs' Config option is turned off for gift subs");

                if (Config.ShowDebug())
                    BNC_Core.Logger.Log($"DEBUGMODE: Adding GiftSub Slime:{sender} to queue...");

            }
            else if (msg.StartsWith("$resub"))
            {
                if (BNC_Core.config.Spawn_Subscriber_Junimo)
                    Spawner.SpawnTwitchJunimo(sender);
                else
                    BNC_Core.Logger.Log($"DEBUGMODE: 'Spawn_Subscriber_Junimo' Config option is turned off for subscribers");

                if (Config.ShowDebug() && BNC_Core.config.Spawn_Subscriber_Junimo)
                    BNC_Core.Logger.Log($"DEBUGMODE: Adding Junimo:{sender} to queue from a new resubs");
            }else if (msg.StartsWith("$help") && DateTime.Now > lastHelp.AddSeconds(15))
            {
                sendMessage("Current Debug Commands: '$bits <amt>', '$gift', '$resub', and '$sub'");
                lastHelp = DateTime.Now;
            }

        }

        private static void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {

            if (!Context.IsWorldReady)
                return;

 

            if (Config.IsDebugMode())
            {
                bool flag = false;

                if (BNC_Core.config.Moderators_Can_Only_Use_Chat_Commands && (e.ChatMessage.IsModerator || e.ChatMessage.IsBroadcaster))
                    flag = true;
                else if (!BNC_Core.config.Moderators_Can_Only_Use_Chat_Commands)
                    flag = true;


                if(flag)
                    DebugCommands(e.ChatMessage.Message, e.ChatMessage.DisplayName);
            }

            if (hasMinePollStarted && !usersVoted.Contains(e.ChatMessage.UserId))
            {
                List<MineBuffManager.AugmentOption> keysvalue = new List<MineBuffManager.AugmentOption>(augvotes.Keys);
                foreach (MineBuffManager.AugmentOption vote in keysvalue)
                {
                    if (vote.id.ToLower().Equals(e.ChatMessage.Message.Trim().ToLower()))
                    {
                        augvotes[vote]++; 
                        if (Config.ShowDebug())
                            BNC_Core.Logger.Log($"Recieved Vote for {vote.id} : #{augvotes[vote]}", LogLevel.Info);
                        usersVoted.Add(e.ChatMessage.UserId);
                        UpdateVotingText();
                    }
                }
            }
            else if (hasPollStarted && !usersVoted.Contains(e.ChatMessage.UserId))
            {
                List<BuffManager.BuffOption> keysvalue = new List<BuffManager.BuffOption>(votes.Keys);
                foreach (BuffManager.BuffOption vote in keysvalue)
                {
                    if (vote.id.ToLower().Equals(e.ChatMessage.Message.Trim().ToLower()))
                    {
                        votes[vote]++;
                        if (Config.ShowDebug())
                            BNC_Core.Logger.Log($"Recieved Vote for {vote.id} : #{votes[vote]}", LogLevel.Info);
                        usersVoted.Add(e.ChatMessage.UserId);
                        UpdateVotingText();
                    }
                }
            }
            else if (BNC_Core.config.Use_Bits_To_Spawn_Mobs && e.ChatMessage.Bits > 0)
            {
                BitCount += e.ChatMessage.Bits;
                MatchCollection matches = Regex.Matches(e.ChatMessage.Message.ToLower(), "cheer([0-9]+)");

                if (matches.Count > 0)
                {
                    if (Config.ShowDebug())
                        BNC_Core.Logger.Log($"Found Cheers {matches.Count} in Message. Should == {e.ChatMessage.Bits}", LogLevel.Info);

                    int[] list = new int[matches.Count];

                    int i = 0;
                    int totalBitsCounted = 0;
                    foreach (Match match in matches) {
                        int bitamt = 0;
                        try
                        {
                            bitamt = int.Parse(match.Groups[1].ToString());
                            list[i++] = bitamt;
                            totalBitsCounted += bitamt;

                        }
                        catch { BNC_Core.Logger.Log($"{match.Groups[1].ToString()} is not a valid number from {sender}", LogLevel.Warn); }
                    }

                    if (totalBitsCounted != e.ChatMessage.Bits)
                    {
                        BNC_Core.Logger.Log($"Could not match the Cheers to the total bit counts. Failed to parse data. {totalBitsCounted} != {e.ChatMessage.Bits}. Contact Developer if this was in error.", LogLevel.Error);
                        return;
                    }

                    foreach (int bitamt in list) {
                        TwitchMobType? type = (TwitchMobType)Spawner.GetMonsterFromBits(bitamt);
                        if (type != null)
                        {
                            if (Config.ShowDebug())
                                BNC_Core.Logger.Log($"Adding monster to queue {type.ToString()} with {bitamt}bits", LogLevel.Info);
                            Spawner.AddMonsterToSpawnFromType((TwitchMobType)type, e.ChatMessage.DisplayName);
                        }
                        else if (Config.ShowDebug())
                            BNC_Core.Logger.Log($"Failed to add monster to queue with {bitamt}bits from {e.ChatMessage.DisplayName}", LogLevel.Info);
                    }
                }
            }
        }

        private static void OnConnected(object sender, OnConnectedArgs e)
        {
            BNC_Core.Logger.Log($"{TwitchUsername} (BNC Bot) has connected to Twitch channel : {TwitchChannel}", LogLevel.Info);

            if(Config.ShowDebug())
                sendMessage("Blessings and Curses has initialized a connection to Twitch!!!");
        }



        private static void OnDisconnect(object sender, OnDisconnectedEventArgs e)
        {
            BNC_Core.Logger.Log($"{TwitchUsername} has disconnected from Twitch channel : {TwitchChannel}", LogLevel.Warn);
            attemptReconnect();
        }


        private static void GraphicsEvents_OnPostRenderHudEvent(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady || (!hasPollStarted && !hasMinePollStarted) || currentDisplayText.Count < 1 &&  Game1.CurrentEvent == null)
                return;

            int num1 = 64;
            SpriteFont smallFont = Game1.smallFont;
            SpriteBatch spriteBatch = Game1.spriteBatch;

            Vector2 vector2 = smallFont.MeasureString("");
            foreach (String str in currentDisplayText)
            {
                Vector2 temp = smallFont.MeasureString(str);
                if (temp.X > vector2.X)
                    vector2 = temp;
            }

            int num2 = num1 / 2;
            int width = (int)((double)vector2.X + (double)num2) + 65;
            int height = Math.Max(60, 60 + 35 * (currentDisplayText.Count - 1));
            int x = 0;
            int y = 0;
            if (x + width > Game1.viewport.Width)
            {
                x = Game1.viewport.Width - width;
                y += num2;
            }
            if (y + height > Game1.viewport.Height)
            {
                x += num2;
                y = Game1.viewport.Height - height;
            }
            int cnt = 0;
            IClickableMenu.drawTextureBox(spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y, width, height+ 35, Color.White, 1f, true);
            Utility.drawTextWithShadow(spriteBatch, "Vote: ", smallFont, new Vector2((float)(x + num1 / 4), (float)(y + num1 / 4) + (cnt++ * vector2.Y)), Color.Purple, 1f, -1f, -1, -1, 1f, 3);
            foreach (String str in currentDisplayText)
            {
                Utility.drawTextWithShadow(spriteBatch, str, smallFont, new Vector2((float)(x + num1 / 4) + 5, (float)(y + num1 / 4) + (cnt * vector2.Y)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
                cnt++;
            }
            Utility.drawTextWithShadow(spriteBatch, $"{currentTick}s", smallFont, new Vector2((x + width) - smallFont.MeasureString(currentTick + "s").Length() - 4, height/2 - 4), Color.Blue, 1f, -1f, -1, -1, 1f, 3);
        }

    }
}
