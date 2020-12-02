/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/SeedsUnboxed
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Linq;

namespace SeedsUnpacked
{
	public class ModEntry : Mod, IAssetEditor
	{
		private static readonly string[] InvalidSeeds =
		{
			"Spring",
			"Summer",
			"Fall",
			"Winter",
			"Mixed",
			"Sesame",
		};

		public override void Entry(IModHelper helper)
		{
			Helper.Events.GameLoop.SaveLoaded += delegate { Helper.Content.InvalidateCache(@"Maps/springobjects"); };
		}

		public bool CanEdit<T>(IAssetInfo asset)
		{
			return Game1.objectInformation != null && asset.AssetNameEquals(@"Maps/springobjects");
		}

		public void Edit<T>(IAssetData asset)
		{
			foreach (var id in Game1.objectInformation.Keys.Where(id => Game1.objectInformation[id].Split('/') is string[] split
				&& (split[0].EndsWith("Seeds") || split[0].EndsWith("Bulb")) // No starters or saplings (stage 0 is taller than 16px)
				&& InvalidSeeds.All(prefix => !split[0].StartsWith(prefix)) // No non-standard (multiple possible harvest) seeds
				&& !split[5].EndsWith("trellis."))) // No trellis seeds (stage 0 is taller than 16px)
			{
				try
				{
					var crop = new Crop(id, 0, 0);
					var sourceArea = Helper.Reflection.GetMethod(crop, "getSourceRect").Invoke<Rectangle>(0);
					sourceArea.Height = 16;
					sourceArea.Y += sourceArea.Height;
					var targetArea = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, id, sourceArea.Width, sourceArea.Height);
					asset.AsImage().PatchImage(Game1.cropSpriteSheet, sourceArea, targetArea, PatchMode.Replace);
				}
				catch (Exception e)
				{
					Monitor.Log($"{e}", LogLevel.Error);
					continue;
				}
			}
		}
	}
}
