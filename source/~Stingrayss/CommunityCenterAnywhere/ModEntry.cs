/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Stingrayss/StardewValley
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace CommunityCenterAnywhere;

public class ModEntry : Mod
{
    //dictionary structure:
    //{0: {0: false}, {1: false}, {2: false}, {3: false},
    // 1: {0: false}, {2: false}, {4: false}, ...}
    private Dictionary<int, MenuBundles> menuBundles = new();
    public override void Entry(IModHelper helper)
    {
        helper.Events.Display.MenuChanged += this.OnMenuChanged;
        helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
    }

    private void OnMenuChanged(object sender, MenuChangedEventArgs e)
    {
        if (!Context.IsWorldReady || e.NewMenu is not JunimoNoteMenu menu)
            return;

        var menuBundle = new MenuBundles(menu);
        menuBundles[menu.whichArea] = menuBundle;

    }

    private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
    {
        //Need to be in the Community Center menu
        if (!Context.IsWorldReady || Game1.activeClickableMenu is not JunimoNoteMenu)
            return;
        
        var menu = Game1.activeClickableMenu as JunimoNoteMenu;
        
        //A key is not immediately found, I think because the OnUpdateTicked is faster than OnMenuChanged by 1 tick
        if (!menuBundles.ContainsKey(menu.whichArea)) return;

        var currentBundle = menu.currentPageBundle;
        var menuBundle = menuBundles[menu.whichArea];
        
        //If deposits are not allowed, we don't need to bother updating anything,
        //since the player can't interact with the bundle
        //this prevents "infinite" loops in the update functions below
        if (!menuBundle.depositsAllowed || currentBundle == null) return;

        //remove bundle from menuBundles if completed
        menuBundle.OnBundleComplete(currentBundle);
        
        //update deposit status if only 1 item is needed to complete a menu bundle
        menuBundle.OnMenuBundleAlmostComplete(menu);
        
        //Allow deposits for the current bundle
        menuBundle.AllowDeposits(menu, currentBundle);
        
    }
}