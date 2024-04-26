/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/thimadera/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley.Menus;

namespace StackEverythingRedux.MenuHandlers
{
    /// <summary>Represents states of input being handled.</summary>
    public enum EInputHandled
    {
        /// <summary>Input should be forwarded and the split menu should be closed.</summary>
        NotHandled,
        /// <summary>Input should be forwarded but the split menu should remain open.</summary>
        Handled,
        /// <summary>Input consumed by the handler, don't propogate it.</summary>
        Consumed
    }

    public interface IMenuHandler
    {
        /// <summary>Checks if the menu this handler wraps is open.</summary>
        /// <returns>True if it is open, false otherwise.</returns>
        bool IsOpen();

        /// <summary>Checks the menu is the correct type.</summary>
        bool IsCorrectMenuType(IClickableMenu menu);

        /// <summary>Notifies the handler that its native menu has been opened.</summary>
        /// <param name="menu">The menu that was opened.</param>
        bool Open(IClickableMenu menu);

        /// <summary>Notifies the handler that its native menu was closed.</summary>
        void Close();

        /// <summary>Runs on tick for handling things like highlighting text.</summary>
        void Update();

        /// <summary>Draws the split menu.</summary>
        void Draw(SpriteBatch spriteBatch);

        /// <summary>Try to perform hover action on split menu.</summary>
        void PerformHoverAction(int x, int y);

        /// <summary>Tells the handler to close the split menu.</summary>
        void CloseSplitMenu();

        /// <summary>Handle user input.</summary>
        /// <param name="button">The pressed button.</param>
        EInputHandled HandleInput(SButton button);
    }
}
