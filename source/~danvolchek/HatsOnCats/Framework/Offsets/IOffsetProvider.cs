/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using HatsOnCats.Framework.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace HatsOnCats.Framework.Offsets
{
    internal interface IOffsetProvider : INamed
    {
        bool GetOffset(Rectangle sourceRectangle, SpriteEffects effects, out Vector2 offset);
        bool CanHandle(string spriteName);
    }
}
