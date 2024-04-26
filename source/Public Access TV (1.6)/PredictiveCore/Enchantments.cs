/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/emurphy42/PredictiveMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Enchantments;
using StardewValley.Extensions;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PredictiveCore
{
	public static class Enchantments
	{
		public struct Prediction
		{
			public uint number;
			public BaseEnchantment enchantment;
		}

		// Whether this module should be available for player use.
		public static bool IsAvailable =>
			Game1.MasterPlayer.hasOrWillReceiveMail ("reachedCaldera") &&
			(Utilities.Config.InaccuratePredictions || Utilities.SupportedVersion);

		// Lists the next several enchantments to be applied to the given tool,
		// up to the given limit.
		public static List<Prediction> ListForTool (Tool tool, uint limit,
			bool cumulative = false)
		{
			Utilities.CheckWorldReady ();
			if (!IsAvailable)
				throw new UnavailableException ("enchantments");

			List<Prediction> predictions = new ();
			Tool fakeTool = tool.getOne () as Tool;

			// Start from the next enchantment and continue to the limit.
			uint fromNumber = Game1.stats.Get("timesEnchanted");
            for (uint number = fromNumber; number < fromNumber + limit; ++number)
			{
				// Select a random available enchantment per the base game rules.
				Random rng = new ((int) number + (int) Game1.uniqueIDForThisGame);
				var candidates = BaseEnchantment.GetAvailableEnchantmentsForItem (fakeTool);
				var enchantment = rng.ChooseFrom(candidates);

				predictions.Add (new ()
				{
					number = number,
					enchantment = enchantment,
				});

				// If the enchantments are cumulative on the tool itself,
				// simulate applying this enchantment to be ready for the next.
				if (cumulative && enchantment != null)
				{
					fakeTool.AddEnchantment (enchantment);
					fakeTool.previousEnchantments.Insert (0, enchantment.GetName ());
					while (fakeTool.previousEnchantments.Count > 2)
						fakeTool.previousEnchantments.RemoveAt (fakeTool.previousEnchantments.Count - 1);
				}
			}

			return predictions;
		}

		internal static void Initialize (bool addConsoleCommands)
		{
			if (!addConsoleCommands)
				return;
			Utilities.Helper.ConsoleCommands.Add ("predict_enchantments",
				"Predicts the next several enchantments to be applied to a tool of a given type.\n\nUsage: predict_enchantments [<type> [<cumulative> [<number>]]]\n- type: weapon, pickaxe, axe, hoe, can, rod (default weapon)\n- cumulative: whether intervening enchantments will be applied to this tool (default false)\n- limit: number of enchantments to predict (default 20).",
				(_command, args) => ConsoleCommand (args.ToList ()));
		}

		private static void ConsoleCommand (List<string> args)
		{
			try
			{
				Utilities.CheckWorldReady ();

				string type = "weapon";
				if (args.Count > 0)
				{
					type = args[0];
					args.RemoveAt (0);
				}

				Tool tool = type switch
				{
					"weapon" => new MeleeWeapon (),
					"pickaxe" => new Pickaxe (),
					"axe" => new Axe (),
					"hoe" => new Hoe (),
					"can" => new WateringCan (),
					"rod" => new FishingRod (),
					_ => null,
				};
				if (tool == null)
					throw new ArgumentException ($"Invalid tool type '{type}', must be weapon, pickaxe, axe, hoe, can or rod.");

				bool cumulative = false;
				if (args.Count > 0)
				{
					if (!bool.TryParse (args[0], out cumulative))
						throw new ArgumentException ($"Invalid cumulative status '{args[0]}', must be true or false.");
					args.RemoveAt (0);
				}

				uint limit = 20;
				if (args.Count > 0)
				{
					if (!uint.TryParse (args[0], out limit) || limit < 1)
						throw new ArgumentException ($"Invalid limit '{args[0]}', must be a number 1 or higher.");
					args.RemoveAt (0);
				}

				List<Prediction> predictions = ListForTool (tool, limit, cumulative);
				Utilities.Monitor.Log ($"Next {limit} enchantment(s) on a(n) {type}{(cumulative ? ", cumulatively" : "")}:",
					LogLevel.Info);
				foreach (Prediction prediction in predictions)
				{
					Utilities.Monitor.Log (string.Format ("{0,5:D}. {1}",
						prediction.number, prediction.enchantment.GetDisplayName ()),
						LogLevel.Info);
				}
			}
			catch (Exception e)
			{
				Utilities.Monitor.Log (e.Message, LogLevel.Error);
			}
		}
	}
}
