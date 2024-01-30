/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/WarpNetwork
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using System;

namespace WarpNetwork.models
{
	internal class LegacyWarpLocation
	{
		public string Location { set; get; }
		public int X { set; get; } = 0;
		public int Y { set; get; } = 1;
		public bool Enabled { set; get; } = false;
		public string Label { set; get; }
		public bool OverrideMapProperty { set; get; } = false;
		public bool AlwaysHide { get; set; } = false;
		public string RequiredBuilding { set; get; } = null;
		public string Icon { set; get; } = "";

		public WarpLocation Convert()
		{
			return new()
			{
				Location = Location,
				Position = new(X, Y),
				Condition =
					Enabled ?
						RequiredBuilding is not null ?
						$"BUILDINGS_CONSTRUCTED All \"{RequiredBuilding}\" 1" :
						"TRUE" :
					"FALSE",
				IconPath = Icon is null ? null : ModEntry.LegacyAssetPath + "/Icons/" + Icon,
				OverrideMapProperty = OverrideMapProperty,
				Label = Label,
				DisplayCondition = AlwaysHide ? "FALSE" : null
			};
		}
	}
}
