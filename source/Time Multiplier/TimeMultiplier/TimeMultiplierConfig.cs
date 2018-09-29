namespace TimeMultiplier
{
    public class TimeMultiplierConfig
    {
        public bool Enabled { get; set; }
        public float TimeMultiplier { get; set; }

        public TimeMultiplierConfig()
        {
            Enabled = false;
            TimeMultiplier = 1.00f;
        }
    }
}
