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
using DailyTasksReport.UI;

namespace DailyTasksReport.TaskEngines
{
    class TerrainTaskEngine : TaskEngine
    {
        private Dictionary<string, List<Fence>> _Fences = new();
        private readonly TerrainTaskId _id;

        internal TerrainTaskEngine(TaskReportConfig config, TerrainTaskId id)
        {
            _config = config;
            _id = id;
            TaskClass = "Terrain";
            TaskSubClass = id.ToString();
            TaskId = (int)id;
            SetEnabled();
        }
        public override void Clear()
        {
            _Fences.Clear();
        }

        public override List<ReportReturnItem> DetailedInfo()
        {
            List<ReportReturnItem> prItem = new List<ReportReturnItem> { };

            if (!Enabled) return prItem;

            var locations = Game1.locations.Where(p => p.IsBuildableLocation());

            foreach (GameLocation location in locations)
            {
                foreach (Fence f in location.Objects.Values.OfType<Fence>())
                {
                    if (f.health.Value < 1)
                    {
                        prItem.Add(new ReportReturnItem
                        {
                            Label = $"{I18n.Tasks_Terrain_Fence()}{FormatLocation(location.Name,null,f.TileLocation)}",
                            WarpTo = Tuple.Create(location.Name, (int)f.TileLocation.X, (int)f.TileLocation.Y)
                        });
                    }
                }
            }
            return prItem;
        }

        public override List<ReportReturnItem> GeneralInfo()
        {
            List<ReportReturnItem> prItem = new List<ReportReturnItem> { };

            if (!Enabled) return prItem;

            foreach (var fenceData in _Fences)
            {
                if (fenceData.Value.Count > 0)
                {
                    prItem.Add(new ReportReturnItem { Label = $"{GetLocationDisplayName(fenceData.Key)}{I18n.Tasks_Terrain_Damaged()}{_Fences.Count}" });
                }
            }
            return prItem;
        }

        internal override void FirstScan()
        {
            List<GameLocation> locations = Game1.locations.Where(p => p.IsBuildableLocation()).ToList();

            foreach (GameLocation location in locations)
            {
                foreach (Fence f in location.Objects.Values.OfType<Fence>())
                {
                    if (!_Fences.ContainsKey(location.Name))
                        _Fences.Add(location.Name, new List<Fence> { });
#if DEBUG
                    //f.health.Value = -0.5f;
#endif
                    if (f.health.Value < 1)
                    {
                        _Fences[location.Name].Add(f);
                    }
                }
            }
        }

        public override void SetEnabled()
        {
            Enabled = _config.BrokenFences;
        }

    }
}
