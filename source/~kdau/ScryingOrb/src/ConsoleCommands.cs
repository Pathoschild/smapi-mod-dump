/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/predictivemods
**
*************************************************/

using Microsoft.Xna.Framework;
using PredictiveCore;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace ScryingOrb
{
	public static class ConsoleCommands
	{
		private static IModHelper Helper => ModEntry.Instance.Helper;
		private static IMonitor Monitor => ModEntry.Instance.Monitor;

		public static void Initialize ()
		{
			Helper.ConsoleCommands.Add ("scrying_orb",
				"Shows the Scrying Orb unlimited use interface, even if one is not present in the world.",
				cmdScryingOrb);
			Helper.ConsoleCommands.Add ("reset_scrying_orbs",
				"Resets the state of Scrying Orbs to default values.",
				cmdResetScryingOrbs);
#if DEBUG
			Helper.ConsoleCommands.Add ("test_scrying_orb",
				"Puts a Scrying Orb and all types of offering into inventory.",
				cmdTestScryingOrb);
			Helper.ConsoleCommands.Add ("test_date_picker",
				"Runs a DatePicker dialog for testing use.",
				cmdTestDatePicker);
#endif
		}

		private static void cmdScryingOrb (string _command, string[] _args)
		{
			try
			{
				Experience.Run<UnlimitedExperience> (null);
			}
			catch (Exception e)
			{
				Monitor.Log ($"Could not run Scrying Orb: {e.Message}", LogLevel.Error);
			}
		}

		private static void cmdResetScryingOrbs (string _command, string[] _args)
		{
			try
			{
				Utilities.CheckWorldReady ();
				UnlimitedExperience.Reset ();
				LuckyPurpleExperience.Reset ();
				MetaExperience.Reset ();
				Monitor.Log ("Scrying Orb state reset to defaults.",
					LogLevel.Info);
			}
			catch (Exception e)
			{
				Monitor.Log ($"Could not reset Scrying Orbs: {e.Message}", LogLevel.Error);
			}
		}

#if DEBUG
		private static void cmdTestScryingOrb (string _command, string[] _args)
		{
			try
			{
				Game1.player.addItemsByMenuIfNecessary (new List<Item>
				{
					new SObject ( 74,  50), // 50 Prismatic Shard for UnlimitedExperience
					new SObject (789,   1), // Lucky Purple Shorts for LuckyPurpleExperience
					// TODO: item for ItemFinderExperience
					new SObject (168, 150), // 50*3 Trash for GarbageExperience
					// TODO: item for ShoppingExperience
					new SObject (767, 150), // 50*3 Bat Wing for NightEventsExperience
					new SObject (541,  50), // 50 Aerinite for GeodesExperience
					new SObject (382, 100), // 50*2 Coal for MiningExperience
					// 3 Scrying Orb:
					new SObject (Vector2.Zero, ModEntry.Instance.parentSheetIndex), 
					new SObject (Vector2.Zero, ModEntry.Instance.parentSheetIndex),
					new SObject (Vector2.Zero, ModEntry.Instance.parentSheetIndex),
				});

				Monitor.Log ("Scrying Orb test kit placed in inventory.",
					LogLevel.Info);
			}
			catch (Exception e)
			{
				Monitor.Log ($"Could not create test kit: {e.Message}", LogLevel.Error);
			}
		}

		private static void cmdTestDatePicker (string _command, string[] _args)
		{
			try
			{
				SDate initialDate = new SDate (2, "spring", 15);
				string prompt = "Where on the wheel of the year do you seek?";
				if (Context.IsWorldReady)
					++ModEntry.Instance.OrbsIlluminated; // use the special cursor in the dialog
				Game1.activeClickableMenu = new DatePicker (initialDate, prompt,
					(date) =>
					{
						if (Context.IsWorldReady)
							--ModEntry.Instance.OrbsIlluminated;
						Monitor.Log ($"DatePicker chose {date}", LogLevel.Info);
					});
			}
			catch (Exception e)
			{
				Monitor.Log ($"Could not test date picker: {e.Message}", LogLevel.Error);
			}
		}
#endif
	}
}
