/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using Microsoft.Xna.Framework;
using PlatoTK;
using StardewModdingAPI;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI.Events;
using xTile;
using xTile.ObjectModel;

namespace MapTK.MapExtras
{
    internal class MapMergerIAssetEditor
    {
        internal readonly IPlatoHelper Plato;
        internal static Dictionary<string, MapMergeData> MapMergeDataSet = new Dictionary<string, MapMergeData>();

        public MapMergerIAssetEditor(IModHelper helper)
        {
            Plato = helper.GetPlatoHelper();
        }

        private IEnumerable<MapMergeData> GetMapMerges()
        {
            return MapMergeDataSet.Select(k => k.Value);
        }

        public void OnAssetRequested(AssetRequestedEventArgs e)
        {
            if (e.DataType == typeof(Map)
                && GetMapMerges().FirstOrDefault(m => e.NameWithoutLocale.IsEquivalentTo(m.Target)) is MapMergeData merge
                && Plato.ModHelper.GameContent.Load<Map>(merge.Source) is Map patch) {
                e.Edit(asset =>
                {
                    var editor = asset.AsMap();

                    Map patched = Plato.Content.Maps.PatchMapArea(editor.Data, patch, new Point(merge.ToArea.X, merge.ToArea.Y), merge.FromArea, merge.PatchMapProperties, merge.RemoveEmpty);
                    editor.PatchMap(patched, merge.ToArea, merge.ToArea);

                    if (merge.PatchMapProperties)
                    {
                        foreach (KeyValuePair<string, PropertyValue> p in patched.Properties)
                            editor.Data.Properties[p.Key] = p.Value;
                    }
                });
            }
            else if (e.Name.IsEquivalentTo(MapExtrasHandler.MapMergeDirectory))
                e.LoadFrom(() => MapMergeDataSet, AssetLoadPriority.Medium);
        }
    }
}
