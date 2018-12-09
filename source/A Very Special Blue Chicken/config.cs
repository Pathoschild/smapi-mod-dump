namespace AVerySpecialBlueChicken
{
    internal class Config
    {
        public double PercentageChance { get; set; }
        public int HeartLevel { get; set; }
        public Config()
        {
            PercentageChance = 0.1;
            HeartLevel = 3;
        }
    }
}
