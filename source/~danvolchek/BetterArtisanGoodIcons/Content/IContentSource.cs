/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using StardewModdingAPI;

namespace BetterArtisanGoodIcons.Content
{
    /// <summary>An abstraction over the ability to load textures and get manifest information. Used to unify loading vanilla and custom texture assets.</summary>
    internal interface IContentSource
    {
        T Load<T>(string path);
        IManifest GetManifest();
    }
}
