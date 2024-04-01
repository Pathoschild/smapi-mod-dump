/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/DailyTaskReportPlus
**
*************************************************/

using DailyTasksReport.Tasks;
using System.Text;

namespace DailyTasksReport
{
    internal static class Report
    {
        public static string GetReportText(List<DailyReportTask> tasks, bool showDetailedInfo)
        {
            var stringBuilder = new StringBuilder();
            var count = 0;

            foreach (var task in tasks)
            {
                stringBuilder.Append(task.GeneralInfo(out var linesUsed));
                count += linesUsed;
            }

            if (count == 0)
            {
                stringBuilder.Append("All done!");
                tasks.ForEach(t => t.FinishedReport());
                return stringBuilder.ToString();
            }

            NextPage(ref stringBuilder, ref count);

            if (!showDetailedInfo)
                return stringBuilder.ToString();

            foreach (var task in tasks)
            {
                stringBuilder.Append(task.DetailedInfo(out var linesUsed, out var skipNextPage));
                if (!skipNextPage)
                    NextPage(ref stringBuilder, ref linesUsed);
            }

            tasks.ForEach(t => t.FinishedReport());
            return stringBuilder.ToString();
        }

        private static void NextPage(ref StringBuilder stringBuilder, ref int count)
        {
            var i = 11 - count % 11;
            stringBuilder.Append('^', i);
            count += i;
        }
    }
}