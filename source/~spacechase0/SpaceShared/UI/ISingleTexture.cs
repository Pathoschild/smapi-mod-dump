/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;

#if IS_SPACECORE
namespace SpaceCore.UI
{
    public
#else

namespace SpaceShared.UI
{
    internal
#endif
    interface ISingleTexture
    {
        public Texture2D Texture { get; set; }
    }
}
