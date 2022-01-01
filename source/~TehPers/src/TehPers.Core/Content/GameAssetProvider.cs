/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using System.IO;
using StardewModdingAPI;
using TehPers.Core.Api.Content;

namespace TehPers.Core.Content
{
    public class GameAssetProvider : IAssetProvider
    {
        private readonly IModHelper helper;

        public GameAssetProvider(IModHelper helper)
        {
            this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
        }

        public T Load<T>(string path)
        {
            return this.helper.Content.Load<T>(path, ContentSource.GameContent);
        }

        public Stream Open(string path, FileMode mode)
        {
            if (mode != FileMode.Open)
            {
                throw new ArgumentException("Game assets can only be read from.", nameof(mode));
            }

            var fullPath = Path.Combine(Constants.DataPath, path);
            return File.OpenRead(fullPath);
        }
    }
}