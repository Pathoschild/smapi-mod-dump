/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AngelaRanna/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace MoreGrassOnTheBeach
{
    public class moreGrassOnTheBeach : Mod
    {
        private ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.DayStarted += this.dayStarted;
        }

        private void dayStarted(object sender, DayStartedEventArgs e)
        {
            if (Game1.whichFarm == 6 || Config.moreGrassOnOtherFarms || Game1.whichFarm == 1)
            {
                Game1.getFarm().growWeedGrass(Config.numberOfSpawns);
            }

            if ((Game1.whichFarm == 5 || Game1.whichFarm == 3) && Config.moreRocksOnTheMines)
            {
                for (int i = 0; i < Config.numberOfSpawns; i++)
                {
                    Game1.getFarm().doDailyMountainFarmUpdate();
                }
            }

            if (Game1.whichFarm == 2 && Config.moreForageInTheForest)
            {
                for (int i = 0; i < Config.numberOfSpawns; i++)
                {
                    doForestFarmUpdate();
                }
            }
        }

        private void doForestFarmUpdate()
        {
            for (int x = 0; x < 20; ++x)
            {
                for (int y = 0; y < Game1.getFarm().map.Layers[0].LayerHeight; ++y)
                {
                    if (Game1.getFarm().map.GetLayer("Paths").Tiles[x, y] != null && Game1.getFarm().map.GetLayer("Paths").Tiles[x, y].TileIndex == 21 && (Game1.getFarm().isTileLocationTotallyClearAndPlaceable(x, y) && Game1.getFarm().isTileLocationTotallyClearAndPlaceable(x + 1, y)) && (Game1.getFarm().isTileLocationTotallyClearAndPlaceable(x + 1, y + 1) && Game1.getFarm().isTileLocationTotallyClearAndPlaceable(x, y + 1)))
                        Game1.getFarm().resourceClumps.Add(new StardewValley.TerrainFeatures.ResourceClump(600, 2, 2, new Vector2((float)x, (float)y)));
                }
            }
            if (!Game1.IsWinter)
            {
                while (Game1.random.NextDouble() < 0.75)
                {
                    Vector2 vector2 = new Vector2((float)Game1.random.Next(18), (float)Game1.random.Next(Game1.getFarm().map.Layers[0].LayerHeight));
                    if (Game1.random.NextDouble() < 0.5)
                        vector2 = Game1.getFarm().getRandomTile();
                    if (Game1.getFarm().isTileLocationTotallyClearAndPlaceable(vector2) && Game1.getFarm().getTileIndexAt((int)vector2.X, (int)vector2.Y, "AlwaysFront") == -1 && ((double)vector2.X < 18.0 || Game1.getFarm().doesTileHavePropertyNoNull((int)vector2.X, (int)vector2.Y, "Type", "Back").Equals("Grass")))
                    {
                        int parentSheetIndex = 792;
                        string currentSeason = Game1.currentSeason;
                        if (!(currentSeason == "spring"))
                        {
                            if (!(currentSeason == "summer"))
                            {
                                if (currentSeason == "fall")
                                {
                                    switch (Game1.random.Next(4))
                                    {
                                        case 0:
                                            parentSheetIndex = 281;
                                            break;
                                        case 1:
                                            parentSheetIndex = 420;
                                            break;
                                        case 2:
                                            parentSheetIndex = 422;
                                            break;
                                        case 3:
                                            parentSheetIndex = 404;
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                switch (Game1.random.Next(4))
                                {
                                    case 0:
                                        parentSheetIndex = 402;
                                        break;
                                    case 1:
                                        parentSheetIndex = 396;
                                        break;
                                    case 2:
                                        parentSheetIndex = 398;
                                        break;
                                    case 3:
                                        parentSheetIndex = 404;
                                        break;
                                }
                            }
                        }
                        else
                        {
                            switch (Game1.random.Next(4))
                            {
                                case 0:
                                    parentSheetIndex = 16;
                                    break;
                                case 1:
                                    parentSheetIndex = 22;
                                    break;
                                case 2:
                                    parentSheetIndex = 20;
                                    break;
                                case 3:
                                    parentSheetIndex = 257;
                                    break;
                            }
                        }
                        Game1.getFarm().dropObject(new StardewValley.Object(vector2, parentSheetIndex, (string)null, false, true, false, true), vector2 * 64f, Game1.viewport, true);
                    }
                }
                if (Game1.getFarm().objects.Count() > 0)
                {
                    for (int index = 0; index < 6; ++index)
                    {
                        StardewValley.Object @object = Game1.getFarm().objects.Pairs.ElementAt(Game1.random.Next(Game1.getFarm().objects.Count())).Value;
                        if (@object.name.Equals("Weeds"))
                            @object.ParentSheetIndex = 792 + Utility.getSeasonNumber(Game1.currentSeason);
                    }
                }
            }
        }
    }

    public class ModConfig
    {
        public bool moreGrassOnOtherFarms { get; set; } = false;
        public bool moreRocksOnTheMines { get; set; } = true;
        public bool moreForageInTheForest { get; set; } = false;
        public int numberOfSpawns { get; set; } = 1;
    }
}
