namespace ReRegeneration
{
    class RegenConfig
    {
        public float staminaRegenPerSecond { get; set; } = 1.0f;
        public int staminaIdleSeconds { get; set; } = 5;
        public float maxStaminaRatioToRegen { get; set; } = 0.8f;
        public float scaleStaminaRegenRateTo { get; set; } = 0.5f;
        public float scaleStaminaRegenDelayTo { get; set; } = 0.5f;

        public float healthRegenPerSecond { get; set; } = 0.1f;
        public int healthIdleSeconds { get; set; } = 10;
        public float maxHealthRatioToRegen { get; set; } = 0.5f;
        public float scaleHealthRegenRateTo { get; set; } = 0.75f;
        public float scaleHealthRegenDelayTo { get; set; } = 0.75f;

        public bool percentageMode { get; set; } = false;
        public float regenWhileActiveRate { get; set; } = 0.8f;
        public float regenWhileRunningRate { get; set; } = 0.2f;
        public float endExhaustionAt { get; set; } = 0.25f;
        public float exhuastionPenalty { get; set; } = 0.9f;
        public float shortenDelayWhenStillBy { get; set; } = 0.5f;
        public float lengthenDelayWhenRunningBy { get; set; } = 0.5f;

        public float timeInterval { get; set; } = 0.25f;
        public bool verboseMode { get; set; } = false;
    }
}
