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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

using static StardewValley.Menus.ClickableComponent;

namespace DeluxeJournal.Menus
{
    /// <summary>The base page that all typical pages should derive from.</summary>
    public class PageBase : IClickableMenu, IPage
    {
        /// <summary>A wrapper for the IClickableMenu.allClickableComponents (or equivalent) field.</summary>
        public List<ClickableComponent> AllClickableComponents => allClickableComponents;

        /// <summary>The page name.</summary>
        public string Name { get; }

        /// <summary>The page title (should be translated for the current locale).</summary>
        public string Title { get; }

        /// <summary>Hover text to be displayed by the parent DeluxeJournalMenu.</summary>
        public string HoverText { get; protected set; }

        /// <summary>Tab ID value assigned by the page manager (this value is set immediately AFTER construction).</summary>
        public int TabID { get; set; }

        /// <summary>Texture for the tab.</summary>
        public Texture2D TabTexture { get; set; }

        /// <summary>Source rect for the tab texture.</summary>
        public Rectangle TabSourceRect { get; set; }

        protected ITranslationHelper Translation { get; }

        public PageBase(string name, string title, int x, int y, int width, int height, Texture2D tabTexture, Rectangle tabSourceRect, ITranslationHelper translation) :
            base(x, y, width, height)
        {
            Name = name;
            Title = title;
            TabTexture = tabTexture;
            TabSourceRect = tabSourceRect;
            Translation = translation;
            HoverText = "";
        }

        /// <summary>Get the ClickableTextureComponent for the page tab.</summary>
        public virtual ClickableTextureComponent GetTabComponent()
        {
            Rectangle bounds = new Rectangle(xPositionOnScreen - 64, yPositionOnScreen + 16 + TabID * 64, 64, 64);

            return new ClickableTextureComponent(Name, bounds, "", Title, TabTexture, TabSourceRect, 4f)
            {
                myID = ((IPage)this).TabComponentID,
                rightNeighborID = SNAP_TO_DEFAULT,
                leftNeighborID = CUSTOM_SNAP_BEHAVIOR,
                fullyImmutable = true
            };
        }

        /// <summary>Called when the page becomes visible and active.</summary>
        public virtual void OnVisible()
        {
        }

        /// <summary>Called when the page is hidden and no longer active.</summary>
        public virtual void OnHidden()
        {
        }

        /// <summary>Returns true if keyboard input should be ignored by the parent DeluxeJournalMenu.</summary>
        public virtual bool KeyboardHasFocus()
        {
            return false;
        }

        /// <summary>Returns true if all input should be ignored by the parent DeluxeJournalMenu.</summary>
        public virtual bool ChildHasFocus()
        {
            return _childMenu != null;
        }

        public override bool shouldDrawCloseButton()
        {
            return _childMenu == null;
        }

        public override void performHoverAction(int x, int y)
        {
            HoverText = "";
        }

        /// <summary>Snap to the tab ClickableComponent of the active page.</summary>
        public void SnapToActiveTabComponent()
        {
            currentlySnappedComponent = getComponentWithID(IPage.TabRegion + DeluxeJournalMenu.ActiveTab);
            snapCursorToCurrentSnappedComponent();
        }

        /// <summary>Set the child menu and snap to the default ClickableComponent.</summary>
        /// <param name="menu">IClickableMenu to be set as the child of this page.</param>
        protected void SetSnappyChildMenu(IClickableMenu menu)
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
