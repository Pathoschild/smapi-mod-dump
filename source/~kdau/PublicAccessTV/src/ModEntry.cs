/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/predictivemods
**
*************************************************/

using PlatoTK.Events;
using PredictiveCore;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;

namespace PublicAccessTV
{
	public class ModEntry : Mod
	{
		internal static ModEntry Instance { get; private set; }

		protected static ModConfig Config => ModConfig.Instance;

		private readonly PerScreen<List<Channel>> channels_ = new (() => new ());
		internal List<Channel> channels => channels_.Value;

		public override void Entry (IModHelper helper)
		{
			// Make resources available.
			Instance = this;
			ModConfig.Load ();

			// Set up PredictiveCore.
			Utilities.Initialize (this, () => ModConfig.Instance);

			// Set up asset editors.
			Helper.Content.AssetEditors.Add (new DialogueEditor ());
			Helper.Content.AssetEditors.Add (new EventsEditor ());
			Helper.Content.AssetEditors.Add (new MailEditor ());

			// Add console commands.
			Helper.ConsoleCommands.Add ("reset_patv_channels",
				"Resets the custom channels to their unlaunched states (before letters, events, etc.).",
				(_command, _args) => resetChannels (true));

			// Listen for game events.
			Helper.Events.GameLoop.GameLaunched += onGameLaunched;
			Helper.Events.GameLoop.DayStarted +=
				(_sender, _e) => updateChannels ();
			Helper.Events.GameLoop.OneSecondUpdateTicked +=
				(_sender, _e) => GarbageChannel.CheckEvent ();
			Helper.Events.Player.Warped +=
				(_sender, _e) => TrainsChannel.CheckEvent ();

			// Listen for PlatoTK events.
			var plato = PlatoTK.HelperExtension.GetPlatoHelper (this);
			plato.Events.QuestionRaised += onQuestionRaised;
			plato.Events.TVChannelSelected += onTVChannelSelected;
		}

		private void onGameLaunched (object _sender, GameLaunchedEventArgs _e)
		{
			// Create the channels.
			channels.Add (new NightEventsChannel ());
			channels.Add (new MiningChannel ());
			channels.Add (new GarbageChannel ());
			channels.Add (new TrainsChannel ());
			channels.Add (new MoviesChannel ());

			// Set up Generic Mod Config Menu, if it is available.
			ModConfig.SetUpMenu ();
		}

		internal void updateChannels ()
		{
			foreach (Channel channel in channels)
				channel.update ();
		}

		private void onQuestionRaised (object _sender, IQuestionRaisedEventArgs e)
		{
			if (e.IsTV)
			{
				foreach (Channel channel in channels)
				{
					if (channel.isAvailable)
						e.AddResponse (new Response (channel.globalID, channel.title));
				}
			}
		}

		private void onTVChannelSelected (object _sender, ITVChannelSelectedEventArgs e)
		{
			var channel = channels.Find (channel => channel.globalID == e.ChannelName);
			if (channel != null)
			{
				e.PreventDefault ();
				channel.show (e.TVInstance);
			}
		}

		internal void resetChannels (bool isCommand = false)
		{
			try
			{
				Utilities.CheckWorldReady ();
				foreach (Channel channel in channels)
					channel.reset ();
				if (isCommand)
				{
					Monitor.Log ("Channels reset to initial states.",
						LogLevel.Info);
				}
			}
			catch (Exception e)
			{
				Monitor.Log ($"Could not reset channels: {e.Message}",
					LogLevel.Error);
			}
		}
	}
}
