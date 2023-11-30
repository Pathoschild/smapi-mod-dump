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
using mouahrarasModuleCollection.FarmView.SubModules;

namespace mouahrarasModuleCollection.Modules
{
	internal class FarmViewModule
	{
		internal static void Apply(Harmony harmony)
		{
			// Apply sub-modules
			FastScrollingSubModule.Apply(harmony);
			ZoomSubModule.Apply(harmony);
		}
	}
}
