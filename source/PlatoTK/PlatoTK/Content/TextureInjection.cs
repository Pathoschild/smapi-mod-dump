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

namespace PlatoTK.Content
{
    internal class TextureInjection : PatchableInjection<Texture2D>
    {
        public TextureInjection(
            IPlatoHelper helper,
            string assetName,
            Texture2D value,
            InjectionMethod method,
            Rectangle? sourceArea = null,
            Rectangle? targetArea = null,
            string conditions = "")
            : base(helper, assetName, value, method, sourceArea, targetArea, conditions)
        {
        }
    }
}
