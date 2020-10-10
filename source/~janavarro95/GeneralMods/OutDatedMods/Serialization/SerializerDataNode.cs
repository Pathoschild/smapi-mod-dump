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

namespace Serialization
{
   public class SerializerDataNode
    {
        public delegate Item ParsingFunction(string data);
        public delegate void SerializingFunction(Item item);
        public delegate void WorldParsingFunction(Item obj);

        public SerializingFunction serialize;
        public ParsingFunction parse;
        public WorldParsingFunction worldObj;

        public SerializerDataNode(SerializingFunction serializeFunction, ParsingFunction parsingFunction, WorldParsingFunction worldObjectParsingFunction)
        {
            serialize = serializeFunction;
            parse = parsingFunction;
            worldObj = worldObjectParsingFunction;          
        }
    }
}
