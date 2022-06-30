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
using ItemPipes.Framework.Items;
using ItemPipes.Framework.Util;
using SObject = StardewValley.Object;
using ItemPipes.Framework.Items.Objects;
using ItemPipes.Framework.Items.Tools;



namespace ItemPipes.Framework.Factories
{
    public static class ItemFactory
    {
        public static CustomObjectItem CreateItem(string name)
        {
            if (name.Equals("extractorpipe"))
            {
                return new ExtractorPipeItem();
            }
            else if (name.Equals("goldextractorpipe"))
            {
                return new GoldExtractorPipeItem();
            }
            else if (name.Equals("iridiumextractorpipe"))
            {
                return new IridiumExtractorPipeItem();
            }
            else if (name.Equals("inserterpipe"))
            {
                return new InserterPipeItem();
            }
            else if (name.Equals("polymorphicpipe"))
            {
                return new PolymorphicPipeItem();
            }
            else if (name.Equals("filterpipe"))
            {
                return new FilterPipeItem();
            }
            else if (name.Equals("ironpipe"))
            {
                return new IronPipeItem();
            }
            else if (name.Equals("goldpipe"))
            {
                return new GoldPipeItem();
            }
            else if (name.Equals("iridiumpipe"))
            {
                return new IridiumPipeItem();
            }
            else if (name.Equals("pipo"))
            {
                return new PIPOItem();
            }
            else
            {
                Printer.Warn($"Item creation for {name} failed.");
                return null;
            }
        }

        public static CustomObjectItem CreateItemFromID(int id)
        {
            switch (id)
            {
                case 222560:
                    return new IronPipeItem();
                case 222561:
                    return new GoldPipeItem();
                case 222562:
                    return new IridiumPipeItem();
                case 222563:
                    return new ExtractorPipeItem();
                case 222564:
                    return new GoldExtractorPipeItem();
                case 222565:
                    return new IridiumExtractorPipeItem();
                case 222566:
                    return new InserterPipeItem();
                case 222567:
                    return new PolymorphicPipeItem();
                case 222568:
                    return new FilterPipeItem();
                case 222660:
                    return new PIPOItem();
                default:
                    return null;
            }
        }

        public static CustomToolItem CreateTool(string name)
        {
            if (name.Equals("wrench"))
            {
                return new WrenchItem();
            }
            else
            {
                Printer.Warn($"Item creation for {name} failed.");
                return null;
            }
        }

        public static CustomObjectItem CreateObject(Vector2 position, string name)
        {
            if (name.Equals("extractorpipe"))
            {
                return new ExtractorPipeItem(position);
            }
            else if (name.Equals("goldextractorpipe"))
            {
                return new GoldExtractorPipeItem(position);
            }
            else if (name.Equals("iridiumextractorpipe"))
            {
                return new IridiumExtractorPipeItem(position);
            }
            else if (name.Equals("inserterpipe"))
            {
                return new InserterPipeItem(position);
            }
            else if (name.Equals("polymorphicpipe"))
            {
                return new PolymorphicPipeItem(position);
            }
            else if (name.Equals("filterpipe"))
            {
                return new FilterPipeItem(position);
            }
            else if (name.Equals("ironpipe"))
            {
                return new IronPipeItem(position);
            }
            else if (name.Equals("goldpipe"))
            {
                return new GoldPipeItem(position);
            }
            else if (name.Equals("iridiumpipe"))
            {
                return new IridiumPipeItem(position);
            }
            else if (name.Equals("pipo"))
            {
                return new PIPOItem(position);
            }
            else
            {
                Printer.Warn($"Object creation for {name} failed.");
                return null;
            }
        }
        public static CustomToolItem CreateToolLegacy(string name)
        {
            if (name.Equals("Wrench"))
            {
                return new WrenchItem();
            }
            else
            {
                Printer.Warn($"Item creation for {name} failed.");
                return null;
            }
        }
        public static CustomObjectItem CreateItemLegacy(string name)
        {
            if (name.Equals("ExtractorPipe"))
            {
                return new ExtractorPipeItem();
            }
            else if (name.Equals("GoldExtractorPipe"))
            {
                return new GoldExtractorPipeItem();
            }
            else if (name.Equals("IridiumExtractorPipe"))
            {
                return new IridiumExtractorPipeItem();
            }
            else if (name.Equals("InserterPipe"))
            {
                return new InserterPipeItem();
            }
            else if (name.Equals("PolymorphicPipe"))
            {
                return new PolymorphicPipeItem();
            }
            else if (name.Equals("FilterPipe"))
            {
                return new FilterPipeItem();
            }
            else if (name.Equals("IronPipe"))
            {
                return new IronPipeItem();
            }
            else if (name.Equals("GoldPipe"))
            {
                return new GoldPipeItem();
            }
            else if (name.Equals("IridiumPipe"))
            {
                return new IridiumPipeItem();
            }
            else if (name.Equals("PIPO"))
            {
                return new PIPOItem();
            }
            else
            {
                Printer.Warn($"Item creation for {name} failed.");
                return null;
            }
        }

        public static CustomObjectItem CreateObjectLegacy(Vector2 position, string name)
        {
            if (name.Equals("ExtractorPipe"))
            {
                return new ExtractorPipeItem(position);
            }
            else if (name.Equals("GoldExtractorPipe"))
            {
                return new GoldExtractorPipeItem(position);
            }
            else if (name.Equals("IridiumExtractorPipe"))
            {
                return new IridiumExtractorPipeItem(position);
            }
            else if (name.Equals("InserterPipe"))
            {
                return new InserterPipeItem(position);
            }
            else if (name.Equals("PolymorphicPipe"))
            {
                return new PolymorphicPipeItem(position);
            }
            else if (name.Equals("FilterPipe"))
            {
                return new FilterPipeItem(position);
            }
            else if (name.Equals("IronPipe"))
            {
                return new IronPipeItem(position);
            }
            else if (name.Equals("GoldPipe"))
            {
                return new GoldPipeItem(position);
            }
            else if (name.Equals("IridiumPipe"))
            {
                return new IridiumPipeItem(position);
            }
            else if (name.Equals("PIPO"))
            {
                return new PIPOItem(position);
            }
            else
            {
                Printer.Warn($"Object creation for {name} failed.");
                return null;
            }
        }
    }
}
