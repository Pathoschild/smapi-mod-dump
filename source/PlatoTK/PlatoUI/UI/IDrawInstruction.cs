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

namespace PlatoUI.UI
{
    public interface IDrawInstruction : IDisposable
    {
        string Tag { get; }
        Texture2D Texture { get; set; }
        Rectangle DestinationRectangle { get; }

        Rectangle SourceRectangle { get; }

        Color Color { get; set; }

        Vector2 Origin { get; }

        float Rotation { get; set; }

        SpriteEffects Effects { get; set; }

        float LayerDepth { get; set; }
    }
}
