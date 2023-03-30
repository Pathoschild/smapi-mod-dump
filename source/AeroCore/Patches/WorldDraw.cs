/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace AeroCore.Patches
{
	[HarmonyPatch(typeof(GameLocation), nameof(GameLocation.draw))]
	internal class WorldDraw
	{
		[HarmonyPostfix]
		internal static void EmitDraw(SpriteBatch b)
		{
			ModEntry.api?.EmitWorldDraw(b);
		}
	}
}
