/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Effects.Skills
{
    class IncreaseFishingBarByCaughtFish : SingleEffect
    {
        const int maxBobberBarHeight = 450;
        const int maxIncrease = 120;

        Farmer farmer;

        public override void Apply(Farmer farmer)
        {
            this.farmer = farmer;

            EffectHelper.ModHelper.Events.Display.MenuChanged += Display_MenuChanged;
        }

        protected Boolean IsRealFish(int index)
        {
            // Seaweed, Green & White Algea
            if (index == 152 || index == 153 || index == 157) return false;

            // Lobster, Crab, Oyster, Clam Shrimp
            if (index == 715 || index == 717 || index == 723 || index == 372 || index == 720) return false;

            // Cockle, Mussel, Snail, Crayfish, Periwinkle
            if (index == 718 || index == 719 || index == 721 || index == 716 || index == 722) return false;

            return true;
        }

        private void Display_MenuChanged(object sender, StardewModdingAPI.Events.MenuChangedEventArgs e)
        {
            if (e.NewMenu is BobberBar bobberBar)
            {
                var bobberBarHeight = EffectHelper.ModHelper.Reflection.GetField<int>(bobberBar, "bobberBarHeight");
                int currentHeight = bobberBarHeight.GetValue();
                // TODO: total number or number of caught fishes by fish type?

                int fishCaught = 0;
                foreach(var fishidx in farmer.fishCaught.Keys)
                {
                    if (IsRealFish(fishidx))
                    {
                        int[] fishStat = farmer.fishCaught[fishidx];
                        if (fishStat != null && fishStat.Length > 0)
                        {
                            fishCaught += fishStat[0];
                        }
                    }
                }

                int increaseBy = (int)Math.Min(Math.Atan(fishCaught / 500.0) * 100, maxIncrease);
                //int increaseBy = Math.Min(fishCaught / 5, maxIncrease);
                int newHeight = Math.Min(currentHeight + increaseBy, maxBobberBarHeight);
                bobberBarHeight.SetValue(newHeight);
                Logger.Debug($"increased bobberBarHeight from {currentHeight} to {newHeight} (#fish: {fishCaught})");

                // adjust bobber bar starting pos
                EffectHelper.ModHelper.Reflection.GetField<int>(bobberBar, "bobberBarPos").SetValue(568 - newHeight);
            }
        }

        protected override EffectDescriptionLine GenerateEffectDescription() => new EffectDescriptionLine(EffectIcon.SkillFishing, "Increase fishing bar based on caught fish");

        public override void Remove(Farmer farmer)
        {
            EffectHelper.ModHelper.Events.Display.MenuChanged -= Display_MenuChanged;
        }
    }
}
