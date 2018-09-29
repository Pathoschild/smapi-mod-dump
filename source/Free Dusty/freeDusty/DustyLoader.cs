using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace freeDusty
{
    // Loads the Dusty sprite
    internal class DustyLoader : IAssetLoader
    {
        private static IModHelper Helper;

        public DustyLoader(IModHelper h)
        {
            Helper = h;
        }

        public bool CanLoad<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals(@"Dusty");
        }

        public T Load<T>(IAssetInfo asset)
        {            
            Texture2D dustyTex = Helper.Content.Load<Texture2D>("assets/Dusty.png", ContentSource.ModFolder);
            return (T)(object)dustyTex;
        }
    }
}