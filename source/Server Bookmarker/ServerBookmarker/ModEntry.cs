/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/ServerBookmarker
**
*************************************************/

using StardewModdingAPI;

namespace ServerBookmarker
{
    class ModEntry : Mod
    {
        private Manager manager;

        public override void Entry(IModHelper helper)
        {
            manager = new Manager(helper.Reflection, (path) => helper.Data.ReadJsonFile<BookmarksDataModel>(path), (data, path) => helper.Data.WriteJsonFile(path, data));
            Patch.PatchAll("me.ilyaki.serverbookmarker");
        }
    }
}
