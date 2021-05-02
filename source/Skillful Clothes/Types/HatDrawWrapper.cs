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
using Microsoft.Xna.Framework.Graphics;
using SkillfulClothes.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Types
{
    class HatDrawWrapper : StardewValley.Objects.Hat
    {
        public IEffect Effect { get; private set; }
        public StardewValley.Objects.Hat WrappedItem { get; private set; }

        public void Assign(StardewValley.Objects.Hat hat, IEffect effect)
        {
            if (WrappedItem == hat) return;
            Effect = effect;

            this._GetOneFrom(hat);            
        }

        public override void drawTooltip(SpriteBatch spriteBatch, ref int x, ref int y, SpriteFont font, float alpha, StringBuilder overrideText)
        {
            base.drawTooltip(spriteBatch, ref x, ref y, font, alpha, overrideText);

            if (Effect != null)
            {
                EffectHelper.drawTooltip(Effect, spriteBatch, ref x, ref y, font, alpha, overrideText);
            }
        }

        public override Point getExtraSpaceNeededForTooltipSpecialIcons(SpriteFont font, int minWidth, int horizontalBuffer, int startingHeight, StringBuilder descriptionText, string boldTitleText, int moneyAmountToDisplayAtBottom)
        {
            Point p = base.getExtraSpaceNeededForTooltipSpecialIcons(font, minWidth, horizontalBuffer, startingHeight, descriptionText, boldTitleText, moneyAmountToDisplayAtBottom);
            if (Effect != null)
            {
                var newP = EffectHelper.getExtraSpaceNeededForTooltipSpecialIcons(Effect, font, minWidth, horizontalBuffer, startingHeight, descriptionText, boldTitleText, moneyAmountToDisplayAtBottom);
                newP.X = Math.Max(p.X, newP.X);
                newP.Y = Math.Max(p.Y, newP.Y);
                return newP;
            }
            else
            {
                return p;
            }
        }
    }
}
