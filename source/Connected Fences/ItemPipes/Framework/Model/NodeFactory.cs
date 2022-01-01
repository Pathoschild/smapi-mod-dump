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
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using ItemPipes.Framework.Objects;

namespace ItemPipes.Framework.Model
{
    public static class NodeFactory
    {
        public static Node CreateElement(Vector2 position, GameLocation location, StardewValley.Object obj)
        {
            if (obj.name.Equals("ExtractorPipe"))
            {
                return new ExtractorPipe(position, location, obj);
            }
            else if (obj.name.Equals("InserterPipe"))
            {
                return new InserterPipe(position, location, obj);
            }
            else if (obj.name.Equals("PolymorphicPipe"))
            {
                return new PolymorphicPipe(position, location, obj);
            }
            else if (obj.name.Equals("FilterPipe"))
            {
                return new FilterPipe(position, location, obj);
            }
            else if (obj.name.Equals("ConnectorPipe"))
            {
                return new ConnectorPipe(position, location, obj);
            }
            else if (obj.name.Equals("Chest"))
            {
                return new ChestContainer(position, location, obj);
            }
            else if (obj.name.Equals("Mini-Fridge"))
            {
                return new ChestContainer(position, location, obj);
            }
            else if (obj.name.Equals("Invisibilizer"))
            {
                return new Invisibilizer(position, location, obj);
            }
            else
            {
                return null;
            }
        }
        public static Node CreateElement(Vector2 position, GameLocation location, StardewValley.Buildings.Building building)
        {
            if (building.GetType().Equals(typeof(ShippingBin)))
            {
                return new ShippingBinContainer(position, location, null, building);
            }
            else
            {
                return null;
            }
        }
    }
}
