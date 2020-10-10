/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace HatsOnCats.Framework
{
    internal static class AnimatedSpriteExtensions
    {
        public static string UniqueName(this AnimatedSprite sprite)
        {
            return sprite.textureName.Value.Split('\\').Last().ToLower();
        }
    }
}
