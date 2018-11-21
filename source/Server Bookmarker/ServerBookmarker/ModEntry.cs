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
