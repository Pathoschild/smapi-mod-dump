using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace MailFrameworkMod
{
    public class DataLoader : IAssetEditor
    {
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data\\mail");
        }

        public void Edit<T>(IAssetData asset)
        {
            var data = asset.AsDictionary<string, string>().Data;
            data["MailFrameworkPlaceholderId"] = " @, ^your farm has been infected with an unexpected bug. ^Don't panic! ^The bug and this message will auto destroy after read. ^   -Digus";
        }
    }
}
