/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using StardewModdingAPI.Events;

namespace SkillPrestige.Framework.InputHandling
{
    /// <summary>A component which can handle user input.</summary>
    internal interface IInputHandler
    {
        /*********
        ** Methods
        *********/
        /// <summary>Raised after the player moves the in-game cursor.</summary>
        /// <param name="e">The event data.</param>
        void OnCursorMoved(CursorMovedEventArgs e);

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="e">The event data.</param>
        /// <param name="isClick">Whether the button press is a click.</param>
        void OnButtonPressed(ButtonPressedEventArgs e, bool isClick);
    }
}
