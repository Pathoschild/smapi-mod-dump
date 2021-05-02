/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/eastscarpe
**
*************************************************/

using StardewModdingAPI;
using System;
using System.IO;

namespace EastScarpe
{
	public class ModEntry : Mod
	{
		internal static ModEntry Instance { get; private set; }

		public ModData Data { get; private set; }

		public override void Entry (IModHelper helper)
		{
			// Make resources available.
			Instance = this;
			Data = Helper.Data.ReadJsonFile<ModData>
				(Path.Combine ("assets", "data.json")) ?? new ModData ();

			// Add console commands.
			Helper.ConsoleCommands.Add ("reset_es_orchard",
				"Resets the spawned trees in the East Scarpe orchard.",
				cmdResetOrchard);

			// Listen for game events.
			Helper.Events.GameLoop.DayStarted += onDayStarted;
		}

		private void onDayStarted (object _sender, EventArgs _e)
		{
			if (Context.IsMainPlayer)
				ESOrchard.DayUpdate ();
		}

		private void cmdResetOrchard (string _command, string[] _args)
		{
			try
			{
				ESOrchard.Reset ();
			}
			catch (Exception e)
			{
				Monitor.Log ($"reset_es_orchard failed: {e.Message}", LogLevel.Error);
#if DEBUG
				Monitor.Log (e.StackTrace, LogLevel.Trace);
#endif
			}
		}
	}
}
