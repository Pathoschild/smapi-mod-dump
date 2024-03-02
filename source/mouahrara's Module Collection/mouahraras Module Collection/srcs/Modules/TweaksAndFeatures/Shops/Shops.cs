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
using mouahrarasModuleCollection.Modules;

namespace mouahrarasModuleCollection.SubSections
{
	internal class ShopsSubSection
	{
		internal static void Apply(Harmony harmony)
		{
			// Apply modules
			AnimalPurchaseModule.Apply(harmony);
			GeodesAutoProcessModule.Apply(harmony);
			SimultaneousServicesModule.Apply(harmony);
		}
	}
}
