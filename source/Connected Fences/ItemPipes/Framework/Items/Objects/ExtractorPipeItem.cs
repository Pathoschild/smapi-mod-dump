/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using System.Xml.Serialization;

namespace ItemPipes.Framework.Items.Objects
{
    [XmlType("Mods_sergiomadd.ItemPipes_ExtractorPipe")]
    public class ExtractorPipeItem : OutputPipeItem
    {

        public ExtractorPipeItem() : base()
        {
            Name = "Extractor Pipe";
            IDName = "ExtractorPipe";
            Description = "Type: Output Pipe\nExtracts items from an adjacent container, and sends them through the network.";
            Init();
        }

        public ExtractorPipeItem(Vector2 position) : base(position)
        {
            Name = "Extractor Pipe";
            IDName = "ExtractorPipe";
            Description = "Type: Output Pipe\nExtracts items from an adjacent container, and sends them through the network.";
            Init();
        }


    }
}
