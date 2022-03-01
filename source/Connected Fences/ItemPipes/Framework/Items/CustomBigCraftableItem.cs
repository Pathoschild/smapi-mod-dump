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
using ItemPipes.Framework.Nodes;
using ItemPipes.Framework.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using System.Xml.Serialization;
using StardewValley.Network;
using SObject = StardewValley.Object;
using ItemPipes.Framework.Factories;
using ItemPipes.Framework.Items;


namespace ItemPipes.Framework.Items
{
    public abstract class CustomBigCraftableItem : CustomObjectItem
    {
		public CustomBigCraftableItem() : base()
		{
			bigCraftable.Value = true;
			setOutdoors.Value = true;
			setIndoors.Value = true;
		}

		public CustomBigCraftableItem(Vector2 position) : base(position)
		{
			bigCraftable.Value = true;
			setOutdoors.Value = true;
			setIndoors.Value = true;
		}
	}
}
