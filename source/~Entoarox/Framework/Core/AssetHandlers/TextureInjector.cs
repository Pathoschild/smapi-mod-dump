/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace Entoarox.Framework.Core.AssetHandlers
{
    internal class TextureInjector : IAssetEditor
    {
        /*********
        ** Accessors
        *********/
        internal static Dictionary<string, List<TextureInjectorInfo>> Map = new Dictionary<string, List<TextureInjectorInfo>>();


        /*********
        ** Public methods
        *********/
        public bool CanEdit<T>(IAssetInfo info)
        {
            return typeof(T) == typeof(Texture2D) && TextureInjector.Map.ContainsKey(info.AssetName);
        }

        public void Edit<T>(IAssetData data)
        {
            IAssetDataForImage img = data.AsImage();
            foreach (TextureInjectorInfo entry in TextureInjector.Map[data.AssetName])
                img.PatchImage(entry.Texture, entry.Source, entry.Destination, PatchMode.Replace);
        }
    }
}
