/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

namespace TehPers.Core.Api.Setup
{
    /// <summary>
    /// A service which requires setup on game launch.
    /// </summary>
    public interface ISetup
    {
        /// <summary>
        /// Sets up this service.
        /// </summary>
        void Setup();
    }
}