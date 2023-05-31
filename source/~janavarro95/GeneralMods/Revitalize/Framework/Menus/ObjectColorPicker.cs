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
using Microsoft.Xna.Framework;
using Omegasis.Revitalize.Framework.World.Objects;
using Omegasis.Revitalize.Framework.World.Objects.Interfaces;
using Omegasis.Revitalize.Framework.World.WorldUtilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace Omegasis.Revitalize.Framework.Menus
{
    public class ObjectColorPicker : DiscreteColorPicker
    {
        public ObjectColorPicker(int xPosition, int yPosition, int startingColor = 0, StardewValley.Object itemToDrawColored = null) : base(xPosition, yPosition, startingColor, itemToDrawColored)
        {
            if (this.itemToDrawColored is ICustomModObject)
            {
                ICustomModObject customModObject = (ICustomModObject)(this.itemToDrawColored);
                this.colorSelection = customModObject.basicItemInformation.colorPickerSelectionNumber.Value;
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (!this.visible)
            {
                return;
            }
            base.receiveLeftClick(x, y, playSound);
            Rectangle area = new Rectangle(base.xPositionOnScreen + IClickableMenu.borderWidth / 2, base.yPositionOnScreen + IClickableMenu.borderWidth / 2, 36 * this.totalColors, 28);
            if (area.Contains(x, y))
            {
                this.colorSelection = (x - area.X) / 36;
                try
                {
                    SoundUtilities.PlaySound(Constants.Enums.StardewSound.coin);
                }
                catch (Exception)
                {
                }
                if (this.itemToDrawColored is ICustomModObject)
                {
                    ICustomModObject customModObject = (ICustomModObject)(this.itemToDrawColored);
                    //Can change the colors to pick by overriding the following line.
                    customModObject.basicItemInformation.DrawColor = this.getColorFromArraySelection(this.colorSelection);
                    customModObject.basicItemInformation.colorPickerSelectionNumber.Value = this.colorSelection;
                }
            }
        }

        public virtual Color getColorFromArraySelection(int selection) => selection switch
        {
            2 => new Color(119, 191, 255),
            1 => new Color(85, 85, 255),
            3 => new Color(0, 170, 170),
            4 => new Color(0, 234, 175),
            5 => new Color(0, 170, 0),
            6 => new Color(159, 236, 0),
            7 => new Color(255, 234, 18),
            8 => new Color(255, 167, 18),
            9 => new Color(255, 105, 18),
            10 => new Color(255, 0, 0),
            11 => new Color(135, 0, 35),
            12 => new Color(255, 173, 199),
            13 => new Color(255, 117, 195),
            14 => new Color(172, 0, 198),
            15 => new Color(143, 0, 255),
            16 => new Color(89, 11, 142),
            17 => new Color(64, 64, 64),
            18 => new Color(100, 100, 100),
            19 => new Color(200, 200, 200),
            20 => new Color(254, 254, 254),
            _ => Color.White,
        };


    }
}
