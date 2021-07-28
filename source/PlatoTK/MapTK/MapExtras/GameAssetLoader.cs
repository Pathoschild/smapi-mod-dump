/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System.IO;
using System.Linq;

namespace MapTK.MapExtras
{
    class GameAssetLoader : IAssetLoader
    {
        private IModHelper helper;

        public GameAssetLoader(IModHelper helper)
        {
            this.helper = helper;
        }

        public bool CanLoad<T>(IAssetInfo asset)
        {
            string[] path = asset.AssetName.Split(new[] { Path.DirectorySeparatorChar, '/', '\\' });
            return path[0] == "GameContent" || (path.Length > 1 && path[0] == "Maps" && path[1] == "GameContent"); 
        }

        public T Load<T>(IAssetInfo asset)
        {
            string[] path = asset.AssetName.Split(new[] { Path.DirectorySeparatorChar, '/', '\\' });

            if (path[0] == "GameContent")
                return (T)(object)helper.Content.Load<Texture>(string.Join(Path.DirectorySeparatorChar.ToString(), path.Skip(1)), ContentSource.GameContent);
            else
                return (T)(object)helper.Content.Load<Texture>(string.Join(Path.DirectorySeparatorChar.ToString(), path.Skip(2)), ContentSource.GameContent);
        }
    }
}
