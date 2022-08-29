/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Shockah.CommonModCode;
using System;
using System.Collections.Generic;

namespace Shockah.FlexibleSprinklers
{
	internal enum SoilType
	{
		Waterable,
		Sprinkler,
		NonWaterable
	}

	internal interface IMap : IEquatable<IMap>
	{
		SoilType this[IntPoint point] { get; }

		void WaterTile(IntPoint point);

		IEnumerable<(IntPoint position, SprinklerInfo info)> GetAllSprinklers();

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Nested in another interface")]
		public interface WithKnownSize : IMap
		{
			public int Width { get; }
			public int Height { get; }
		}
	}
}