/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/DailyTaskReportPlus
**
*************************************************/

using DailyTasksReport.TaskEngines;
using DailyTasksReport.UI;

namespace DailyTasksReport.Tasks
{
    public class MiscTask : DailyReportTask
    {

        internal MiscTask(TaskReportConfig config)
        {
            _config = config;
            _Engine = new MiscTaskEngine(config);
            SettingsMenu.ReportConfigChanged += SettingsMenu_ReportConfigChanged;
        }

        public override string GeneralInfo(out int usedLines)
        {
            usedLines = 0;

            if (!Enabled) return "";


            _Engine.UpdateList();
            usedLines = _Engine.GeneralInfo().Count();

            return string.Join("^", ((MiscTaskEngine)_Engine).GeneralInfo(false)) + (_Engine.GeneralInfo().Count > 0 ? "^" : "");
        }

        public override string DetailedInfo(out int usedLines, out bool skipNextPage)
        {
            usedLines = 0;
            skipNextPage = false;
            return "";
        }
        private void SettingsMenu_ReportConfigChanged(object? sender, EventArgs e)
        {
            _Engine.SetEnabled();
        }
    }
}