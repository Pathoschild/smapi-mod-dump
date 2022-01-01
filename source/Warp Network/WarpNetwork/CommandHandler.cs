/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/WarpNetwork
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarpNetwork.models;

namespace WarpNetwork
{
    class CommandHandler
    {

        private static readonly Dictionary<string, string> CmdDescs = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            {"help", "Displays a list of commands and their descriptions. Type 'warpnet help <subcommand>' to view help for a specific command."},
            {"tp", "Teleport to a registered destination."},
            {"destinations", "Lists registered warp destinations."},
            {"items", "Shows registered warp items."},
            {"json", "Prints the player's current position & location as a destination json."},
            {"held_id", "Prints the ID of the currently held item."},
            {"menu", "Activates the warp menu in-game."},
            {"debug", "Outputs diagnostic information."}
        };
        private static readonly Dictionary<string, string> CmdHelp = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            {"help", "Type 'warpnet help <subcommand>' to view help for a specific command, or leave empty to show a list of commands."},
            {"tp", "Takes one argument, the id of the warp location. Teleports you to that destination."},
            {"destinations", "Lists registered warp destinations."},
            {"items", "Shows registered warp items."},
            {"json", "Prints the player's current position & location as a destination json."},
            {"held_id", "Prints the ID of the currently held item."},
            {"menu", "Takes one optional argument, the name of the location to warp from. Activates the warp menu in-game."},
            {"debug", "Outputs diagnostic information."}
        };
        private static readonly Dictionary<string, Action<string[]>> Cmds = new Dictionary<string, Action<string[]>>(StringComparer.InvariantCultureIgnoreCase)
        {
            {"help", ShowHelp},
            {"tp", TP},
            {"destinations", GetLocations},
            {"items", GetItems},
            {"json", CopyJson},
            {"held_id", GetHeldID},
            {"menu", WarpMenu},
            {"debug", PrintDebug}
        };
        public static void Main(string cmd, string[] args)
        {
            if (args.Length == 0)
            {
                ShowHelp(args);
                return;
            }
            if (Cmds.ContainsKey(args[0]))
            {
                Cmds[args[0]](args.Skip(1).ToArray());
            }
            else
            {
                print("\nCommand not recognized.\n");
            }
        }
        private static void print(object what)
        {
            ModEntry.monitor.Log(what.ToString(), LogLevel.Debug);
        }
        private static void ShowHelp(string[] args)
        {
            if (args.Length > 0)
            {
                if (CmdHelp.ContainsKey(args[0]))
                {
                    print(args[0] + ":\n\t" + CmdHelp[args[0]]);
                    return;
                }
            }
            StringBuilder builder = new StringBuilder(4 * CmdHelp.Count);
            foreach (string key in CmdHelp.Keys)
            {
                builder.AppendLine();
                builder.Append(key);
                builder.Append(":\n\t");
                builder.AppendLine(CmdDescs[key]);
            }
            print(builder.ToString());
        }
        private static void TP(string[] args)
        {
            if (Game1.currentLocation is null || Game1.player is null)
            {
                print("\nGame not loaded, cannot warp!\n");
                return;
            }
            if (args.Length == 0)
            {
                print("\nMust specify warp network location\n");
                return;
            }
            WarpHandler.DirectWarp(args[0], true);
        }
        private static void GetLocations(string[] args)
        {
            Dictionary<string, WarpLocation> dict = Utils.GetWarpLocations();
            StringBuilder builder = new StringBuilder(16 * dict.Count);
            foreach ((string key, WarpLocation loc) in dict)
            {
                builder.AppendLine();
                builder.Append(key).AppendLine(":");
                builder.Append("\tLocation: ").Append(loc.Location);
                builder.Append(", Enabled: ").Append(loc.Enabled);
                builder.Append(", X: ").Append(loc.X);
                builder.Append(", Y: ").Append(loc.Y);
                builder.Append(", Label: '").Append(loc.Label).AppendLine("'");
                builder.Append(", Is Custom Handler: ").Append(loc is CustomWarpLocation ? "Yes" : "No");
            }
            print(builder.ToString());
        }
        private static void GetItems(string[] args)
        {
            Dictionary<string, WarpItem> dict = Utils.GetWarpItems();
            StringBuilder builder = new StringBuilder(10 * dict.Count);
            foreach (string key in dict.Keys)
            {
                WarpItem item = dict[key];
                builder.AppendLine();
                builder.Append(key).AppendLine(": ");
                builder.Append("\tDestination: ").Append(item.Destination);
                builder.Append(", IgnoreDisabled: ").Append(item.IgnoreDisabled);
                builder.Append(", Color: '").Append(item.Color).AppendLine("'");
            }
            print(builder.ToString());
        }
        private static void CopyJson(string[] args)
        {
            GameLocation loc = Game1.currentLocation;
            Farmer who = Game1.player;
            if (loc is null || who is null)
            {
                print("\nPlayer not loaded!\n");
                return;
            }
            StringBuilder builder = new StringBuilder(13);
            builder.AppendLine("\"warpid\": {");
            builder.Append("\t\"Location\": \"").Append(loc.Name).AppendLine("\",");
            builder.Append("\t\"X\": ").Append(who.getTileX()).AppendLine(",");
            builder.Append("\t\"Y\": ").Append(who.getTileY()).AppendLine(",");
            builder.AppendLine("\t\"Enabled\": true,");
            builder.AppendLine("\t\"Label\": \"label\"");
            builder.Append("}");
            if (DesktopClipboard.IsAvailable)
            {
                DesktopClipboard.SetText(builder.ToString());
                print("\nJSON copied to clipboard!\n");
            }
            else
            {
                print("\nClipboard not available, printing to console!\n");
                print(builder.ToString());
            }
        }
        private static void GetHeldID(string[] args)
        {
            Farmer who = Game1.player;
            if (who is null)
            {
                print("\nPlayer not loaded!\n");
                return;
            }
            if (who.ActiveObject is null)
            {
                print("\nHand is empty!\n");
                return;
            }
            print("\nHeld item ID: " + who.ActiveObject.ParentSheetIndex.ToString() + "\n");
        }
        private static void WarpMenu(string[] args)
        {
            if (Game1.player is null || Game1.currentLocation is null)
            {
                print("\nGame not loaded, cannot warp\n");
            }
            else
            {
                WarpHandler.ShowWarpMenu((args.Length > 0) ? args[0] : "");
            }
        }
        private static void PrintDebug(string[] args)
        {
            GetLocations(args);
            GetItems(args);
            GetHeldID(args);
            print(ModEntry.config.AsText());
            StringBuilder sb = new StringBuilder(15);
            sb.AppendLine();
            sb.Append("Location: ").AppendLine(Game1.player.currentLocation.Name);
            sb.Append("Position: ").AppendLine(Game1.player.getTileLocationPoint().ToString());
            sb.Append("DesertWarp: ").AppendLine(WarpHandler.DesertWarp.ToString());
            sb.Append("WarpNetworkEntry: ").AppendLine(Game1.player.currentLocation.getMapProperty("WarpNetworkEntry"));
            sb.Append("Is Multiplayer: ").AppendLine(Game1.IsMultiplayer.ToString());
            sb.Append("Is Host: ").AppendLine(Game1.IsMasterGame.ToString());
            sb.Append("Default Farm Totem Warp: ").AppendLine(Utils.GetActualFarmPoint(48, 7).ToString());
            print(sb.ToString());
        }
    }
}
