/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/BlueberryMushroomMachine
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley;
using static BlueberryMushroomMachine.ModEntry;
using Object = StardewValley.Object;
using Microsoft.Xna.Framework;

namespace BlueberryMushroomMachine
{
	internal static class Utils
	{
		/// <summary>
		/// Reassigns the unique ID of any mismatched or shuffled held objects.
		/// </summary>
		public static void FixPropagatorObjectIds()
		{
			try
			{
				Utility.ForAllLocations((GameLocation location) =>
				{
					foreach (Propagator propagator in Utils.GetMachinesIn(location))
					{
						if (propagator.SourceMushroomName is not null
							&& ModEntry.JsonAssetsAPI.GetObjectId(name: propagator.SourceMushroomName) is int id
							&& id > 0 && id != propagator.SourceMushroomIndex)
						{
							Log.D($"Updating mushroom ID for mushroom propagator located at" +
								$" {location.NameOrUniqueName}::{propagator.TileLocation}:" +
								$" {propagator.SourceMushroomName} {propagator.SourceMushroomIndex} => {id}",
								ModEntry.Config.DebugMode);
							propagator.SourceMushroomIndex = id;
						}
					}
				});
			}
			catch (Exception e)
			{
				Log.E($"Error while deshuffling held mushrooms\n\n{e}");
			}
		}

		/// <summary>
		/// Fetches all propagator machines in a given location.
		/// </summary>
		/// <param name="location">Location to search.</param>
		/// <returns>All objects of type propagator.</returns>
		public static IEnumerable<Propagator> GetMachinesIn(GameLocation location)
		{
			return location.Objects.Values.Where((Object o) => o is Propagator).Cast<Propagator>();
		}

		/// <summary>
		/// Determines the frame to be used for showing held mushroom growth.
		/// </summary>
		/// <param name="currentDays">Current days since last growth.</param>
		/// <param name="goalDays">Number of days when next growth happens.</param>
		/// <param name="currentStack">Current count of mushrooms.</param>
		/// <param name="goalStack">Maximum amount of mushrooms of this type.</param>
		/// <returns>Frame for mushroom growth progress.</returns>
		public static int GetOverlayGrowthFrame(float currentDays, int goalDays, int currentStack, int goalStack)
		{
			int frames = ModValues.OverlayMushroomFrames - 1;
			float maths = currentStack == goalStack ? frames : frames
				* (currentStack - 1 + (currentDays / goalDays))
				* goalDays / (goalStack * goalDays);
			return (int)Math.Clamp(value: maths, min: 0, max: frames);
		}

		/// <summary>
		/// Generates a clipping rectangle for the mushroom overlay,
		/// appropriate to the current held mushroom, and its held quantity.
		/// Undefined mushrooms will use their default object rectangle.
		/// </summary>
		/// <returns>Source rectangle for mushroom overlay from overlay texture.</returns>
		public static Rectangle GetOverlaySourceRect(GameLocation location, int index, int whichFrame)
		{
			int frames = ModValues.OverlayMushroomFrames;
			bool isBasicMushroom = Enum.IsDefined(enumType: typeof(Mushrooms), value: index);
			Point size = isBasicMushroom
				? Propagator.OverlaySize
				: new Point(x: Game1.smallestTileSize, y: Game1.smallestTileSize);
			return isBasicMushroom
				? new Rectangle(
					x: (Utils.IsDarkLocation(location) ? size.X * frames : 0) + whichFrame * size.X,
					y: GetMushroomSourceRectIndex(index: index) * size.Y,
					width: size.X,
					height: size.Y)
				: Game1.getSourceRectForStandardTileSheet(
					tileSheet: Game1.objectSpriteSheet,
					tilePosition: index,
					width: size.X,
					height: size.Y);
		}

		/// <summary>
		/// Generates a clipping rectangle for the propagator machine,
		/// appropriate to the current location.
		/// </summary>
		/// <returns>Source rectangle for propagator from machine texture.</returns>
		public static Rectangle GetMachineSourceRect(GameLocation location, Vector2 tile)
		{
			// random magical maths to pick a value
			// based on a predictable but scattered pattern for the current tile
			return Game1.getSourceRectForStandardTileSheet(
					tileSheet: ModEntry.MachineTexture,
					tilePosition: (Utils.IsDarkLocation(location) ? 2 : 0) + ((tile.X + tile.Y) % 3 == 1 ? 1 : 0),
					width: Propagator.MachineSize.X,
					height: Propagator.MachineSize.Y);
		}

		/// <summary>
		/// Assigns an arbitrary flip value to some given tile coordinates.
		/// </summary>
		/// <returns>Whether object at the current tile is flipped.</returns>
		public static bool GetMachineIsFlipped(Vector2 tile)
		{
			// random magical maths to pick a value
			// based on a predictable but scattered pattern for the current tile
			// distinct from arbitrary alternate sprite value
			return (tile.X + tile.Y) % 4 == 1;
		}

		/// <summary>
		/// Check for dark locations, used to determine the visual style of the propagator.
		/// </summary>
		/// <param name="location">Location to check.</param>
		/// <returns>Whether the given location is 'dark', or otherwise cave-flavoured.</returns>
		public static bool IsDarkLocation(GameLocation location)
		{
			return location is FarmCave or IslandFarmCave;
		}

		public static bool IsValidMushroom(Object o)
		{
			// From the vanilla Utility.IsPerfectlyNormalObjectAtParentSheetIndex or whatever that method was again
			// Don't want to start growing wallpaper
			Type type = o.GetType();
			if (o is null || (type != typeof(Object) && type != typeof(ColoredObject)))
			{
				return false;
			}

			return Enum.IsDefined(enumType: typeof(Mushrooms), value: o.ParentSheetIndex)
				|| ModEntry.Config.OtherObjectsThatCanBeGrown.Contains(o.Name)
				|| ((o.Category == Object.VegetableCategory || o.Category == Object.GreensCategory)
					&& (o.Name.Contains("mushroom", StringComparison.InvariantCultureIgnoreCase)
						|| o.Name.Contains("fungus", StringComparison.InvariantCultureIgnoreCase)));
		}

		public static int GetMushroomSourceRectIndex(int index)
		{
			return index switch
			{
				(int)Mushrooms.Morel => 2,
				(int)Mushrooms.Chantarelle => 1,
				(int)Mushrooms.Common => 0,
				(int)Mushrooms.Red => 3,
				(int)Mushrooms.Purple => 4,
				_ => -1
			};
		}

		public static void GetMushroomGrowthRate(Object o, out float rate)
		{
			rate = o.ParentSheetIndex switch
			{
				(int)Mushrooms.Morel => 0.5f,
				(int)Mushrooms.Chantarelle => 0.5f,
				(int)Mushrooms.Common => 1.0f,
				(int)Mushrooms.Red => 0.5f,
				(int)Mushrooms.Purple => 0.25f,
				_ => o.Price < 50 ? 1.0f : o.Price < 100 ? 0.75f : o.Price < 200 ? 0.5f : 0.25f
			};
		}

		public static void GetMushroomMaximumQuantity(Object o, out int quantity)
		{
			quantity = o.ParentSheetIndex switch
			{
				(int)Mushrooms.Morel => 4,
				(int)Mushrooms.Chantarelle => 4,
				(int)Mushrooms.Common => 6,
				(int)Mushrooms.Red => 3,
				(int)Mushrooms.Purple => 2,
				_ => o.Price < 50 ? 5 : o.Price < 100 ? 4 : o.Price < 200 ? 3 : 2
			};
			quantity *= ModEntry.Config.MaximumQuantityLimitsDoubled ? 2 : 1;
		}
	}
}
