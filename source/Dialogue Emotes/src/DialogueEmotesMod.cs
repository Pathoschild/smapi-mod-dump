/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/DialogueEmotes
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;

namespace DialogueEmotes
{
    public class DialogueEmotesMod : Mod
    {
        private string lastEmotion;
        private Dictionary<string, Dictionary<string, int>> emoteBank;

        internal static IMonitor _monitor;

        public override void Entry(IModHelper helper)
        {
            _monitor = this.Monitor;

            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.Display.MenuChanged += this.OnDialogueOpen;
            this.emoteBank = ContentPackLoader.LoadEmotes(helper.ContentPacks.GetOwned());
        }

        private void OnDialogueOpen(object sender, StardewModdingAPI.Events.MenuChangedEventArgs e)
        {
            if (e.NewMenu is DialogueBox)
                this.lastEmotion = null;
        }

        private void OnUpdateTicked(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (Game1.dialogueUp && Game1.activeClickableMenu is DialogueBox dialogueBox && Game1.CurrentEvent == null)
            {
                var dialogue = this.Helper.Reflection.GetField<Dialogue>(dialogueBox, "characterDialogue").GetValue();
                bool transitioning = this.Helper.Reflection.GetField<bool>(dialogueBox, "transitioning").GetValue();

                if (dialogue != null && !transitioning)
                {
                    var currentEmotion = this.GetEmotion(dialogue);

                    if (currentEmotion != this.lastEmotion)
                    {
                        this.lastEmotion = currentEmotion;
                        this.ShowLastEmote(dialogue.speaker);
                    }
                }
            }
        }

        private string GetEmotion(Dialogue dialogue)
        {
            if (dialogue.getNPCResponseOptions()?.Count > 0 && dialogue.CurrentEmotion == "$neutral")
            {
                return "$q";
            }

            return dialogue.CurrentEmotion;
        }

        private int GetCustomEmoteFrameId(NPC speaker, string emotion)
        {
            if (this.emoteBank.ContainsKey(speaker.Name) && this.emoteBank[speaker.Name].ContainsKey(emotion))
                return this.emoteBank[speaker.Name][emotion];

            return 0;
        }

        private void ShowLastEmote(NPC speaker)
        {
            int whichEmote = this.GetCustomEmoteFrameId(speaker, this.lastEmotion);

            if (whichEmote <= 0)
            {
                switch (this.lastEmotion)
                {
                    case "$h":
                        whichEmote = 32;
                        break;
                    case "$s":
                        whichEmote = 28;
                        break;
                    case "$l":
                        whichEmote = 20;
                        break;
                    case "$a":
                        whichEmote = 12;
                        break;
                    case "$q":
                        whichEmote = 8;
                        break;
                }
            }

            if (whichEmote > 0)
                speaker.doEmote(whichEmote);
        }
    }
}
