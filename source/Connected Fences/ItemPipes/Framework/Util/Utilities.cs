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
using System.Linq;
using StardewValley;
using SObject = StardewValley.Object;
using Microsoft.Xna.Framework;

namespace ItemPipes.Framework.Util
{
    public static class Utilities
    {
		public static void DropItem(Item item, Vector2 position, GameLocation location)
        {
            Vector2 convertedPosition = new Vector2(position.X * 64, position.Y * 64);
            Debris itemDebr = new Debris(item, convertedPosition);
            location.debris.Add(itemDebr);
        }

        public static string GetIDName(string name)
        {
            string trimmed = "";
            if (name.Equals("PIPO"))
            {
                trimmed = name.ToLower();
            }
            else
            {
                trimmed = String.Concat(name.Where(c => !Char.IsWhiteSpace(c))).ToLower();
            }
            return trimmed;
        }

        public static string GetIDNameFromType(Type type)
        {
            string name = type.Name;
            string trimmed = name.Substring(0, name.Length - 4).ToLower();
            return trimmed;
        }

        //Keep for 1.6
        /*
        public static Item GetItemFromIndex(string type, int index)
        {
			switch(type)
            {
				case "O":
					break;
            }
			if (item is SObject && (item as SObject).bigCraftable.Value)
			{

			}
			else if (item is Tool)
			{
				Tool tool = (Tool)item;
			}
			//Boots = standard
			else if (item is Boots)
			{

			}
			//rings = standard
			else if (item is Ring)
			{

			}
			else if (item is Hat)
			{

			}
			else if (item is Clothing)
			{

			}
			else
			{

			}
		}
		*/
    }
}
