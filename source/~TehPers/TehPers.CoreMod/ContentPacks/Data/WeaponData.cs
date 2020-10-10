/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using TehPers.CoreMod.Api.Items;

namespace TehPers.CoreMod.ContentPacks.Data {
    internal class WeaponData : ItemData {
        public WeaponType Type { get; set; } = WeaponType.Sword;
        public int MinDamage { get; set; } = 0;
        public int MaxDamage { get; set; } = 0;
        public float Knockback { get; set; } = 1;
        public int Speed { get; set; } = 0;
        public int Accuracy { get; set; } = 0;
        public int Defense { get; set; } = 0;
        public int AreaOfEffect { get; set; } = 1;
        public float CritChance { get; set; } = 0.02f;
        public float CritMultiplier { get; set; } = 3;
    }
}