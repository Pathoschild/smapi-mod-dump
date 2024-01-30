/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using MultiplayerMod.Framework.Patch;
using MultiplayerMod.Framework;
using System.Reflection;
using System;
using MultiplayerMod.Framework.Command;
using StardewValley.Menus;
using MultiplayerMod.Framework.Mobile.Menus;
using StardewValley.Monsters;
using System.IO;
using StardewValley;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Network;
using MultiplayerMod.Framework.Network;
using MultiplayerMod.Framework.Mobile;
using System.Threading;
using Netcode;

namespace MultiplayerMod
{
    public class ModEntry : Mod
    {
        internal Config config;
        internal PatchManager PatchManager { get; set; }
        internal static IModHelper ModHelper;
        internal static IMonitor ModMonitor;
        internal CommandManager CommandManager { get; set; }
        internal static PropertyInfo tapToMoveProperty;
        public override void Entry(IModHelper helper)
        {             
            ModUtilities.Helper = helper;
            ModUtilities.ModMonitor = Monitor;
            config = helper.ReadConfig<Config>();
            ModUtilities.ModConfig = config;
            ModHelper = helper;
            ModMonitor = Monitor;
            PatchManager = new PatchManager(Helper, ModManifest, config);
            PatchManager.Apply();
            CommandManager = new CommandManager();
            CommandManager.Apply(helper);
            if (ModUtilities.IsAndroid)
            {
                tapToMoveProperty = typeof(GameLocation).GetProperty("tapToMove");
            }
            //ApplyDebug();
        }

        


        void OnSaveLoaded(object sender, SaveLoadedEventArgs eventArgs)
        {
            if (Game1.player.slotCanHost)
            {
                if (Game1.server != null)
                {
                    List<Server> servers = ModUtilities.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                    foreach (Server server in servers)
                    {
                        if (server != null)
                        {
                            if (!server.connected())
                            {
                                server.initialize();
                            }
                        }
                    }
                }

            }
        }

        void OnMenuChanged(object sender, MenuChangedEventArgs eventArgs)
        {
            if (eventArgs.NewMenu != null)
            {
                if (eventArgs.NewMenu is TitleMenu)
                {
                    Game1.client?.disconnect(false);
                    Game1.server?.stopServer();
                }
            }
        }


        /// <summary>
        /// This is for debugging, never mind it ...
        /// </summary>
        void ApplyDebug()
        {
#if DEBUG

            this.Helper.Events.Display.MenuChanged += (o, e) => 
            {
                if (e.NewMenu == null) return;
                Monitor.Log($"MENU CONTEXT " + e.NewMenu.ToString(), LogLevel.Info);
            };


            new Thread(
                () =>
                {
                    while (true)
                    {
                        if (Game1.activeClickableMenu != null)
                        {
                            Monitor.Log($"MENU CONTEXT " + Game1.activeClickableMenu.ToString(), LogLevel.Info);
                        }

                        Monitor.Log($"GAME CONTEXT: " + Game1.gameMode, LogLevel.Info);
                        if(Game1.IsMultiplayer && Game1.newDaySync != null)
                        {
                            var var_ = Helper.Reflection.GetField<Dictionary<string, INetObject<INetSerializable>>>(Game1.newDaySync, "variables").GetValue();
                            foreach (var item in var_.Keys)
                            {
                                Monitor.Log($"GAME VAR WAIT: " + item, LogLevel.Info);
                            }
                        }
                        Thread.Sleep(1000);
                    }
                }
                ) { IsBackground = true }.Start();
#endif
        }

    }
}
