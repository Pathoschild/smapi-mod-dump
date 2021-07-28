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
using Microsoft.Xna.Framework.Graphics;
using PlatoTK;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile;
using xTile.ObjectModel;

namespace MapTK.MapExtras
{
    internal class MapMergerIAssetEditor : IAssetEditor, IAssetLoader
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

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return GetMapMerges().Any(m => asset.AssetNameEquals(m.Target));
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.DataType == typeof(Map)
                && GetMapMerges().FirstOrDefault(m => asset.AssetNameEquals(m.Target)) is MapMergeData merge
                && Plato.ModHelper.Content.Load<Map>(merge.Source, ContentSource.GameContent) is Map patch) {
                Map patched = Plato.Content.Maps.PatchMapArea(asset.AsMap().Data, patch, new Point(merge.ToArea.X,merge.ToArea.Y), merge.FromArea, merge.PatchMapProperties,merge.RemoveEmpty);
                asset.AsMap().PatchMap(patched, merge.ToArea, merge.ToArea);

                if (merge.PatchMapProperties)
                    foreach (KeyValuePair<string, PropertyValue> p in patched.Properties)
                        asset.AsMap().Data.Properties[p.Key] = p.Value;
            }
        }

        public bool CanLoad<T>(IAssetInfo asset)
        {
           return asset.AssetNameEquals(MapExtrasHandler.MapMergeDirectory);
        }

        public T Load<T>(IAssetInfo asset)
        {
            return (T)(object)MapMergeDataSet;
        }
    }
}
