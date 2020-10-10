/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/steven-kraft/StardewDiscord
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Flurl.Http;
using Newtonsoft.Json;
using System.IO;
using Entoarox.Framework.UI;

namespace StardewDiscord
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.GameLoop.OneSecondUpdateTicked += this.OnOneSecondUpdateTicked;
            helper.Events.GameLoop.Saved += this.OnSaved;
            emojiFile = Path.Combine(helper.DirectoryPath, "emojis.json");
            LoadEmojis(emojiFile);
            settingsFile = Path.Combine(helper.DirectoryPath, "config.json");
            LoadSettings(settingsFile);
            helper.ConsoleCommands.Add("discord", "Opens the Stardew Discord config menu.", this.ShowConfig);
        }
        private IReflectedField<List<ChatMessage>> messagesField;
        int msgCount;
        private int lastMsg = 0;
        private string emojiFile;
        private Dictionary<int, string> emojis;
        private string settingsFile;
        private Settings settings;
        List<ChatMessage> messages = new List<ChatMessage>();

        Dictionary<string, string> special_char = new Dictionary<string, string>() { { "=", "☆" }, { "$", "⭗" }, { "*", "💢" }, { "@", "◁" }, { "<", "♡" }, { ">", "▷" } };

        struct Settings
        {
            public Dictionary<string, string> Farms { get; set; }
        }

        private string ReplaceSpecialChar(string msg)
        {
            foreach (string s in special_char.Keys) {
                msg = msg.Replace(s, special_char[s]);
            }
            return msg;
        }

        /// <summary>Loads settings from config file.</summary>
        /// <param name="file">Name of config file.</param>
        private void LoadSettings(string file)
        {
            string json = File.ReadAllText(file);
            settings = JsonConvert.DeserializeObject<Settings>(json);
        }

        private void ShowConfig(string command, string[] args)
        {
            if (!Context.IsWorldReady)
                return;
            FrameworkMenu Menu = new FrameworkMenu(new Point(200, 40));
            TextComponent label = new TextComponent(new Point(0, 0), "Webhook URL:");
            TextboxFormComponent webhookUrlTextbox = new TextboxFormComponent(new Point(0, 8), 175, null);
            ButtonFormComponent setButton = new ButtonFormComponent(new Point(0, 21), "Set", (t, p, m) =>  this.SetWebhook(webhookUrlTextbox.Value, Game1.player.farmName));
            Texture2D icon = this.Helper.Content.Load<Texture2D>("assets/icon.png");
            TextureComponent iconTexture = new TextureComponent(new Rectangle(-16, -16, 16, 16), icon);
            Menu.AddComponent(label);
            Menu.AddComponent(webhookUrlTextbox);
            Menu.AddComponent(setButton);
            Menu.AddComponent(iconTexture);
            Game1.activeClickableMenu = Menu;
        }

        /// <summary>Associates webhook with current farm</summary>
        /// <param name="url">Url of Discord Webhook</param>
        /// <param name="farm">Name of current farm</param>
        private async void SetWebhook(string url, string farm)
        {
            string name = await GetWebhookName(url);
            if (name != null)
            {
                settings.Farms[farm] = url;
                string json = JsonConvert.SerializeObject(settings);
                File.WriteAllText(settingsFile, json);
                if (settings.Farms.ContainsKey(farm) && settings.Farms[farm] == url)
                {
                    ShowSuccessMessage($"Connected {farm} Farm's chat with Webhook: {name}");
                }
            }
            else { ShowFailureMessage("Invalid Webhook"); }
            Game1.activeClickableMenu = null;
        }

        /// <summary>Gets username of Discord Webhook</summary>
        /// <param name="url">Url of Discord Webhook</param>
        private static async Task<string> GetWebhookName(string url)
        {
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute)) { return null; }
            string json = await url.GetStringAsync();
            Dictionary<string, string> result;
            try
            {
                result = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
            catch (JsonReaderException e) { return null; }
            return result["name"];
        }

        /// <summary>Shows success popup message</summary>
        /// <param name="message">Message to display</param>
        private void ShowSuccessMessage(string message)
        {
            Game1.addHUDMessage(new HUDMessage(message, HUDMessage.stamina_type));
        }

        /// <summary>Shows failure popup message</summary>
        /// <param name="message">Message to display</param>
        private void ShowFailureMessage(string message)
        {
            Game1.addHUDMessage(new HUDMessage(message, HUDMessage.error_type));
        }

        /// <summary>Sends message to a Discord channel via webhook.</summary>
        /// <param name="msg">Message to be sent</param>
        /// <param name="farm">Name of farm</param>
        /// <param name="notification">Indicates whether message should be treated as a notification</param>
        private async Task SendMessage(string msg, string farm, bool notification = false)
        {
            if (!settings.Farms.ContainsKey(farm))
                return;

            msg = ReplaceSpecialChar(msg);
            string url = settings.Farms[farm];
            if (notification)
            {
                object data = new { embeds = new List<object> { new { description = $"**{msg}**", color = 16098851 } } };
                var responseString = await url.PostJsonAsync(data);
            }
            else
            {
                var responseString = await url.PostUrlEncodedAsync(new { content = msg }).ReceiveString();
            }
        }

        /// <summary>Loads emoji aliases from config file.</summary>
        /// <param name="file">Name of config file.</param>
        private void LoadEmojis(string file)
        {
            string json = File.ReadAllText(file);
            emojis = JsonConvert.DeserializeObject<Dictionary<int, string>>(json);
        }

        /// <summary>Returns a list of active players.</summary>
        private List<string> GetPlayers()
        {
            List<string> players = new List<string>();
            foreach (Farmer farmer in Game1.getOnlineFarmers())
            {
                players.Add(farmer.name.ToString());
            }
            return players;
        }

        /// <summary>Raised once every second.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private async void OnOneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            foreach(ChatMessage message in messages)
            {
                string msg = "";
                foreach (ChatSnippet m in message.message)
                {
                    msg += m.message;
                    if (m.message == null)
                    {
                        if (emojis.ContainsKey(m.emojiIndex))
                        {
                            if (emojis[m.emojiIndex] != "") { msg += $":{emojis[m.emojiIndex]}:"; }
                            else { Monitor.Log($"Emoji {m.emojiIndex} does not have an associated Discord emoji", LogLevel.Info); }
                        }
                        else { Monitor.Log($"Could not find emoji with index {m.emojiIndex}", LogLevel.Info); }
                    }
                }
                if (message.color == Color.Yellow && msg.IndexOf(">  -") != 0)
                {
                    await SendMessage(msg, Game1.player.farmName, true);
                }
                else if (GetPlayers().Contains(message.message[0].message.Split(':')[0]))
                {
                    await SendMessage(msg, Game1.player.farmName);
                }
            }
            messages.Clear();
        }

        /// <summary>Raised once every tick.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            messagesField = Helper.Reflection.GetField<List<ChatMessage>>(Game1.chatBox, "messages");
            msgCount = messagesField.GetValue().Count;
            if (msgCount == 0) { return; }

            ChatMessage message = messagesField.GetValue()[msgCount - 1];
            if (message.GetHashCode() != lastMsg)
            {
                lastMsg = message.GetHashCode();
                messages.Add(message);
            }
        }

        /// <summary>Raised once after game is saved.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaved(object sender, SavedEventArgs e)
        {
            messagesField = Helper.Reflection.GetField<List<ChatMessage>>(Game1.chatBox, "messages");
            msgCount = messagesField.GetValue().Count;
            if (msgCount == 0) { return; }

            int count = 1;
            ChatMessage message = messagesField.GetValue()[msgCount - count];
            while (message.GetHashCode() != lastMsg)
            {
                messages.Add(message);
                message = messagesField.GetValue()[msgCount - ++count];
            }
            lastMsg = messagesField.GetValue()[msgCount - 1].GetHashCode();
            messages.Reverse();
        }
    }
}