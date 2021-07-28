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
using xTile;

namespace PlatoTK.Content
{
    public interface IMapHelper
    {
        Map PatchMapArea(Map map, Map patch, Point position, Rectangle? sourceArea = null, bool patchProperties = true, bool removeEmpty = false);
    }
}
