/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class CharacterGetBoundingBoxPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal CharacterGetBoundingBoxPatch()
		{
			Original = typeof(Character).MethodNamed(nameof(Character.GetBoundingBox));
			//Postfix = new HarmonyMethod(GetType(), nameof(CharacterGetBoundingBoxPostfix));
		}

		#region harmony patches

		[HarmonyPostfix]
		private static void CharacterGetBoundingBoxPostfix(Character __instance, ref Rectangle __result)
		{
			if (__instance is GreenSlime slime && slime.Scale > 1f)
			{
				var deltaHeight = __result.Height * (slime.Scale - 1f);
				var deltaWidth = __result.Width * (slime.Scale - 1f);

				__result.Height += (int)deltaHeight;
				__result.Width += (int)deltaWidth;

				__result.Y -= (int)(deltaHeight / 2f);
				__result.X -= (int)(deltaWidth / 2f);
			}
		}

		#endregion harmony patches
	}
}
