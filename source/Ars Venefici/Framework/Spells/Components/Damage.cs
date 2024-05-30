/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using ArsVenefici.Framework.Interfaces;
using ArsVenefici.Framework.Interfaces.Spells;
using ArsVenefici.Framework.Util;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buffs;
using StardewValley.Enchantments;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static StardewValley.Minigames.TargetGame;
using static System.Net.Mime.MediaTypeNames;

namespace ArsVenefici.Framework.Spells.Components
{
    public class Damage : AbstractComponent
    {
        //private readonly Func<Character, DamageSource> damageSourceFunction;
        private readonly Func<Character, double> damage;
        private readonly Predicate<Character> failIf;

        private string id;

        private int manaCost;

        public Damage(string id, int manaCost, Func<Character, double> damage, Predicate<Character> failIf) : base(new SpellPartStats(SpellPartStatType.DAMAGE), new SpellPartStats(SpellPartStatType.HEALING))
        {
            this.id = id;

            this.manaCost = manaCost;
            this.damage = damage;
            this.failIf = failIf;
        }

        public Damage(string id, int manaCost, Func<double> damage, Predicate<Character> failIf): this(id, manaCost, e => damage(), failIf)
        {

        }

        public Damage(string id, int manaCost, Func<Character, double> damage): this(id, manaCost, damage, e => false)
        {

        }

        public Damage(string id, int manaCost, Func<double> damage): this(id, manaCost, e => damage(), e => false)
        {

        }

        public override string GetId()
        {
            return id;
        }

        public override SpellCastResult Invoke(ModEntry modEntry, ISpell spell, IEntity caster, GameLocation gameLocation, List<ISpellModifier> modifiers, CharacterHitResult target, int index, int ticksUsed)
        {
            //modEntry.Monitor.Log("Invoking Spell Part " + GetId(), StardewModdingAPI.LogLevel.Info);

            Character living = target.GetCharacter();

            if (failIf != null)
            {
                if (failIf.Invoke(living))
                    return new SpellCastResult(SpellCastResultType.EFFECT_FAILED);
            }

            float damage = (float)this.damage.Invoke(living);

            //if (living is Farmer && living != caster && !((ServerLevel)level).getServer().isPvpAllowed() && damage > 0)
            //    return new SpellCastResult(SpellCastResultType.EFFECT_FAILED);

            if (living is Monster monster)
            {
                if (damage < 0)
                {
                    damage = SpellHelper.Instance().GetModifiedStat(damage, new SpellPartStats(SpellPartStatType.HEALING), modifiers, spell, caster, target, index);
                }

                damage = SpellHelper.Instance().GetModifiedStat(damage, new SpellPartStats(SpellPartStatType.DAMAGE), modifiers, spell, caster, target, index);

                if(caster.entity is Farmer f)
                {
                    //if (gameLocation.damageMonster(((Character)caster.entity).GetBoundingBox(), (int)damage, (int)(damage * (1f + f.buffs.AttackMultiplier)), false, f))
                    //{
                    //    return new SpellCastResult(SpellCastResultType.SUCCESS);
                    //}

                    //if (gameLocation.damageMonster(monster.GetBoundingBox(), (int)damage, (int)(damage * (1f + f.buffs.AttackMultiplier)), false, f))
                    //{
                    //    return new SpellCastResult(SpellCastResultType.SUCCESS);
                    //}

                    return gameLocation.damageMonster(monster.GetBoundingBox(), (int)damage, (int)(damage * (1f + f.buffs.AttackMultiplier)), false, f) ? new SpellCastResult(SpellCastResultType.SUCCESS) : new SpellCastResult(SpellCastResultType.EFFECT_FAILED);
                }

            }

            if (living is Farmer farmer)
            {
                if (damage < 0)
                {
                    damage = SpellHelper.Instance().GetModifiedStat(damage, new SpellPartStats(SpellPartStatType.HEALING), modifiers, spell, caster, target, index);
                }

                damage = SpellHelper.Instance().GetModifiedStat(damage, new SpellPartStats(SpellPartStatType.DAMAGE), modifiers, spell, caster, target, index);
                
                farmer.health -= (int)damage;

                return new SpellCastResult(SpellCastResultType.SUCCESS);
            }

            //return living.hurt(damageSourceFunction.apply(caster), damage) ? new SpellCastResult(SpellCastResultType.SUCCESS) : new SpellCastResult(SpellCastResultType.EFFECT_FAILED);

            return new SpellCastResult(SpellCastResultType.EFFECT_FAILED);
        }

        //public void DamageFarmer(Farmer who, int minDamage, int maxDamage)
        //{
        //    bool flag3 = false;

        //    int num3;
        //    if (maxDamage >= 0)
        //    {
        //        num3 = Game1.random.Next(minDamage, maxDamage + 1);
        //        if (who != null && Game1.random.NextDouble() < (double)(critChance + (float)who.LuckLevel * (critChance / 40f)))
        //        {
        //            flag3 = true;
        //            who.currentLocation.playSound("crit");
        //            if (who.hasTrinketWithID("IridiumSpur"))
        //            {
        //                BuffEffects buffEffects = new BuffEffects();
        //                buffEffects.Speed.Value = 1f;
        //                who.applyBuff(new Buff("iridiumspur", null, Game1.content.LoadString("Strings\\1_6_Strings:IridiumSpur_Name"), who.getFirstTrinketWithID("IridiumSpur").GetEffect().general_stat_1 * 1000, Game1.objectSpriteSheet_2, 76, buffEffects, false));
        //            }
        //        }

        //        num3 = (flag3 ? ((int)((float)num3 * critMultiplier)) : num3);
        //        num3 = Math.Max(1, num3 + ((who != null) ? (who.Attack * 3) : 0));
        //        if (who != null && who.professions.Contains(24))
        //        {
        //            num3 = (int)Math.Ceiling((float)num3 * 1.1f);
        //        }

        //        if (who != null && who.professions.Contains(26))
        //        {
        //            num3 = (int)Math.Ceiling((float)num3 * 1.15f);
        //        }

        //        if (who != null && flag3 && who.professions.Contains(29))
        //        {
        //            num3 = (int)((float)num3 * 2f);
        //        }

        //        //if (who != null)
        //        //{
        //        //    foreach (BaseEnchantment enchantment in who.enchantments)
        //        //    {
        //        //        enchantment.OnCalculateDamage(monster, this, who, ref num3);
        //        //    }
        //        //}
        //    }
        //}

        public override SpellCastResult Invoke(ModEntry modEntry, ISpell spell, IEntity caster, GameLocation gameLocation, List<ISpellModifier> modifiers, TerrainFeatureHitResult target, int index, int ticksUsed)
        {
            return new SpellCastResult(SpellCastResultType.EFFECT_FAILED);
        }

        public override int ManaCost()
        {
            return manaCost;
        }
    }
}
