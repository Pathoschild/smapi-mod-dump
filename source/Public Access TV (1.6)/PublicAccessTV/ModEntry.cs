/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/emurphy42/PredictiveMods
**
*************************************************/

using PredictiveCore;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Framework.ModLoading.Rewriters.StardewValley_1_5;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using HarmonyLib;
using static StardewValley.GameLocation;
using StardewValley.Objects;
using System.Threading.Channels;

namespace PublicAccessTV
{
	public class ModEntry : Mod
	{
		internal static ModEntry Instance { get; private set; }

		protected static ModConfig Config => ModConfig.Instance;

		private readonly PerScreen<List<Channel>> channels_ = new (() => new ());
		internal List<Channel> channels => channels_.Value;

		// Set up asset editors.
		private readonly DialogueEditor dialogueEditor = new();
		private readonly EventsEditor eventsEditor = new();
		private readonly MailEditor mailEditor = new();

		private static bool questionModified = false;

		public override void Entry (IModHelper helper)
		{
			// Make resources available.
			Instance = this;
			ModConfig.Load ();

			// Set up PredictiveCore.
			Utilities.Initialize (this, () => ModConfig.Instance);

			// Set up asset editors.
			Helper.Events.Content.AssetRequested += dialogueEditor.OnAssetRequested;
            Helper.Events.Content.AssetRequested += eventsEditor.OnAssetRequested;
            Helper.Events.Content.AssetRequested += mailEditor.OnAssetRequested;

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

            // Add TV channels.
            var harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.Patch(
				original: AccessTools.Method(
					typeof(GameLocation),
					nameof(GameLocation.createQuestionDialogue),
					new Type[]
					{
						typeof(string),
						typeof(Response[]),
						typeof(afterQuestionBehavior),
						typeof(NPC)
					}
				),
				prefix: new HarmonyMethod(
					typeof(ModEntry),
					nameof(ModEntry.createQuestionDialogue_prefix)
				)
			);
			harmony.Patch(
				original: AccessTools.Method(
					typeof(TV),
					nameof(TV.selectChannel)
				),
				prefix: new HarmonyMethod(
					typeof(ModEntry),
					nameof(ModEntry.selectChannel_prefix)
				)
			);
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

		public static bool createQuestionDialogue_prefix(GameLocation __instance, string question, Response[] answerChoices, afterQuestionBehavior afterDialogueBehavior, NPC speaker = null)
		{
			return Instance.onQuestionRaised(__instance, question, answerChoices, afterDialogueBehavior, speaker);
		}

		private bool onQuestionRaised(GameLocation __instance, string question, Response[] answerChoices, afterQuestionBehavior afterDialogueBehavior, NPC speaker = null)
		{
			if (questionModified)
			{
                questionModified = false;
				return true;
			}

			if (question != Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13120"))
			{
				return true;
			}

			List<Response> answerChoicesList = new();
			foreach (Response response in answerChoices)
			{
				if (response.responseKey != "(Leave)")
				{
					answerChoicesList.Add(response);
				}
			}
            foreach (Channel channel in channels)
            {
                if (channel.isAvailable)
				{
					answerChoicesList.Add(new Response(channel.globalID, channel.title));
                }
			}
            foreach (Response response in answerChoices)
            {
                if (response.responseKey == "(Leave)")
                {
                    answerChoicesList.Add(response);
                }
            }
            questionModified = true;
            __instance.createQuestionDialogue(question, answerChoicesList.ToArray(), afterDialogueBehavior, speaker);
            questionModified = false;
            return false;
        }


        public static bool selectChannel_prefix(TV __instance, Farmer who, string answer)
		{
			return Instance.onTVChannelSelected(__instance, who, answer);
		}

		private bool onTVChannelSelected (TV __instance, Farmer who, string answer)
		{
            if (Game1.IsGreenRainingHere())
            {
				return true;
            }
            
			var channel = channels.Find (channel => channel.globalID == answer);
			if (channel != null)
			{
				channel.show (__instance);
				return false;
			}

			return true;
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
