/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MapTK.Api
{
    public interface IMapTKAPI
    {
        void HandleTileDraws(Func<Texture2D, Rectangle, Rectangle?, Color, float, Vector2, SpriteEffects, float, bool> handler);
    }
}
