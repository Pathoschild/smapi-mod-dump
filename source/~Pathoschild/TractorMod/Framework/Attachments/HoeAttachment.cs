/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Common;
using Pathoschild.Stardew.TractorMod.Framework.Config;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using SObject = StardewValley.Object;

namespace Pathoschild.Stardew.TractorMod.Framework.Attachments
{
    /// <summary>An attachment for the hoe.</summary>
    internal class HoeAttachment : BaseAttachment
    {
        /*********
        ** Fields
        *********/
        /// <summary>The attachment settings.</summary>
        private readonly HoeConfig Config;

        /// <summary>The minimum delay before attempting to re-till the same empty dirt tile.</summary>
        private readonly TimeSpan TillDirtDelay = TimeSpan.FromSeconds(1);


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="config">The mod configuration.</param>
        /// <param name="modRegistry">Fetches metadata about loaded mods.</param>
        public HoeAttachment(HoeConfig config, IModRegistry modRegistry)
            : base(modRegistry)
        {
            this.Config = config;
        }

        /// <summary>Get whether the tool is currently enabled.</summary>
        /// <param name="player">The current player.</param>
        /// <param name="tool">The tool selected by the player (if any).</param>
        /// <param name="item">The item selected by the player (if any).</param>
        /// <param name="location">The current location.</param>
        public override bool IsEnabled(Farmer player, Tool? tool, Item? item, GameLocation location)
        {
            return
                (this.Config.TillDirt || this.Config.ClearWeeds || this.Config.DigArtifactSpots || this.Config.DigSeedSpots)
                && tool is Hoe;
        }

        /// <summary>Apply the tool to the given tile.</summary>
        /// <param name="tile">The tile to modify.</param>
        /// <param name="tileObj">The object on the tile.</param>
        /// <param name="tileFeature">The feature on the tile.</param>
        /// <param name="player">The current player.</param>
        /// <param name="tool">The tool selected by the player (if any).</param>
        /// <param name="item">The item selected by the player (if any).</param>
        /// <param name="location">The current location.</param>
        public override bool Apply(Vector2 tile, SObject? tileObj, TerrainFeature? tileFeature, Farmer player, Tool? tool, Item? item, GameLocation location)
        {
            tool = tool.AssertNotNull();

            // clear weeds
            if (this.Config.ClearWeeds && tileObj?.IsWeeds() == true)
                return this.UseToolOnTile(tool, tile, player, location);

            // collect artifact spots
            if (this.Config.DigArtifactSpots && tileObj?.QualifiedItemId == SObject.artifactSpotQID)
                return this.UseToolOnTile(tool, tile, player, location);

            // collect seed spots
            if (this.Config.DigSeedSpots && tileObj?.QualifiedItemId == $"{ItemRegistry.type_object}SeedSpot")
                return this.UseToolOnTile(tool, tile, player, location);

            // harvest ginger
            if (this.Config.HarvestGinger && tileFeature is HoeDirt dirt && dirt.crop?.whichForageCrop.Value == Crop.forageCrop_ginger.ToString() && dirt.crop.hitWithHoe((int)tile.X, (int)tile.Y, location, dirt))
            {
                dirt.destroyCrop(showAnimation: false);
                return true;
            }

            // till plain dirt
            if (this.Config.TillDirt && tileFeature == null && tileObj == null && this.TryStartCooldown(tile.ToString(), this.TillDirtDelay))
                return this.UseToolOnTile(tool, tile, player, location);

            return false;
        }
    }
}
