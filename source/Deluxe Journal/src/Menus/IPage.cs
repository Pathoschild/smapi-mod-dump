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
using DeluxeJournal.Framework;

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

        /// <summary>ID value for the tab's <see cref="ClickableComponent"/>.</summary>
        public int TabComponentId => TabId + TabRegion;

        /// <summary>Tab ID value assigned by the <see cref="PageRegistry"/> (this value is set immediately AFTER construction).</summary>
        public int TabId { get; set; }

        /// <summary>Registered page ID value assigned by the <see cref="PageRegistry"/> (this value is set immediately AFTER construction).</summary>
        public string PageId { get; set; } = string.Empty;

        /// <summary>Hover text to be displayed by the parent <see cref="DeluxeJournalMenu"/>.</summary>
        public virtual string HoverText { get; set; }

        /// <summary>Disable clickable elements in the parent <see cref="DeluxeJournalMenu"/>.</summary>
        public virtual bool ParentElementsDisabled => false;

        public IPage(string name, string title, int x, int y, int width, int height, bool showUpperRightCloseButton = false)
            : base(x, y, width, height, showUpperRightCloseButton)
        {
            Name = name;
            Title = title;
            HoverText = string.Empty;
        }

        /// <summary>Get the <see cref="ClickableTextureComponent"/> for the page tab.</summary>
        public abstract ClickableTextureComponent GetTabComponent();

        /// <summary>Called when the page becomes visible and active.</summary>
        public abstract void OnVisible();

        /// <summary>Called when the page is hidden and no longer active.</summary>
        public abstract void OnHidden();

        /// <summary>Returns true if keyboard input should be ignored by the parent <see cref="DeluxeJournalMenu"/>.</summary>
        public abstract bool KeyboardHasFocus();
    }
}
