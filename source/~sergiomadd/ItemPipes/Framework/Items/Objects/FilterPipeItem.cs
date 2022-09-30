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
using StardewValley.Menus;
using Netcode;
using ItemPipes.Framework.Items.CustomFilter;
using SObject = StardewValley.Object;
using ItemPipes.Framework.Util;
using StardewValley.Objects;
using ItemPipes.Framework.Nodes.ObjectNodes;
using MaddUtil;


namespace ItemPipes.Framework.Items.Objects
{
    public class FilterPipeItem : InputPipeItem
    {
		public Filter Filter { get; set; }

		public FilterPipeItem() : base()
        {
		}

        public FilterPipeItem(Vector2 position) : base(position)
        {
			Filter = new Filter(9, this);
		}

		public override SObject SaveObject()
		{
			if (!modData.ContainsKey("ItemPipes")) { modData.Add("ItemPipes", "true"); }
			else { modData["ItemPipes"] = "true"; }
			if (!modData.ContainsKey("Type")) { modData.Add("Type", IDName); }
			else { modData["Type"] = IDName; }
			if (!modData.ContainsKey("Stack")) { modData.Add("Stack", stack.Value.ToString()); }
			else { modData["Stack"] = stack.Value.ToString(); }
			if (!modData.ContainsKey("State")) { modData.Add("State", State); }
			else { modData["State"] = State; }
			if (!modData.ContainsKey("signal")) { modData.Add("signal", Signal); }
			else { modData["signal"] = Signal; }
			if (!modData.ContainsKey("filter_quality")) { modData.Add("filter_quality", Filter.Options["quality"]); }
			else { modData["filter_quality"] = Filter.Options["quality"]; }

			Chest filterChest = null;
			if (Filter != null)
            {
				filterChest = new Chest(true, TileLocation);
				foreach(Item item in Filter.items)
                {
					filterChest.addItem(item);
                }
			}
			
			filterChest.modData = modData;
			return filterChest;
		}



		public override void LoadObject(Item item)
		{
			modData = item.modData;
			stack.Value = Int32.Parse(modData["Stack"]);
			Signal = modData["signal"];
			if(modData.ContainsKey("filter_quality"))
            {
				Filter.Options["quality"] = modData["filter_quality"];
				Filter.UpdateOption("quality", Filter.Options["quality"]);
			}
			if (item is Chest)
            {
				Filter.items = (item as Chest).items;
            }
		}
	
		public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
		{
			if (justCheckingForActivity)
			{
				return true;
			}
			if (Game1.didPlayerJustRightClick(ignoreNonMouseHeldInput: true))
			{
				Filter.ShowMenu();
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
