/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/andyruwruw/stardew-valley-pet-bowl-sprinklers
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using PetBowlSprinklers.Framework;

namespace PetBowlSprinklers
{
    /// <summary>
    /// The mod entry point.
    /// </summary>
    public class ModEntry : Mod
    {
        /// <summary>
        /// The mod configuration from the player.
        /// </summary>
        private ModConfig Config;

        /// <summary>
        /// Save data.
        /// </summary>
        private ModData Data;

        /// <summary>
        /// The mod entry point, called after the mod is first loaded.
        /// </summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += this.GameLaunched;
            helper.Events.GameLoop.SaveLoaded += this.SaveLoaded;
            helper.Events.GameLoop.DayEnding += this.DayEnding;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        }

        /// <summary>
        /// Event handler on game launch.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event data.</param>
        public void GameLaunched(
            object sender,
            GameLaunchedEventArgs e
        )
        {
            // Is GenericModConfigMenu installed.
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            // If we can't find it that's okay.
            if (configMenu is null)
            {
                return;
            }

            // Register PetBowlSprinklers
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );

            // Add our options.
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Sprinklers Fill Bowls",
                tooltip: () => "Should sprinklers be able to fill pet bowls?",
                getValue: () => this.Config.ForceExactBowlTile,
                setValue: value => this.Config.ForceExactBowlTile = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Force Exact Pet Bowl Tile",
                tooltip: () => "Limit watering to the exact bowl tile, not the full platform.",
                getValue: () => this.Config.ForceExactBowlTile,
                setValue: value => this.Config.ForceExactBowlTile = value
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Days Before Refill",
                tooltip: () => "How many days should a filled bowl last?",
                getValue: () => this.Config.BowlFilledDuration,
                setValue: value => this.Config.BowlFilledDuration = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Snow Fills Bowl",
                tooltip: () => "Should snow be able to fill the bowl?",
                getValue: () => this.Config.SnowFillsBowl,
                setValue: value => this.Config.SnowFillsBowl = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Cheaty Watering",
                tooltip: () => "Just fill my pet bowl automatically, sprinkler or no sprinkler.",
                getValue: () => this.Config.CheatyWatering,
                setValue: value => this.Config.CheatyWatering = value
            );
        }

        /// <summary>
        /// Event handler on save loaded.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event data.</param>
        public void SaveLoaded(
            object sender,
            SaveLoadedEventArgs e
        )
        {
            this.Data = this.Helper.Data.ReadSaveData<ModData>("pet-bowl-sprinklers");

            // Create a new data file.
            if (this.Data == null)
            {
                this.Data = new ModData();

                foreach (Building building in Game1.getFarm().buildings)
                {
                    if (building != null && building is PetBowl)
                    {
                        this.Data.BowlIds.Add(building.id.Value.ToString());
                        this.Data.LastFilled.Add(-1);
                        this.Data.StartedWatered.Add((building as PetBowl).watered.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Event handler on save.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event data.</param>
        public void DayEnding(
            object sender,
            DayEndingEventArgs e
        )
        {
            foreach (PetBowl bowl in this.GetBowlList())
            {
                if (this.WasWateredManually(bowl))
                {
                    this.WaterABowl(bowl);
                }
            }

            this.Helper.Data.WriteSaveData(
                "pet-bowl-sprinklers",
                this.Data
            );
        }

        /// <summary>
        /// Event handler per day.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event data.</param>
        public void OnDayStarted(
            object sender,
            EventArgs e
        )
        {
            foreach (PetBowl bowl in this.GetBowlList())
            {
                // Ignore if it's raining, that's covered.
                if (Game1.getFarm().IsRainingHere())
                {
                    this.WaterABowl(bowl);
                    continue;
                }

                // Cheater cheater pumkin eater...
                if (this.Config.CheatyWatering)
                {
                    this.WaterABowl(bowl);
                    continue;
                }

                // Ez snow.
                if (Game1.getFarm().IsSnowingHere()
                    && this.Config.SnowFillsBowl)
                {
                    this.WaterABowl(bowl);
                    continue;
                }

                // Check if it should be watered from previous days.
                if (this.Config.BowlFilledDuration > 1
                    && this.GetLastFilled(bowl) != -1)
                {
                    int low = this.GetLastFilled(bowl);
                    int high = this.GetLastFilled(bowl) + this.Config.BowlFilledDuration;

                    // Is it still within the range?
                    if ((Game1.dayOfMonth > low && Game1.dayOfMonth < high)
                        || Game1.dayOfMonth + 28 > low && Game1.dayOfMonth + 28 < high)
                    {
                        this.WaterABowl(
                            bowl,
                            false
                        );
                        continue;
                    }
                    else
                    {
                        this.LostItsWater(bowl);
                    }
                }

                // Should sprinklers work.
                if (this.Config.SprinklersFillBowls)
                {
                    // Find location of the bowl.
                    IList<Vector2> validWateringLocations = this.GetValidWateringLocations(bowl);
                    bool shouldWater = false;

                    foreach (StardewValley.Object sprinkler in this.GetSprinklerList())
                    {
                        // Get the location and range.
                        Vector2 sprinklerLocation = sprinkler.TileLocation;
                        float range = sprinkler.GetBaseRadiusForSprinkler();

                        // For each valid location.
                        foreach (Vector2 point in validWateringLocations)
                        {
                            // If it's within range.
                            if (range == 0)
                            {
                                if ((Math.Abs(point.X - sprinklerLocation.X) == 1
                                    && point.Y == sprinklerLocation.Y)
                                    || (Math.Abs(point.Y - sprinklerLocation.Y) == 1
                                    && point.X == sprinklerLocation.X))
                                {
                                    shouldWater = true;
                                    break;
                                }
                            }
                            else
                            {
                                if (Math.Abs(point.X - sprinklerLocation.X) <= 1
                                    && Math.Abs(point.Y - sprinklerLocation.Y) <= 1)
                                {
                                    shouldWater = true;
                                    break;
                                }
                            }
                        }

                        if (shouldWater)
                        {
                            break;
                        }
                    }

                    if (shouldWater)
                    {
                        this.WaterABowl(bowl);
                        return;
                    }
                }

                this.DidntGetWatered(bowl);
            }
        }

        /// <summary>
        /// Retrieves a list of all pet bowls.
        /// </summary>
        /// <returns>List of pet bowls.</returns>
        private IList<PetBowl> GetBowlList()
        {
            IList<PetBowl> bowls = new List<PetBowl>();

            foreach (Building building in Game1.getFarm().buildings)
            {
                if (building != null && building is PetBowl)
                {
                    bowls.Add(building as PetBowl);
                }
            }

            return bowls;
        }

        /// <summary>
        /// Retrieves all sprinklers.
        /// </summary>
        private IList<StardewValley.Object> GetSprinklerList()
        {
            IList<StardewValley.Object> sprinklers = new List<StardewValley.Object>();

            // Look through object chart.
            foreach (KeyValuePair<Vector2, StardewValley.Object> objectPair in Game1.getFarm().objects.Pairs)
            {
                // If it's a sprinkler.
                if (objectPair.Value.IsSprinkler())
                {
                    sprinklers.Add(objectPair.Value);
                }
            }

            return sprinklers;
        }

        /// <summary>
        /// Day of the month the bowl was last watered.
        /// </summary>
        /// <param name="bowl">Pet bowl in question.</param>
        private int GetLastFilled(PetBowl bowl)
        {
            // Find the saved bowl.
            string id = bowl.id.Value.ToString();
            int index = this.Data.BowlIds.IndexOf(id);

            // If not found add it.
            if (index == -1)
            {
                index = this.Data.BowlIds.Count;
                this.Data.BowlIds.Add(id);
                this.Data.StartedWatered.Add(false);
                this.Data.LastFilled.Add(-1);
            }

            return this.Data.LastFilled[index];
        }

        /// <summary>
        /// Whether the bowl was watered manually.
        /// </summary>
        /// <param name="bowl">Pet bowl in question.</param>
        private bool WasWateredManually(PetBowl bowl)
        {
            // Find the saved bowl.
            string id = bowl.id.Value.ToString();
            int index = this.Data.BowlIds.IndexOf(id);

            // If not found add it.
            if (index == -1)
            {
                this.Data.BowlIds.Add(id);
                this.Data.StartedWatered.Add(false);
                this.Data.LastFilled.Add(-1);
            }

            return !this.Data.StartedWatered[index] && bowl.watered.Value;
        }

        /// <summary>
        /// Handles watering a bowl.
        /// </summary>
        /// <param name="bowl">Pet bowl to water.</param>
        /// <param name="happenedToday">Whether the bowl was watered today.</param>
        private void WaterABowl(
            PetBowl bowl,
            bool happenedToday = true
        )
        {
            // Find the saved bowl.
            string id = bowl.id.Value.ToString();
            int index = this.Data.BowlIds.IndexOf(id);

            // If not found add it.
            if (index == -1)
            {
                index = this.Data.BowlIds.Count;
                this.Data.BowlIds.Add(id);
                this.Data.StartedWatered.Add(false);
                this.Data.LastFilled.Add(-1);
            }

            // Water the bowl.
            bowl.watered.Value = true;
            this.Data.StartedWatered[index] = true;

            if (happenedToday)
            {
                this.Data.LastFilled[index] = Game1.dayOfMonth;
            }
        }

        /// <summary>
        /// Handles a watering bowl going out.
        /// </summary>
        /// <param name="bowl">Pet bowl to unwater.</param>
        private void LostItsWater(PetBowl bowl)
        {
            // Find the saved bowl.
            string id = bowl.id.Value.ToString();
            int index = this.Data.BowlIds.IndexOf(id);

            // If not found add it.
            if (index == -1)
            {
                index = this.Data.BowlIds.Count;
                this.Data.BowlIds.Add(id);
                this.Data.StartedWatered.Add(false);
                this.Data.LastFilled.Add(-1);
                return;
            }

            this.Data.LastFilled[index] = -1;
        }

        /// <summary>
        /// Handles a watering bowl not being watered.
        /// </summary>
        /// <param name="bowl">Pet bowl to unwater.</param>
        private void DidntGetWatered(PetBowl bowl)
        {
            // Find the saved bowl.
            string id = bowl.id.Value.ToString();
            int index = this.Data.BowlIds.IndexOf(id);

            // If not found add it.
            if (index == -1)
            {
                index = this.Data.BowlIds.Count;
                this.Data.BowlIds.Add(id);
                this.Data.StartedWatered.Add(false);
                this.Data.LastFilled.Add(-1);
                return;
            }

            this.Data.StartedWatered[index] = false;
        }

        /// <summary>
        /// A list of points the bowl can be watered from.
        /// </summary>
        /// <param name="bowl">Pet bowl in question.</param>
        private IList<Vector2> GetValidWateringLocations(PetBowl bowl)
        {
            // Find location of the bowl.
            IList<Vector2> validLocations = new List<Vector2>{
                new Vector2(
                    bowl.tileX.Value + 1,
                    bowl.tileY.Value
                )
            };

            // The ground too?
            if (!this.Config.ForceExactBowlTile)
            {
                for (int i = 0; i < bowl.tilesWide; i++)
                {
                    for (int j = 0; j < bowl.tilesHigh; j++)
                    {
                        // We already did this tile.
                        if (i == 1 && j == 0)
                        {
                            continue;
                        }

                        validLocations.Add(new Vector2(
                            bowl.tileX.Value + i,
                            bowl.tileY.Value + j
                        ));
                    }
                }
            }

            return validLocations;
        }
    }
}
