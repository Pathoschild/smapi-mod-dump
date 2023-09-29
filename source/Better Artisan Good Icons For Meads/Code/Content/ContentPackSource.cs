/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/bagi-meads
**
*************************************************/

using StardewModdingAPI;

namespace DaLion.Meads.Content;

internal class ContentPackSource : TextureDataContentSource
{
	private readonly IContentPack _pack;
    
	public override CustomTextureData TextureData { get; }

	public ContentPackSource(IContentPack pack)
	{
		this._pack = pack;
		TextureData = pack.ReadJsonFile<CustomTextureData>("data.json");
	}

	public override T Load<T>(string path)
	{
		return _pack.ModContent.Load<T>(path);
	}

	public override IManifest GetManifest()
	{
		return _pack.Manifest;
	}
}
