/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using StardewModdingAPI;

namespace StardewAquarium.Editors
{
    class AchievementEditor : IAssetEditor

    {
        private IModHelper _helper;
        private IMonitor _monitor;

        public const int AchievementId = 637201;

        public AchievementEditor(IModHelper helper, IMonitor monitor)
        {
            _helper = helper;
            _monitor = monitor;
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data\\Achievements");
        }

        public void Edit<T>(IAssetData asset)
        {
            var data = asset.AsDictionary<int, string>().Data;
            data[AchievementId]
                = $"{_helper.Translation.Get("AchievementName")}^{_helper.Translation.Get("AchievementDescription")}^true^-1^-1";
        }
    }
}
