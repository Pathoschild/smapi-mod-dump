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
using Newtonsoft.Json;

namespace Entoarox.AdvancedLocationLoader.Configs
{
    internal class ShopConfig
    {
#pragma warning disable CS0649
        /*********
        ** Accessors
        *********/
        [JsonIgnore]
        public string Name;

        [JsonIgnore]
        public Texture2D PortraitTexture;

        public int ParserVersion;
        public string Portrait;
        public string Owner;
        public List<string> Messages;
        public List<ShopItem> Items;
#pragma warning restore CS0649
    }
}
