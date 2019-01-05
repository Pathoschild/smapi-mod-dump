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
