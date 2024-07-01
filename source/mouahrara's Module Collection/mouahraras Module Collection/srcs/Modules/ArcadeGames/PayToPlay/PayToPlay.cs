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
using mouahrarasModuleCollection.ArcadeGames.PayToPlay.Handlers;
using mouahrarasModuleCollection.ArcadeGames.PayToPlay.Patches;

namespace mouahrarasModuleCollection.Modules
{
	internal class PayToPlayModule
	{
		internal static void Apply(Harmony harmony)
		{
			// Load Harmony patches
			try
			{
				// Apply locations patches
				AbigailGamePatch.Apply(harmony);
				MineCartPatch.Apply(harmony);
			}
			catch (Exception e)
			{
				ModEntry.Monitor.Log($"Issue with Harmony patching of the {typeof(PayToPlayModule)} module: {e}", LogLevel.Error);
				return;
			}

			// Subscribe to events
			ModEntry.Helper.Events.GameLoop.SaveLoaded += SaveLoadedHandler.Apply;
		}
	}
}
