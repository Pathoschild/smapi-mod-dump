/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

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
