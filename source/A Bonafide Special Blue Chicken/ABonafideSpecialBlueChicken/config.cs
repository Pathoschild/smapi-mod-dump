/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/rikai/ABonafideSpecialBlueChicken
**
*************************************************/

namespace ABonafideSpecialBlueChicken
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
