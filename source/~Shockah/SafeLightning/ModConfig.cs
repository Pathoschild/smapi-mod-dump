/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

namespace Shockah.SafeLightning
{
	internal class ModConfig
	{
		public bool SafeTiles { get; set; } = true;
		public bool SafeFruitTrees { get; set; } = true;
		public BigLightningBehavior BigLightningBehavior { get; set; } = BigLightningBehavior.WhenSupposedToStrike;
	}
}