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

namespace WarpNetwork.framework
{
	class CommandHandler
	{
		private static readonly Dictionary<string, string> CmdDescs = new(StringComparer.InvariantCultureIgnoreCase)
		{
			{"help", "Displays a list of commands and their descriptions. Type 'warpnet help <subcommand>' to view help for a specific command."},
			{"tp", "Teleport to a registered destination."},
			{"destinations", "Lists registered warp destinations."},
			{"items", "Shows registered warp items."},
			{"json", "Prints the player's current position & location as a destination json."},
			{"held_id", "Prints the ID of the currently held item."},
			{"menu", "Activates the warp menu in-game."},
			{"debug", "Outputs diagnostic information."},
			{"objects", "Lists registered warp objects."}
		};
		private static readonly Dictionary<string, string> CmdHelp = new(StringComparer.InvariantCultureIgnoreCase)
		{
			{"help", "Type 'warpnet help <subcommand>' to view help for a specific command, or leave empty to show a list of commands."},
			{"tp", "Takes one argument, the id of the warp location. Teleports you to that destination."},
			{"destinations", "Lists registered warp destinations."},
			{"items", "Shows registered warp items."},
			{"json", "Prints the player's current position & location as a destination json."},
			{"held_id", "Prints the ID of the currently held item."},
			{"menu", "Takes one optional argument, the name of the location to warp from. Activates the warp menu in-game."},
			{"debug", "Outputs diagnostic information."},
			{"objects", "Lists registered warp objects."}
		};
		private static readonly Dictionary<string, Action<string[]>> Cmds = new(StringComparer.InvariantCultureIgnoreCase)
		{
			{"help", ShowHelp},
			{"tp", TP},
			{"items", GetItems},
			{"json", CopyJson},
			{"held_id", GetHeldID},
			{"menu", WarpMenu},
			{"objects", GetObjects}
		};
		public static void Main(string _, string[] args)
		{
			if (args.Length == 0)
				ShowHelp(args);
			else if (Cmds.ContainsKey(args[0]))
				Cmds[args[0]](args.Skip(1).ToArray());
			else
				print("\nCommand not recognized.\n");
		}
		private static void print(object what) => ModEntry.monitor.Log(what.ToString(), LogLevel.Debug);
		private static void ShowHelp(string[] args)
		{
			if (args.Length > 0 && CmdHelp.ContainsKey(args[0]))
			{
				print(args[0] + ":\n\t" + CmdHelp[args[0]]);
				return;
			}
			StringBuilder builder = new();
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
				print("\nGame not loaded, cannot warp!\n");
			else if (args.Length == 0)
				print("\nMust specify warp network location\n");
			else
				WarpHandler.DirectWarp(args[0], true, Game1.currentLocation, Game1.player);
		}
		private static void GetItems(string[] args)
		{
			StringBuilder sb = new();
			foreach ((var k, var v) in Utils.GetWarpItems())
			{
				sb.AppendLine();
				sb.Append(k).AppendLine(": ");
				sb.Append("\tDestination: ").Append(v.Destination);
				sb.Append(", IgnoreDisabled: ").Append(v.IgnoreDisabled);
				sb.Append(", Color: '").Append(v.Color).Append('\'');
				sb.Append(", Consume: ").AppendLine(v.Consume ? "TRUE" : "FALSE");
			}
			print(sb.ToString());
		}
		private static void GetObjects(string[] args)
		{
			StringBuilder sb = new();
			foreach ((var k, var v) in Utils.GetWarpObjects())
			{
				sb.AppendLine();
				sb.Append(k).AppendLine(": ");
				sb.Append("\tDestination: ").Append(v.Destination);
				sb.Append(", IgnoreDisabled: ").Append(v.IgnoreDisabled);
				sb.Append(", Color: '").Append(v.Color).AppendLine("'");
			}
			print(sb.ToString());
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
			StringBuilder builder = new();
			builder.AppendLine("\"warpid\": {");
			builder.Append("\t\"Location\": \"").Append(loc.Name).AppendLine("\",");
			builder.AppendLine("\t\"Position: {");
			builder.Append("\t\t\"X\": ").Append(who.TilePoint.X).AppendLine(",");
			builder.Append("\t\t\"Y\": ").AppendLine(who.TilePoint.Y.ToString());
			builder.AppendLine("\t},");
			builder.AppendLine("\t\"Enabled\": \"TRUE\",");
			builder.AppendLine("\t\"Label\": \"label\"");
			builder.Append('}');
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
			if (who?.ActiveObject is null)
				print("\nHand is empty!\n");
			else
				print("\nHeld item ID: " + who.ActiveObject.ParentSheetIndex.ToString() + "\n");
		}
		private static void WarpMenu(string[] args)
		{
			if (Game1.player is null || Game1.currentLocation is null)
				print("\nGame not loaded, cannot warp\n");
			else
				WarpHandler.ShowWarpMenu(Game1.currentLocation, Game1.player, args.Length > 0 ? args[0] : "");
		}
	}
}
