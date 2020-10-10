/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/cropbeasts
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace Cropbeasts
{
	internal static class ConsoleCommands
	{
		private static IModHelper Helper => ModEntry.Instance.Helper;
		private static IMonitor Monitor => ModEntry.Instance.Monitor;

		public static void Initialize ()
		{
			Helper.ConsoleCommands.Add ("spawn_cropbeast",
				"Spawns cropbeast(s) from crop(s) in the current location.\n\nUsage: spawn_cropbeast [B:force] [I:count] [S:filter]\n- force: whether to ignore the usual preconditions (default true)\n- count: how many cropbeasts to spawn (default 1).\n- filter: fuzzy-matched name of a cropbeast to spawn or crop to spawn it from (default any)",
				cmdSpawnCropbeast);
#if DEBUG
			Helper.ConsoleCommands.Add ("fake_cropbeast",
				"Spawns a cropbeast for the given crop without affecting the crops in the current location.\n\nUsage: fake_cropbeast [I:count] <S:harvest> [B:giant]\n- count: how many cropbeasts to spawn (default 1).\n- harvest: the fuzzy-matched name of the harvest to create a cropbeast for\n- giant: whether to treat the crop as giant (default false)",
				cmdFakeCropbeast);
#endif
			Helper.ConsoleCommands.Add ("revert_cropbeasts",
				"Reverts all active cropbeasts to the harvestable crops they came from.",
				cmdRevertCropbeasts);
			Helper.ConsoleCommands.Add ("farm_monsters",
				"Gets or sets whether monsters should spawn on the farm.\n\nUsage: farm_monsters [B:value]\n- value: whether to enable (true) or disable (false); if omitted, shows the current setting.",
				cmdFarmMonsters);
		}

		private static void cmdSpawnCropbeast (string _command, string[] args)
		{
			Queue<string> argq = new Queue<string> (args);
			bool force = true;
			uint count = 1u;
			string filter = null;

			if (argq.Count > 0 && Boolean.TryParse (argq.Peek (), out bool Force))
			{
				force = Force;
				argq.Dequeue ();
			}

			if (argq.Count > 0 && UInt32.TryParse (argq.Peek (), out uint Count))
			{
				count = Count;
				argq.Dequeue ();
				if (count > 100u)
				{
					count = 100u;
					Monitor.Log ("Limiting spawn count to 100 for stability.",
						LogLevel.Warn);
				}
			}

			if (argq.Count > 0)
				filter = argq.Dequeue ();

			try
			{
				ModEntry.Instance.cleanUpMonsters ();

				for (uint i = 0; i < count; ++i)
				{
					if (!ModEntry.Instance.spawnCropbeast (console: true, force, filter))
						break;
				}
			}
			catch (Exception e)
			{
				Monitor.Log ($"spawn_cropbeast failed: {e.Message}", LogLevel.Error);
#if DEBUG
				Monitor.Log (e.StackTrace, LogLevel.Trace);
#endif
			}
		}

#if DEBUG
		private static void cmdFakeCropbeast (string _command, string[] args)
		{
			Queue<string> argq = new Queue<string> (args);
			uint count = 1u;

			if (argq.Count > 0 && UInt32.TryParse (argq.Peek (), out uint Count))
			{
				count = Count;
				argq.Dequeue ();
				if (count > 100u)
				{
					count = 100u;
					Monitor.Log ("Limiting spawn count to 100 for stability.",
						LogLevel.Warn);
				}
			}

			if (argq.Count == 0)
			{
				Monitor.Log ($"Must specify a harvest for fake_cropbeast.", LogLevel.Error);
				return;
			}
			string harvestName = argq.Dequeue ();

			bool giant = false;
			if (argq.Count > 0)
				Boolean.TryParse (argq.Dequeue (), out giant);

			try
			{
				Faker.fake (harvestName, giant, count);
			}
			catch (Exception e)
			{
				Monitor.Log ($"fake_cropbeast failed: {e.Message}", LogLevel.Error);
				Monitor.Log (e.StackTrace, LogLevel.Trace);
			}
		}
#endif

		private static void cmdRevertCropbeasts (string _command, string[] _args)
		{
			try
			{
				ModEntry.Instance.revertCropbeasts (console: true);
			}
			catch (Exception e)
			{
				Monitor.Log ($"revert_cropbeasts failed: {e.Message}", LogLevel.Error);
#if DEBUG
				Monitor.Log (e.StackTrace, LogLevel.Trace);
#endif
			}
		}

		private static void cmdFarmMonsters (string _command, string[] args)
		{
			if (!Context.IsWorldReady)
			{
				Monitor.Log ($"farm_monsters failed: The world is not ready.", LogLevel.Error);
				return;
			}

			if (args.Length == 0)
			{
				string status = Game1.spawnMonstersAtNight ? "activated" : "deactivated";
				Monitor.Log ($"Farm monsters are currently {status}.", LogLevel.Info);
				return;
			}

			if (!Context.IsMainPlayer)
			{
				Monitor.Log ($"farm_monsters failed: Only the host can do that.", LogLevel.Error);
				return;
			}

			string arg = args[0].ToLower ();
			switch (arg)
			{
			case "true":
			case "on":
				Game1.spawnMonstersAtNight = true;
				Monitor.Log ("Farm monsters are now activated. You owe the Dark Shrine of Night Terrors a Strange Bun.", LogLevel.Info);
				break;
			case "false":
			case "off":
				Game1.spawnMonstersAtNight = false;
				Monitor.Log ("Farm monsters are now deactivated. You owe the Dark Shrine of Night Terrors a Strange Bun.", LogLevel.Info);
				break;
			default:
				Monitor.Log ($"farm_monsters: Invalid value '{arg}', must be 'true'/'on' or 'false'/'off'.", LogLevel.Error);
				break;
			}
		}
	}
}