using Microsoft.Xna.Framework.Graphics;
using System;

namespace NpcAdventure.Events
{
    public interface ISpecialModEvents
    {
        event EventHandler<ILocationRenderedEventArgs> RenderedLocation;
    }

    public interface ILocationRenderedEventArgs
    {
        SpriteBatch SpriteBatch { get; }
    }
}
