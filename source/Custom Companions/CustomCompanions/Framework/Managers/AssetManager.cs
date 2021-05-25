/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/CustomCompanions
**
*************************************************/

using CustomCompanions.Framework.Managers;
using CustomCompanions.Framework.Models.Companion;
using CustomCompanions.Framework.Utilities;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomCompanions.Framework.Assets
{
    internal class AssetManager : IAssetLoader
    {
        internal static Dictionary<string, string> idToAssetToken;

        public bool CanLoad<T>(IAssetInfo asset)
        {
            return idToAssetToken.Keys.Any(i => asset.AssetNameEquals($"{CustomCompanions.TOKEN_HEADER}{i}"));
        }

        public T Load<T>(IAssetInfo asset)
        {
            var model = CompanionManager.companionModels.First(c => asset.AssetNameEquals($"{CustomCompanions.TOKEN_HEADER}{c.GetId()}"));

            //CustomCompanions.monitor.Log($"TEST: {JsonParser.Serialize<object>(model).ToString()}", LogLevel.Warn);
            return (T)(object)new Dictionary<string, object>() { { CustomCompanions.COMPANION_KEY, JsonParser.Serialize<object>(model) } };
        }

        internal static object GetCompanionModelObject(Dictionary<string, object> companionModelAsset)
        {
            if (!companionModelAsset.ContainsKey(CustomCompanions.COMPANION_KEY))
            {
                return null;
            }

            return companionModelAsset[CustomCompanions.COMPANION_KEY];
        }
    }
}