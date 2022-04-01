/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.Menus;

namespace DeluxeJournal.Menus
{
    public interface IPage
    {
        const int TabRegion = 9500;

        /// <summary>A wrapper for the IClickableMenu.allClickableComponents (or equivalent) field.</summary>
        List<ClickableComponent> AllClickableComponents { get; }

        /// <summary>Hover text to be displayed by the parent DeluxeJournalMenu.</summary>
        string HoverText { get; }

        /// <summary>ID value for the tab ClickableComponent.</summary>
        int TabComponentID => TabID + TabRegion;

        /// <summary>Tab ID value assigned by the page manager (this value is set immediately AFTER construction).</summary>
        int TabID { get; set; }

        /// <summary>Get the ClickableTextureComponent for the page tab.</summary>
        ClickableTextureComponent GetTabComponent();

        /// <summary>Called when the page becomes visible and active.</summary>
        void OnVisible();

        /// <summary>Called when the page is hidden and no longer active.</summary>
        void OnHidden();

        /// <summary>Returns true if keyboard input should be ignored by the parent DeluxeJournalMenu.</summary>
        bool KeyboardHasFocus();

        /// <summary>Returns true if all input should be ignored by the parent DeluxeJournalMenu.</summary>
        bool ChildHasFocus();

        // ---
        // Methods below are provided by StardewValley.Menus.IClickableMenu
        // ---

        void populateClickableComponentList();

        ClickableComponent getCurrentlySnappedComponent();

        void setCurrentlySnappedComponentTo(int id);

        void automaticSnapBehavior(int direction, int oldRegion, int oldID);

        void snapToDefaultClickableComponent();

        void snapCursorToCurrentSnappedComponent();

        void receiveGamePadButton(Buttons b);

        void setUpForGamePadMode();

        void receiveLeftClick(int x, int y, bool playSound = true);

        void leftClickHeld(int x, int y);

        void releaseLeftClick(int x, int y);

        void receiveRightClick(int x, int y, bool playSound = true);

        void receiveScrollWheelAction(int direction);

        void receiveKeyPress(Keys key);

        void performHoverAction(int x, int y);

        bool readyToClose();

        bool shouldDrawCloseButton();

        void update(GameTime time);

        void draw(SpriteBatch b);
    }
}
