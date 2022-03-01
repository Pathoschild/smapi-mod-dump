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
using SkillfulClothes.Types;
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
    abstract class AttributeRegenEffect : SingleEffect<AttributeRegenParameters>
    {
        int standingStillForSeconds;
        Vector2? previousLocation;        

        protected abstract string AttributeName { get; }

        public virtual EffectIcon Icon => EffectIcon.None;        

        protected abstract int GetMaxValue(Farmer farmer);

        protected abstract int GetCurrentValue(Farmer farmer);

        protected abstract void SetCurrentValue(Farmer farmer, int newValue);

        protected override EffectDescriptionLine GenerateEffectDescription() => new EffectDescriptionLine(Icon, $"Regenerate {AttributeName} when standing still");

        public AttributeRegenEffect(AttributeRegenParameters parameters)
            : base(parameters)
        {
            // --
        }

        public override void Apply(Item sourceItem, EffectChangeReason reason)
        {
            EffectHelper.ModHelper.Events.GameLoop.OneSecondUpdateTicked += GameLoop_OneSecondUpdateTicked;
        }

        private void GameLoop_OneSecondUpdateTicked(object sender, StardewModdingAPI.Events.OneSecondUpdateTickedEventArgs e)
        {
            // Todo: check that player is not in menu/cutscene
            if (previousLocation == null)
            {
                previousLocation = Game1.player.getStandingPosition();
                return;
            }

            // being in a menu / cutscene does not count as standing still        
            // and it is only a challenge if the player can move                            
            if (previousLocation.Value.Equals(Game1.player.getStandingPosition()))
            {
                if (Context.IsPlayerFree)
                {
                    standingStillForSeconds++;
                    // Logger.Debug($"Standing still for {standingStillForSeconds} s");

                    previousLocation = Game1.player.getStandingPosition();

                    if (standingStillForSeconds >= Parameters.SecondsToStandStill)
                    {
                        if ((standingStillForSeconds - Parameters.SecondsToStandStill) % Parameters.RegenIntervalSeconds == 0)
                        {
                            int currValue = GetCurrentValue(Game1.player);
                            int max = GetMaxValue(Game1.player);

                            if (!Game1.player.isGlowing && currValue < max)
                            {
                                Game1.player.startGlowing(Parameters.GlowColor, false, 1 / 60.0f);
                            }                            
                            
                            if (currValue < max)
                            {
                                Logger.Debug($"{AttributeName} regen +{Parameters.RegenAmount}");
                                SetCurrentValue(Game1.player, Math.Min(currValue + Parameters.RegenAmount, max));                                
                            }

                            if (currValue >= max && Game1.player.isGlowing && Game1.player.glowingColor == Parameters.GlowColor)
                            {
                                Game1.player.stopGlowing();
                            }
                        }
                    }
                }
            }
            else
            {
                if (standingStillForSeconds > 0 && Game1.player.isGlowing && Game1.player.glowingColor == Parameters.GlowColor)
                {
                    Game1.player.stopGlowing();
                }

                standingStillForSeconds = 0;
                previousLocation = Game1.player.getStandingPosition();
            }
        }

        public override void Remove(Item sourceItem, EffectChangeReason reason)
        {
            EffectHelper.ModHelper.Events.GameLoop.OneSecondUpdateTicked -= GameLoop_OneSecondUpdateTicked;
        }       
    }

    public class AttributeRegenParameters : IEffectParameters
    {
        public int SecondsToStandStill { get; set; } = 1;
        public int RegenIntervalSeconds { get; set; } = 1;
        public int RegenAmount { get; set; } = 10;

        public Color GlowColor { get; set; } = Color.Green;

        public AttributeRegenParameters()
        {
            // --
        }

        public static AttributeRegenParameters With(Color glowColor, int secondsToStandStill, int regenIntervalSeconds = 1, int regenAmount = 1)
        {
            return new AttributeRegenParameters() { GlowColor = glowColor, SecondsToStandStill = secondsToStandStill, RegenIntervalSeconds = regenIntervalSeconds, RegenAmount = regenAmount };
        }
    }
}
