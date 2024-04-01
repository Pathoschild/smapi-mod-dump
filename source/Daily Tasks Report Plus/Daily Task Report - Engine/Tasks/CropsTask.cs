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
using StardewValley.TerrainFeatures;
using DailyTasksReport.TaskEngines;


//#pragma warning disable CC0021 // Use nameof

namespace DailyTasksReport.Tasks
{
    public class CropsTask : DailyReportTask
    {
        internal CropsTask(TaskReportConfig config, CropsTaskId id)
        {
            _config = config;
            _Engine = new CropTaskEngine(config, id);

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

            if (!Enabled || _Engine.GeneralInfo().Count() == 0) return "";

            usedLines = _Engine.DetailedInfo().Count();
            string sHeader = (CropsTaskId)_Engine.TaskId switch
            {
                CropsTaskId.DeadCropFarm => I18n.Tasks_Crop_Dead(),
                CropsTaskId.DeadCropGreenhouse => I18n.Tasks_Crop_Dead(),
                CropsTaskId.DeadCropWestIsland => I18n.Tasks_Crop_Dead(),
                CropsTaskId.FruitTreesFarm => I18n.Tasks_Crop_Fruit(),
                CropsTaskId.FruitTreesGreenhouse => I18n.Tasks_Crop_Fruit(),
                CropsTaskId.FruitTreesWestIsland => I18n.Tasks_Crop_Fruit(),
                CropsTaskId.UnharvestedCropFarm => I18n.Tasks_Crop_ReadyToHarvest(),
                CropsTaskId.UnharvestedCropGreenhouse => I18n.Tasks_Crop_ReadyToHarvest(),
                CropsTaskId.UnharvestedCropWestIsland => I18n.Tasks_Crop_ReadyToHarvest(),
                CropsTaskId.UnwateredCropFarm => I18n.Tasks_Crop_Label_Unwatered(),
                CropsTaskId.UnwateredCropGreenhouse => I18n.Tasks_Crop_Label_Unwatered(),
                CropsTaskId.UnwateredCropWestIsland => I18n.Tasks_Crop_Label_Unwatered(),
                _ => ""
            };

            return sHeader + ":^" + string.Join("^", _Engine.DetailedInfo()) + (_Engine.DetailedInfo().Count > 0 ? "^" : "");
        }
        private void SettingsMenu_ReportConfigChanged(object? sender, EventArgs e)
        {
            _Engine.SetEnabled();
        }
        public override void Draw(SpriteBatch b)
        {
            if (CropTaskEngine._who != (CropsTaskId)_Engine.TaskId) return;

            var x = Game1.viewport.X / Game1.tileSize;
            var xLimit = (Game1.viewport.X + Game1.viewport.Width) / Game1.tileSize;
            var yStart = Game1.viewport.Y / Game1.tileSize;
            var yLimit = (Game1.viewport.Y + Game1.viewport.Height) / Game1.tileSize + 1;
            for (; x <= xLimit; ++x)
                for (var y = yStart; y <= yLimit; ++y)
                    if (Game1.currentLocation.terrainFeatures.TryGetValue(new Vector2(x, y), out var t) &&
                        t is HoeDirt dirt && dirt.crop != null)
                    {
                        var v = new Vector2(x * Game1.tileSize - Game1.viewport.X + Game1.tileSize / 8,
                            y * Game1.tileSize - Game1.viewport.Y - Game1.tileSize * 2 / 4);

                        var cropDead = dirt.crop.dead.Value;
                        var dirtState = dirt.state.Value;
                        if (cropDead && _config.DrawBubbleDeadCrops)
                            DrawBubble(b, Game1.mouseCursors, new Rectangle(269, 471, 14, 15), v);
                        else if (dirt.readyForHarvest() && _config.DrawBubbleUnharvestedCrops)
                        {
                            bool drawBubble = true;
                            if (_config.SkipFlowersInHarvest)
                            {
                                if(Game1.objectData.TryGetValue(dirt.crop.indexOfHarvest.Value, out var crop))
                                {
                                    drawBubble = crop.Category != -80;
                                }
                            }
                            if (drawBubble)
                                DrawBubble(b, Game1.mouseCursors, new Rectangle(32, 0, 10, 10), v);
                        }
                        else if (dirtState == HoeDirt.dry && _config.DrawBubbleUnwateredCrops)
                            DrawBubble(b, Game1.toolSpriteSheet, new Rectangle(49, 226, 15, 13), v);
                    }
        }


    }
}

//#pragma warning restore CC0021 // Use nameof