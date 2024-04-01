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
    class ObjectEditor
    {
        private const string ObjInfoPath = "Data/ObjectInformation";

        public bool CanEdit(IAssetName assetName)
        {
            return ModEntry.JsonAssets != null && assetName.IsEquivalentTo(ObjInfoPath);
        }

        public void Edit(IAssetData asset)
        {
            if (asset.NameWithoutLocale.IsEquivalentTo(ObjInfoPath))
            {
                int id = ModEntry.JsonAssets.GetObjectId(ModEntry.LegendaryBaitName);
                var data = asset.AsDictionary<int, string>().Data;
                if (data.ContainsKey(id))
                {
                    string[] fields = data[id].Split('/');
                    fields[3] = "Basic -21";
                    data[id] = string.Join("/", fields);
                }

            }
        }
    }
}
