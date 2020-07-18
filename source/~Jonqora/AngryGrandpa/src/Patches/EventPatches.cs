using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace AngryGrandpa
{
	/// <summary>The class for patching methods on the StardewValley.Event class.</summary>
	class EventPatches
	{
		/*********
        ** Accessors
        *********/
		private static IModHelper Helper => ModEntry.Instance.Helper;
		private static IMonitor Monitor => ModEntry.Instance.Monitor;
		private static ModConfig Config => ModConfig.Instance;
		private static HarmonyInstance Harmony => ModEntry.Instance.Harmony;


		/*********
        ** Fields
        *********/
		protected static ITranslationHelper i18n = Helper.Translation;


		/*********
        ** Public methods
        *********/
		/// <summary>
		/// Applies the harmony patches defined in this class.
		/// </summary>
		public static void Apply()
		{
			Harmony.Patch(
				original: AccessTools.Method(typeof(Event),
					nameof(Event.command_grandpaEvaluation)),
				prefix: new HarmonyMethod(typeof(EventPatches),
					nameof(EventPatches.grandpaEvaluations_Prefix)),
				postfix: new HarmonyMethod(typeof(EventPatches),
					nameof(EventPatches.grandpaEvaluations_Postfix))
			);
			Harmony.Patch(
				original: AccessTools.Method(typeof(Event),
					nameof(Event.command_grandpaEvaluation2)),
				prefix: new HarmonyMethod(typeof(EventPatches),
					nameof(EventPatches.grandpaEvaluations_Prefix)),
				postfix: new HarmonyMethod(typeof(EventPatches),
					nameof(EventPatches.grandpaEvaluations_Postfix))
			);
			Harmony.Patch(
				original: AccessTools.Method(typeof(Event),
					nameof(Event.skipEvent)),
				postfix: new HarmonyMethod(typeof(EventPatches),
					nameof(EventPatches.grandpaEvaluations_Postfix))
			);
		}

		/// <summary>
		/// Invalidates cached assets for grandpaEvaluations right before they are used.
		/// This ensures that the correct dialogue (for years, fifthCandle etc.) is applied for the event.
		/// </summary>
		public static void grandpaEvaluations_Prefix()
		{
			var game = Game1.game1;
			try
			{
				Helper.Content.InvalidateCache("Strings\\StringsFromCSFiles"); // Refresh cache before use
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(grandpaEvaluations_Prefix)}:\n{ex}",
					LogLevel.Error);
			}
		}

		/// <summary>
		/// Displays the points total on the screen during evaluation events.
		/// Also handles event flag removal, mail flag assignment, and checks for a host-owned Statue of Perfection (including for skipped events).
		/// Called after grandpaEvaluation, grandpaEvaluation2, or when any event is skipped.
		/// </summary>
		/// <param name="__instance">The Event instance (Always containing grandpaEvaluation/grandpaEvaluation2, or any skipped event)</param>
		public static void grandpaEvaluations_Postfix(Event __instance)
		{
			try
			{
				switch (__instance.id) // Check which event this is... we're patching skipEvent and don't want to affect all.
				{
					case 558291: // Initial
					case 558292: // Reevaluation

						CheckWorldForStatueOfPerfection(); // Add reward flag to host if any pre-existing statue
						foreach (int e in new List<int> { 2146991, 321777 }) // Remove candles event, re-evaluation flag
						{
							while (Game1.player.eventsSeen.Contains(e)) { Game1.player.eventsSeen.Remove(e); }
						}
						// Add a mail flag the FIRST time this mod is used for any evaluation. 
						if (!Game1.player.mailReceived.Contains("6324hasDoneModdedEvaluation"))
						{
							Game1.player.mailReceived.Add("6324hasDoneModdedEvaluation"); // This activates bonus rewards if enabled.
						}

						if (Config.ShowPointsTotal && !__instance.skipped) // Don't show if disabled in config or the event was skipped
						{
							int grandpaScore = Utility.getGrandpaScore();
							int maxScore = Config.GetMaxScore();
							string displayText = i18n.Get("Event.cs.ShowGrandpaScore", new { grandpaScore, maxScore });
							Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite()
							{
								text = displayText,
								local = true,
								position = new Vector2((float)(Game1.viewport.Width / 2) - Game1.dialogueFont.MeasureString(displayText).X / 2f, (float)(Game1.tileSize / 2)), // was originally /4,  
								color = Color.White,
								interval = 20000f, // Lasts for 15 seconds -> changed to 20
								layerDepth = 1f,
								animationLength = 1,
								initialParentTileIndex = 1,
								currentParentTileIndex = 1,
								totalNumberOfLoops = 1
							});
						}
						Monitor.Log($"Ran patch for evaluation or re-evaluation event: {nameof(grandpaEvaluations_Postfix)}", LogLevel.Trace);
						break;
					default:
						break;
				}
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(grandpaEvaluations_Postfix)}:\n{ex}",
					LogLevel.Error);
			}
		}


		/*********
        ** Private methods
        *********/
		/// <summary>
		/// Searches for any pre-existing statue of perfection in the world. If found, it assigns a mail flag.
		/// The search is only done for main (host) players, and not if a modded grandpa evaluation event has already occurred.
		/// </summary>
		private static void CheckWorldForStatueOfPerfection()
		{
			if (Game1.player.IsMainPlayer) // Host will always be present for evaluation events
			{
				if (!Game1.player.mailReceived.Contains("6324hasDoneModdedEvaluation") && // No modded evaluations yet
					!Game1.player.mailReceived.Contains("6324reward4candle") && // They don't already have the flag
					Utility.doesItemWithThisIndexExistAnywhere(160, true)) // They DO have an existing Statue of Perfection somewhere
				{
					Game1.player.mailReceived.Add("6324reward4candle"); // Assume the existing statue belongs to this Host player
					Monitor.Log($"Found existing Statue of Perfection in world: {nameof(CheckWorldForStatueOfPerfection)}", LogLevel.Trace);
				}
			}
		}
	}
}