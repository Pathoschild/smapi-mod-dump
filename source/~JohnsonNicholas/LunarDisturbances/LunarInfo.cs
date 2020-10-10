/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JohnsonNicholas/SDVMods
**
*************************************************/

namespace TwilightShards.LunarDisturbances
{
    public class LunarInfo
    {
        public bool FullMoonThisSeason { get; set; } = false;
        public string CurrentSeason { get; set; } = "";
        public bool IsEclipseTomorrow { get; set; } = false;

        public LunarInfo()
        {
            FullMoonThisSeason = false;
            CurrentSeason = "";
            IsEclipseTomorrow = false;
        }

        public LunarInfo(bool f, string s, bool i)
        {
            FullMoonThisSeason = f;
            CurrentSeason = s;
            IsEclipseTomorrow = i;
        }

        public LunarInfo(LunarInfo l)
        {
            FullMoonThisSeason = l.FullMoonThisSeason;
            CurrentSeason = l.CurrentSeason;
            IsEclipseTomorrow = l.IsEclipseTomorrow;
        }
    }
}
