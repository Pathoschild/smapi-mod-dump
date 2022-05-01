/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillfulClothes.Effects.SharedParameters;
using SkillfulClothes.Types;
using StardewValley;

namespace SkillfulClothes.Effects.Skills
{
    class IncreaseSpeed : SingleEffect<AmountEffectParameters>
    {
        // Implementation adapted from CJBCheatsMenu
        // W ehave to use a speed buff since otherwis ethe player's speed value will be reset by the game occasionally

        private Item currentSourceItem;

        private readonly int BuffBaseID = 84632498;

        public int BuffId => BuffBaseID + Parameters.Amount;

        protected override EffectDescriptionLine GenerateEffectDescription() => new EffectDescriptionLine(EffectIcon.Speed, $"+{Parameters.Amount} Speed");

        public IncreaseSpeed(AmountEffectParameters parameters) 
            : base(parameters)
        {
            // --
        }

        public IncreaseSpeed(int speed)
            : base(AmountEffectParameters.With(speed))
        {
            // --
        }       

        public override void Apply(Item sourceItem, EffectChangeReason reason)
        {
            currentSourceItem = sourceItem;

            EffectHelper.ModHelper.Events.GameLoop.UpdateTicked -= GameLoop_UpdateTicked;
            EffectHelper.ModHelper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
        }

        private void GameLoop_UpdateTicked(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            // ignore in cutscenes
            if (Game1.eventUp)
                return;

            // ignore if walking
            bool running = Game1.player.running;
            bool runEnabled = running || Game1.options.autoRun != Game1.isOneOfTheseKeysDown(Game1.GetKeyboardState(), Game1.options.runButton); // auto-run enabled and not holding walk button, or disabled and holding run button
            if (!runEnabled)
                return;
            
            Buff buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == BuffId);
            if (buff == null)
            {
                Game1.buffsDisplay.addOtherBuff(
                    buff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, speed: Parameters.Amount, 0, 0, minutesDuration: 1, source: currentSourceItem?.Name ?? "", displaySource: currentSourceItem?.DisplayName ?? "") { which = BuffId }
                );
            }
            buff.millisecondsDuration = 50;
        }

        public override void Remove(Item sourceItem, EffectChangeReason reason)
        {
            EffectHelper.ModHelper.Events.GameLoop.UpdateTicked -= GameLoop_UpdateTicked;

            Buff buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == BuffId);
            if (buff != null)
            {
                Game1.buffsDisplay.removeOtherBuff(BuffId);
            }            
        }

       
    }
}
