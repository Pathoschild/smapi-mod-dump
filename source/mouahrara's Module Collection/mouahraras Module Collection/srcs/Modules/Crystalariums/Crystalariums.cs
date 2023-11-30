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
using mouahrarasModuleCollection.Crystalariums.SubModules;

namespace mouahrarasModuleCollection.Modules
{
	internal class CrystalariumsModule
	{
		internal static void Apply(Harmony harmony)
		{
			// Apply sub-modules
			SafeReplacementSubModule.Apply(harmony);
		}
	}
}
