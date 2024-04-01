/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/DailyTaskReportPlus
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using DailyTasksReport.TaskEngines;
using StardewValley.Locations;
using DailyTasksReport.UI;

namespace DailyTasksReport.Tasks
{
    public class PetTask : DailyReportTask
    {
        private PetTaskEngine _PetEngine;
        internal PetTask(TaskReportConfig config)
        {
            _config = config;
            _PetEngine = new PetTaskEngine(config);
            _Engine = _PetEngine;
            SettingsMenu.ReportConfigChanged += SettingsMenu_ReportConfigChanged;
        }

        public override string GeneralInfo(out int usedLines)
        {
            usedLines = 0;


            if (!Enabled || _Engine.GeneralInfo().Count() == 0)
                return "";

            _Engine.UpdateList();
            usedLines = _Engine.GeneralInfo().Count();
            return string.Join("^", _Engine.GeneralInfo()) + "^";
        }

        public override string DetailedInfo(out int usedLines, out bool skipNextPage)
        {
            usedLines = 0;
            skipNextPage = true;
            return "";
        }
        private void SettingsMenu_ReportConfigChanged(object? sender, EventArgs e)
        {
            _Engine.SetEnabled();
        }
        public override void Draw(SpriteBatch b)
        {
            if (_PetEngine.IsPetPetted()) return;
            if (!_config.DrawBubbleUnpettedPet || _PetEngine._pet == null || _PetEngine._pet.currentLocation != Game1.currentLocation ||
                !(Game1.currentLocation is Farm) && !(Game1.currentLocation is FarmHouse)) return;

            //if (_PetEngine._petPetted) return;

            var v = new Vector2(_PetEngine._pet.StandingPixel.X - Game1.viewport.X - Game1.tileSize * 0.3f,
                _PetEngine._pet.StandingPixel.Y - Game1.viewport.Y - Game1.tileSize * (_PetEngine._pet.petType.Value == "Cat" ? 1.5f : 1.9f));
            DrawBubble(Game1.spriteBatch, Game1.mouseCursors, new Rectangle(117, 7, 9, 8), v);
        }
    }
}