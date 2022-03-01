/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

namespace PacifistValley
{
    class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public int MillisecondsPerLove { get; set; } = 300;
        public int DeviceSpeedFactor { get; set; } = 2;
        public int AreaOfKissEffectModifier { get; set; } = 20;
        public bool PreventUnlovedMonsterDamage { get; set; } = true;
        public bool ShowMonsterHeartEmote { get; set; } = true;
        public bool LovedMonstersStillSwarm { get; set; } = false;
        public bool MonstersIgnorePlayer { get; set; } = false;
    }
}
