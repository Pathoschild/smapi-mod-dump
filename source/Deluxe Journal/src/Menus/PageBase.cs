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
using StardewValley;
using StardewValley.Menus;

using static StardewValley.Menus.ClickableComponent;

namespace DeluxeJournal.Menus
{
    /// <summary>The base page that all typical pages derive from.</summary>
    public class PageBase : IPage
    {
        /// <summary>Texture for the tab.</summary>
        public Texture2D TabTexture { get; set; }

        /// <summary>Source rect for the tab texture.</summary>
        public Rectangle TabSourceRect { get; set; }

        public PageBase(string name, string title, int x, int y, int width, int height, Texture2D tabTexture, Rectangle tabSourceRect)
            : base(name, title, x, y, width, height)
        {
            TabTexture = tabTexture;
            TabSourceRect = tabSourceRect;
            exitFunction = Game1.exitActiveMenu;
        }

        /// <summary>Get the <see cref="ClickableTextureComponent"/> for the page tab.</summary>
        public override ClickableTextureComponent GetTabComponent()
        {
            Rectangle bounds = new Rectangle(xPositionOnScreen - 64, yPositionOnScreen + 16 + TabId * 64, 64, 64);

            return new ClickableTextureComponent(Name, bounds, "", Title, TabTexture, TabSourceRect, 4f)
            {
                myID = TabComponentId,
                rightNeighborID = SNAP_TO_DEFAULT,
                leftNeighborID = CUSTOM_SNAP_BEHAVIOR,
                fullyImmutable = true
            };
        }

        /// <summary>Called when the page becomes visible and active.</summary>
        public override void OnVisible()
        {
        }

        /// <summary>Called when the page is hidden and no longer active.</summary>
        public override void OnHidden()
        {
        }

        /// <summary>Returns true if keyboard input should be ignored by the parent <see cref="DeluxeJournalMenu"/>.</summary>
        public override bool KeyboardHasFocus()
        {
            return false;
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
        }

        public override void performHoverAction(int x, int y)
        {
            HoverText = string.Empty;
        }

        /// <summary>Snap to the tab <see cref="ClickableComponent"/> of the active page.</summary>
        public virtual void SnapToActiveTabComponent()
        {
            currentlySnappedComponent = getComponentWithID(TabRegion + DeluxeJournalMenu.ActiveTab);
            snapCursorToCurrentSnappedComponent();
        }

        /// <summary>Set the child menu and snap to the default <see cref="ClickableComponent"/>.</summary>
        /// <param name="menu"><see cref="IClickableMenu"/> to be set as the child of this page.</param>
        protected void SetSnappyChildMenu(IClickableMenu? menu)
        {
            SetChildMenu(menu);

            if (_childMenu != null)
            {
                if (_childMenu.allClickableComponents == null)
                {
                    _childMenu.populateClickableComponentList();
                }

                if (Game1.options.SnappyMenus)
                {
                    _childMenu.snapToDefaultClickableComponent();
                }
            }
        }

        /// <summary>Exit the active <see cref="DeluxeJournalMenu"/>.</summary>
        /// <param name="playSound">Allow sound to be played while exiting.</param>
        protected void ExitJournalMenu(bool playSound = true)
        {
            if (Game1.activeClickableMenu is DeluxeJournalMenu menu)
            {
                menu.exitThisMenu(playSound);
            }
            else
            {
                exitThisMenu(playSound);
            }
        }
    }
}
