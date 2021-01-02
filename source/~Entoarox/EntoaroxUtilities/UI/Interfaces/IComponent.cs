/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entoarox.Utilities.UI.Interfaces
{
    public interface IComponent
    {
        IComponentContainer Container { get; set; }
        string Id { get; set; }
        int Layer { get; set; }
        bool Visible { get; set; }
        Rectangle DisplayRegion { get; set; }

        void Draw(Rectangle drawRect, SpriteBatch batch);
    }
}
