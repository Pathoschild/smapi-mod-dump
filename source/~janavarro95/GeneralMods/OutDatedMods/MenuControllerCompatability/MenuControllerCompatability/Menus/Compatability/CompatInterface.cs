/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compatability
{
  public  interface CompatInterface
    {

       Dictionary<Point,Rectangle> ComponentList
        {
            get;
            set;
        }

        void Compatability();
        void Update();
        void moveLeft();
        void moveRight();
        void moveUp();
        void moveDown();
        void resize();
     //   void updateMouse(Point p);
       // Point getComponentCenter(Rectangle r);

    }

}
