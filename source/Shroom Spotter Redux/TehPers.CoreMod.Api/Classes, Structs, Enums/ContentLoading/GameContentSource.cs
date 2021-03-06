/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;

namespace TehPers.CoreMod.Api.ContentLoading {
    public class GameContentSource : IContentSource {
        public string Path { get; } = System.IO.Path.Combine(Constants.ExecutionPath, "Content");

        public T Load<T>(string path) {
            return Game1.content.Load<T>(path);
        }
    }
}