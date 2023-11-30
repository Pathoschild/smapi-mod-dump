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
using mouahrarasModuleCollection.Festivals.EndTime.Patches;

namespace mouahrarasModuleCollection.Festivals.SubModules
{
	internal class EndTimeSubModule
	{
		internal static void Apply(Harmony harmony)
		{
			// Load Harmony patches
			try
			{
				// Apply locations patches
				EventPatch.Apply(harmony);
			}
			catch (Exception e)
			{
				ModEntry.Monitor.Log($"Issue with Harmony patching of the {typeof(EndTimeSubModule)} module: {e}", LogLevel.Error);
				return;
			}
		}
	}
}
