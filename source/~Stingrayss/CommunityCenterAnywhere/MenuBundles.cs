/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Stingrayss/StardewValley
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Delegates;
using StardewValley.Menus;

namespace CommunityCenterAnywhere;

public class MenuBundles
{
    public Dictionary<int, bool> incompleteBundles { get; set; }

    public int Count => incompleteBundles.Count;

    public bool depositsAllowed { get; set; } = true;

    public MenuBundles()
    {
        incompleteBundles = new Dictionary<int, bool>();
    }

    public MenuBundles(JunimoNoteMenu menu)
    {
        incompleteBundles = new Dictionary<int, bool>();
        GenerateIncompleteBundles(menu);
    }

    public void GenerateIncompleteBundles(JunimoNoteMenu menu)
    {
        foreach (var bundle in menu.bundles.Where(bundle => !bundle.complete))
        {
            incompleteBundles.Add(bundle.bundleIndex, bundle.complete);
        }
    }
    public void AllowDeposits(JunimoNoteMenu menu, Bundle bundle)
    {
        //have to check again because this can change from OnMenuBundleAlmostComplete
        if (!depositsAllowed) return;
        bundle.depositsAllowed = true;

        //Purchase buttons for the vault bundles needs to be done every tick
        //since only one button can be created at a time in OnMenuChanged.
        //Players otherwise have to hit the forward or back button to refresh the vault bundles page
        //Vault bundles: 2,500g, 5,000g, 10,000g, 25,000g in order
        if (bundle.bundleIndex is 23 or 24 or 25 or 26)
        {
            menu.purchaseButton = new ClickableTextureComponent(
                new Rectangle(menu.xPositionOnScreen + 800, menu.yPositionOnScreen + 504, 260, 72),
                menu.noteTexture, new Rectangle(517, 286, 65, 20), 4f);
        }
    }

    public void OnBundleComplete(Bundle bundle)
    {
        if (!incompleteBundles.ContainsKey(bundle.bundleIndex) || 
            incompleteBundles[bundle.bundleIndex] == bundle.complete) return;

        incompleteBundles.Remove(bundle.bundleIndex);
    }

    public void OnMenuBundleAlmostComplete(JunimoNoteMenu menu)
    {
        //Checking for location prevents the player from being soft locked
        if (Count != 1 || Game1.player.currentLocation.DisplayName == "Community Center"
                       || Game1.player.currentLocation.DisplayName == "Abandoned JojaMart") return;

        var bundleIndex = incompleteBundles.Keys.ElementAt(0);
        var bundle = menu.bundles.First(bundle => bundle.bundleIndex == bundleIndex);
        var completedIngredients = bundle.ingredients.Count(ingredient => ingredient.completed);
        
        if (bundle.numberOfIngredientSlots - completedIngredients != 1) return;

        depositsAllowed = false;
        bundle.depositsAllowed = false;
        
    }
}