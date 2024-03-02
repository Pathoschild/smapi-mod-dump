/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/mouahrarasModuleCollection
**
*************************************************/

using System;
using HarmonyLib;
using StardewModdingAPI;
using mouahrarasModuleCollection.TweaksAndFeatures.Shops.GeodesAutoProcess.Patches;

namespace mouahrarasModuleCollection.Modules
{
	internal class GeodesAutoProcessModule
	{
		internal static void Apply(Harmony harmony)
		{
			// Load Harmony patches
			try
			{
				// Apply menus patches
				IClickableMenuPatch.Apply(harmony);
				MenuWithInventoryPatch.Apply(harmony);
				GeodeMenuPatch.Apply(harmony);

				// Apply objects patches
				FarmerPatch.Apply(harmony);
			}
			catch (Exception e)
			{
				ModEntry.Monitor.Log($"Issue with Harmony patching of the {typeof(GeodesAutoProcessModule)} module: {e}", LogLevel.Error);
				return;
			}
		}
	}
}
