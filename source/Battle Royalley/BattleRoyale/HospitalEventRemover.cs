/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/BattleRoyalley
**
*************************************************/

using StardewValley;
using System;

namespace BattleRoyale
{
	class HospitalEventRemover : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(Farmer), "Update");

		public static void Prefix(Farmer __instance)
		{
			if (__instance == Game1.player && __instance.health <= 0)
			{
				Console.WriteLine("Killed by internal game event, e.g. explosion");
				ModEntry.BRGame.TakeDamage(100, __instance.UniqueMultiplayerID);
			}
		}
	}
}
