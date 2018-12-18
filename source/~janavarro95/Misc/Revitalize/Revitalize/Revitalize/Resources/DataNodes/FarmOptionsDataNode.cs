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
