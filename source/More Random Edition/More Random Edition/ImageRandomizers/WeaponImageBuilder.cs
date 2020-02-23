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
		/// A map of the weapon position in the dictionary to the id it belongs to
		/// </summary>
		private readonly Dictionary<Point, int> WeaponPositionToIDMap = new Dictionary<Point, int>()
		{
			{ new Point(0, 0), 0 },
			{ new Point(1, 0), 1 },
			{ new Point(2, 0), 2 },
			{ new Point(3, 0), 3 },
			{ new Point(4, 0), 4 },
			{ new Point(5, 0), 5 },
			{ new Point(6, 0), 6 },
			{ new Point(7, 0), 7 },

			{ new Point(0, 1), 8 },
			{ new Point(1, 1), 9 },
			{ new Point(2, 1), 10 },
			{ new Point(3, 1), 11 },
			{ new Point(4, 1), 12 },
			{ new Point(5, 1), 13 },
			{ new Point(6, 1), 14 },
			{ new Point(7, 1), 15 },

			{ new Point(0, 2), 16 },
			{ new Point(1, 2), 17 },
			{ new Point(2, 2), 18 },
			{ new Point(3, 2), 19 },
			{ new Point(4, 2), 20 },
			{ new Point(5, 2), 21 },
			{ new Point(6, 2), 22 },
			{ new Point(7, 2), 23 },

			{ new Point(0, 3), 24 },
			{ new Point(1, 3), 25 },
			{ new Point(2, 3), 26 },
			{ new Point(3, 3), 27 },
			{ new Point(4, 3), 28 },
			{ new Point(5, 3), 29 },
			{ new Point(6, 3), 30 },
			{ new Point(7, 3), 31 },

			// (0, 4) - 32 is Slingshot
			// (1, 4) - 33 is Master Slingshot
			// (2, 4) - 34 is Galaxy Slingshot
			{ new Point(3, 4), 35 },
			{ new Point(4, 4), 36 },
			{ new Point(5, 4), 37 },
			{ new Point(6, 4), 38 },
			{ new Point(7, 4), 39 },

			{ new Point(0, 5), 40 },
			{ new Point(1, 5), 41 },
			{ new Point(2, 5), 42 },
			{ new Point(3, 5), 43 },
			{ new Point(4, 5), 44 },
			{ new Point(5, 5), 45 },
			{ new Point(6, 5), 46 },
			// ID 47 is the scythe - skipping

			{ new Point(0, 6), 48 },
			{ new Point(1, 6), 49 },
			{ new Point(2, 6), 50 },
			{ new Point(3, 6), 51 },
			{ new Point(4, 6), 52 },
		};

		public WeaponImageBuilder() : base()
		{
			BaseFileName = "weapons.png";
			SubDirectory = "Weapons";
			PositionsToOverlay = WeaponPositionToIDMap.Keys.ToList();

			SwordImages = Directory.GetFiles($"{ImageDirectory}/{SwordSubDirectory}").Where(x => x.EndsWith(".png")).OrderBy(x => x).ToList();
			DaggerImages = Directory.GetFiles($"{ImageDirectory}/{DaggerSubDirectory}").Where(x => x.EndsWith(".png")).OrderBy(x => x).ToList();
			HammerAndClubImages = Directory.GetFiles($"{ImageDirectory}/{HammerAndClubSubDirectory}").Where(x => x.EndsWith(".png")).OrderBy(x => x).ToList();
			SlingshotImages = Directory.GetFiles($"{ImageDirectory}/{SlingshotSubDirectory}").Where(x => x.EndsWith(".png")).OrderBy(x => x).ToList();
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
				return $"{ImageDirectory}/default.png";
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
	}
}
