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

namespace BetterArtisanGoodIcons.Content
{
    /// <inheritdoc />
    /// <summary>A content source that comes from a mod. </summary>
    internal class ModSource : TextureDataContentSource
    {
        private readonly IModHelper helper;

        public override CustomTextureData TextureData { get; }

        public ModSource(IModHelper helper)
        {
            this.helper = helper;
            //1.3.31
            TextureData = helper.Data.ReadJsonFile<CustomTextureData>("assets/data.json");
        }

        public override T Load<T>(string path)
        {
            return helper.ModContent.Load<T>(path);
        }

        public override IManifest GetManifest()
        {
            return helper.ModRegistry.Get(helper.ModRegistry.ModID).Manifest;
        }
    }
}
