/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Traktori7/StardewValleyMods
**
*************************************************/

using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using BiggerCraftables;
using SObject = StardewValley.Object;


/// <summary>
/// BIG problem: the bigger craftable is infact 8 normal craftables with different textures and all contain their own mod data
/// 
/// Also big craftables (BC) seem to reset their mod data when you pick them up
/// => How to save ores to the furnace?
///		The top left BC should work well as the information holder
///		Suggestion: Listen for Object removed events and check if the base BC was removed and parse the ores from the mod data
///			to either drop them to the ground or something else.
///			
/// Do I need a furnace controller helper?
/// => Saving the ores:
///		The ores could be stored inside the field heldObject that is actually a chest, so that LookupAnything can find them.
///		Problem: The bigger craftable is 8 different BCs so should the ores be stored in all of them?
///		Is mod data needed at all then? To tag the base object and connect it to its controller?
///	=> 
/// </summary>

namespace CraftableIndustrialFurnace
{
	public class ModEntry : StardewModdingAPI.Mod
	{
		internal Config config;
		internal ITranslationHelper i18n => Helper.Translation;

		internal readonly List<SObject> clickedObjects = new List<SObject>();

		public override void Entry(IModHelper helper)
		{
			config = helper.ReadConfig<Config>();

			helper.Events.Input.ButtonPressed += OnButtonPressed;
		}

		private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
		{
			if (!Context.IsPlayerFree)
				return;

			if (e.Button == SButton.MouseLeft || e.Button == SButton.MouseRight)
			{
				string testString = "FurnaceModDataTest";
				Vector2 tile = e.Cursor.GrabTile;

				// See if the player clicked a craftable
				// TODO: Remember to use something else than grabtile for controller support!
				var objects = Game1.currentLocation.objects;
				objects.TryGetValue(tile, out SObject craftable);

				if (craftable != null)
				{
					//DebugLogging(craftable, testString, tile);

					Vector2? baseTile = GetBaseTile(craftable);

					if (baseTile == null)
					{
						Monitor.Log("Couldn't find the base tile", LogLevel.Debug);
						DebugLogging(craftable, testString, tile);
					}
					else
					{
						objects.TryGetValue((Vector2)baseTile, out SObject baseCraftable);

						if (baseCraftable != null)
						{
							DebugLogging(baseCraftable, testString, (Vector2)baseTile);

							// Remember to remove the added item from the player. Does this do it?
							if (Game1.player.ActiveObject != null && Game1.player.ActiveObject is SObject)
							{
								Game1.player.ActiveObject = PlaceItemToSmelt(baseCraftable, Game1.player.ActiveObject);
							}
						}
					}
				}
			}
		}

		private SObject PlaceItemToSmelt(SObject furnace, SObject item)
		{
			if (furnace == null || item == null)
			{
				Monitor.Log("null object encountered in PlaceItemToSmelt", LogLevel.Error);
				return item;
			}

			Chest chest;

			if (furnace.heldObject.Value == null)
			{
				chest = new Chest();
				//chest.playerChest.Value = true;
				furnace.heldObject.Value = chest;
			}
			else
			{
				// Problematic cast?
				chest = (Chest)furnace.heldObject.Value;
			}

			// Possibly dangerous cast
			SObject returnItem = (SObject)chest.addItem(item);

			Monitor.Log($"Chest currently contains {chest.items}", LogLevel.Debug);

			return returnItem;
		}


		/// <summary>
		/// Copying the functionality of BiggerCraftable's OnObjectListChanged to find the base tile
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		private Vector2? GetBaseTile(SObject obj)
		{
			if (obj == null)
				return null;

			// Tile the object is on
			var pos = obj.TileLocation;

			// This checks if "something" exists
			var entry = BiggerCraftables.Mod.entries.SingleOrDefault(ClearingActivity => ClearingActivity.Name == obj.Name);

			if (entry == null)
				return null;

			// This is probably the "tile index" from top left
			int ind = obj.GetBiggerIndex();

			int relPosX = ind % entry.Width, relPosY = entry.Length - 1 - ind / entry.Width;
			Vector2 basePos = new Vector2(pos.X - relPosX, pos.Y - relPosY);

			return basePos;
		}


		private void DebugLogging(SObject craftable, string testString, Vector2 tile)
		{
			Monitor.Log($"Clicked {craftable.DisplayName} at {tile}", LogLevel.Debug);

			if (clickedObjects.Contains(craftable))
			{
				Monitor.Log("You have already clicked on this!", LogLevel.Debug);
			}
			else
			{
				clickedObjects.Add(craftable);
				Monitor.Log($"Added {craftable.displayName} to the list", LogLevel.Debug);
			}

			if (craftable.modData.ContainsKey(testString))
			{
				Monitor.Log("Object has mod data", LogLevel.Debug);
			}
			else
			{
				craftable.modData.Add(testString, "Testi");
				Monitor.Log("Mod data was added", LogLevel.Debug);
			}
		}
	}
}
