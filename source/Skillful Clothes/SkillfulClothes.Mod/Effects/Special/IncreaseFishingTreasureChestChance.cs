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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Effects.Special
{
    class IncreaseFishingTreasureChestChance : SingleEffect<NoEffectParameters>
    {

        public IncreaseFishingTreasureChestChance()
            : base(NoEffectParameters.Default)
        {
            // --
        }

        public override void Apply(Item sourceItem, EffectChangeReason reason)
        {
            EffectHelper.ModHelper.Events.Display.MenuChanged += Display_MenuChanged;
        }

        private void Display_MenuChanged(object sender, StardewModdingAPI.Events.MenuChangedEventArgs e)
        {
            if (e.NewMenu is BobberBar bobberBar)
            {
                if (!Game1.isFestival())
                {
                    var hasTreasure = EffectHelper.ModHelper.Reflection.GetField<bool>(bobberBar, "treasure");
                    if (!hasTreasure.GetValue())
                    {
                        if (Game1.random.Next(0, 11) <= Game1.player.LuckLevel)
                        {
                            Logger.Debug("Added treasure");
                            var treasureAppearTimer = EffectHelper.ModHelper.Reflection.GetField<float>(bobberBar, "treasureAppearTimer");
                            treasureAppearTimer.SetValue(Game1.random.Next(1000, 3000)); // as in the base game

                            hasTreasure.SetValue(true);
                        }
                    }
                }
            }
        }

        public override void Remove(Item sourceItem, EffectChangeReason reason)
        {
            EffectHelper.ModHelper.Events.Display.MenuChanged -= Display_MenuChanged;
        }

        protected override EffectDescriptionLine GenerateEffectDescription() => new EffectDescriptionLine(EffectIcon.TreasureChest, "Slightly increases chances to find treasure chests when fishing");
    }
}
