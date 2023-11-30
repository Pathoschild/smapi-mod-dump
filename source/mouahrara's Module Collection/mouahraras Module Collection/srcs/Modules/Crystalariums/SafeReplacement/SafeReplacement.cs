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
using mouahrarasModuleCollection.Crystalariums.SafeReplacement.Patches;

namespace mouahrarasModuleCollection.Crystalariums.SubModules
{
	internal class SafeReplacementSubModule
	{
		internal static void Apply(Harmony harmony)
		{
			// Load Harmony patches
			try
			{
				// Apply objects patches
				ObjectPatch.Apply(harmony);

				// Apply locations patches
				GameLocationPatch.Apply(harmony);
			}
			catch (Exception e)
			{
				ModEntry.Monitor.Log($"Issue with Harmony patching of the {typeof(SafeReplacementSubModule)} module: {e}", LogLevel.Error);
				return;
			}
		}
	}
}
