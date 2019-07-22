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

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buildings;

namespace CompostPestsCultivation
{
    public class CompostingBin : ShippingBin
    {
        private readonly BluePrint bluePrint;
        private readonly Vector2 tileLocation;

        public static CompostingBin FromShippingBin(ShippingBin bin) => new CompostingBin(Composting.GetComposterBlueprint(), new Vector2(bin.tileX, bin.tileY));

        public ShippingBin ToShippingBin() => new ShippingBin(bluePrint, tileLocation);

        public CompostingBin(BluePrint bp, Vector2 tileLocation) : base(bp, tileLocation)
        {
            ModEntry.GetHelper().Reflection.GetField<TemporaryAnimatedSprite>(this, "shippingBinLid").SetValue(null);
            this.bluePrint = bp;
            this.tileLocation = tileLocation;
        }

        public override bool doAction(Vector2 tileLocation, Farmer who)
        {
            if ((int)daysOfConstructionLeft <= 0 && tileLocation.X >= (float)(int)tileX && tileLocation.X <= (float)((int)tileX + 1) && (int)tileLocation.Y == (int)tileY)
            {
                Game1.showGlobalMessage("CompostingBin.doAction");
                if (Game1.activeClickableMenu == null)
                    Game1.activeClickableMenu = new ComposterMenu(this);
            }
            return false;

            //return base.doAction(tileLocation, who);
        }

        public override bool leftClicked()
        {
            return false;
            //return base.leftClicked();
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);
        }

        public override void Update(GameTime time)
        {

        }
    }
}
