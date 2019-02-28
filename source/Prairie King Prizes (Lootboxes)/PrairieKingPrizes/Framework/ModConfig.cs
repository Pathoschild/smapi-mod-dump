namespace PrairieKingPrizes.Framework
{
    internal class ModConfig
    {
        public int BasicBoxCost { get; set; } = 10;
        public int PremiumBoxCost { get; set; } = 50;
        public bool RequireGameCompletion { get; set; } = false;
        public bool AlternateCoinMethod { get; set; } = false;
    }
}
