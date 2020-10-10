/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/chaos234/StardewValleyMods
**
*************************************************/

using System.Linq;
using StardewConfigFramework;
using StardewModdingAPI;

namespace CommunityCenterBundleOverhaul.Framework
{
    internal class BundleEditor : IAssetEditor
    {
        /*********
        ** Properties
        *********/
        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;
        private readonly ModOptionSelection DropDown;


        /*********
        ** Public methods
        *********/
        public BundleEditor(IModHelper helper, IMonitor monitor, ModOptionSelection dropDown)
        {
            this.Helper = helper;
            this.Monitor = monitor;
            this.DropDown = dropDown;
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals(@"Data\Bundles");
        }

        public void Edit<T>(IAssetData asset)
        {
            // get bundle
            Bundle[] data = this.Helper.ReadJsonFile<Bundle[]>(@"bundles\bundles.json");
            Bundle bundle = data.FirstOrDefault(p => p.ID == this.DropDown.SelectionIndex);
            if (bundle == null)
                return;

            // edit asset
            foreach (Content content in bundle.Content)
            {
                if (!content.Key.Contains("Vault"))
                {
                    string translation = this.Helper.Translation.Get(content.BundleName);
                    this.Monitor.Log($"[{content.Key}] = {content.BundleName}{content.BundleContent}/{translation}");
                    asset.AsDictionary<string, string>().Set(content.Key, content.BundleName + content.BundleContent + "/" + translation);
                }
                else
                {
                    this.Monitor.Log($"[{content.Key}] = {content.BundleName}{content.BundleContent}");
                    asset.AsDictionary<string, string>().Set(content.Key, content.BundleName + content.BundleContent);
                }
            }
        }
    }
}
