/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/M3ales/ChestNaming
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChestNaming.UI
{
    /// <summary>
    /// All drawable elements which are rendered onto a frame
    /// </summary>
    public interface IFrameDrawable
    {
        void Draw(SpriteBatch b, int x, int y, Frame parentFrame);
        int SizeX { get; }
        int SizeY { get; }
    }
}
