/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Minigames;

namespace ResetTerrainFeaturesRedux.Framework.Menu
{
    public class CheckBoxes : MenuComponent
    {
        public static Rectangle UncheckedSourceRectangle = new Rectangle(227, 425, 9, 9);
        
        public static Rectangle CheckedSourceRectangle = new Rectangle(236, 425, 9, 9);

        public const int PixelsWide = 9;

        public bool IsChecked;

        public string Which;

        public List<CheckBoxes> CheckedToBeDisabledWhen = new List<CheckBoxes>();


        public CheckBoxes(string name, string which, int x = -1, int y = -1, List<CheckBoxes> toDisable = null) : base(
            name, x, y, 36, 36)
        {
            this.Which = which;
            this.IsChecked = Generator.GeneratorOptions.ContainsKey(this.Which)
                ? Generator.GeneratorOptions[this.Which]
                : false;
            this.CheckedToBeDisabledWhen = toDisable;
        }

        public void Enable()
        {
            this.Disabled = false;
            this.IsChecked = false;
            Generator.GeneratorOptions[this.Which] = false;
        }

        public void Disable(bool autoCheck = true)
        {
            this.Disabled = true;
            this.IsChecked = autoCheck;
            Generator.GeneratorOptions[this.Which] = false;
        }

        public override void ReceiveLeftClick(int x, int y)
        {
            if (!this.Disabled)
            {
                base.ReceiveLeftClick(x, y);
                this.Check(false);
            }
        }

        public void Check(bool autoCheck = false)
        {
            Game1.playSound("drumkit6");
            this.IsChecked = autoCheck;
            foreach (CheckBoxes checkBoxes in this.CheckedToBeDisabledWhen)
            {
                if (this.IsChecked)
                    checkBoxes.Disable(true);
                else
                    checkBoxes.Enable();
            }
        }

        public override void Draw(SpriteBatch b, int slotX, int slotY)
        {
            var bat = new Vector2((slotX + this.Bound.X), (slotY + this.Bound.Y));
            b.Draw(Game1.mouseCursors, 
                bat, 
                new Rectangle?(this.IsChecked ? OptionsCheckbox.sourceRectChecked : OptionsCheckbox.sourceRectUnchecked), 
                Color.White * (this.Disabled ? 0.5f : 1f), 
                0f, 
                Vector2.Zero, 
                4f, 
                SpriteEffects.None,
                0);
            base.Draw(b, slotX, slotY);
        }
    }
}
