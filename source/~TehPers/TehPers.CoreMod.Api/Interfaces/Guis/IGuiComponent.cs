using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TehPers.CoreMod.Api.Guis {
    public interface IGuiComponent {
        IGuiComponent Parent { get; }

        void Render(SpriteBatch batch, GameTime gameTime);
    }

    public interface IGuiParentComponent {

    }
}
