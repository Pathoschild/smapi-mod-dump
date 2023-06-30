/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
namespace FindObjectMod.Framework
{
    public abstract class ModTool
    {
        public IModHelper Helper { get; }

        public IMonitor Monitor { get; }

        public ModConfig Config { get; }

        public ModTool(IModHelper modHelper, IMonitor monitor, ModConfig modConfig)
        {
            this.Helper = modHelper;
            this.Monitor = monitor;
            this.Config = modConfig;
            this.rendered = delegate (object o, RenderedWorldEventArgs e)
            {
                bool flag = Game1.activeClickableMenu != null;
                if (!flag)
                {
                    this.WorldRendered(e.SpriteBatch);
                }
            };
            this.rendering = delegate (object o, RenderingWorldEventArgs e)
            {
                bool flag = Game1.activeClickableMenu != null;
                if (!flag)
                {
                    this.WorldRendering(e.SpriteBatch);
                }
            };
        }

        public virtual void WorldRendered(SpriteBatch batch)
        {
        }

        public virtual void WorldRendering(SpriteBatch batch)
        {
        }

        internal xTile.Dimensions.Rectangle ViewPort()
        {
            return Game1.viewport;
        }

        public virtual void Initialization()
        {
            this.Helper.Events.Display.RenderedWorld += this.rendered.Invoke;
            this.Helper.Events.Display.RenderingWorld += this.rendering.Invoke;
        }

        public virtual void Destroy()
        {
            this.Helper.Events.Display.RenderedWorld -= this.rendered.Invoke;
            this.Helper.Events.Display.RenderingWorld -= this.rendering.Invoke;
        }

        private Action<object, RenderedWorldEventArgs> rendered;

        private Action<object, RenderingWorldEventArgs> rendering;
    }
}
