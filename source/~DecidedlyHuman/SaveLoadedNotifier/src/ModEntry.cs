/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace SaveLoadedNotifier
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            // It helps to initialise the i18n stuff.
            I18n.Init(helper.Translation);

            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        }

        [EventPriority((EventPriority)int.MinValue)]
        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            try
            {
                // This will throw if the cue doesn't exist.
                Game1.soundBank.GetCueDefinition("whistle");
            }
            catch (Exception ex)
            {
                this.Monitor.Log("The \"whistle\" sound cue doesn't appear to exist.");
            }

            // If we've reached here, we're fine to play the sound, and display our dialogue.
            Game1.soundBank.PlayCue("whistle");
            Game1.drawDialogueNoTyping(I18n.IntoTheGame_SaveLoaded());
        }
    }
}
