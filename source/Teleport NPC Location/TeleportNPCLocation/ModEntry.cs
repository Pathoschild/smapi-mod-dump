/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/chencrstu/TeleportNPCLocation
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Menus;
using TeleportNPCLocation.framework;
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

namespace TeleportNPCLocation
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        /// <summary>The mod configuration.</summary>
        private ModConfig Config = null!; // set in Entry

        // shoud find npc, cli debug info
        private readonly string[] NPCNames = { "Robin", "Shane", "George", "Evelyn", "Alex", "Haley", "Emily", "Jodi", "Vincent", "Sam", "Clint", "Pierre", "Caroline", "Abigail", "Gus", "Willy", "Maru", "Demetrius", "Sebastian", "Linus", "Marnie", "Jas", "Leah", "Dwarf", "Bouncer", "Gunther", "Marlon", "Henchman", "Birdie", "Mister Qi" };

        /// <summary>The previous menus shown before the current npc menu was opened.</summary>
        private readonly PerScreen<Stack<IClickableMenu>> PreviousMenus = new(() => new());

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;

            helper.ConsoleCommands.Add("teleport_start", "start .\n\nUsage: teleport_start <value>\n- value: the npc name in below list.\n" + string.Join("\n", this.NPCNames), this.CommandStartTeleport);
        }

        /*********
        ** Private methods
        *********/

        /// <inheritdoc cref="IGameLoopEvents.GameLaunched"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();

            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );

            // add config
            configMenu.AddKeybindList(
                mod: this.ModManifest,
                name: () => "Toggle npc menu",
                getValue: () => this.Config.ToggleNPCMenu,
                setValue: value => this.Config.ToggleNPCMenu = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Show npc location",
                getValue: () => this.Config.showMoreInfo,
                setValue: value => this.Config.showMoreInfo = value
            );

            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Ban npc list",
                getValue: () => this.Config.banNPCListString,
                setValue: value => this.Config.banNPCListString = value
            );
        }

        /// <summary>Set the player's money when the 'player_setmoney' command is invoked.</summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private void CommandStartTeleport(string command, string[] args)
        {
            string npcName = args.Count() > 0 ? args[0] : "Emily";
            NPC npc = this.fetchNPCInfo(npcName);
            if (npc == null)
            {
                this.Monitor.Log($"can't find npc info name: {npcName}.", LogLevel.Info);
                return;
            }
                
            TeleportHelper.teleportToNPCLocation(npc);
        }

        /// <summary>fetch npc info with name.</summary>
        private NPC fetchNPCInfo(string name)
        {
            List<NPC> villagers = GetVillagers();
            NPC findNPC = null;

            foreach (var npc in villagers)
            {
                if (npc.currentLocation == null)
                    continue;

                string locationName = npc.currentLocation.uniqueName.Value ?? npc.currentLocation.Name;
                GameLocation location = npc.currentLocation;
                if (npc.Name.Equals(name))
                {
                    findNPC = npc;
                    break;
                }
            }
            return findNPC;
        }

        /// <inheritdoc cref="IDisplayEvents.MenuChanged"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            // restore the previous menu if it was hidden to show the npc menu
            if (e.NewMenu == null && (e.OldMenu is NPCMenu) && this.PreviousMenus.Value.Any())
                Game1.activeClickableMenu = this.PreviousMenus.Value.Pop();
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            KeybindList toggleNPCMenu = this.Config.ToggleNPCMenu;
            if (!toggleNPCMenu.JustPressed())
                return;

            // try toggle npc menu list
            TryToggleNPCMenu();

        }

        private static IEnumerable<GameLocation> GetAllStaticLocations()
        {
            return Game1.locations
                .Concat(
                    from location in Game1.locations.OfType<BuildableGameLocation>()
                    from building in location.buildings
                    where building.indoors.Value != null
                    select building.indoors.Value
                );
        }

        /// <summary>Parse ban npc list string.</summary>
        private string[] ParseBanNPCListString()
        {
            return this.Config.banNPCListString.Split('|').Where(s=>!string.IsNullOrEmpty(s)).ToArray();
        }

        private void AppendBanList(List<string> banList)
        {
            if (banList.Any())
            {
                string finals = this.Config.banNPCListString;
                if (finals.Length > 0)
                {
                    finals += "|";
                }
                finals += String.Join("|", banList);
                this.Config.banNPCListString = finals;
                this.Helper.WriteConfig(this.Config);
                this.Monitor.Log($"current ban npc list: {finals}.", LogLevel.Info);
            }
        }

        /// <summary>Get only relevant villagers for the world map.</summary>
        private static List<NPC> GetVillagers()
        {
            var villagers = new List<NPC>();

            foreach (GameLocation location in GetAllStaticLocations())
            {
                foreach (var npc in location.characters)
                {
                    if (npc != null && !villagers.Contains(npc) && npc.isVillager())
                        villagers.Add(npc);
                }
            }

            return villagers;
        }

        /****
        ** NPC menu helpers
        ****/
        /// <summary>Toggle the npc UI if applicable.</summary>
        private void TryToggleNPCMenu()
        {
            if (Game1.activeClickableMenu is NPCMenu)
                this.hideNPCMenu();
            else if (Context.IsWorldReady && Game1.activeClickableMenu is not NPCMenu)
                this.showNPCMenu();
        }

        private void hideNPCMenu()
        {
            this.Monitor.InterceptErrors("closing npc menu", () =>
            {
                if (Game1.activeClickableMenu is NPCMenu menu)
                    menu.QueueExit();
            });
        }

        private void showNPCMenu()
        {
            this.Monitor.InterceptErrors("opening npc menu", () =>
            {
                List<NPC> villagers = GetVillagers().Where(npc=>!this.ParseBanNPCListString().Contains(npc.Name)).ToList();

                this.PushMenu(new NPCMenu(npcList: villagers, monitor: this.Monitor, config:this.Config, scroll: 160, callback:(s)=>this.AppendBanList(s)));

            });
        }

        private void PushMenu(IClickableMenu menu)
        {
            if (this.ShouldRestoreMenu(Game1.activeClickableMenu))
            {
                this.PreviousMenus.Value.Push(Game1.activeClickableMenu);
                this.Helper.Reflection.GetField<IClickableMenu>(typeof(Game1), "_activeClickableMenu").SetValue(menu); // bypass Game1.activeClickableMenu, which disposes the previous menu
            }
            else
                Game1.activeClickableMenu = menu;
        }

        /// <summary>Get whether a given menu should be restored when the npcmenu ends.</summary>
        /// <param name="menu">The menu to check.</param>
        private bool ShouldRestoreMenu(IClickableMenu? menu)
        {
            // no menu
            if (menu == null)
                return false;

            return true;
        }
    }
}

