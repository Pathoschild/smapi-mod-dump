using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcAdventure.Events
{
    internal class SpecialModEvents : ISpecialModEvents
    {
        public event EventHandler<ILocationRenderedEventArgs> RenderedLocation;

        internal void HandleRenderedLocation(object sender, LocationRenderedEventArgs e)
        {
            this.RenderedLocation?.Invoke(sender, e);
        }
    }

    internal class LocationRenderedEventArgs : ILocationRenderedEventArgs
    {
        public SpriteBatch SpriteBatch { get; internal set; }
    }
}
