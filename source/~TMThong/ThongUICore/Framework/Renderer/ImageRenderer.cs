/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThongUICore.Framework.Style;

namespace ThongUICore.Framework.Renderer
{
    public class ImageRenderer : BaseRenderer
    {

        public ImageRendererStyle Style { get; set; } = ImageRendererStyle.Fixed;

        public override void Draw(SpriteBatch spriteBatch, int x, int y, int width, int height)
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

         
    }
}
