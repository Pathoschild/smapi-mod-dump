/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/p-holodynski/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;

namespace MoreClocks.RadioactiveClock
{
	public class RadioactiveClockBuilding : Building
	{
		private static readonly BluePrint Blueprint = new("Radioactive Clock");

		public RadioactiveClockBuilding()
			: base(RadioactiveClockBuilding.Blueprint, Vector2.Zero) { }
	}
}