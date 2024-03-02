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
using mouahrarasModuleCollection.SubSections;

namespace mouahrarasModuleCollection.Sections
{
	internal class TweaksAndFeaturesSection
	{
		internal static void Apply(Harmony harmony)
		{
			// Apply sub-sections
			ArcadeGamesSubSection.Apply(harmony);
			MachinesSubSection.Apply(harmony);
			ShopsSubSection.Apply(harmony);
			UserInterfaceSubSection.Apply(harmony);
			OtherSubSection.Apply(harmony);
		}
	}
}
