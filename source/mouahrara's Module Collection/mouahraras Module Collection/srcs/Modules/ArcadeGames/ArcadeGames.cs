/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/mouahrarasModuleCollection
**
*************************************************/

using HarmonyLib;
using mouahrarasModuleCollection.ArcadeGames.SubModules;

namespace mouahrarasModuleCollection.Modules
{
	internal class ArcadeGamesModule
	{
		internal static void Apply(Harmony harmony)
		{
			// Apply sub-modules
			KonamiCodeSubModule.Apply(harmony);
			NonRealisticLeaderboardSubModule.Apply(harmony);
			PayToPlaySubModule.Apply(harmony);
		}
	}
}
