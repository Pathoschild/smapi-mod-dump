/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using xTile;
using xTile.Display;

namespace PlatoWarpMenu
{
    public interface IMapAPI
    {
        void EnableMoreMapLayers(Map map);
        IDisplayDevice GetPyDisplayDevice(ContentManager contentManager, GraphicsDevice graphicsDevice);
        IDisplayDevice GetPyDisplayDevice(ContentManager contentManager, GraphicsDevice graphicsDevice, bool compatibility);
    }
}
