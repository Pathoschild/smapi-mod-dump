/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ribeena/BusLocations
**
*************************************************/

namespace LineSprinklers.Framework
{
    /// <summary>The API methods provided by Json Assets used by this mod.</summary>
    public interface IJsonAssetsApi
    {
        /// <summary>Load assets from a given path.</summary>
        /// <param name="path">The absolute folder path.</param>
        void LoadAssets(string path);

        /// <summary>Get the object ID for a given big craftable name.</summary>
        /// <param name="name">The item name.</param>
        int GetBigCraftableId(string name);
    }
}
