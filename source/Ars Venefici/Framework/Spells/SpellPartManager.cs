/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using ArsVenefici.Framework.GUI;
using ArsVenefici.Framework.Interfaces.Spells;
using ArsVenefici.Framework.Spells.Components;
using ArsVenefici.Framework.Spells.Modifiers;
using ArsVenefici.Framework.Spells.Shape;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buffs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Effect = ArsVenefici.Framework.Spells.Components.Effect;

namespace ArsVenefici.Framework.Spells
{
    public class SpellPartManager
    {
        public Dictionary<string, ISpellPart> spellParts = new Dictionary<string, ISpellPart>();
        private ModEntry modEntry;

        public SpellPartManager(ModEntry modEntry) 
        {
            this.modEntry = modEntry;
            PopluateDictionary();
        }

        public void PopluateDictionary()
        {
            AddShapes();
            AddComonents();
            AddModifiers();
        }

        private void AddShapes()
        {
            Self self = new Self();
            Projectile projectile = new Projectile();
            Touch touch = new Touch();
            EtherialTouch etherialTouch = new EtherialTouch();
            AoE aoE = new AoE();
            Zone zone = new Zone();

            spellParts.Add(self.GetId(), self);
            spellParts.Add(projectile.GetId(), projectile);
            spellParts.Add(touch.GetId(), touch);
            spellParts.Add(etherialTouch.GetId(), etherialTouch);
            spellParts.Add(aoE.GetId(), aoE);
            spellParts.Add(zone.GetId(), zone);
        }

        private void AddComonents()
        {
            Dig dig = new Dig(modEntry);
            Plow plow = new Plow(modEntry);
            Grow grow = new Grow();
            Harvest harvest = new Harvest();
            CreateWater createWater = new CreateWater();
            Explosion explosion = new Explosion();
            Blink blink = new Blink();
            Light light = new Light(modEntry.Helper.Multiplayer.GetNewID);

            Heal heal = new Heal();
            LifeDrain lifeDrain = new LifeDrain();
            LifeTap lifeTap = new LifeTap();

            // -> e instanceof Player p ? p.damageSources().playerAttack(p) : e.damageSources().mobAttack(e), Config.SERVER.DAMAGE)
            //Func<double> physicalDamageValue = () => 5.0;
            Func<double> physicalDamageValue = () => 5.0 * (Game1.player.CombatLevel + 1);
            Damage physicalDamage = new Damage("physical_damage", 25, physicalDamageValue);

            Buff hasteBuff = new Buff(
                id: "HeyImAmethyst.ArsVenifici_Haste",
                displayName: modEntry.Helper.Translation.Get($"spellpart.haste.name"),
                iconTexture: Game1.buffsIcons,
                iconSheetIndex: 9,
                duration: 30_000, // 30 seconds
                effects: new BuffEffects()
                {
                    Speed = { 2 } // shortcut for buff.Speed.Value = 10
                }
            );

            Effect haste = new Effect("haste", 30, hasteBuff);

            spellParts.Add(createWater.GetId(), createWater);
            spellParts.Add(heal.GetId(), heal);
            spellParts.Add(physicalDamage.GetId(), physicalDamage);
            spellParts.Add(dig.GetId(), dig);
            spellParts.Add(plow.GetId(), plow);
            spellParts.Add(grow.GetId(), grow);
            spellParts.Add(harvest.GetId(), harvest);
            spellParts.Add(lifeDrain.GetId(), lifeDrain);
            spellParts.Add(lifeTap.GetId(), lifeTap);
            spellParts.Add(explosion.GetId(), explosion);
            spellParts.Add(blink.GetId(), blink);
            spellParts.Add(light.GetId(), light);
            spellParts.Add(haste.GetId(), haste);
        }

        private void AddModifiers()
        {
            GenericSpellModifier damage = new GenericSpellModifier().AddStatModifier(new SpellPartStats(SpellPartStatType.DAMAGE), DefaultSpellPartStatModifier.Add(5f));
            //GenericSpellModifier damage = new GenericSpellModifier().addStatModifier(new SpellPartStats(SpellPartStatType.DAMAGE), DefaultSpellPartStatModifier.add(50f));
            damage.SetId("damage");

            //GenericSpellModifier range = new GenericSpellModifier().addStatModifier(new SpellPartStats(SpellPartStatType.RANGE), DefaultSpellPartStatModifier.multiply(4f));
            GenericSpellModifier range = new GenericSpellModifier().AddStatModifier(new SpellPartStats(SpellPartStatType.RANGE), DefaultSpellPartStatModifier.Add(1f));
            range.SetId("range");

            GenericSpellModifier bounce = new GenericSpellModifier().AddStatModifier(new SpellPartStats(SpellPartStatType.BOUNCE), DefaultSpellPartStatModifier.Add(2f));
            bounce.SetId("bounce");

            GenericSpellModifier piercing = new GenericSpellModifier().AddStatModifier(new SpellPartStats(SpellPartStatType.PIERCING), DefaultSpellPartStatModifier.COUNTING);
            piercing.SetId("piercing");

            GenericSpellModifier velocity = new GenericSpellModifier().AddStatModifier(new SpellPartStats(SpellPartStatType.SPEED), DefaultSpellPartStatModifier.AddMultipliedBase(0.5f));
            velocity.SetId("velocity");

            GenericSpellModifier healing = new GenericSpellModifier().AddStatModifier(new SpellPartStats(SpellPartStatType.HEALING), DefaultSpellPartStatModifier.Multiply(2f));
            healing.SetId("healing");

            GenericSpellModifier duration = new GenericSpellModifier().AddStatModifier(new SpellPartStats(SpellPartStatType.DURATION), DefaultSpellPartStatModifier.Multiply(2f));
            duration.SetId("duration");

            GenericSpellModifier miningPower = new GenericSpellModifier().AddStatModifier(new SpellPartStats(SpellPartStatType.POWER), DefaultSpellPartStatModifier.Add(1f));
            miningPower.SetId("mining_power");

            spellParts.Add(damage.GetId(), damage);
            spellParts.Add(range.GetId(), range);
            spellParts.Add(bounce.GetId(), bounce);
            spellParts.Add(piercing.GetId(), piercing);
            spellParts.Add(velocity.GetId(), velocity);
            spellParts.Add(healing.GetId(), healing);
            spellParts.Add(duration.GetId(), duration);
            spellParts.Add(miningPower.GetId(), miningPower);
        }
    }
}
