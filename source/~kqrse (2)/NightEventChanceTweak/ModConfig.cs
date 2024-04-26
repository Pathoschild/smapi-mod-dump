/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/


using StardewModdingAPI;

namespace NightEventChanceTweak
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public bool CumulativeChance { get; set; } = true;
        public float FairyChance { get; set; } = 1;
        public float WitchChance { get; set; } = 1;
        public float MeteorChance { get; set; } = 1;
        public float OwlChance { get; set; } = 0.5f;
        public float CapsuleChance { get; set; } = 0.8f;

    }
}
