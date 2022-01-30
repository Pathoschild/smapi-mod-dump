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
using ItemPipes.Framework.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System.Xml.Serialization;

namespace ItemPipes.Framework.Items
{
    [XmlType("Mods_sergiomadd.ItemPipes_ExtractorPipe")]
    public class ExtractorPipeItem : OutputItem
    {

        public ExtractorPipeItem() : base()
        {
            State = "unconnected";
            Name = "Extractor Pipe";
            IDName = "ExtractorPipe";
            Description = "Type: Output Pipe\nExtracts items from an adjacent container, and sends them through the network.";
            LoadTextures();
        }

        public ExtractorPipeItem(Vector2 position) : base(position)
        {
            State = "unconnected";
            Name = "Extractor Pipe";
            IDName = "ExtractorPipe";
            Description = "Type: Output Pipe\nExtracts items from an adjacent container, and sends them through the network.";
            LoadTextures();
        }


    }
}
