/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Shockah.FlexibleSprinklers
{
	public interface ILineSprinklersApi
	{
		/// <summary>Get the relative tile coverage by supported sprinkler ID. Note that sprinkler IDs may change after a save is loaded due to Json Assets reallocating IDs.</summary>
		IDictionary<int, Vector2[]> GetSprinklerCoverage();
	}
}