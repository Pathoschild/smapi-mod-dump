/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/eastscarpe
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace EastScarp
{
	public static class Obelisks
	{
		private static IModHelper Helper => ModEntry.Instance.Helper;
		private static IMonitor Monitor => ModEntry.Instance.Monitor;
		private static ModData Data => ModEntry.Instance.data;
		private static Harmony Harmony => ModEntry.Instance.harmony;

		private static string ObeliskName => Data.ObeliskWarp.Name;

		// The obelisk is replaced by this one in save files.
		private const string ObeliskReplacement = "Earth Obelisk";

		// Key for modData entry.
		private const string ObeliskKey = "ESObelisk";

		internal static void Patch ()
		{
			Harmony.Patch (
				original: AccessTools.Method (typeof (Building), "obeliskWarpForReal"),
				prefix: new HarmonyMethod (typeof (Obelisks),
					nameof (Obelisks.Building_obeliskWarpForReal_Prefix))
			);
		}

		private static bool Building_obeliskWarpForReal_Prefix (Building __instance)
		{
			try
			{
				if (__instance.buildingType.Value.Equals (ObeliskName))
				{
					Game1.warpFarmer (Data.ObeliskWarp.Location,
						Data.ObeliskWarp.X, Data.ObeliskWarp.Y, flip: false);
				}
			}
			catch (Exception e)
			{
				Monitor.Log ($"Failed in {nameof (Building_obeliskWarpForReal_Prefix)}:\n{e}",
					LogLevel.Error);
				Monitor.Log (e.StackTrace, LogLevel.Trace);
			}
			return true;
		}

		// Add the obelisk to the Wizard's construction menu.
		public static void UpdateMenu (IClickableMenu menu)
		{
			if (menu is CarpenterMenu cMenu && IsContentPresent ())
			{
				if (Helper.Reflection.GetField<bool> (cMenu, "magicalConstruction").GetValue ())
				{
					var blueprints = Helper.Reflection.GetField<List<BluePrint>> (cMenu, "blueprints").GetValue ();
					blueprints.Add (new BluePrint (ObeliskName));
					Monitor.Log ($"Added {ObeliskName} to Wizard's construction menu",
						LogLevel.Trace);
				}
			}
		}

		// Change the East Scarp obelisk to an Earth Obelisk for save files.
		public static void SanitizeAll ()
		{
			if (!Game1.player.IsMainPlayer)
				return;

			foreach (var building in Game1.getFarm ().buildings)
			{
				if (building != null &&
					building.buildingType.Value == ObeliskName)
				{
					building.buildingType.Value = ObeliskReplacement;
					// Store the original type in modData.
					if (!building.modData.ContainsKey (ObeliskKey))
						building.modData.Add (ObeliskKey, ObeliskName);
					Monitor.Log ($"Replaced {ObeliskName} at {building.tileX}, {building.tileY} with {ObeliskReplacement}",
						LogLevel.Trace);
				}
			}
		}

		// Change placeholder Earth Obelisks back to East Scarp obelisks.
		public static void RestoreAll ()
		{
			if (!Game1.player.IsMainPlayer || !IsContentPresent ())
				return;

			foreach (var building in Game1.getFarm ().buildings)
			{
				if (building != null &&
					building.buildingType.Value == ObeliskReplacement &&
					building.modData.ContainsKey (ObeliskKey) &&
					building.modData[ObeliskKey] == ObeliskName)
				{
					building.buildingType.Value = ObeliskName;
					Monitor.Log ($"Restored {ObeliskName} at {building.tileX}, {building.tileY}");
				}
			}
		}

		// Check if the needed content files have been patched.
		private static bool IsContentPresent ()
		{
			Dictionary<string, string> blueprintDict = Game1.content.Load<Dictionary<string, string>> (PathUtilities.NormalizePath("Data/Blueprints"));
			if (!blueprintDict.ContainsKey (ObeliskName))
				return false;

			var texture = Game1.content.Load<Texture2D> (PathUtilities.NormalizePath ("Buildings/" + ObeliskName));
			if (texture == null)
				return false;

			return true;
		}
	}
}
