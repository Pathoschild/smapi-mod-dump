using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace InventoryCycle
{
	public class ModConfig
	{
		public Keys frontCycleKeyASCIINumber { get; set; }
		public Keys backCycleKeyASCIINumber { get; set; }

		public ModConfig()
		{
			this.frontCycleKeyASCIINumber = Keys.E;
			this.backCycleKeyASCIINumber = Keys.Q;
		}
	}
}
