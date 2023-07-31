/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enteligenz/StardewMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

using System.Threading.Tasks;
using AsyncAwaitBestPractices;

namespace TwitchChatIntegration
{
    internal sealed class ModEntry : Mod
    {

        /// <summary> List of all possible chat colors. </summary>
        readonly Color[] ChatColors =
        {
            Color.MediumTurquoise,
            Color.SeaGreen,
            new Color(220, 20, 20),
            Color.DodgerBlue,
            new Color(50, 230, 150),
            new Color(0, 180, 10),
            new Color(182, 214, 0),
            Color.HotPink,
            new Color(240, 200, 0),
            new Color(255, 100, 0),
            new Color(138, 43, 250),
            Color.Gray,
            new Color(255, 255, 180),
            new Color(255, 180, 120),
            new Color(160, 80, 30),
            Color.Salmon,
            new Color(190, 0, 190),
        };

        /// <summary> Mod configuration from the player containing login credentials for a Twitch account. </summary>
        private ModConfig Config;

        public override async void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            var twitchBot = new TwitchBot(this.Config.Username, this.Config.Password, this.Monitor);

            twitchBot.Start().SafeFireAndForget();
            await twitchBot.JoinChannel(this.Config.TargetChannel);

            twitchBot.OnMessage += this.OnTwitchMessage;

            await Task.Delay(-1);

        }

        /// <summary> Picks a color for a specific sender and prints the message that was sent in Twitch chat into the Stardew Valley chat. </summary>
        private async void OnTwitchMessage(object sender, TwitchBot.TwitchChatMessage twitchChatMessage)
        {
            int ColorIdx = Math.Abs(twitchChatMessage.Sender.GetHashCode()) % this.ChatColors.Length;
            Color chatColor = this.ChatColors[ColorIdx];

            Game1.chatBox.addMessage(twitchChatMessage.Sender + ": " + twitchChatMessage.Message, chatColor);
        }
    }
}