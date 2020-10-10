/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/BattleRoyalley
**
*************************************************/

namespace BattleRoyale
{
    class ModConfig
	{
		public bool ShouldHostParticipate { get; set; } = true;
		public bool KillAllNPCs { get; set; } = true;
		public int TimeInSecondsBetweenRounds { get; set; } = 15;
		public int TimeInMillisecondsBetweenPlayerJoiningAndServerExpectingTheirVersionNumber { get; set; } = 60000;
        public int PlayerLimit { get; set; } = 125;
		public int StormDamagePerSecond { get; set; } = 10;
	}
}
