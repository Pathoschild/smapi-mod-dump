using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using SObject = StardewValley.Object;

namespace VersatileGrabber
{
	class GrabberController
	{
		public static readonly Dictionary<Tuple<GameLocation, Vector2>, VersatileGrabber> versatileGrabbers = new Dictionary<Tuple<GameLocation, Vector2>, VersatileGrabber>();


		/// <summary>
		/// Checks if the item should be converted to a versatile grabber
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public static bool ItemShouldBeVersatileGrabber(SObject item)
		{
			// Don't convert if it already is to avoid an infinite loop
			if (item is VersatileGrabber)
				return false;

			// Check if the item is a big craftable
			if (item.bigCraftable.Value)
			{
				if (item.ParentSheetIndex == ModEntry.GrabberID)
				{
					return true;
				}
			}

			return false;
		}
	}
}
