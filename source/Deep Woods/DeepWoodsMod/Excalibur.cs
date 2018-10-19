using StardewValley.Tools;
using static DeepWoodsMod.DeepWoodsSettings;

namespace DeepWoodsMod
{
    class Excalibur
    {
        public static readonly string EXCALIBUR_BASE_NAME = "Excalibur";
        public static readonly int EXCALIBUR_TILE_INDEX = 3;

        public static MeleeWeapon GetOne()
        {
            MeleeWeapon excalibur = new MeleeWeapon(EXCALIBUR_TILE_INDEX)
            {
                BaseName = EXCALIBUR_BASE_NAME,
                description = I18N.ExcaliburDescription,
                DisplayName = I18N.ExcaliburDisplayName
            };
            excalibur.minDamage.Value = Settings.Objects.Excalibur.MinDamage;
            excalibur.maxDamage.Value = Settings.Objects.Excalibur.MaxDamage;
            excalibur.knockback.Value = Settings.Objects.Excalibur.Knockback;
            excalibur.speed.Value = Settings.Objects.Excalibur.Speed;
            excalibur.addedPrecision.Value = Settings.Objects.Excalibur.Precision;
            excalibur.addedDefense.Value = Settings.Objects.Excalibur.Defense;
            excalibur.addedAreaOfEffect.Value = Settings.Objects.Excalibur.AreaOfEffect;
            excalibur.critChance.Value = Settings.Objects.Excalibur.CriticalChance;
            excalibur.critMultiplier.Value = Settings.Objects.Excalibur.CriticalMultiplier;
            return excalibur;
        }
    }
}
