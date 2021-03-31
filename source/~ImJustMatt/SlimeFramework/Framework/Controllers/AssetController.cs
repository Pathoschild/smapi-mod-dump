/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

using System;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace ImJustMatt.SlimeFramework.Framework.Controllers
{
    internal class AssetController : IAssetLoader, IAssetEditor
    {
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data\\Monsters\\");
        }

        public void Edit<T>(IAssetData asset)
        {
            var monsters = asset.AsDictionary<string, string>().Data;
            foreach (var slime in SlimeFramework.Slimes)
            {
                monsters[slime.Key] = slime.Value.Data;
            }
        }

        public bool CanLoad<T>(IAssetInfo asset)
        {
            return SlimeFramework.Slimes.Keys.Any(slime => asset.AssetName.StartsWith(PathUtilities.NormalizePath($"Characters\\Monsters\\${slime}.png")));
        }

        public T Load<T>(IAssetInfo asset)
        {
            throw new NotImplementedException();
        }
    }
}