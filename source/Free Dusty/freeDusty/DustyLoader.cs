/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/skuldomg/freeDusty
**
*************************************************/

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