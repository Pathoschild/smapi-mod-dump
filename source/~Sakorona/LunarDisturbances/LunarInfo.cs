namespace TwilightShards.LunarDisturbances
{
    public class LunarInfo
    {
        public bool FullMoonThisSeason { get; set; } = false;
        public string CurrentSeason { get; set; } = "";

        public LunarInfo()
        {
            FullMoonThisSeason = false;
            CurrentSeason = "";
        }

        public LunarInfo(bool f, string s)
        {
            FullMoonThisSeason = f;
            CurrentSeason = s;
        }

        public LunarInfo(LunarInfo l)
        {
            FullMoonThisSeason = l.FullMoonThisSeason;
            CurrentSeason = l.CurrentSeason;
        }
    }
}
