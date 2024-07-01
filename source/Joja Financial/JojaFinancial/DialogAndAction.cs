/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/JojaFinancial
**
*************************************************/

using System;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace StardewValleyMods.JojaFinancial
{

    /// <summary>
    /// Shamelessly stolen from RSV: https://github.com/Rafseazz/Ridgeside-Village-Mod/blob/main/Ridgeside%20SMAPI%20Component%202.0/RidgesideVillage/DialogueMenu.cs.
    /// </summary>
    /// <param name="dialogue">Initial dialogue.</param>
    /// <param name="responses">List of responses.</param>
    /// <param name="actions">List of associated actions.</param>
    /// <param name="inputHelper">SMAPI's input helper.</param>
    public sealed class DialogueAndAction(string dialogue, Response[] responses, Action?[] actions, IInputHelper inputHelper) : DialogueBox(dialogue, responses)
    {
        /// <summary>
        /// Handles a key press.
        /// </summary>
        /// <param name="key">Key.</param>
        public override void receiveKeyPress(Keys key)
        {
            base.receiveKeyPress(key);
            if (this.safetyTimer > 0)
            {
                return;
            }
            for (int i = 0; i < this.responses.Length; i++)
            {
                if (this.responses[i].hotkey == key)
                {
                    if (i < actions.Length)
                    {
                        inputHelper.Suppress(key.ToSButton());
                        actions[i]?.Invoke();
                        this.closeDialogue();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Handles a left click.
        /// </summary>
        /// <param name="x">x location clicked.</param>
        /// <param name="y">y location clicked.</param>
        /// <param name="playSound">whether or not to play sounds.</param>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (this.safetyTimer <= 0 && this.selectedResponse >= 0 && this.selectedResponse < actions.Length)
            {
                actions[this.selectedResponse]?.Invoke();
            }
            if (Game1.activeClickableMenu is not null)
            {
                base.receiveLeftClick(x, y, playSound);
            }
        }
    }
}
