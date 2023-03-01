/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Shockah.Kokoro;
using System;

namespace Shockah.JunimoWarp
{
	internal static class NetMessage
	{
		public record NextWarpRequest(
			Guid ID,
			string LocationName,
			IntPoint Point
		);

		public record NextWarpResponse(
			Guid ID,
			string LocationName,
			IntPoint Point
		);
	}
}