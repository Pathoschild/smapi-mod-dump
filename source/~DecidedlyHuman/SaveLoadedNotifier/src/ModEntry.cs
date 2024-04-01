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
using DecidedlyShared.APIs;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace SaveLoadedNotifier
{
    public class ModEntry : Mod
    {
        private ModConfig config;

        public override void Entry(IModHelper helper)
        {
            // It helps to initialise the i18n stuff.
            I18n.Init(helper.Translation);

            this.config = helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.GameLaunched += this.GameLoopOnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        }

        private void GameLoopOnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            if (this.Helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu"))
            {
                var gmcm = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

                gmcm.Register(this.ModManifest,
                    () => this.config = new ModConfig(),
                    () => this.Helper.WriteConfig(this.config));

                gmcm.AddTextOption(
                    this.ModManifest,
                    () => this.config.SoundCue,
                    setValue: (value) => this.config.SoundCue = value,
                    () => "Sound cue"
                    );
            }
        }

        [EventPriority((EventPriority)int.MinValue)]
        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            // Apparently multiplayer exists.
            if (Game1.activeClickableMenu is CharacterCustomization)
                return;

            try
            {
                // This will throw if the cue doesn't exist.
                Game1.soundBank.GetCueDefinition(this.config.SoundCue);
            }
            catch (Exception ex)
            {
                this.Monitor.Log($"The \"{this.config.SoundCue}\" sound cue doesn't appear to exist.");
            }

            // If we've reached here, we're fine to play the sound, and display our dialogue.
            Game1.soundBank.PlayCue(this.config.SoundCue);
            Game1.drawDialogueNoTyping(I18n.IntoTheGame_SaveLoaded());
        }
    }
}
