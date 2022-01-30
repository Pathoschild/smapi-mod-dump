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

namespace ItemPipes.Framework.Items
{
    [XmlType("Mods_sergiomadd.ItemPipes_InserterPipeItem")]

    public class InserterPipeItem : InputItem
    {
        public InserterPipeItem() : base()
        {
            Name = "Inserter Pipe";
            IDName = "InserterPipe";
            Description = "Type: Input Pipe\nInserts items into an adjacent container, it doesn't filter items.";
            LoadTextures();
        }
        public InserterPipeItem(Vector2 position) : base(position)
        {
            Name = "Inserter Pipe";
            IDName = "InserterPipe";
            Description = "Type: Input Pipe\nInserts items into an adjacent container, it doesn't filter items.";
            LoadTextures();
        }
    }
}
