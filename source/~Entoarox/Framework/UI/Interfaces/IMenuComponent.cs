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

namespace Entoarox.Framework.UI
{
    public interface IMenuComponent
    {
        /*********
        ** Accessors
        *********/
        bool Visible { get; set; }
        int Layer { get; set; }
        IComponentContainer Parent { get; }


        /*********
        ** Public methods
        *********/
        void Update(GameTime t);
        void Draw(SpriteBatch b, Point offset);
        void Attach(IComponentContainer collection);
        void Detach(IComponentContainer collection);
        void OnAttach(IComponentContainer parent);
        void OnDetach(IComponentContainer parent);
        Point GetPosition();
        Rectangle GetRegion();
    }
}
