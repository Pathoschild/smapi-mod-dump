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
using mouahrarasModuleCollection.ArcadeGames.KonamiCode.Patches;

namespace mouahrarasModuleCollection.Modules
{
	internal class KonamiCodeModule
	{
		internal static void Apply(Harmony harmony)
		{
			// Load Harmony patches
			try
			{
				// Apply minigames patches
				AbigailGamePatch.Apply(harmony);
				MineCartPatch.Apply(harmony);

				// Apply Game1 patches
				Game1Patch.Apply(harmony);

				// Apply network patches
				MultiplayerPatch.Apply(harmony);
			}
			catch (Exception e)
			{
				ModEntry.Monitor.Log($"Issue with Harmony patching of the {typeof(KonamiCodeModule)} module: {e}", LogLevel.Error);
				return;
			}
		}
	}
}
