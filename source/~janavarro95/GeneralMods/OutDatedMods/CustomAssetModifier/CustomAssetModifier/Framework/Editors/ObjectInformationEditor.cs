using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomAssetModifier.Framework.Editors
{

    /// <summary>
    /// Asset editor for assets stored in ObjectInformation.xnb
    /// </summary>
    public class ObjectInformationEditor : IAssetEditor
    {
        /// <summary>Get whether this instance can edit the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals(@"Data\ObjectInformation");
        }

        /// <summary>Edit a matched asset.</summary>
        /// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
        public void Edit<T>(IAssetData asset)
        {
            string[] files = Directory.GetFiles(CustomAssetModifier.objectInformationPath);
            foreach (var file in files)
            {
                var ok = AssetInformation.readJson(file);
                asset
                    .AsDictionary<int, string>()
                    .Set(Convert.ToInt32(ok.id), ok.informationString);
            }
        }
    }

}
