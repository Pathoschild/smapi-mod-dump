/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FricativeMelon/SkipDialogue
**
*************************************************/

using System;
using System.Net.Mail;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;

namespace YourProjectName
{

    class ModConfig
    { 
        public KeybindList SkipKey { get; set; } = KeybindList.Parse("K, Y");
        public int SpeedMode { get; set; } = 1;
    }

    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration from the player.</summary>
        private ModConfig Config;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            int runs = 0;

            if (this.Config.SkipKey.JustPressed())
            {
                if (Game1.activeClickableMenu is DialogueBox box)
                {
                    box.characterIndexInDialogue = box.getCurrentString().Length - 1;
                    while (this.Config.SpeedMode > runs && Game1.activeClickableMenu is DialogueBox box2 && !box2.isQuestion)
                    {
                        runs++;
                        box2.transitioning = false;
                        box2.safetyTimer = 0;
                        box2.receiveLeftClick(0, 0, true);
                        box2.characterIndexInDialogue = box.getCurrentString().Length - 1;
                    }
                }
                if (Game1.activeClickableMenu is LetterViewerMenu m)
                {
                    while (this.Config.SpeedMode + 1 > runs && m.page < m.mailMessage.Count - 1)
                    {
                        runs++;
                        m.page++;
                        Game1.playSound("shwip");
                        m.OnPageChange();
                    }
                    if (Game1.activeClickableMenu is LetterViewerMenu m2 && !m2.HasInteractable())
                    {
                        if (this.Config.SpeedMode > runs)
                        {
                            Game1.playSound("bigDeSelect");
                            if (!m2.isFromCollection)
                            {
                                m2.exitThisMenu(m2.ShouldPlayExitSound());
                            }
                            else
                            {
                                m2.destroy = true;
                            }
                        }
                    }
                }
            }
        }
    }
}