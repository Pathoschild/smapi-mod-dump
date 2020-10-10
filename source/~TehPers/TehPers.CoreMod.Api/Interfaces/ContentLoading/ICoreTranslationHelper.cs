/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.SqlServer.Server;
using StardewModdingAPI;

namespace TehPers.CoreMod.Api.ContentLoading {
    public interface ICoreTranslationHelper {
        /// <summary>Gets the translation for the specified key.</summary>
        /// <param name="key">The key for the translation.</param>
        /// <returns>The translation associated with the given key.</returns>
        ICoreTranslation Get(string key);

        /// <summary>Gets all translations.</summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of all the registered translations.</returns>
        IEnumerable<ICoreTranslation> GetAll();
    }
}