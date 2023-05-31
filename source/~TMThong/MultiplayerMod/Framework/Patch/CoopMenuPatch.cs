/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using HarmonyLib;
using MultiplayerMod.Framework.Network;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LidgrenClient = MultiplayerMod.Framework.Network.LidgrenClient;

namespace MultiplayerMod.Framework.Patch
{
    internal class CoopMenuPatch : IPatch
    {
        private static readonly Type PatchType = Assembly.GetAssembly(typeof(IClickableMenu)).GetType("StardewValley.Menus.CoopMenu");


        public void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(PatchType, "connectionFinished"), postfix: new HarmonyMethod(AccessTools.Method(this.GetType(), nameof(postfix_connectionFinished))));
            harmony.Patch(AccessTools.Method(Assembly.GetAssembly(typeof(IClickableMenu)).GetType("StardewValley.Menus.CoopMenu+LanSlot"), "Activate"), prefix: new HarmonyMethod(this.GetType(), nameof(prefix_Activate)));
        }

        private static bool prefix_Activate(object __instance)
        {

            try
            {
                string message = ModUtilities.Helper.Reflection.GetField<string>(__instance, "message").GetValue();
                if (message == ModUtilities.Helper.Translation.Get("client.join"))
                {
                    object menu = ModUtilities.Helper.Reflection.GetField<object>(__instance, "menu").GetValue();
                    string title = Game1.content.LoadString("Strings\\UI:CoopMenu_EnterIP");
                    TitleTextInputMenu titleTextInputMenu = new TitleTextInputMenu(title, (address) =>
                    {
                        try
                        {
                            StartupPreferences startupPreferences2 = new StartupPreferences();
                            startupPreferences2.loadPreferences(false, false);
                            startupPreferences2.lastEnteredIP = address;
                            startupPreferences2.savePreferences(false, false);
                        }
                        catch (Exception)
                        {
                        }
                        // object farmHandMenu = Assembly.GetAssembly(typeof(IClickableMenu)).GetType("StardewValley.Menus.FarmhandMenu").CreateInstance<object>(new object[] { ModUtilities.multiplayer.InitClient(new ModClient(ModUtilities.ModConfig, address)) });
                        object farmHandMenu = Assembly.GetAssembly(typeof(IClickableMenu)).GetType("StardewValley.Menus.FarmhandMenu").CreateInstance<object>(new object[] { ModUtilities.multiplayer.InitClient(new LidgrenClient(address)) });
                        ModUtilities.Helper.Reflection.GetMethod(menu, "setMenu").MethodInfo.Invoke(menu, new object[] { farmHandMenu });
                        ModUtilities.ModConfig.LastIP = address;
                        ModUtilities.Helper.WriteConfig(ModUtilities.ModConfig);

                    }, ModUtilities.ModConfig.LastIP, "join_menu");
                    ModUtilities.Helper.Reflection.GetMethod(menu, "setMenu").MethodInfo.Invoke(menu, new object[] { titleTextInputMenu });
                    return true;
                }
                else
                {
                    object menu = ModUtilities.Helper.Reflection.GetField<object>(__instance, "menu").GetValue();
                    ModUtilities.Helper.Reflection.GetMethod(menu, "enterIPPressed").Invoke();
                    return true;
                }
            }
            catch (Exception e)
            {
                throw e;
                return false;
            }
            return false;
        }

        private static void postfix_connectionFinished(CoopMenu __instance)
        {
            List<LoadGameMenu.MenuSlot> menuSlots = ModUtilities.Helper.Reflection.GetField<List<LoadGameMenu.MenuSlot>>(__instance, "menuSlots").GetValue();
            object slot = Assembly.GetAssembly(typeof(IClickableMenu)).GetType("StardewValley.Menus.CoopMenu+LanSlot").CreateInstance<object>(new object[] { __instance });
            ModUtilities.Helper.Reflection.GetField<string>(slot, "message").SetValue(ModUtilities.Helper.Translation.Get("client.join"));
            ModUtilities.Helper.Reflection.GetMethod(menuSlots, "Add").Invoke(slot);
            
        }
    }
}
