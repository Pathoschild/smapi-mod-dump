/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Revitalize.Framework.Minigame.SeasideScrambleMinigame.Interfaces;
using StardewValley;

namespace Revitalize.Framework.Minigame.SeasideScrambleMinigame.SSCStatusEffects
{
    /// <summary>
    /// TODO: Have it so that this determines where to draw the status effects.
    /// </summary>
    public class StatusEffectManager
    {
        /// <summary>
        /// All of the current status effects afflicting the entity.
        /// </summary>
        public List<StatusEffect> statusEffects;
        /// <summary>
        /// Used to clean up old status effects.
        /// </summary>
        private List<StatusEffect> garbageCollection;

        public Dictionary<Type, double> resistences;

        private Random random
        {
            get
            {
                return Game1.random;
            }
        }

        public object owner;

        /// <summary>
        /// Constructor.
        /// </summary>
        public StatusEffectManager(object Owner)
        {
            this.statusEffects = new List<StatusEffect>();
            this.garbageCollection = new List<StatusEffect>();
            this.resistences = new Dictionary<Type, double>();
            this.setResistence(typeof(StatusEffect), 0.0d);
            this.owner = Owner;
        }

        /// <summary>
        /// Adds a status effect to the manager.
        /// </summary>
        /// <param name="effect"></param>
        public void addStatusEffect(StatusEffect effect, bool ignoreEffectChance = false)
        {
            if (effect == null) return;
            if (ignoreEffectChance == false)
            {
                double rng = this.random.NextDouble();
                rng += this.getResistence(effect);
                if (effect.chanceToAfflict < rng) return;
            }

            if (this.ContainsStatusEffect(effect) && effect.canStack)
            {
                this.statusEffects.Add(effect);
            }
            if (this.ContainsStatusEffect(effect) && effect.canStack == false)
            {
                return;
            }
            if (this.ContainsStatusEffect(effect) == false)
            {
                this.statusEffects.Add(effect);
            }

            //SO MUCH DANGER!
            foreach(StatusEffect other in this.statusEffects)
            {
                if (effect.canReset)
                {
                    other.resetDuration();
                }
            }

        }

        /// <summary>
        /// Removes the given status effect.
        /// </summary>
        /// <param name="effect"></param>
        public void removeStatusEffect(StatusEffect effect)
        {
            this.garbageCollection.Add(effect);
        }

        /// <summary>
        /// Updates all staus effects.
        /// </summary>
        /// <param name="time"></param>
        public void update(GameTime time)
        {
            foreach (StatusEffect effect in this.garbageCollection)
            {
                this.statusEffects.Remove(effect);
            }
            foreach (StatusEffect effect in this.statusEffects)
            {
                effect.update(time);
                if (effect.shouldTrigger)
                {
                    this.applyStatusEffect(effect);
                    effect.resetFrequencyTimer();
                }
                if (effect.IsFinished) this.removeStatusEffect(effect); //this.garbageCollection.Add(effect);
            }
        }

        protected virtual void applyStatusEffect(StatusEffect effect)
        {
            if (this.owner is ISSCLivingEntity)
            {
                this.applyEffectToLivingEntity(effect);
            }
            if(this.owner is SSCPlayer)
            {
                this.applyEffectToPlayer(effect);
            }

        }
        /// <summary>
        /// Applies more general status effects to entities.
        /// </summary>
        /// <param name="effect"></param>
        protected virtual void applyEffectToLivingEntity(StatusEffect effect)
        {
            if (effect is StatusEffect)
            {

            }
            if(effect is SE_Burn)
            {
                (this.owner as ISSCLivingEntity).CurrentHealth -= (effect as SE_Burn).damage;
            }
        }

        /// <summary>
        /// Applies player specific status effects.
        /// </summary>
        /// <param name="effect"></param>
        protected virtual void applyEffectToPlayer(StatusEffect effect)
        {
            if (effect is StatusEffect)
            {
                //Do nothing.
            }
        }


        /// <summary>
        /// Draw all status effects to the screen.
        /// </summary>
        /// <param name="b"></param>
        public void draw(SpriteBatch b)
        {
            foreach (StatusEffect effect in this.statusEffects)
            {
                effect.draw(b);
            }
        }

        /// <summary>
        /// Draw all status effects to the screen.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="Scale"></param>
        public void draw(SpriteBatch b, float Scale)
        {
            foreach (StatusEffect effect in this.statusEffects)
            {
                effect.draw(b, Scale, 0f);
            }
        }

        /// <summary>
        /// Checks to see if the entity is afflicted with a given status effect.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool ContainsStatusEffect(Type t)
        {
            foreach (StatusEffect effect in this.statusEffects)
            {
                if (effect.GetType() == t) return true;
            }
            return false;
        }

        /// <summary>
        /// Checks to see if the entity is afflicted with a given status effect.
        /// </summary>
        /// <param name="effect"></param>
        /// <returns></returns>
        public bool ContainsStatusEffect(StatusEffect effect)
        {
            return this.ContainsStatusEffect(effect.GetType());
        }

        /// <summary>
        /// Gets a resistence for the given status effect.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public double getResistence(Type t)
        {
            if (this.resistences.ContainsKey(t))
            {
                return this.resistences[t];
            }
            else return 0.0d;
        }

        public double getResistence(StatusEffect effect)
        {
            return this.getResistence(effect.GetType());
        }

        public void setResistence(StatusEffect effect, double amount)
        {
            this.setResistence(effect.GetType(), amount);
        }

        private void setResistence(Type t, double amount)
        {
            if (this.resistences.ContainsKey(t))
            {
                this.resistences[t] = amount;
            }
            else
            {
                this.resistences.Add(t, amount);
            }
        }

    }
}
