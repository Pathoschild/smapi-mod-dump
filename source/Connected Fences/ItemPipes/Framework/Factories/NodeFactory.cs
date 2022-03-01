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
using ItemPipes.Framework.Nodes;
using ItemPipes.Framework.Model;
using ItemPipes.Framework.Util;
using ItemPipes.Framework.Nodes.ObjectNodes;


namespace ItemPipes.Framework.Factories
{
    public static class NodeFactory
    {

        public static Node CreateElement(Vector2 position, GameLocation location, StardewValley.Object obj)
        {
            if (obj.name.Equals("Extractor Pipe"))
            {
                return new ExtractorPipeNode(position, location, obj);
            }
            else if (obj.name.Equals("Gold Extractor Pipe"))
            {
                return new GoldExtractorPipeNode(position, location, obj);
            }
            else if (obj.name.Equals("Iridium Extractor Pipe"))
            {
                return new IridiumExtractorPipeNode(position, location, obj);
            }
            else if (obj.name.Equals("Inserter Pipe"))
            {
                return new InserterPipeNode(position, location, obj);
            }
            else if (obj.name.Equals("Polymorphic Pipe"))
            {
                return new PolymorphicPipeNode(position, location, obj);
            }
            else if (obj.name.Equals("Filter Pipe"))
            {
                return new FilterPipeNode(position, location, obj);
            }
            else if (obj.name.Equals("Iron Pipe"))
            {
                return new IronPipeNode(position, location, obj);
            }
            else if (obj.name.Equals("Gold Pipe"))
            {
                return new GoldPipeNode(position, location, obj);
            }
            else if (obj.name.Equals("Iridium Pipe"))
            {
                return new IridiumPipeNode(position, location, obj);
            }
            else if (obj.name.Equals("Chest"))
            {
                return new ChestContainerNode(position, location, obj);
            }
            else if (obj.name.Equals("Mini-Fridge"))
            {
                return new ChestContainerNode(position, location, obj);
            }
            else if (obj.name.Equals("P.P.M."))
            {
                return new PPMNode(position, location, obj);
            }
            else
            {
                Printer.Info($"Node creation for {obj.Name} failed.");
                return null;
            }
        }
        public static Node CreateElement(Vector2 position, GameLocation location, StardewValley.Buildings.Building building)
        {
            if (building.GetType().Equals(typeof(ShippingBin)))
            {
                return new ShippingBinContainerNode(position, location, null, building);
            }
            else
            {
                return null;
            }
        }
    }
}
