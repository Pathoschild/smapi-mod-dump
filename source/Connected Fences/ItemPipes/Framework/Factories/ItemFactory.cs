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

namespace ItemPipes.Framework.Factories
{
    public static class ItemFactory
    {
        public static SObject CreateItem(SObject obj)
        {
            if (obj.name.Equals("Extractor Pipe"))
            {
                return new ExtractorPipeItem();
            }
            else if (obj.name.Equals("Inserter Pipe"))
            {
                return new InserterPipeItem();
            }
            else if (obj.name.Equals("Polymorphic Pipe"))
            {
                return new PolymorphicPipeItem();
            }
            else if (obj.name.Equals("Filter Pipe"))
            {
                return new FilterPipeItem();
            }
            else if (obj.name.Equals("Iron Pipe"))
            {
                return new IronPipeItem();
            }
            else if (obj.name.Equals("Gold Pipe"))
            {
                return new GoldPipeItem();
            }
            else
            {
                Printer.Info($"Item creation for {obj.Name} failed.");
                return null;
            }
        }

        public static SObject CreateObject(Vector2 position, SObject obj)
        {
            if (obj.name.Equals("Extractor Pipe"))
            {
                return new ExtractorPipeItem(position);
            }
            else if (obj.name.Equals("Inserter Pipe"))
            {
                return new InserterPipeItem(position);
            }
            else if (obj.name.Equals("Polymorphic Pipe"))
            {
                return new PolymorphicPipeItem(position);
            }
            else if (obj.name.Equals("Filter Pipe"))
            {
                return new FilterPipeItem(position);
            }
            else if (obj.name.Equals("Iron Pipe"))
            {
                return new IronPipeItem(position);
            }
            else if (obj.name.Equals("Gold Pipe"))
            {
                return new GoldPipeItem(position);
            }
            else
            {
                Printer.Info($"Object creation for {obj.Name} failed.");
                return null;
            }
        }
    }
}
