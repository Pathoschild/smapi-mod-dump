/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/explosivetortellini/bluechickensaregreen
**
*************************************************/

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using Microsoft.Xna.Framework.Graphics;

namespace BlueChickensAreGreen
{
    public class ModEntry : Mod, IAssetLoader
    {
        /*********
        ** Public methods
        *********/

        /// <summary> The mod entry point, called after the mod is first loaded</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            
        }
        /// <summary>Get whether this instance can load the initial version of the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanLoad<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals(Path.Combine("Animals", "BabyBlue Chicken")) || asset.AssetNameEquals(Path.Combine("Animals", "Blue Chicken")))
            {
                return true;
            }

            return false;
        }

        /// <summary>Load a matched asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public T Load<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals(Path.Combine("Animals", "BabyBlue Chicken")))
            {
                return this.Helper.Content.Load<T>(Path.Combine("assets", "BabyBlue Chicken.xnb"), ContentSource.ModFolder);
            }
            else if (asset.AssetNameEquals(Path.Combine("Animals", "Blue Chicken")))
            {
                return this.Helper.Content.Load<T>(Path.Combine("assets", "Blue Chicken.xnb"), ContentSource.ModFolder);
            }

            throw new InvalidOperationException($"Unexpected asset '{asset.AssetName}'.");
        }
    }
}

