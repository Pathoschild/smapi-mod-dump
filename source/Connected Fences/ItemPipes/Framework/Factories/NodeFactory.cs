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
            switch(obj.ParentSheetIndex)
            {
                case 222560:
                    return new IronPipeNode(position, location, obj);
                case 222561:
                    return new GoldPipeNode(position, location, obj);
                case 222562:
                    return new IridiumPipeNode(position, location, obj);
                case 222563:
                    return new ExtractorPipeNode(position, location, obj);
                case 222564:
                    return new GoldExtractorPipeNode(position, location, obj);
                case 222565:
                    return new IridiumExtractorPipeNode(position, location, obj);
                case 222566:
                    return new InserterPipeNode(position, location, obj);
                case 222567:
                    return new PolymorphicPipeNode(position, location, obj);
                case 222568:
                    return new FilterPipeNode(position, location, obj);
                case 130:
                    return new ChestContainerNode(position, location, obj);
                case 216:
                    return new ChestContainerNode(position, location, obj);
                case 222660:
                    return new PPMNode(position, location, obj);
                default:
                    throw new Exception($"Node creation for {obj.Name} failed.");
            }
        }
        /*
        public static Node CreateElement(Vector2 position, GameLocation location, StardewValley.Object obj)
        {
            if (obj.name.Equals("ExtractorPipe"))
            {
                return new ExtractorPipeNode(position, location, obj);
            }
            else if (obj.name.Equals("GoldExtractorPipe"))
            {
                return new GoldExtractorPipeNode(position, location, obj);
            }
            else if (obj.name.Equals("IridiumExtractorPipe"))
            {
                return new IridiumExtractorPipeNode(position, location, obj);
            }
            else if (obj.name.Equals("InserterPipe"))
            {
                return new InserterPipeNode(position, location, obj);
            }
            else if (obj.name.Equals("PolymorphicPipe"))
            {
                return new PolymorphicPipeNode(position, location, obj);
            }
            else if (obj.name.Equals("FilterPipe"))
            {
                return new FilterPipeNode(position, location, obj);
            }
            else if (obj.name.Equals("IronPipe"))
            {
                return new IronPipeNode(position, location, obj);
            }
            else if (obj.name.Equals("GoldPipe"))
            {
                return new GoldPipeNode(position, location, obj);
            }
            else if (obj.name.Equals("IridiumPipe"))
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
            else if (obj.name.Equals("PPM"))
            {
                return new PPMNode(position, location, obj);
            }
            else
            {
                throw new Exception($"Node creation for {obj.Name} failed.");
            }
        }
        */

        public static Node CreateElement(Vector2 position, GameLocation location, StardewValley.Buildings.Building building)
        {
            if (building.GetType().Equals(typeof(ShippingBin)))
            {
                return new ShippingBinContainerNode(position, location, null, building);
            }
            else
            {
                throw new Exception($"Node creation for {building.nameOfIndoors} failed.");
            }
        }
    }
}
