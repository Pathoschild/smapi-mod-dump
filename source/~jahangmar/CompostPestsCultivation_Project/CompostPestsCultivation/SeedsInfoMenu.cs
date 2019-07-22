//Copyright (c) 2019 Jahangmar

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU Lesser General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//GNU Lesser General Public License for more details.

//You should have received a copy of the GNU Lesser General Public License
//along with this program. If not, see <https://www.gnu.org/licenses/>.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

using System.Collections.Generic;
using System.Linq;

namespace CompostPestsCultivation
{
    public class SeedsInfoMenu : LetterViewerMenu
    {
        private StardewValley.Object crop;
        private StardewValley.Object seeds;

        private List<CropTrait> drawTraits;

        public SeedsInfoMenu(StardewValley.Object seeds, StardewValley.Object crop) : base(" ")
        {
            this.seeds = seeds;
            this.crop = crop;

            List<CropTrait> traits = Cultivation.GetTraits(seeds);
            drawTraits = Cultivation.GetTraits(seeds).Where((CropTrait trait) =>
            {
                if (traits.Contains(CropTrait.PestResistanceII) && trait == CropTrait.PestResistanceI)
                    return false;
                if (traits.Contains(CropTrait.WaterII) && trait == CropTrait.WaterI)
                    return false;
                if (traits.Contains(CropTrait.SpeedII) && trait == CropTrait.SpeedI)
                    return false;
                if (traits.Contains(CropTrait.QualityII) && trait == CropTrait.QualityI)
                    return false;

                return true;
            }).ToList();
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);
            if ((int)ModEntry.GetHelper().Reflection.GetField<float>(this, "scale").GetValue() == 1)
            {
                SpriteText.drawString(b, "Traits for " + seeds.DisplayName + ":", xPositionOnScreen + 32, yPositionOnScreen + 32, 999999, width - 64, 999999, 0.75f, 0.865f, false, -1, "", -1/*8 7*/);

                for (int i = 1; i <= drawTraits.Count; i++)
                {
                    SpriteText.drawString(b, Cultivation.GetTraitName(drawTraits[i - 1])+": "+ Cultivation.GetTraitLongDescr(drawTraits[i - 1]), xPositionOnScreen + 32, i * 2 * (48+16) + yPositionOnScreen + 32, 999999, width - 64, 999999, 0.75f, 0.865f, false, -1, "", -1/*8 7*/);
                    //SpriteText.drawString(b, Cultivation.GetTraitLongDescr(drawTraits[i - 1]), xPositionOnScreen + 32 + 32, (i+1) * 2 * 48 + yPositionOnScreen + 32, 999999, width - 64, 999999, 0.75f, 0.865f, false, -1, "", -1/*8 7*/);
                }

                if (drawTraits.Count == 0)
                    SpriteText.drawString(b, ModEntry.GetHelper().Translation.Get("cult.msg_non"), xPositionOnScreen + 32, yPositionOnScreen + 32, 999999, width - 64, 999999, 0.75f, 0.865f, false, -1, "", -1/*8 7*/);
            }

            if (!Game1.options.hardwareCursor)
            {
                b.Draw(Game1.mouseCursors, new Vector2((float)Game1.getMouseX(), (float)Game1.getMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 0, 16, 16), Color.White, 0f, Vector2.Zero, 4f + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
            }
        }
    }
}
