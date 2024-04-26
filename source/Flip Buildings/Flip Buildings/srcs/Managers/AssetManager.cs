/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/FlipBuildings
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;

namespace FlipBuildings.Managers
{
	internal static class AssetManager
	{
		public static Texture2D flipButton;

		internal static void Apply()
		{
			flipButton = ModEntry.Helper.ModContent.Load<Texture2D>("assets/flip_button");
		}
	}
}
