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
using Microsoft.Xna.Framework.Graphics;

namespace Revitalize.Framework.Minigame.SeasideScrambleMinigame.SSCStatusEffects
{
    public class StatusEffect
    {
        public StardustCore.Animations.AnimatedSprite sprite;

        public double timeRemainingUntilNextTrigger;
        public double frequency;

        public double timeRemaining;
        public double duration;
        public bool canStack;
        /// <summary>
        /// Can this status effect reset the duration on other effects? Combined with canStack make this REALLY broken.
        /// </summary>
        public bool canReset;
        public double chanceToAfflict;

        public bool IsFinished
        {
            get
            {
                return this.timeRemaining <= 0;
            }
        }

        public bool isAffecting
        {
            get
            {
                return !this.IsFinished;
            }
        }
        /// <summary>
        /// Should this status effect trigger?
        /// </summary>
        public bool shouldTrigger
        {
            get
            {
                return this.timeRemainingUntilNextTrigger <= 0;
            }
        }

        public StatusEffect()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Sprite"></param>
        /// <param name="Duration">The number of milliseconds the effect lasts.</param>
        /// <param name="Frequency">How many milliseconds pass between the effect triggering.</param>
        /// <param name="ChanceToAfflict">The chance this effect has to trigger. Value between 0-1.0</param>
        /// <param name="CanStack">Can this status effect stack?</param>
        public StatusEffect(StardustCore.Animations.AnimatedSprite Sprite,double Duration,double Frequency,double ChanceToAfflict,bool CanStack,bool ResetsSameTimers)
        {
            this.sprite = Sprite;
            this.duration = Duration;
            this.timeRemaining = Duration;
            this.canStack = CanStack;
            this.chanceToAfflict = ChanceToAfflict;
            this.frequency = Frequency;
            this.timeRemainingUntilNextTrigger = 0;
            this.canReset = ResetsSameTimers;
        }

        public virtual void update(Microsoft.Xna.Framework.GameTime Time)
        {
            this.timeRemaining -= Time.ElapsedGameTime.Milliseconds;
            this.timeRemainingUntilNextTrigger -= Time.ElapsedGameTime.Milliseconds;
        }


        /// <summary>
        /// Resets the trigger on when the status effect actually effcts.
        /// </summary>
        public void resetFrequencyTimer()
        {
            this.timeRemainingUntilNextTrigger = this.frequency;
        }

        public void resetDuration()
        {
            this.timeRemaining = this.duration;
        }

        public virtual void draw(SpriteBatch b)
        {
            this.sprite.draw(b);
        }

        public virtual void draw(SpriteBatch b, float Scale, float Depth)
        {
            this.sprite.draw(b, Scale, Depth);
        }

        
    }
}
