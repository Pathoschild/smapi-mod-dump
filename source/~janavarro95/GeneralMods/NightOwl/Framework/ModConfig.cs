namespace Omegasis.NightOwl.Framework
{
    /// <summary>The mod configuration.</summary>
    internal class ModConfig
    {
        /// <summary>Whether lighting should transition to day from 2am to 6am. If <c>false</c>, the world will stay dark until the player passes out or goes to bed.</summary>
        public bool MorningLightTransition { get; set; } = true;

        /// <summary>Whether the player can stay up until 6am.</summary>
        public bool StayUp { get; set; } = true;

        /// <summary>Whether to remove the mail received for collapsing like 'we charged X gold for your health fees'.</summary>
        public bool SkipCollapseMail { get; set; } = true;

        /// <summary>Whether to restore the player's position after they collapse.</summary>
        public bool KeepPositionAfterCollapse { get; set; } = true;

        /// <summary>Whether to restore the player's money after they collapse (i.e. prevent the automatic deduction).</summary>
        public bool KeepMoneyAfterCollapse { get; set; } = true;

        /// <summary>Whether to keep stamina as-is after the player collapses.</summary>
        public bool KeepHealthAfterCollapse { get; set; } = true;

        /// <summary>Whether to keep stamina as-is after the player collapses.</summary>
        public bool KeepStaminaAfterCollapse { get; set; } = true;


        /// <summary>Whether or not to use the internal NightFish asset editor. When false, it will just use the Fish.xnb file.</summary>
        public bool UseInternalNightFishAssetEditor { get; set; } = true;
    }
}
