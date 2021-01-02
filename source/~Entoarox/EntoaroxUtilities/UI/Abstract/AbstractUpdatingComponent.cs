/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using Entoarox.Utilities.UI.Interfaces;

using Microsoft.Xna.Framework;

namespace Entoarox.Utilities.UI.Abstract
{
    public abstract class AbstractUpdatingComponent : AbstractComponent, IUpdatingComponent
    {
        public abstract void Update(GameTime time);
    }
}
