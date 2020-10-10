/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;

namespace TehPers.CoreMod.Api.ContentLoading {
    public class ModContentSource : IContentSource {
        private readonly IContentHelper _contentHelper;
        public string Path { get; }

        public ModContentSource(IMod mod) : this(mod.Helper) { }
        public ModContentSource(IModHelper helper) {
            this._contentHelper = helper.Content;
            this.Path = helper.DirectoryPath;
        }

        public T Load<T>(string path) {
            return this._contentHelper.Load<T>(path);
        }
    }
}