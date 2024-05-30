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
        }

        /// <summary>Get the ClickableTextureComponent for the page tab.</summary>
        public override ClickableTextureComponent GetTabComponent()
        {
            Rectangle bounds = new Rectangle(xPositionOnScreen - 64, yPositionOnScreen + 16 + TabID * 64, 64, 64);

            return new ClickableTextureComponent(Name, bounds, "", Title, TabTexture, TabSourceRect, 4f)
            {
                myID = TabComponentID,
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

        /// <summary>Returns true if keyboard input should be ignored by the parent DeluxeJournalMenu.</summary>
        public override bool KeyboardHasFocus()
        {
            return false;
        }

        public override void performHoverAction(int x, int y)
        {
            HoverText = string.Empty;
        }

        protected override void cleanupBeforeExit()
        {
            Game1.exitActiveMenu();
        }

        /// <summary>Snap to the tab ClickableComponent of the active page.</summary>
        public virtual void SnapToActiveTabComponent()
        {
            currentlySnappedComponent = getComponentWithID(TabRegion + DeluxeJournalMenu.ActiveTab);
            snapCursorToCurrentSnappedComponent();
        }

        /// <summary>Set the child menu and snap to the default ClickableComponent.</summary>
        /// <param name="menu">IClickableMenu to be set as the child of this page.</param>
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
    }
}
