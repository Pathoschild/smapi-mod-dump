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
using mouahrarasModuleCollection.ClintsShop.SimultaneousServices.Patches;

namespace mouahrarasModuleCollection.ClintsShop.SubModules
{
	internal class SimultaneousServicesSubModule
	{
		internal static void Apply(Harmony harmony)
		{
			// Load Harmony patches
			try
			{
				// Apply locations patches
				GameLocationPatch.Apply(harmony);
			}
			catch (Exception e)
			{
				ModEntry.Monitor.Log($"Issue with Harmony patching of the {typeof(SimultaneousServicesSubModule)} module: {e}", LogLevel.Error);
				return;
			}
		}
	}
}
