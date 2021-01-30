/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Windmill-City/InputFix
**
*************************************************/

using Harmony;
using InputFix.Properties;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Threading;

namespace InputFix
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        public static IMonitor monitor;
        public static IModHelper _helper;
        public static NotifyHelper notifyHelper;

        /*********
        ** Public methods
        *********/

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            monitor = Monitor;
            _helper = helper;
            notifyHelper = new NotifyHelper(monitor, _helper);

            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            {
                ModEntry.notifyHelper.Notify(Resources.WARN_STA_LAUNCHER, NotifyPlace.Monitor, NotifyMoment.GameLaunched, LogLevel.Warn);
                ModEntry.notifyHelper.Notify(Resources.WARN_STA_LAUNCHER, NotifyPlace.GameHUD, NotifyMoment.SaveLoaded);
            }

            KeyboardInput_.Initialize(Game1.game1.Window);

            RegCommand(helper);
            HarmonyInstance harmony = HarmonyInstance.Create(ModManifest.UniqueID);
            harmony.PatchAll();

            //compatible with ChatCommands
            if (Helper.ModRegistry.Get("cat.chatcommands") != null)
            {
                notifyHelper.NotifyMonitor("Compatible with ChatCommands");
                Compatibility.PatchChatCommands(monitor, harmony);
            }
        }

        private void RegCommand(IModHelper helper)
        {
            helper.ConsoleCommands.Add("animal_textbox", "Open animal naming textbox", new Action<string, string[]>((res1, res2) =>
            {
                if (!Game1.debugMode)
                    return;
                if (Game1.gameMode != 3)
                {
                    monitor.Log("Not In Playing Mode", LogLevel.Error);
                    return;
                }
                FarmAnimal animal = new FarmAnimal();
                Game1.activeClickableMenu = new AnimalQueryMenu(animal);
                monitor.Log("Open Succeed", LogLevel.Info);
            }));
            helper.ConsoleCommands.Add("naming_textbox", "Open normal naming textbox", new Action<string, string[]>((res1, res2) =>
            {
                if (!Game1.debugMode)
                    return;
                if (Game1.gameMode != 3)
                {
                    monitor.Log("Not In Playing Mode", LogLevel.Error);
                    return;
                }
                if (res2.Length > 0)
                    Game1.activeClickableMenu = new NamingMenu(new NamingMenu.doneNamingBehavior(new Action<string>((name) =>
                    {
                        monitor.Log(name, LogLevel.Info);
                        Game1.activeClickableMenu = null;
                    })),
                        Game1.content.LoadString("Strings\\Characters:NameYourHorse"), Game1.content.LoadString("Strings\\Characters:DefaultHorseName"));
                else
                    Game1.activeClickableMenu = new NamingMenu(new NamingMenu.doneNamingBehavior(new Action<string>((name) =>
                    {
                        monitor.Log(name, LogLevel.Info);
                        Game1.activeClickableMenu = null;
                    })), Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1236"), Game1.player.IsMale ? (Game1.player.catPerson ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1794") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1795")) : (Game1.player.catPerson ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1796") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1797")));
                monitor.Log("Open Succeed", LogLevel.Info);
            }));
            helper.ConsoleCommands.Add("numsel_textbox", "Open number selection textbox", new Action<string, string[]>((res1, res2) =>
            {
                if (!Game1.debugMode)
                    return;
                if (Game1.gameMode != 3)
                {
                    monitor.Log("Not In Playing Mode", LogLevel.Error);
                    return;
                }
                Game1.activeClickableMenu = new NumberSelectionMenu(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1774"),
                    new NumberSelectionMenu.behaviorOnNumberSelect(new Action<int, int, Farmer>((var1, var2, var3) =>
                    {
                        monitor.Log(var1.ToString(), LogLevel.Info);
                    })), 50, 0, 999, 0);
                monitor.Log("Open Succeed", LogLevel.Info);
            }));
            helper.ConsoleCommands.Add("textbox", "Open textbox", new Action<string, string[]>((res1, res2) =>
            {
                if (!Game1.debugMode)
                    return;
                if (Game1.gameMode != 3)
                {
                    monitor.Log("Not In Playing Mode", LogLevel.Error);
                    return;
                }
                Game1.game1.parseDebugInput("warp AnimalShop");
                Game1.activeClickableMenu = new PurchaseAnimalsMenu(Utility.getPurchaseAnimalStock());
                monitor.Log("Open Succeed", LogLevel.Info);
            }));
            helper.ConsoleCommands.Add("inputfix_debug", "Set debug", new Action<string, string[]>((res1, res2) =>
            {
                Game1.debugMode = !Game1.debugMode;
                Program.releaseBuild = !Game1.debugMode;
                string str = string.Format("Debug:{0}", Game1.debugMode);
                monitor.Log(str, LogLevel.Info);
            }));
        }
    }
}