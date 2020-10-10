/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

namespace Entoarox.Framework
{
    public delegate T AssetLoader<T>(string assetName);

    public delegate void AssetInjector<T>(string assetName, ref T asset);

    public delegate void ReceiveMessage(string modID, string channel, string message, bool broadcast);
}
