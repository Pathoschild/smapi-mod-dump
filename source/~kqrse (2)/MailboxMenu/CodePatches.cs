/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using StardewValley.Tools;

namespace MailboxMenu
{
    public partial class ModEntry
    {
        public class GameLocation_mailbox_Patch
        {
            private static bool UsingMailServices() {
                if (!isMailServicesActive) return false;

                return Game1.player.ActiveObject != null ||
                       Game1.player.CurrentTool is Axe ||
                       Game1.player.CurrentTool is Pickaxe ||
                       Game1.player.CurrentTool is Hoe ||
                       Game1.player.CurrentTool is WateringCan;
            }            
            
            public static bool Prefix(GameLocation __instance)
            {
                if (!Config.ModEnabled || 
                    !Config.MenuOnMailbox ||
                    (Config.ModKey != SButton.None && !SHelper.Input.IsDown(Config.ModKey)) || 
                    UsingMailServices()) 
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