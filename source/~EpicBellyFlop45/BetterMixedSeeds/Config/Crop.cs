namespace BetterMixedSeeds.Config
{
    public class Crop
    {
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public int Chance { get; set; }

        public Crop(string name, bool enabled, int chance)
        {
            Name = name;
            Enabled = enabled;
            Chance = chance;
        }
    }
}
