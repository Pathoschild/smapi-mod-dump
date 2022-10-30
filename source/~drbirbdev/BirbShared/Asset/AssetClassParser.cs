/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;
using System.Reflection;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace BirbShared.Asset
{

    public class AssetClassParser
    {

        private readonly IMod Mod;
        private readonly object Assets;
        private readonly AssetClass AssetClass;


        public AssetClassParser(IMod mod, object assets)
        {
            this.Mod = mod;
            this.Assets = assets;

            this.AssetClass = this.ParseClass();
        }

        private AssetClass ParseClass()
        {
            foreach (object attr in this.Assets.GetType().GetCustomAttributes(false))
            {
                if (attr is AssetClass assetClass)
                {
                    return assetClass;
                }
            }
            Log.Error("Mod is attempting to parse asset file, but is not using AssetClass attribute.  Please reach out to mod author to enable Asset Class.");
            return null;
        }

        public bool IsEnabled()
        {
            return this.AssetClass != null;
        }

        public bool ParseAssets()
        {
            if (!this.IsEnabled())
            {
                return false;
            }

            this.ParseAllProperties();

            return true;
        }

        private void ParseAllProperties()
        {
            foreach(PropertyInfo propertyInfo in this.Assets.GetType().GetProperties())
            {
                foreach(Attribute attr in propertyInfo.GetCustomAttributes(false))
                {
                    if (attr is AssetProperty assetProperty)
                    {
                        this.AddAssetEvents(propertyInfo, assetProperty);
                    }
                }
            }
        }

        private void AddAssetEvents(PropertyInfo propertyInfo, AssetProperty assetProperty)
        {
            string assetId = this.GetAssetId(propertyInfo);
            string localPath = PathUtilities.NormalizePath(assetProperty.LocalPath);

            PropertyInfo pathPropertyInfo = this.Assets.GetType().GetProperty(propertyInfo.Name + "Path");

            this.Mod.Helper.Events.Content.AssetRequested += (object sender, AssetRequestedEventArgs e) =>
            {
                if (e.Name.IsEquivalentTo(assetId))
                {
                    object value = e.GetType().GetMethod("LoadFromModFile")
                        .MakeGenericMethod(propertyInfo.PropertyType)
                        .Invoke(e, new object[] { localPath, assetProperty.Priority });
                    propertyInfo.SetValue(this.Assets, value);
                    if (pathPropertyInfo is not null)
                    {
                        pathPropertyInfo.SetValue(this.Assets, PathUtilities.NormalizePath(localPath));
                    }
                }
            };

            this.Mod.Helper.Events.Content.AssetReady += (object sender, AssetReadyEventArgs e) =>
            {
                if (e.Name.IsEquivalentTo(assetId))
                {
                    propertyInfo.SetValue(this.Assets, LoadValue(propertyInfo, assetId));
                    if (pathPropertyInfo is not null)
                    {
                        pathPropertyInfo.SetValue(this.Assets, PathUtilities.NormalizeAssetName(assetId));
                    }
                }
            };

            this.Mod.Helper.Events.GameLoop.GameLaunched += (object sender, GameLaunchedEventArgs e) =>
            {
                propertyInfo.SetValue(this.Assets, LoadValue(propertyInfo, assetId));
                if (pathPropertyInfo is not null)
                {
                    pathPropertyInfo.SetValue(this.Assets, PathUtilities.NormalizeAssetName(assetId));
                }
            };
        }

        private static object LoadValue(PropertyInfo propertyInfo, string modId)
        {
            return Game1.content.GetType().GetMethod("Load", new[] {typeof(string)} )
                .MakeGenericMethod(propertyInfo.PropertyType)
                .Invoke(Game1.content, new string[] { modId });
        }

        private string GetAssetId(PropertyInfo propertyInfo)
        {
            return "Mods/" + this.Mod.ModManifest.UniqueID + "/" + propertyInfo.Name;
        }
    }
}
