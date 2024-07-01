/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/focustense/StardewRadialMenu
**
*************************************************/

using GenericModConfigMenu.Framework;
using StardewValley.Menus;
using StardewValley;
using System.Diagnostics.CodeAnalysis;
using GenericModConfigMenu.Framework.ModOption;
using SpaceShared.UI;

namespace RadialMenu.Gmcm;

// Collection of scary, fragile methods that depend on GMCM/SpaceCore internals and are at severe
// risk of breakage due to mod updates, but that we nevertheless can't live without.
internal static class UiInternals
{
    public static BaseModOption? ForceResetOption(this ModConfigPage page, string fieldId)
    {
        var option = page.Options.FirstOrDefault(opt => opt.FieldId == fieldId);
        option?.AfterReset();
        return option;
    }

    public static void ForceSaveOption(this ModConfigPage page, string fieldId)
    {
        page.Options
            .FirstOrDefault(opt => opt.FieldId == fieldId)?
            .BeforeSave();
    }
    public static void ForceUpdateElement<T>(
        this SpecificModConfigMenu menu, int childIndex, Action<T> update)
        where T : Element
    {
        ForceUpdateElement(menu.Table, childIndex, update);
    }

    public static void ForceUpdateElement<T>(this Table table, int childIndex, Action<T> update)
        where T : Element
    {
        var element = table.Children.Length > childIndex ? table.Children[childIndex] : null;
        if (element is HiddenElement hiddenElement)
        {
            element = hiddenElement.OriginalElement;
        }
        if (element is T typedElement)
        {
            update(typedElement);
        }
    }

    // Hiding rows is already a hack, but simply making them zero height doesn't actually fully hide
    // them, because the Table class adds row padding regardless of whether or not a row is visible
    // or has any height; so having 3 empty rows will add a very noticeable 48 px of blank space.
    //
    // This method will remove the rows entirely from the table - which does _not_ remove the
    // elements from the Table's Children.
    //
    // Rows are simply arrays, so it advisable for callers to save the rows before calling this for
    // the first time, in order to be able to "unhide" the rows later. Otherwise the rows will be
    // permanently gone with no way to get them back until the game is restarted.
    public static void RemoveHiddenRows(this Table table)
    {
        for (int i = table.RowCount - 1; i >= 0; i--)
        {
            if (table.Rows[i].All(e => e is HiddenElement))
            {
                table.Rows.RemoveAt(i);
            }
        }
    }

    // Elements can't really been hidden in the GMCM/SpaceCore UI - there is a "ForceHide" property
    // on the Option, but it doesn't do anything close to what we actually want; it just prevents
    // them from actually drawing, without removing them from the flow.
    //
    // Unfortunately, Element height is read-only, determined entirely by the Element subclass.
    // Fortunately, Element references themselves are not read-only in the Table's Children or Rows.
    // Enter our hack: replacing all hidden Elements with one of our own, a "HiddenElement" which
    // has zero width and zero height, and keeps a reference to the original Element so that we can
    // un-hide it later.
    //
    // This method is a little fragile because it needs to keep the Table Rows and Table Children in
    // sync. If they get out of sync - for example, if a child is no longer present in any of the
    // rows, perhaps due to a `RemoveHiddenRows` call, or the caller restoring an old/invalid list
    // of rows, then it will crash.
    //
    // We could of course avoid the crash by simply ignoring failed row lookups, but that would lead
    // to even more mysterious and hard-to-debug issues later on. Fail-fast is preferred here.
    public static bool SetHidden(this Table table, int fromIndex, int count, bool hidden)
    {
        bool anyChanged = false;
        var currentRowIndex = table.Rows.FindIndex(row => row.Contains(table.Children[fromIndex]));
        for (int i = 0; i < count; i++)
        {
            int childIndex = fromIndex + i;
            var element = table.Children[childIndex];
            var row = table.Rows[currentRowIndex];
            var indexInRow = Array.IndexOf(row, element);
            if (indexInRow == -1)
            {
                row = table.Rows[++currentRowIndex];
                indexInRow = 0;
            }
            if (hidden && element is not HiddenElement)
            {
                row[indexInRow] = table.ChildrenImpl[childIndex] = new HiddenElement(element);
                anyChanged = true;
            }
            else if (!hidden && element is HiddenElement hiddenElement)
            {
                row[indexInRow] = table.ChildrenImpl[childIndex] = hiddenElement.OriginalElement;
                anyChanged = true;
            }
        }
        return anyChanged;
    }

    public static bool TryGetModConfigMenu([MaybeNullWhen(false)] out SpecificModConfigMenu menu)
    {
        // GMCM uses this to get the SpecificModConfigMenu, which is where the options are stored.
        var activeMenu = Game1.activeClickableMenu is TitleMenu
            ? TitleMenu.subMenu
            : Game1.activeClickableMenu;
        if (activeMenu is SpecificModConfigMenu configMenu)
        {
            menu = configMenu;
            return true;
        }
        menu = null;
        return false;
    }

    public static bool TryGetModConfigPage(
        [MaybeNullWhen(false)] out ModConfigPage page,
        string? expectedPageId = null)
    {
        if (TryGetModConfigMenu(out var menu))
        {
            page = menu.ModConfig.ActiveDisplayPage;
            return expectedPageId is null || page.PageId == expectedPageId;
        }
        page = null;
        return false;
    }

    public static bool TryGetModConfigMenuAndPage(
        [MaybeNullWhen(false)] out SpecificModConfigMenu menu,
        [MaybeNullWhen(false)] out ModConfigPage page,
        string? expectedPageId = null)
    {
        menu = null;
        page = null;
        
        var activeMenu = Game1.activeClickableMenu is TitleMenu
            ? TitleMenu.subMenu
            : Game1.activeClickableMenu;
        if (activeMenu is not SpecificModConfigMenu configMenu)
        {
            return false;
        }
        menu = configMenu;
        page = menu.ModConfig.ActiveDisplayPage;
        return expectedPageId is null || page.PageId == expectedPageId;
    }
}
