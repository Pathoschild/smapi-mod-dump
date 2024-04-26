/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/chsiao58/EvenBetterArtisanGoodIcons
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;

namespace BetterArtisanGoodIcons.Content
{
    /// <inheritdoc />
    /// <summary>A basic content source provider - is able to group data from each CustomTextureData together properly.</summary>
    internal abstract class TextureDataContentSource : IContentSource
    {
        public abstract CustomTextureData TextureData { get; }
        public abstract T Load<T>(string path);
        public abstract IManifest GetManifest();

        /// <summary>Group each texture path with its corresponding source list and artisan good type.</summary>
        public IEnumerable<Tuple<string, List<string>, ArtisanGood>> GetData()
        {
            yield return new Tuple<string, List<string>, ArtisanGood>(TextureData.Honey,
                TextureData.Flowers, ArtisanGood.Honey);

            yield return new Tuple<string, List<string>, ArtisanGood>(TextureData.Juice,
                TextureData.Vegetables, ArtisanGood.Juice);

            yield return new Tuple<string, List<string>, ArtisanGood>(TextureData.Pickles,
                TextureData.Vegetables, ArtisanGood.Pickles);

            yield return new Tuple<string, List<string>, ArtisanGood>(TextureData.Wine,
                TextureData.Fruits, ArtisanGood.Wine);

            yield return new Tuple<string, List<string>, ArtisanGood>(TextureData.Jelly,
                TextureData.Fruits, ArtisanGood.Jelly);

            yield return new Tuple<string, List<string>, ArtisanGood>(TextureData.DriedMushrooms,
                TextureData.Mushrooms, ArtisanGood.DriedMushrooms);
        }
    }
}
