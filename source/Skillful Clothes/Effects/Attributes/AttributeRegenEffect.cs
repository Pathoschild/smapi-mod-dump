/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Effects.Attributes
{
    /// <summary>
    /// Slowly restores an attribute's value if the player does not move for certain time
    /// </summary>     
    abstract class AttributeRegenEffect : SingleEffect
    {
        Farmer farmer;
        int secondsToStandStill;
        int standingStillForSeconds;
        int regenIntervalSeconds;
        int regenAmount;

        Vector2? previousLocation;

        protected abstract string AttributeName { get; }

        public virtual EffectIcon Icon => EffectIcon.None;        

        protected abstract int GetMaxValue(Farmer farmer);

        protected abstract int GetCurrentValue(Farmer farmer);

        protected abstract void SetCurrentValue(Farmer farmer, int newValue);

        protected override EffectDescriptionLine GenerateEffectDescription() => new EffectDescriptionLine(Icon, $"Regenerate {AttributeName} when standing still");

        public AttributeRegenEffect(int secondsToStandStill, int regenIntervalSeconds = 1, int regenAmount = 1)
        {
            this.secondsToStandStill = secondsToStandStill;
            this.regenIntervalSeconds = regenIntervalSeconds;
            this.regenAmount = regenAmount;
        }

        public override void Apply(Farmer farmer)
        {
            this.farmer = farmer;
            EffectHelper.ModHelper.Events.GameLoop.OneSecondUpdateTicked += GameLoop_OneSecondUpdateTicked;
        }

        private void GameLoop_OneSecondUpdateTicked(object sender, StardewModdingAPI.Events.OneSecondUpdateTickedEventArgs e)
        {
            // Todo: check that player is not in menu/cutscene
            if (previousLocation == null)
            {
                previousLocation = farmer.getStandingPosition();
                return;
            }

            // being in a menu / cutscene does not count as standing still        
            // and it is only a challenge if the player can move                            
            if (previousLocation.Value.Equals(farmer.getStandingPosition()))
            {
                if (Context.IsPlayerFree)
                {
                    standingStillForSeconds++;
                    // Logger.Debug($"Standing still for {standingStillForSeconds} s");

                    previousLocation = farmer.getStandingPosition();

                    if (standingStillForSeconds >= secondsToStandStill)
                    {
                        if ((standingStillForSeconds - secondsToStandStill) % regenIntervalSeconds == 0)
                        {
                            int currValue = GetCurrentValue(farmer);
                            int max = GetMaxValue(farmer);
                            if (currValue < max)
                            {
                                Logger.Debug($"{AttributeName} regen +{regenAmount}");
                                SetCurrentValue(farmer, Math.Min(currValue + regenAmount, max));
                            }
                        }
                    }
                }
            }
            else
            {
                standingStillForSeconds = 0;
            }
        }

        public override void Remove(Farmer farmer)
        {
            if (this.farmer == farmer)
            {
                EffectHelper.ModHelper.Events.GameLoop.OneSecondUpdateTicked -= GameLoop_OneSecondUpdateTicked;
            }
        }        
    }
}
