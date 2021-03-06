/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using Harmony;
using StardewModdingAPI;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class ObjectGetMinutesForCrystalariumPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal ObjectGetMinutesForCrystalariumPatch(ModConfig config, IMonitor monitor)
		: base(config, monitor) { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(SObject), name: "getMinutesForCrystalarium"),
				postfix: new HarmonyMethod(GetType(), nameof(ObjectGetMinutesForCrystalariumPostfix))
			);
		}

		/// <summary>Patch to speed up Gemologist crystalarium processing time.</summary>
		protected static void ObjectGetMinutesForCrystalariumPostfix(ref int __result)
		{
			if (Utils.PlayerHasProfession("gemologist"))
			{
				__result = (int)(__result * 0.75f);
			}
		}
	}
}
