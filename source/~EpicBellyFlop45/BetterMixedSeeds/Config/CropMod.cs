namespace BetterMixedSeeds.Config
{
    public class CropMod
    {
        public Season Spring { get; set; }
        public Season Summer { get; set; }
        public Season Fall { get; set; }
        public Season Winter { get; set; }

        public CropMod(Season spring, Season summer, Season fall, Season winter)
        {
            Spring = spring;
            Summer = summer;
            Fall = fall;
            Winter = winter;
        }
    }
}
