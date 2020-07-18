using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using static System.Math;

namespace AngryGrandpa
{
	/// <summary>The class for editing grandpaEvaluation data assets.</summary>
	internal class EvaluationEditor : IAssetEditor
	{
		/*********
        ** Accessors
        *********/
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;
		protected static ModConfig Config => ModConfig.Instance;


		/*********
        ** Fields
        *********/
		protected static ITranslationHelper i18n = Helper.Translation;

		/****
        ** Constants
        ****/
		/// <summary>A list of i18n data keys (base) for the evaluation and re-evaluation dialogue scripts</summary>
		public static readonly List<string> EvaluationStrings =
			new List<string>
		{
			"1CandleResult",
			"2CandleResult",
			"3CandleResult",
			"4CandleResult",
			"1CandleReevaluation",
			"2CandleReevaluation",
			"3CandleReevaluation",
			"4CandleReevaluation"
		};


		/*********
        ** Public methods
        *********/
		/// <summary>Get whether this instance can edit the given asset.</summary>
		/// <typeparam name="_T">The asset Type.</typeparam>
		/// <param name="asset">Basic metadata about the asset being loaded.</param>
		/// <returns>true for asset Strings\StringsFromCSFiles, false otherwise</returns>
		public bool CanEdit<_T> (IAssetInfo asset)
		{
			return asset.AssetNameEquals($"Strings\\StringsFromCSFiles");
		}

		/// <summary>Edit the evaluation entries in Strings\StringsFromCSFiles with new dialogues and tokens.</summary>
		/// <typeparam name="_T">The asset Type</typeparam>
		/// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it</param>
		public void Edit<_T> (IAssetData asset)
		{
			// Can't edit these assets without an active game for spouse, NPC, and year info

			if (!Context.IsWorldReady)
			{ return; }

			// Prepare tokens

			string pastYears;
			int yearsPassed = Max(Game1.year - 1, Config.YearsBeforeEvaluation); // Accurate dialogue even for delayed event
			if (yearsPassed >= 10)
			{
				if (Config.GrandpaDialogue == "Nuclear") { pastYears = i18n.Get("GrandpaDuringManyYears.Nuclear"); }
				else { pastYears = i18n.Get("GrandpaDuringManyYears"); }
			}
			else // yearsPassed < 10
			{
				pastYears = i18n.Get("GrandpaDuringPastYears").ToString().Split('|')[yearsPassed];
			}

			string spouseOrLewis;
			if (Game1.player.isMarried()) { spouseOrLewis = "%spouse"; }
			else {spouseOrLewis = Game1.getCharacterFromName<NPC>("Lewis").displayName; }

			string fifthCandle = "";
			bool inOneYear = Game1.year == 1 || (Game1.year == 2 && Game1.currentSeason == "spring" && Game1.dayOfMonth == 1);
			if (Utility.getGrandpaScore() >= 21 && inOneYear)
			{ 
				fifthCandle = i18n.Get("FifthCandle." + Config.GrandpaDialogue);
			}

			// Collect all portrait tokens & others

			var allEvaluationTokens = new Dictionary<string, string>(Config.PortraitTokens)
			{
				["pastYears"] = pastYears,
				["spouseOrLewis"] = spouseOrLewis,
				["fifthCandle"] = fifthCandle
			};

			// Prepare data

			var data = asset.AsDictionary<string, string>().Data;

			// Main patching loop

			if (asset.AssetNameEquals($"Strings\\StringsFromCSFiles"))
			{
				foreach (string entry in EvaluationStrings)
				{
					string gameKey = i18n.Get(entry + ".gameKey");
					string modKey = entry + "." + Config.GrandpaDialogue;
					if (Config.GenderNeutrality) { modKey += "-gn"; }
					string value = i18n.Get(modKey, allEvaluationTokens);

					data[gameKey] = value;
				}
			}
		}
	}
}