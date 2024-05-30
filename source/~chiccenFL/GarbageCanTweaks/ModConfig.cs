/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/chiccenFL/StardewValleyMods
**
*************************************************/

namespace GarbageCanTweaks
{
    public class ModConfig
    {

        public bool EnableMod { get; set; } = true;
        public string LootTable { get; set; } = "default";
        public bool EnableBirthday { get; set; } = true;
        public float BirthdayChance { get; set; } = 0.75f;
        public float LootChance { get; set; } = 1f;
        public bool Debug { get; set; } = false;

    }
}
