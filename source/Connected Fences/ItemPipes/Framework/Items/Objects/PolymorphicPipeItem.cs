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
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using StardewValley;


namespace ItemPipes.Framework.Items.Objects
{
    [XmlType("Mods_sergiomadd.ItemPipes_PolymorphicPipeItem")]

    public class PolymorphicPipeItem : InputPipeItem
    {
        public PolymorphicPipeItem() : base()
        {
            Name = "Polymorphic Pipe";
            IDName = "PolymorphicPipe";
            Description = "Type: Input Pipe\nInserts items into an adjacent container, it filters only the items already on the adjacent container.";
            Init();
        }
        public PolymorphicPipeItem(Vector2 position) : base(position)
        {
            Name = "Polymorphic Pipe";
            IDName = "PolymorphicPipe";
            Description = "Type: Input Pipe\nInserts items into an adjacent container, it filters only the items already on the adjacent container.";
            Init();
        }
    }
}
