/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile;

namespace Revitalize.Resources.DataNodes
{
   public class FarmOptionsDataNode
    {
       public ClickableTextureComponent clicky;
       public Map map;

       public FarmOptionsDataNode(ClickableTextureComponent component,Map m)
        {
            clicky = component;
            map = m;
          
        }



    }
}
