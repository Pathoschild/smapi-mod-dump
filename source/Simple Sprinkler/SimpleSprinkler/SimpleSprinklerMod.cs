using System.Linq;
using SimpleSprinkler.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace SimpleSprinkler
{
    /// <summary>The mod entry class.</summary>
    internal class SimpleSprinklerMod : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration.</summary>
        private SimpleConfig Config;

        /// <summary>Encapsulates the logic for building sprinkler grids.</summary>
        private GridHelper GridHelper;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<SimpleConfig>();
            this.GridHelper = new GridHelper(this.Config);

            helper.Events.Player.Warped += OnWarped;
        }

        /// <summary>Get an API that other mods can access. This is always called after <see cref="Entry" />.</summary>
        public override object GetApi()
        {
            return new SimpleSprinklerApi(this.Config, this.GridHelper);
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after a player warps to a new location..</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnWarped(object sender, WarpedEventArgs e)
        {
            if (e.IsLocalPlayer && this.Config.Locations.Contains(e.NewLocation.Name))
                this.ApplyWatering(e.NewLocation);
        }

        /// <summary>Apply watering for supported sprinklers in a location.</summary>
        /// <param name="location">The location whose sprinklers to apply.</param>
        private void ApplyWatering(GameLocation location)
        {
            // get sprinklers
            var sprinklers = location.Objects.Values.Where(obj => this.IsSprinkler(obj.ParentSheetIndex));
            foreach (Object sprinkler in sprinklers)
            {
                foreach (var tile in this.GridHelper.GetGrid(sprinkler.ParentSheetIndex, sprinkler.TileLocation))
                {
                    if (location.terrainFeatures.TryGetValue(tile, out TerrainFeature terrainFeature) && terrainFeature is HoeDirt dirt)
                        dirt.state.Value = HoeDirt.watered;
                }
            }
        }

        /// <summary>Get whether the given object ID matches a supported sprinkler.</summary>
        /// <param name="objectId">The object ID.</param>
        private bool IsSprinkler(int objectId)
        {
            return this.Config.Radius.ContainsKey(objectId);
        }
    }
}
