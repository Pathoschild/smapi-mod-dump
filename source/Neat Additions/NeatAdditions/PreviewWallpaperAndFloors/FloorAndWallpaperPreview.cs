using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NeatAdditions.PreviewWallpaperAndFloors
{
	public class FloorAndWallpaperPreview
	{
		private int oldFloorHovered = -1;
		private int oldWallHovered = -1;
		private StardewValley.Object oldObjectHeld = null;

		public FloorAndWallpaperPreview()
		{
			ModEntry.Events.GameLoop.UpdateTicked += GameEvents_UpdateTick;
		}

		private void GameEvents_UpdateTick(object sender, EventArgs e)
		{
			bool isHoveringFloor = IsHoveringOverFloor(out int floor);
			bool isHoveringWall = IsHoveringOverWall(out int wall);

			if (floor != oldFloorHovered || Game1.player.ActiveObject != oldObjectHeld)
			{
				if (Game1.currentLocation is DecoratableLocation decLocation)
				{
					//Reset floor back to whatever it should be
					for (int i = 0; i < decLocation.floor.Count; i++)
						InvokeMethod(decLocation, "doSetVisibleFloor", i, decLocation.floor[i]);

					//Set what is visisble. This doesn't change the location.floor variable, so it won't affect saving/etc.
					if (isHoveringFloor && Game1.player.ActiveObject is Wallpaper floorOrWallpaperObject && floorOrWallpaperObject.isFloor.Value)
						InvokeMethod(decLocation, "doSetVisibleFloor", floor, floorOrWallpaperObject.ParentSheetIndex);
				}
			}

			//Copy-paste except for wallpapers
			if (wall != oldWallHovered || Game1.player.ActiveObject != oldObjectHeld)
			{
				if (Game1.currentLocation is DecoratableLocation decLocation)
				{
					for (int i = 0; i < decLocation.wallPaper.Count; i++)
						InvokeMethod(decLocation, "doSetVisibleWallpaper", i, decLocation.wallPaper[i]);

					if (isHoveringWall && Game1.player.ActiveObject is Wallpaper floorOrWallpaperObject && !floorOrWallpaperObject.isFloor.Value)
						InvokeMethod(decLocation, "doSetVisibleWallpaper", wall, floorOrWallpaperObject.ParentSheetIndex);
				}
			}

			oldFloorHovered = floor;
			oldWallHovered = wall;
			oldObjectHeld = Game1.player.ActiveObject;
		}

		private bool IsHoveringOverFloor(out int floor)
		{
			if (Game1.activeClickableMenu == null && Game1.currentLocation is DecoratableLocation location)
			{
				Vector2 position = (!Game1.wasMouseVisibleThisFrame) ? Game1.player.GetToolLocation(false) : new Vector2((float)(Game1.getOldMouseX() + Game1.viewport.X), (float)(Game1.getOldMouseY() + Game1.viewport.Y));
				Point tile = new Point((int)position.X / 64, (int)position.Y / 64);
				List<Rectangle> floors = location.getFloors();

				for (int i = 0; i < floors.Count; i++)
				{
					if (floors[i].Contains(tile))
					{
						floor = i;
						return true;
					}
				}
			}
			floor = -1;
			return false;
		}

		private bool IsHoveringOverWall(out int wall)
		{
			if (Game1.activeClickableMenu == null && Game1.currentLocation is DecoratableLocation location)
			{
				Vector2 position = (!Game1.wasMouseVisibleThisFrame) ? Game1.player.GetToolLocation(false) : new Vector2((float)(Game1.getOldMouseX() + Game1.viewport.X), (float)(Game1.getOldMouseY() + Game1.viewport.Y));
				Point tile = new Point((int)position.X / 64, (int)position.Y / 64);
				List<Rectangle> walls = location.getWalls();

				for (int i = 0; i < walls.Count; i++)
				{
					if (walls[i].Contains(tile))
					{
						wall = i;
						return true;
					}
				}
			}
			wall = -1;
			return false;
		}

		private static object InvokeMethod<T>(T obj, string methodName, params object[] args)
		{
			var type = typeof(T);
			var method = type.GetTypeInfo().GetDeclaredMethod(methodName);
			return method.Invoke(obj, args);
		}
	}
}
