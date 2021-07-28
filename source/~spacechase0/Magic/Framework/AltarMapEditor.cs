/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using xTile;

namespace Magic.Framework
{
    /// <summary>An asset editor which adds the altar to Pierre's shop map.</summary>
    internal class AltarMapEditor : IAssetEditor
    {
        /*********
        ** Fields
        *********/
        /// <summary>The mod configuration.</summary>
        private readonly Configuration Config;

        /// <summary>The SMAPI API for loading content assets.</summary>
        private readonly IContentHelper Content;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="config">The mod configuration.</param>
        /// <param name="content">The SMAPI API for loading content assets.</param>
        public AltarMapEditor(Configuration config, IContentHelper content)
        {
            this.Config = config;
            this.Content = content;
        }

        /// <inheritdoc />
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals($"Maps/{this.Config.AltarLocation}");
        }

        /// <inheritdoc />
        public void Edit<T>(IAssetData asset)
        {
            Map altar = this.Content.Load<Map>("assets/altar.tmx");
            asset.AsMap().PatchMap(altar, targetArea: new Rectangle(this.Config.AltarX, this.Config.AltarY, 3, 3));
        }
    }
}
