/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using StardewValley.Menus;

namespace DeluxeJournal.Menus
{
    public abstract class IPage : IClickableMenu
    {
        /// <summary>Reserved starting <see cref="ClickableComponent.myID"/> region for tabs.</summary>
        public const int TabRegion = 9500;

        /// <summary>End of reserved <see cref="ClickableComponent.myID"/> region for tabs.</summary>
        public const int TabRegionEnd = 9599;

        /// <summary>The page name.</summary>
        public string Name { get; }

        /// <summary>The page title (should be translated for the current locale).</summary>
        public string Title { get; }

        /// <summary>ID value for the tab ClickableComponent.</summary>
        public int TabComponentID => TabID + TabRegion;

        /// <summary>Tab ID value assigned by the page manager (this value is set immediately AFTER construction).</summary>
        public int TabID { get; set; }

        /// <summary>Hover text to be displayed by the parent DeluxeJournalMenu.</summary>
        public virtual string HoverText { get; set; }

        public IPage(string name, string title, int x, int y, int width, int height, bool showUpperRightCloseButton = false)
            : base(x, y, width, height, showUpperRightCloseButton)
        {
            Name = name;
            Title = title;
            HoverText = string.Empty;
        }

        /// <summary>Get the ClickableTextureComponent for the page tab.</summary>
        public abstract ClickableTextureComponent GetTabComponent();

        /// <summary>Called when the page becomes visible and active.</summary>
        public abstract void OnVisible();

        /// <summary>Called when the page is hidden and no longer active.</summary>
        public abstract void OnHidden();

        /// <summary>Returns true if keyboard input should be ignored by the parent DeluxeJournalMenu.</summary>
        public abstract bool KeyboardHasFocus();
    }
}
