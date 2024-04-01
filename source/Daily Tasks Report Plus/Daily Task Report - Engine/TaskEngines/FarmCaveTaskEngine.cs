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
using StardewValley.Locations;

namespace DailyTasksReport.TaskEngines
{
    class FarmCaveTaskEngine : TaskEngine
    {
        public static readonly string[] Fruits = { "296", "396", "406", "410", "613", "634", "635", "636", "637", "638" };
        private string _farmCaveItemName = "";

        private readonly Dictionary<string, int> _objectsList = new Dictionary<string, int>();

        internal FarmCaveTaskEngine(TaskReportConfig config)
        {
            _config = config;
            TaskClass = "FarmCave";
            SetEnabled();
        }
        public override void Clear()
        {
            SetEnabled();
            _farmCaveItemName = Game1.player.caveChoice.Value == 1 ? I18n.Tasks_Cave_ItemName_Fruits() : I18n.Tasks_Cave_ItemName_Mushrooms();
            _objectsList.Clear();
        }

        public override List<ReportReturnItem> DetailedInfo()
        {

            List<ReportReturnItem> prItem = new List<ReportReturnItem> { };

            if (!Enabled || _objectsList.Count == 0) return prItem;

            foreach (KeyValuePair<string, int> pair in _objectsList)
            {
                string name = Pluralize(pair.Key, pair.Value);
                prItem.Add(new ReportReturnItem { Label = $"{pair.Value} {name}" });
            }

            return prItem;
        }

        public override List<ReportReturnItem> GeneralInfo()
        {
            List<ReportReturnItem> prItem = new List<ReportReturnItem> { };

            if (!Enabled) return prItem;

            if (_objectsList.Count == 0) return prItem;

            prItem.Add(new ReportReturnItem { Label =$"{_farmCaveItemName}{I18n.Tasks_Cave_InCave()}", Details = _objectsList.Sum(o => o.Value).ToString() });

            return prItem;
        }

        public override void SetEnabled()
        {
            Enabled = _config.FarmCave && (Game1.player != null && (Game1.player.caveChoice.Value != 0 || Game1.player.totalMoneyEarned >= 25000));
        }
        internal override void FirstScan()
        {
            FarmCave? farmCave = Game1.locations.OfType<FarmCave>().FirstOrDefault();

            if (farmCave != null)
            {
                foreach (SDObject? obj in farmCave.objects.Values)
                {
                    if (obj.ItemId == "128" && obj.readyForHarvest.Value)
                    {
                        SDObject heldObject = obj.heldObject.Value;

                        if (heldObject != null)
                        {
                            string nameKey = string.IsNullOrEmpty(heldObject.displayName) ? heldObject.name : heldObject.displayName;
                            if (_objectsList.ContainsKey(nameKey))
                                _objectsList[nameKey] += 1;
                            else
                                _objectsList[nameKey] = 1;
                        }
                    }
                    else if (Array.BinarySearch(Fruits, obj.ItemId) >= 0)
                    {
                        string nameKey = string.IsNullOrEmpty(obj.displayName) ? obj.name : obj.displayName;

                        if (_objectsList.ContainsKey(nameKey))
                            _objectsList[nameKey] += 1;
                        else
                            _objectsList[nameKey] = 1;
                    }
                }
            }
        }
    }
}
