/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MailboxMenu
{
    public partial class ModEntry
    {

        [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.mailbox))]
        public class GameLocation_mailbox_Patch
        {
            public static bool Prefix(GameLocation __instance)
            {
                if (!Config.ModEnabled || !Config.MenuOnMailbox || (Config.ModKey != SButton.None && !SHelper.Input.IsDown(Config.ModKey)))
                    return true;
                List<string> list = new List<string>();
                foreach(var str in Game1.mailbox)
                {
                    if (GetMailString(str) == "")
                    {
                        list.Add(str);
                    }
                }
                if(list.Count > 0) 
                {
                    foreach (var str in Game1.mailbox)
                    {
                        if (!list.Contains(str))
                            list.Add(str);
                    }
                    Game1.mailbox.Clear();
                    foreach (var str in list)
                    {
                        Game1.mailbox.Add(str);
                    }
                    return true;
                }
                Game1.activeClickableMenu = new MailMenu();
                return false;
            }
        }
    }
}