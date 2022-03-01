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
using System.Xml.Serialization;
using StardewValley;

namespace ItemPipes.Framework.Items.Objects
{
    [XmlType("Mods_sergiomadd.ItemPipes_GoldPipeItem")]
    public class GoldPipeItem : ConnectorPipeItem
    {
        public GoldPipeItem() : base()
        {
            Name = "Gold Pipe";
            IDName = "GoldPipe";
            Description = "Type: Connector Pipe\nThe link between IO pipes. It moves items at 5 tiles/1 second.";
            base.Init();
        }

        public GoldPipeItem(Vector2 position) : base(position)
        {
            Name = "Gold Pipe";
            IDName = "GoldPipe";
            Description = "Type: Connector Pipe\nThe link between IO pipes. It moves items at 5 tiles/1 second.";
            base.Init();
        }
    }
}
