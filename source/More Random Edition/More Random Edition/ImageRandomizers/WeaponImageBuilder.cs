/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.IO;
using System.Linq;
using StardewValley.GameData.Weapons;

namespace Randomizer
{
	public class WeaponImageBuilder : ImageBuilder
	{
		private const string SwordSubDirectory = "Swords";
		private const string DaggerSubDirectory = "Daggers";
		private const string HammerAndClubSubDirectory = "HammersAndClubs";
		private const string SlingshotSubDirectory = "Slingshots";

		private List<string> SwordImages { get; set; }
		private List<string> DaggerImages { get; set; }
		private List<string> HammerAndClubImages { get; set; }
		private List<string> SlingshotImages { get; set; }

		/// <summary>
		/// A map of the weapon overlay data to the id it belongs to
		/// </summary>
		private Dictionary<SpriteOverlayData, string> OverlayDataToIdMap;

		public WeaponImageBuilder() : base()
		{
            Rng = RNG.GetFarmRNG(nameof(WeaponImageBuilder));
			GlobalStardewAssetPath = "TileSheets/weapons";
            SubDirectory = "Weapons";
			SetUpWeaponPositionToIDMap();
			OverlayData = OverlayDataToIdMap.Keys.ToList();

			SwordImages = Directory.GetFiles(Path.Combine(ImageDirectory, SwordSubDirectory))
				.Where(x => x.EndsWith(".png"))
				.OrderBy(x => x).
				ToList();
			DaggerImages = Directory.GetFiles(Path.Combine(ImageDirectory, DaggerSubDirectory))
				.Where(x => x.EndsWith(".png"))
				.OrderBy(x => x)
				.ToList();
			HammerAndClubImages = Directory.GetFiles(Path.Combine(ImageDirectory, HammerAndClubSubDirectory))
				.Where(x => x.EndsWith(".png"))
				.OrderBy(x => x)
				.ToList();

			//TODO: enable this when we actually randomize slingshot images
			//SlingshotImages = Directory.GetFiles(Path.Combine(ImageDirectory, SlingshotSubDirectory))
			//	.Where(x => x.EndsWith(".png"))
			//	.OrderBy(x => x)
			//	.ToList();
		}

		/// <summary>
		/// Sets up the weapon ID map
		/// </summary>
		private void SetUpWeaponPositionToIDMap()
		{
			OverlayDataToIdMap = new Dictionary<SpriteOverlayData, string>();
			foreach (string stringKey in WeaponRandomizer.Weapons.Keys)
			{
				// TODO when this is reworked: no need to parse this - the weapon info in Weapons will have the sprite index
				// If this isn't an integer, then it's from a mod, so skip it
				if (int.TryParse(stringKey, out int id))
				{
					var spriteOverlayData = new SpriteOverlayData(GlobalStardewAssetPath, GetPointFromId(id));
					OverlayDataToIdMap[spriteOverlayData] = stringKey;
				}
			}
		}

		/// <summary>
		/// Gets the point in the weapons file that belongs to the given item id
		/// </summary>
		/// <param name="id">The id</param>
		/// <returns />
		protected Point GetPointFromId(int id)
		{
			int itemsPerRow = GetItemsPerRow();
			return new Point(
				x: id % itemsPerRow, 
				y: id / itemsPerRow);
		}

		/// <summary>
		/// Gets a random file name that matches the weapon type at the given position
		/// Will remove the name found from the list
		/// </summary>
		/// <param name="overlayData">The overlay data</param>
		/// <returns>The selected file name</returns>
		protected override string GetRandomFileName(SpriteOverlayData overlayData)
		{
			string fileName = "";
			var position = overlayData.TilesheetPosition;
			switch (GetWeaponTypeFromPosition(overlayData))
			{
				case WeaponType.SlashingSword:
				case WeaponType.StabbingSword:
					fileName = Rng.GetAndRemoveRandomValueFromList(SwordImages);
					break;
				case WeaponType.Dagger:
					fileName = Rng.GetAndRemoveRandomValueFromList(DaggerImages);
					break;
				case WeaponType.ClubOrHammer:
					fileName = Rng.GetAndRemoveRandomValueFromList(HammerAndClubImages);
					break;
				case WeaponType.Slingshot:
					// TODO:Use slingshot images when we actually randomize them
					break;
				default:
					Globals.ConsoleError($"No weapon type defined at image {overlayData.TilesheetName} at position: {position.X}, {position.Y}");
					break;

			}

			if (string.IsNullOrEmpty(fileName))
			{
				Globals.ConsoleWarn($"Using default image for weapon at image position - you may not have enough weapon images: {overlayData.TilesheetName} - {position.X}, {position.Y}");
				return null;
			}
			return fileName;
		}

		/// <summary>
		/// Gets the weapon type from the given position in the image
		/// </summary>
		/// <param name="overlayData">The overlay data</param>
		/// <returns />
		private WeaponType GetWeaponTypeFromPosition(SpriteOverlayData overlayData)
		{
			string weaponId = OverlayDataToIdMap[overlayData];
			WeaponData weapon = WeaponRandomizer.Weapons[weaponId.ToString()];
			return (WeaponType)weapon.Type;
		}

		/// <summary>
		/// Whether the settings premit random weapon images
		/// </summary>
		/// <returns>True if so, false otherwise</returns>
		public override bool ShouldSaveImage()
		{
			return Globals.Config.Weapons.UseCustomImages;
		}
	}
}
