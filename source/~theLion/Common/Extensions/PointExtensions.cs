/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace TheLion.Common
{
	public static class PointExtensions
	{
		/// <summary>Convert the calling Point to a Vector2.</summary>
		public static Vector2 ToVector2(this Point p)
		{
			return new Vector2(p.X, p.Y);
		}
	}
}