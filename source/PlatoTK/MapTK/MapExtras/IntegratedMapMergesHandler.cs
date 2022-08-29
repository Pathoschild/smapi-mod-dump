/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace MapTK.MapExtras
{
    internal class IntegratedMapMergesHandler
    {
        private readonly IntegratedMapEditsAssetEditor AssetEditor;

        public IntegratedMapMergesHandler(IModHelper helper)
        {
            this.AssetEditor = new IntegratedMapEditsAssetEditor(helper);
            helper.Events.Content.AssetRequested += OnAssetRequested;
        }

        public void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            this.AssetEditor.OnAssetRequested(e);
        }
    }
}
