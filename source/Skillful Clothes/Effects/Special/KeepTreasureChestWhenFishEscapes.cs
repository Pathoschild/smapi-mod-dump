/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using SkillfulClothes.Types;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Effects.Special
{
    class KeepTreasureChestWhenFishEscapes : SingleEffect
    {
        public override void Apply(Item sourceItem, EffectChangeReason reason)
        {
            EffectHelper.ModHelper.Events.Display.MenuChanged += Display_MenuChanged;
        }

        private void Display_MenuChanged(object sender, StardewModdingAPI.Events.MenuChangedEventArgs e)
        {
#if DEBUG
            if (e.NewMenu is BobberBar bobberBar2)
            {
                var hasTreasure = EffectHelper.ModHelper.Reflection.GetField<bool>(bobberBar2, "treasure");
                if (!hasTreasure.GetValue())
                {
                    Logger.Debug("Added treasure");
                    var treasureAppearTimer = EffectHelper.ModHelper.Reflection.GetField<float>(bobberBar2, "treasureAppearTimer");
                    treasureAppearTimer.SetValue(Game1.random.Next(1000, 3000)); // as in the base game

                    hasTreasure.SetValue(true);
                }
            }
#endif

            if (e.OldMenu is BobberBar bobberBar && Game1.player.CurrentTool is FishingRod fishingRod)
            {                
                var distanceFromCatching = EffectHelper.ModHelper.Reflection.GetField<float>(bobberBar, "distanceFromCatching");
                var treasureCaught = EffectHelper.ModHelper.Reflection.GetField<bool>(bobberBar, "treasureCaught");
                // fish escaped but treasure was caught                
                if (distanceFromCatching.GetValue() < 0.1 && treasureCaught.GetValue())
                {
                    fishingRod.openChestEndFunction(0);
                }                                                            
            }
        }

        public override void Remove(Item sourceItem, EffectChangeReason reason)
        {
            EffectHelper.ModHelper.Events.Display.MenuChanged -= Display_MenuChanged;
        }

        protected override EffectDescriptionLine GenerateEffectDescription() => new EffectDescriptionLine(EffectIcon.TreasureChest, "Keep treasure chests even when fish escape");    
    }
}
