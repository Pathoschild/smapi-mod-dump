/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Shockah.CommonModCode.UI;

namespace Shockah.XPDisplay
{
	internal class ModConfig
	{
		public Orientation SmallBarOrientation { get; set; } = Orientation.Vertical;
		public Orientation BigBarOrientation { get; set; } = Orientation.Horizontal;
		public float Alpha { get; set; } = 0.6f;
	}
}
