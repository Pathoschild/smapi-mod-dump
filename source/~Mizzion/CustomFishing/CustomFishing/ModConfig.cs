/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

namespace CustomFishing
{
   internal class ModConfig
    {
        //Main Config

        //Turns the mod on or off
        public bool Enabled { get; set; } = true;

        //Difficulty of the mod. 0= Normal, 1 = Easy, 2 Hard, 3 Very Hard
        public int Difficulty { get; set; } = 0;

        //Increases the Bobber Bar Height, so that it just about fills the gauge.
        public bool EasyFishing { get; set; } = false;

        //Turns Max Cast on or off
        public bool AlwaysMaxCast { get; set; } = false;

        //Base bobber height (Works only if AutoFishing = false),Used in calculating the bar height
        public int BaseBobberBarHeight { get; set; } = 100;

        //Turns Instant Biting on or off.
        public bool InstantFishBite { get; set; } = false;

        //Turns Instant Catching on or off
        public bool InstantCatch { get; set; } = false;

        //Enable or Disable Perfect Catches
        public bool AlwaysPerfect { get; set; } = false;

        //Enables Infinite Tackle
        public bool InfiniteTackle { get; set; } = false;

        //Enables or Disables whether a treasure chest will show up.
        public bool AlwaysSpawnTreasure { get; set; } = false;

        //Enables or Disables whether the player always gets treasure while fishing
        public bool AlwaysGetTreasure { get; set; } = false;
    }
}
