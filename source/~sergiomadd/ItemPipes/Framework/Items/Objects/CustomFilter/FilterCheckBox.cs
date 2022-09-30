/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Menus;
using ItemPipes.Framework.Items.CustomFilter;
using ItemPipes.Framework.Util;
using Microsoft.Xna.Framework;
using MaddUtil;

namespace ItemPipes.Framework.Items.Objects.CustomFilter
{
    public class FilterCheckBox : OptionsCheckbox
    {
        public Filter parentFilter { get; set; }
        public string Name { get; set; }

        public FilterCheckBox(string label, string name, int whichOption, Filter filter, int x = -1, int y = -1) : base(label, x, y, whichOption)
        {
            Name = name;
            bounds = new Rectangle(x, y, 36, 36);
            parentFilter = filter;
            isChecked = Utilities.ToBool(filter.Options["quality"]);
        }

        public override void receiveLeftClick(int x, int y)
        {
            if (!base.greyedOut)
            {
                Game1.playSound("drumkit6");
                selected = this;
                this.isChecked = !this.isChecked;
                parentFilter.Options["quality"] = isChecked.ToString();
                parentFilter.UpdateOption("quality", isChecked.ToString());
                selected = null;
            }
        }
    }
}
