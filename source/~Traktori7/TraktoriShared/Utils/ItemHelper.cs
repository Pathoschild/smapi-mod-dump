/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Traktori7/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using SObject = StardewValley.Object;


namespace TraktoriShared.Utils
{
	internal static class ItemHelper
	{
		/// <summary>
		/// Gets the item matching the given qualified item ID.
		/// </summary>
		/// <param name="qualifiedItemID">Item's qualified ID in the format "(ItemType)ItemName" or just "ItemName", which defaults to "(O)ItemName".</param>
		/// <returns>The item, or an error object.</returns>
		internal static Item GetItemFromQualifiedItemID(string qualifiedItemID)
		{
			int index = qualifiedItemID.IndexOf(')');

			string itemTypeString;
			string itemName = qualifiedItemID[(index + 1)..];

			if (index > 0)
			{
				itemTypeString = qualifiedItemID[1..index];
			}
			else
			{
				itemTypeString = "O";
			}

			ItemType itemType = GetItemType(itemTypeString);

			return GetItemFromItemName(itemName, itemType);
		}


		/// <summary>
		/// Tries to get the object's index key that matches the given name.
		/// </summary>
		/// <param name="objectName">The object's internal name.</param>
		/// <returns>Returns the key in Game1.objectInformation, or 0 if the object wasn't found.</returns>
		internal static int GetIDFromObjectName(string objectName)
		{
			// Special case handling for stone, since there are multiple entries with the same name
			if (objectName.Equals("Stone"))
			{
				return 390;
			}

			if (GenericHelper.TryGetIndexByName(Game1.objectInformation, objectName, out int index))
			{
				return index;
			}

			return 0;
		}


		/// <summary>
		/// Tries to get the item matching the given name.
		/// </summary>
		/// <param name="itemName">Item's internal name.</param>
		/// <param name="type">Item's item type.</param>
		/// <returns>The item, or an error object.</returns>
		internal static Item GetItemFromItemName(string itemName, ItemType type = ItemType.Object)
		{
			// TODO: Implement a cache for the results in a string-int dictionary?
			// Probably unnecessary and potentially problematic if calling for multiple different values.
			// So far doesn't take too long, but in the future consider a performant variant if need arises.

			Item? returnItem = new SObject(0, 1);
			int index;

			switch (type)
			{
				case ItemType.Object:
					returnItem = new SObject(GetIDFromObjectName(itemName), 1);
					break;
				case ItemType.Ring:
					returnItem = new StardewValley.Objects.Ring(GetIDFromObjectName(itemName));
					break;
				case ItemType.BigCraftable:
					if (GenericHelper.TryGetIndexByName(Game1.bigCraftablesInformation, itemName, out index))
					{
						returnItem = new SObject(Vector2.Zero, index);
					}
					break;
				case ItemType.Hat:
					if (GenericHelper.TryGetIndexByName(Game1.content.Load<Dictionary<int, string>>("Data\\hats"), itemName, out index))
					{
						returnItem = new StardewValley.Objects.Hat(index);
					}
					break;
				case ItemType.Boots:
					if (GenericHelper.TryGetIndexByName(Game1.content.Load<Dictionary<int, string>>("Data\\Boots"), itemName, out index))
					{
						returnItem = new StardewValley.Objects.Boots(index);
					}
					break;
				case ItemType.Weapon:
					if (GenericHelper.TryGetIndexByName(Game1.content.Load<Dictionary<int, string>>("Data\\weapons"), itemName, out index))
					{
						if (index is 32 or 33 or 34)
						{
							returnItem = new StardewValley.Tools.Slingshot(index);
						}
						else
						{
							returnItem = new StardewValley.Tools.MeleeWeapon(index);
						}
					}
					break;
				case ItemType.None:
				default:
					break;
			}

			return returnItem;
		}


		/// <summary>
		/// Places an item on the ground that can be picked up.
		/// </summary>
		/// <param name="location">The location to spawn the item into.</param>
		/// <param name="tile">The tile to spawn the item into.</param>
		/// <param name="objectName">The internal name of item to spawn. Spawns weeds if the name doesn't match any defined.</param>
		internal static void PlacePickableItem(GameLocation? location, Point tile, string objectName)
		{
			int objectID = GetIDFromObjectName(objectName);
			PlacePickableItem(location, tile, objectID);
		}


		/// <summary>
		/// Places an item on the ground that can be picked up.
		/// </summary>
		/// <param name="location">The location to spawn the item into.</param>
		/// <param name="tile">The tile to spawn the item into.</param>
		/// <param name="objectID">The ID of the item to spawn.</param>
		internal static void PlacePickableItem(GameLocation? location, Point tile, int objectID)
		{
			SObject obj = new SObject(objectID, 1);
			PlacePickableItem(location, tile, obj);
		}


		/// <summary>
		/// Places an item on the ground that can be picked up.
		/// </summary>
		/// <param name="location">The location to spawn the item into.</param>
		/// <param name="tile">The tile to spawn the item into.</param>
		/// <param name="obj">The item to spawn.</param>
		internal static void PlacePickableItem(GameLocation? location, Point tile, SObject obj)
		{
			// Multiply the tile coordinates by 64 to get the right spawning point
			location?.dropObject(obj, tile.ToVector2() * 64f, Game1.viewport, true);
		}


		/// <summary>
		/// Gets the item type matching the provided string code.
		/// </summary>
		/// <param name="itemTypeString">The item type string code.</param>
		/// <returns>The item type, or ItemType.None if there was no matches.</returns>
		internal static ItemType GetItemType(string itemTypeString)
		{
			if (itemTypeString.Equals("O"))
			{
				return ItemType.Object;
			}
			else if (itemTypeString.Equals("R"))
			{
				return ItemType.Ring;
			}
			else if (itemTypeString.Equals("B"))
			{
				return ItemType.Boots;
			}
			else if (itemTypeString.Equals("BC"))
			{
				return ItemType.BigCraftable;
			}
			else if (itemTypeString.Equals("H"))
			{
				return ItemType.Hat;
			}
			else if (itemTypeString.Equals("W"))
			{
				return ItemType.Weapon;
			}

			return ItemType.None;
		}
	}


	internal enum ItemType
	{
		None,
		Object,
		Ring,
		BigCraftable,
		Hat,
		Boots,
		Weapon
	}
}
