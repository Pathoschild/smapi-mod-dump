/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace JsonAssets.Data
{
    public class ShirtData : ClothingData
    {
        [JsonIgnore]
        public Texture2D textureMaleColor;
        [JsonIgnore]
        public Texture2D textureFemaleColor;
    }
}
