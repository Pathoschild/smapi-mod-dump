/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace TheLion.AwesomeProfessions
{
	/// <summary>The mod persisted data.</summary>
	public class ProfessionsData
	{
		public uint OenologyFameAccrued { get; set; } = 0;
		public uint HighestOenologyAwardEarned { get; set; } = 0;
		public uint LowestMineLevelReached { get; set; } = 0;
		public uint ItemsForaged { get; set; } = 0;
		public uint MineralsCollected { get; set; } = 0;
		public uint ScavengerHuntStreak { get; set; } = 0;
		public uint ProspectorHuntStreak { get; set; } = 0;
		public uint WaterTrashCollectedThisSeason { get; set; } = 0;
		public float ConservationistTaxBonusThisSeason { get; set; } = 0;
	}
}
