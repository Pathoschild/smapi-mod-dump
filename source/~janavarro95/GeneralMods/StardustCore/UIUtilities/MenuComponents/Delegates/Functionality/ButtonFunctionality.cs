/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

namespace Omegasis.StardustCore.UIUtilities.MenuComponents.Delegates.Functionality
{
    /// <summary>Holds all of the functionality for handeling what happens with button interactivity.</summary>
    public class ButtonFunctionality
    {
        /// <summary>Handles functions that run when a button is left clicked.</summary>
        public DelegatePairing leftClick;

        /// <summary>Handles functions that run when a button is right clicked.</summary>
        public DelegatePairing rightClick;

        /// <summary>Handles functions that run when a button is being hovered over.</summary>
        public DelegatePairing hover;

        /// <summary>Construct an instance.</summary>
        /// <param name="LeftClick">Functionality that occurs when a button is left clicked.</param>
        /// <param name="RightClick">Functionality that occurs when a button is right clicked.</param>
        /// <param name="OnHover">Functionalit that occurs when a button is hovered over.</param>
        public ButtonFunctionality(DelegatePairing LeftClick, DelegatePairing RightClick, DelegatePairing OnHover)
        {
            this.leftClick = LeftClick;
            this.rightClick = RightClick;
            this.hover = OnHover;
        }
    }
}
