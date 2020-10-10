/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/eastscarpe
**
*************************************************/

using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace EastScarpe
{
	public enum FishingArea
	{
		Pond = 0,
		Tidepool = 1,
		Ocean = 2,
	}

	public class ModData
	{
		public int ForestBeachYLine { get; set; } = 85;

		public float SeabirdSoundChance { get; set; } = 0.003f;
		public int MaxSeabirdCount { get; set; } = 3;

		public List<int> TidepoolTiles { get; set; } = new List<int>
		{
			 60,  61,  62,  63,
			 77,  78,  79,  80,
			 94,  95,  96,  97,
			111, 112, 113, 114,
			                    253, 254,
			                    270, 271,
			                    287, 288,
		};

		public float SeaMonsterChance { get; set; } = 0.000001f;
		public Rectangle SeaMonsterRange { get; set; } =
			new Rectangle (40, 120, 12, 9);
	}
}
