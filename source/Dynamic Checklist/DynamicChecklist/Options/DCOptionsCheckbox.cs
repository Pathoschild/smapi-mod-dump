/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gunnargolf/DynamicChecklist
**
*************************************************/

namespace DynamicChecklist.Options
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using StardewValley.Menus;

    public class DCOptionsCheckbox : OptionsCheckbox
    {
        private int whichOption2; // Needed so that the receiveLeftClick base function doesn't change some game option
        private ModConfig config;

        public DCOptionsCheckbox(string label, int whichOption, ModConfig config, int x = -1, int y = -1)
            : base(label, 9999, x, y)
        {
            this.whichOption2 = whichOption;
            this.config = config;
            switch (whichOption)
            {
                case 3:
                    this.isChecked = config.ShowAllTasks;
                    break;
                case 4:
                    this.isChecked = config.AllowMultipleOverlays;
                    break;
                case 5:
                    this.isChecked = config.ShowArrow;
                    break;
                case 6:
                    this.isChecked = config.ShowOverlay;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public override void receiveLeftClick(int x, int y)
        {
            base.receiveLeftClick(x, y);
            switch (this.whichOption2)
            {
                case 3:
                    this.config.ShowAllTasks = this.isChecked;
                    break;
                case 4:
                    this.config.AllowMultipleOverlays = this.isChecked;
                    break;
                case 5:
                    this.config.ShowArrow = this.isChecked;
                    break;
                case 6:
                    this.config.ShowOverlay = this.isChecked;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
