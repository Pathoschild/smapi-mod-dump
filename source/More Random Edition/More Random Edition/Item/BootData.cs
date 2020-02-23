using System.Collections.Generic;

namespace Randomizer
{
	/// <summary>
	/// Contains data about boots
	/// </summary>
	public class BootData
	{
		/// <summary>
		/// All the boot data from the xnb data file
		/// </summary>
		public static List<BootItem> AllBoots = new List<BootItem>
		{
			new BootItem(504, "Sneakers", 50, 1, 0, 0),
			new BootItem(505, "Rubber Boots", 50, 0, 1, 1),
			new BootItem(506, "Leather Boots", 50, 1, 1, 2),
			new BootItem(507, "Work Boots", 50, 2, 0, 3),
			new BootItem(508, "Combat Boots", 150, 3, 0, 4),
			new BootItem(509, "Tundra Boots", 150, 2, 1, 5),
			new BootItem(510, "Thermal Boots", 50, 1, 2, 6),
			new BootItem(511, "Dark Boots", 250, 4, 2, 7),
			new BootItem(512, "Firewalker Boots", 250, 3, 3, 8),
			new BootItem(513, "Genie Shoes", 250, 1, 6, 9),
			new BootItem(514, "Space Boots", 450, 4, 4, 10),
			new BootItem(515, "Cowboy Boots", 250, 2, 2, 11),
			new BootItem(804, "Emily's Magic Boots", 250, 4, 4, 13),
			new BootItem(806, "Leprechaun Shoes", 250, 2, 1, 14),
		};
	}
}
