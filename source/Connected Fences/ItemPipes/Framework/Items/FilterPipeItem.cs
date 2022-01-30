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
using System.Xml.Serialization;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;
using ItemPipes.Framework.Model;
using ItemPipes.Framework.Nodes;

namespace ItemPipes.Framework.Items
{
    [XmlType("Mods_sergiomadd.ItemPipes_FilterPipeItem")]
    public class FilterPipeItem : InputItem
    {
        public FilterPipeItem() : base()
        {
			Name = "Filter Pipe";
			IDName = "FilterPipe";
			Description = "Type: Input Pipe\nInserts items into an adjacent container, it filters only the items already on the Filter Pipe Inventory. Right click the Filter Pipe to open the Inventory.";
			LoadTextures();
		}

        public FilterPipeItem(Vector2 position) : base(position)
        {
			Name = "Filter Pipe";
			IDName = "FilterPipe";
			Description = "Type: Input Pipe\nInserts items into an adjacent container, it filters only the items already on the Filter Pipe Inventory. Right click the Filter Pipe to open the Inventory.";
			LoadTextures();
		}

		public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
		{
			if (justCheckingForActivity)
			{
				return true;
			}
			DataAccess DataAccess = DataAccess.GetDataAccess();
			if (Game1.didPlayerJustRightClick(ignoreNonMouseHeldInput: true))
			{
				List<Node> nodes = DataAccess.LocationNodes[Game1.currentLocation];
				FilterPipeNode pipe = (FilterPipeNode)nodes.Find(n => n.Position.Equals(TileLocation));
				pipe.Chest.ShowMenu();
				return false;
			}
			if (!justCheckingForActivity && who != null && who.currentLocation.isObjectAtTile(who.getTileX(), who.getTileY() - 1) && who.currentLocation.isObjectAtTile(who.getTileX(), who.getTileY() + 1) && who.currentLocation.isObjectAtTile(who.getTileX() + 1, who.getTileY()) && who.currentLocation.isObjectAtTile(who.getTileX() - 1, who.getTileY()) && !who.currentLocation.getObjectAtTile(who.getTileX(), who.getTileY() - 1).isPassable() && !who.currentLocation.getObjectAtTile(who.getTileX(), who.getTileY() + 1).isPassable() && !who.currentLocation.getObjectAtTile(who.getTileX() - 1, who.getTileY()).isPassable() && !who.currentLocation.getObjectAtTile(who.getTileX() + 1, who.getTileY()).isPassable())
			{
				this.performToolAction(null, who.currentLocation);
			}
			return true;
		}
	}
}
