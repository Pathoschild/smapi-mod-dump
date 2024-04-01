/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/DailyTaskReportPlus
**
*************************************************/


namespace DailyTasksReport
{
    public class ReportReturnItem
    {
        public string Label { get; set; }
        public string Details { get; set; }
        public Tuple<string, int, int> WarpTo { get; set; }
        public string SortKey { get; set; }
        public override string ToString()
        {
            return Label + (string.IsNullOrEmpty(Details)?"":  ": " + Details);
        }
    }
}
