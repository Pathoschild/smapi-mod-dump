/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revitalize.Resources.DataNodes
{
    class SerializerDataNode
    {
        public Dictionaries.ser serialize;
        public Dictionaries.par parse;
        public Dictionaries.world worldObj;

        public SerializerDataNode(Dictionaries.ser ser, Dictionaries.par par,Dictionaries.world wor)
        {
            serialize = ser;
            parse = par;
            worldObj = wor;
        }
    }
}
