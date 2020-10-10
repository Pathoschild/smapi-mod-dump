/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMP
**
*************************************************/

using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Reflection;

namespace StardewValleyMP.Vanilla
{
    class NewShippingMenu : ShippingMenu
    {
        public NewShippingMenu(List<Item> items)
        :   base( items )
        {
        }

        public override void update(GameTime time)
        {
            base.update(time);

            FieldInfo f = typeof(ShippingMenu).GetField("saveGameMenu", BindingFlags.Instance | BindingFlags.NonPublic);
            SaveGameMenu menu = (SaveGameMenu)f.GetValue(this);
            if ( menu != null && menu.GetType() != typeof(NewSaveGameMenu) )
                f.SetValue(this, new NewSaveGameMenu());
        }
    }
}
