/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;

namespace MoreGiantCrops;

public interface IMoreGiantCropsApi {

	public Texture2D? GetTexture(int productIndex);

	public int[] RegisteredCrops();

}
