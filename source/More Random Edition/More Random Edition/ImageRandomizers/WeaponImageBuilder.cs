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
using System.Drawing;
using System.IO;
using System.Linq;

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
		/// The number of items per row in the weapon image file
		/// </summary>
		protected const int ItemsPerRow = 8;

		/// <summary>
		/// A map of the weapon position in the dictionary to the id it belongs to
		/// </summary>
		private Dictionary<Point, int> WeaponPositionToIDMap;

		public WeaponImageBuilder() : base()
		{
			BaseFileName = "weapons.png";
			SubDirectory = "Weapons";
			SetUpWeaponPositionToIDMap();
			PositionsToOverlay = WeaponPositionToIDMap.Keys.ToList();

			SwordImages = Directory.GetFiles($"{ImageDirectory}/{SwordSubDirectory}").Where(x => x.EndsWith(".png")).OrderBy(x => x).ToList();
			DaggerImages = Directory.GetFiles($"{ImageDirectory}/{DaggerSubDirectory}").Where(x => x.EndsWith(".png")).OrderBy(x => x).ToList();
			HammerAndClubImages = Directory.GetFiles($"{ImageDirectory}/{HammerAndClubSubDirectory}").Where(x => x.EndsWith(".png")).OrderBy(x => x).ToList();

			//TODO: enable this when we actually randomize slingshot images
			//SlingshotImages = Directory.GetFiles($"{ImageDirectory}/{SlingshotSubDirectory}").Where(x => x.EndsWith(".png")).OrderBy(x => x).ToList();
		}

		/// <summary>
		/// Sets up the weapon ID map
		/// </summary>
		private void SetUpWeaponPositionToIDMap()
		{
			WeaponPositionToIDMap = new Dictionary<Point, int>();
			foreach (int id in WeaponRandomizer.Weapons.Keys)
			{
				WeaponPositionToIDMap[GetPointFromId(id)] = id;
			}
		}

		/// <summary>
		/// Gets the point in the weapons file that belongs to the given item id
		/// </summary>
		/// <param name="id">The id</param>
		/// <returns />
		protected Point GetPointFromId(int id)
		{
			return new Point(id % ItemsPerRow, id / ItemsPerRow);
		}

		/// <summary>
		/// Gets a random file name that matches the weapon type at the given position
		/// Will remove the name found from the list
		/// </summary>
		/// <param name="position">The position</param>
		/// <returns>The selected file name</returns>
		protected override string GetRandomFileName(Point position)
		{
			string fileName = "";
			switch (GetWeaponTypeFromPosition(position))
			{
				case WeaponType.SlashingSword:
				case WeaponType.StabbingSword:
					fileName = Globals.RNGGetAndRemoveRandomValueFromList(SwordImages);
					break;
				case WeaponType.Dagger:
					fileName = Globals.RNGGetAndRemoveRandomValueFromList(DaggerImages);
					break;
				case WeaponType.ClubOrHammer:
					fileName = Globals.RNGGetAndRemoveRandomValueFromList(HammerAndClubImages);
					break;
				case WeaponType.Slingshot:
					// TODO:Use slingshot images when we actually randomize them
					break;
				default:
					Globals.ConsoleError($"No weapon type defined at image position: {position.X}, {position.Y}");
					break;

			}

			if (string.IsNullOrEmpty(fileName))
			{
				Globals.ConsoleWarn($"Using default image for weapon at image position - you may not have enough weapon images: {position.X}, {position.Y}");
				return null;
			}
			return fileName;
		}

		/// <summary>
		/// Gets the weapon type from the given position in the image
		/// </summary>
		/// <param name="position">The position</param>
		/// <returns />
		private WeaponType GetWeaponTypeFromPosition(Point position)
		{
			int weaponId = WeaponPositionToIDMap[position];
			WeaponItem weapon = WeaponRandomizer.Weapons[weaponId];
			return weapon.Type;
		}

		/// <summary>
		/// Whether the settings premit random weapon images
		/// </summary>
		/// <returns>True if so, false otherwise</returns>
		public override bool ShouldSaveImage()
		{
			return Globals.Config.Weapons.Randomize && Globals.Config.Weapons.UseCustomImages;
		}
	}
}
