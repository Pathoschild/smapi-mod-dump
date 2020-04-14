namespace HAS_Tweaks
{
    internal class ModConfig
    {
        /// <summary>The default amount of health the player starts with.</summary>
        public int StartingHealth { get; set; } = 100;

        /// <summary>Health gained from Stardrops.</summary>
        public int StarDropHealth { get; set; } = 0;

        /// <summary>Health gained from Iridium Snake Milk.</summary>
        public int SnakeMilkHealth { get; set; } = 25;

        /// <summary>Health gained from combat skill level.</summary>
        public int CombatLevelHealth { get; set; } = 5;

        /// <summary>The default amount of stamina the player starts with.</summary>
        public int StartingStamina { get; set; } = 270;

        /// <summary>Stamina gained from Stardrops.</summary>
        public int StarDropStamina { get; set; } = 34;

        /// <summary>Stamina gained from Iridium Snake Milk.</summary>
        public int SnakeMilkStamina { get; set; } = 0;

        /// <summary>Stamina gained from non-combat skill levels.</summary>
        public int SkillLevelStamina { get; set; } = 0;

        /// <summary>Whether to regenerate stamina by a constant amount.</summary>
        public bool RegenStaminaConstant { get; set; } = false;

        /// <summary>The amount of stamina to constantly regenerate per second.</summary>
        public float RegenStaminaConstantAmountPerSecond { get; set; } = 0.0f;

        /// <summary>Whether to regenerate stamina by percentage.</summary>
        public bool RegenStaminaPercent { get; set; } = false;

        /// <summary>The percent of stamina to constantly regenerate per second.</summary>
        public float RegenStaminaPercentPerSecond { get; set; } = 0.0f;

        /// <summary>Whether to only regenerate health while standing still.</summary>
        public bool RegenStaminaMoving { get; set; } = false;

        /// <summary>Multiplier for health regen while moving -- keep less than 1.0 if you don't want to regen more health while moving.</summary>
        public float RegenStaminaMovingMult { get; set; } = 0.0f;

        /// <summary>The amount of time the player must stand still to regenerate stamina.</summary>
        public int RegenStaminaStillTimeRequiredMS { get; set; } = 5000;

        /// <summary>Whether you regenerate stamina while fishing.</summary>
        public bool RegenStaminaFishing { get; set; } = false;

        /// <summary>Whether you regenerate stamina during an event.</summary>
        public bool RegenStaminaEvent { get; set; } = false;

        /// <summary>Whether to regenerate health by a constant amount.</summary>
        public bool RegenHealthConstant { get; set; } = false;

        /// <summary>The amount of health to constantly regenerate per second.</summary>
        public float RegenHealthConstantAmountPerSecond { get; set; } = 0.0f;

        /// <summary>Whether to regenerate health by percentage.</summary>
        public bool RegenHealthPercent { get; set; } = false;

        /// <summary>The percent of health to constantly regenerate per second.</summary>
        public float RegenHealthPercentPerSecond { get; set; } = 0.0f;

        /// <summary>Whether to only regenerate health while standing still.</summary>
        public bool RegenHealthMoving { get; set; } = false;

        /// <summary>Multiplier for health regen while moving -- keep less than 1.0 if you don't want to regen more health while moving.</summary>
        public float RegenHealthMovingMult { get; set; } = 0.0f;

        /// <summary>The amount of time (in milliseconds) the player must stand still to regenerate health.</summary>
        public int RegenHealthStillTimeRequiredMS { get; set; } = 5000;

        /// <summary>Whether you regenerate health while fishing.</summary>
        public bool RegenHealthFishing { get; set; } = false;

        /// <summary>Whether you regenerate health during an event.</summary>
        public bool RegenHealthEvent { get; set; } = false;
    }
}