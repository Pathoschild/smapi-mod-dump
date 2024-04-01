/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/DailyTaskReportPlus
**
*************************************************/

using DailyTasksReport.UI;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Buildings;

using DailyTasksReport.TaskEngines;

namespace DailyTasksReport.Tasks
{
    public class AnimalsTask : DailyReportTask
    {

        internal AnimalsTask(TaskReportConfig config, AnimalsTaskId id)
        {
            _config = config;
            _Engine = new AnimalTaskEngine(config, id);

            SettingsMenu.ReportConfigChanged += SettingsMenu_ReportConfigChanged;
        }


        public override string GeneralInfo(out int usedLines)
        {
            usedLines = 0;

            if (!Enabled) return "";

            _Engine.UpdateList();

            usedLines = _Engine.GeneralInfo().Count();

            return string.Join("^", _Engine.GeneralInfo()) + (_Engine.GeneralInfo().Count > 0 ? "^" : "");

        }

        public override string DetailedInfo(out int usedLines, out bool skipNextPage)
        {
            usedLines = 0;
            skipNextPage = false;

            if (!Enabled) return "";

            usedLines = _Engine.DetailedInfo().Count();
            if (usedLines == 0) return "";

            string sHeader = (AnimalsTaskId)_Engine.TaskId switch
            {
                AnimalsTaskId.PondsWithItems => I18n.Tasks_Ponds_Collect(),
                AnimalsTaskId.PondsNeedingAttention => I18n.Tasks_Ponds_Attention(),
                AnimalsTaskId.MissingHay => I18n.Tasks_Animal_MissingHay(),
                AnimalsTaskId.AnimalProducts => I18n.Tasks_Animal_Uncollected(),
                AnimalsTaskId.UnpettedAnimals => I18n.Tasks_Animal_NotPetted(),
                _ => ""
            };

            return sHeader + ":^" + string.Join("^", _Engine.DetailedInfo()) + "^";

        }
        private void SettingsMenu_ReportConfigChanged(object? sender, EventArgs e)
        {
            _Engine.SetEnabled();
        }

        public override void Draw(SpriteBatch b)
        {
            if ((AnimalsTaskId)_Engine.TaskId != AnimalTaskEngine._who) return;

            // Truffles
            if (_config.DrawBubbleTruffles && Game1.currentLocation is Farm)
            {
                int x = Game1.viewport.X / Game1.tileSize;
                int xLimit = (Game1.viewport.X + Game1.viewport.Width) / Game1.tileSize;
                var yStart = Game1.viewport.Y / Game1.tileSize;
                int yLimit = (Game1.viewport.Y + Game1.viewport.Height) / Game1.tileSize + 1;
                for (; x <= xLimit; ++x)
                    for (int y = yStart; y <= yLimit; ++y)
                    {
                        if (!Game1.currentLocation.objects.TryGetValue(new Vector2(x, y), out SDObject? o)) continue;

                        Vector2 v = new Vector2(o.TileLocation.X * Game1.tileSize - Game1.viewport.X + Game1.tileSize / 8f,
                            o.TileLocation.Y * Game1.tileSize - Game1.viewport.Y - Game1.tileSize * 2 / 4f);
                        if (o.name == "Truffle")
                            DrawBubble(Game1.spriteBatch, Game1.objectSpriteSheet, new Rectangle(352, 273, 14, 14), v);
                    }
            }

            // Animals

            //var animalDict = (Game1.currentLocation as Farm)?.animals ??
            //                 (Game1.currentLocation as AnimalHouse)?.animals;
            var animalDict = Game1.currentLocation?.animals ?? null;


            if (animalDict == null) return;

            foreach (var animal in animalDict.Pairs)
            {
                if (animal.Value.isEmoting) continue;

                var currentProduce = animal.Value.currentProduce.Value;

                var needsPet = _config.DrawBubbleUnpettedAnimals && !animal.Value.wasPet.Value;
                var hasProduct = currentProduce != "430" &&
                                !string.IsNullOrEmpty(currentProduce) &&
                                 _config.DrawBubbleAnimalsWithProduce;

                var v = new Vector2(animal.Value.StandingPixel.X  - Game1.viewport.X ,
                    animal.Value.StandingPixel.Y - Game1.viewport.Y );
                if (animal.Value.home?.GetData()?.ValidOccupantTypes?.Contains("Coop") ?? false)
                {
                    v.X -= Game1.tileSize * 0.3f;
                    v.Y -= Game1.tileSize * 6 / 4f;
                }
                else
                {
                    v.X -= Game1.tileSize * 0.2f;
                    v.Y -= Game1.tileSize * 2f;
                }

                if (needsPet)
                {
                    if (hasProduct)
                    {
                        DrawBubble2Icons(b, Game1.mouseCursors, new Rectangle(117, 7, 9, 8),
                            Game1.objectSpriteSheet,
                            Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet,
                            Game1.objectData[currentProduce].SpriteIndex, 16, 16),
                            v);
                        continue;
                    }
                    DrawBubble(b, Game1.mouseCursors, new Rectangle(117, 7, 9, 8), v);
                }
                else if (hasProduct)
                {
                    DrawBubble(b, Game1.objectSpriteSheet,
                        Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, Game1.objectData[currentProduce].SpriteIndex,
                            16, 16),
                        v);
                }
            }

            // Animal Houses

            if (!Game1.currentLocation?.IsBuildableLocation()??false) return;

            foreach (Building? building in Game1.currentLocation.buildings)
                if (building.indoors.Value is AnimalHouse animalHouse)
                {
                    bool anyHayMissing = _config.DrawBubbleBuildingsMissingHay &&
                                        animalHouse.numberOfObjectsWithName("Hay") < animalHouse.animalLimit.Value;
                    bool anyProduce = _config.DrawBubbleBuildingsWithProduce && (building.GetData()?.ValidOccupantTypes?.Contains("Coop") ?? false) &&
                                     animalHouse.objects.Values.Any(o =>
                                         Array.BinarySearch(AnimalTaskEngine.CollectableAnimalProducts, o.ItemId) >= 0);

                    Vector2 v = new Vector2(building.tileX.Value * Game1.tileSize - Game1.viewport.X + Game1.tileSize * 1.1f,
                        building.tileY.Value * Game1.tileSize - Game1.viewport.Y + Game1.tileSize / 2);

                    if (building.GetData()?.ValidOccupantTypes?.Contains("Barn")??false)
                        v.Y += Game1.tileSize / 2f;

                    if (anyHayMissing)
                    {
                        if (anyProduce)
                        {
                            DrawBubble2Icons(b, Game1.mouseCursors, new Rectangle(32, 0, 10, 10),
                                Game1.objectSpriteSheet, new Rectangle(160, 112, 16, 16), v);
                            continue;
                        }
                        DrawBubble(b, Game1.objectSpriteSheet, new Rectangle(160, 112, 16, 16), v);
                    }
                    else if (anyProduce)
                    {
                        DrawBubble(b, Game1.mouseCursors, new Rectangle(32, 0, 10, 10), v);
                    }
                }
        }

    }

}