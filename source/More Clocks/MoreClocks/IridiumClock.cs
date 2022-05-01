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

namespace MoreClocks.IridiumClock
{
	public class IridiumClockBuilding : Building
	{
		private static readonly BluePrint Blueprint = new("Iridium Clock");

		public IridiumClockBuilding()
			: base(IridiumClockBuilding.Blueprint, Vector2.Zero) { }
	}
}