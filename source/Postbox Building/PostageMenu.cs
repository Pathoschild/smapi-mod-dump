/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/i-saac-b/PostBoxMod
**
*************************************************/

using System.Collections.Generic;
using StardewValley.Menus;
using StardewValley;
using StardewModdingAPI;

namespace PostBoxMod
{
    // SocialMenu may end up being a better option for this -- might do a lot of the logic for me.
    class PostageMenu : ChooseFromListMenu
    {
        private static IModHelper Helper;

        private static IMonitor Monitor;

        public static string currentSelection { get; set; }

        public static void Initialize(IMonitor monitor, IModHelper helper)
        {
            Monitor = monitor;
            Helper = helper;
        }
        public PostageMenu(List<string> options, actionOnChoosingListOption chooseAction) : base(options, chooseAction)
        {
        }
        public static void prepGift(string name)
        {
            Game1.showGlobalMessage($"{Helper.Translation.Get("postageMenu-shippingTo")} {name}");
            Postbox.target = name;
            Game1.activeClickableMenu.exitThisMenu();
        }
    }
}
