/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

namespace EventSystem.Framework.FunctionEvents
{
    /// <summary>Used to handle events that happens when a mouse enters/leaves a specified position.</summary>
    public class MouseEntryLeaveEvent
    {
        /// <summary>A function that is called when a mouse enters a certain position.</summary>
        public functionEvent onMouseEnter;

        /// <summary>A function that is called when a mouse leaves a certain position.</summary>
        public functionEvent onMouseLeave;

        /// <summary>Construct an instance.</summary>
        /// <param name="OnMouseEnter">The function that occurs when the mouse enters a certain position.</param>
        /// <param name="OnMouseLeave">The function that occurs when the mouse leaves a certain position.</param>
        public MouseEntryLeaveEvent(functionEvent OnMouseEnter, functionEvent OnMouseLeave)
        {
            this.onMouseEnter = OnMouseEnter;
            this.onMouseLeave = OnMouseLeave;
        }
    }
}
