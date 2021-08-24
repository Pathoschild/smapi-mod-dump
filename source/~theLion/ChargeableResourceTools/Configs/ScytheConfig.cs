/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace TheLion.Stardew.Tools.Configs
{
	/// <summary>Configuration for the axe shockwave.</summary>
	public class ScytheConfig
	{
		/// <summary>Enables charging the Golden Scythe.</summary>
		public bool EnableScytheCharging { get; set; } = true;

		/// <summary>The bonus radius of the charged Golden Scythe.</summary>
		public float RadiusMultiplier { get; set; } = 1.5f;
	}
}