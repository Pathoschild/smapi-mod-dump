/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using StardewValley;
using TehPers.CoreMod.Api.ContentLoading;
using TehPers.CoreMod.Api.Drawing.Sprites;

namespace TehPers.CoreMod.Api.Items {
    public class ModWeapon : ModItem, IModWeapon {
        public int MinDamage { get; }
        public int MaxDamage { get; }
        public float Knockback { get; }
        public int Speed { get; }
        public int Precision { get; }
        public int Defense { get; }
        public WeaponType Type { get; }
        public int AreaOfEffect { get; }
        public float CritChance { get; }
        public float CritMultiplier { get; }

        public ModWeapon(ICoreTranslationHelper translationHelper, string rawName, ISprite sprite, WeaponType type, int minDamage, int maxDamage, float knockback = 1, int speed = 0, int precision = 0, int defense = 0, int areaOfEffect = 1, float critChance = 0.02f, float critMultiplier = 3) : base(translationHelper, rawName, sprite) {
            this.MinDamage = minDamage;
            this.MaxDamage = maxDamage;
            this.Knockback = knockback;
            this.Speed = speed;
            this.Precision = precision;
            this.Defense = defense;
            this.Type = type;
            this.AreaOfEffect = areaOfEffect;
            this.CritChance = critChance;
            this.CritMultiplier = critMultiplier;
        }

        public string GetRawWeaponInformation() {
            ICoreTranslation displayName = this.TranslationHelper.Get($"item.{this.RawName}").WithDefault($"item.{this.RawName}");
            ICoreTranslation description = this.TranslationHelper.Get($"item.{this.RawName}.description").WithDefault("No description available.");
            const int minMineLevel = -1; // TODO
            const int someMineThing = -1; // TODO: Utility.getUncommonItemForThisMineLevel
            return string.Join("/", displayName, description, this.MinDamage, this.MaxDamage, this.Knockback, this.Speed, this.Precision, this.Defense, (int) this.Type, minMineLevel, someMineThing, this.AreaOfEffect, this.CritChance, this.CritMultiplier);
        }
    }
}