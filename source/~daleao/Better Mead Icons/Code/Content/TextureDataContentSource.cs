/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;

namespace DaLion.Meads.Content;

internal abstract class TextureDataContentSource : IContentSource
{
	public abstract CustomTextureData TextureData { get; }

	public abstract T Load<T>(string path);

	public abstract IManifest GetManifest();

	public (string, List<string>, object) GetData()
	{
		return new(TextureData.Mead, TextureData.Flowers, Globals.MeadAsArtisanGoodEnum);
	}
}
