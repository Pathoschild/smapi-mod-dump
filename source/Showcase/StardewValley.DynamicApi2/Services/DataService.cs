/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/Stardew_Valley_Showcase_Mod
**
*************************************************/

using System;
using System.Collections.Generic;
using Igorious.StardewValley.DynamicApi2.Compatibility;
using Igorious.StardewValley.DynamicApi2.Data;
using StardewValley;

namespace Igorious.StardewValley.DynamicApi2.Services
{
    public sealed class DataService
    {
        private static readonly Lazy<DataService> Lazy = new Lazy<DataService>(() => new DataService());
        public static DataService Instance => Lazy.Value;

        private IList<FurnitureInfo> FurnitureData { get; } = new List<FurnitureInfo>();

        private DataService()
        {
            EntoaroxFramework�ompatibilityLayout.Instance.ContentIsReadyToOverride += OnLoadContent;
        }

        public DataService RegisterFurniture(FurnitureInfo furnitureInfo)
        {
            FurnitureData.Add(furnitureInfo);
            return this;
        }

        public IReadOnlyDictionary<int, string> GetFurniture() => GetFurnitureInternal();

        private void OnLoadContent()
        {
            EntoaroxFramework�ompatibilityLayout.Instance.ContentIsReadyToOverride -= OnLoadContent;

            var furnitureData = GetFurnitureInternal();
            foreach (var furnitureInfo in FurnitureData)
            {
                furnitureData.Add(furnitureInfo.ID, furnitureInfo.ToString());
            }

            // TODO: Other data.
        }

        private Dictionary<int, string> GetFurnitureInternal()
        {
            return Game1.content.Load<Dictionary<int, string>>(@"Data\Furniture");
        }
    }
}