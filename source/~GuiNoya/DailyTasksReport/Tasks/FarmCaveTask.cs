using DailyTasksReport.UI;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable CC0021 // Use nameof

namespace DailyTasksReport.Tasks
{
    public class FarmCaveTask : Task
    {
        private static readonly int[] Fruits = { 296, 396, 406, 410, 613, 634, 635, 636, 637, 638 };

        private readonly ModConfig _config;
        private string _farmCaveItemName;

        private readonly Dictionary<string, int> _objectsList = new Dictionary<string, int>();

        internal FarmCaveTask(ModConfig config)
        {
            _config = config;

            SettingsMenu.ReportConfigChanged += SettingsMenu_ReportConfigChanged;
        }

        private void SettingsMenu_ReportConfigChanged(object sender, EventArgs e)
        {
            Enabled = _config.FarmCave && (Game1.player.caveChoice.Value != 0 || Game1.player.totalMoneyEarned >= 25000);
        }

        protected override void FirstScan()
        {
        }

        private void Update()
        {
            var farmCave = Game1.locations.OfType<FarmCave>().FirstOrDefault();

            foreach (var obj in farmCave.objects.Values)
            {
                if (obj.ParentSheetIndex == 128 && obj.readyForHarvest.Value)
                {
                    var heldObject = obj.heldObject.Value;
                    if (heldObject != null)
                        if (_objectsList.ContainsKey(heldObject.Name))
                            _objectsList[heldObject.Name] += 1;
                        else
                            _objectsList[heldObject.Name] = 1;
                }
                else if (Array.BinarySearch(Fruits, obj.ParentSheetIndex) >= 0)
                    if (_objectsList.ContainsKey(obj.name))
                        _objectsList[obj.name] += 1;
                    else
                        _objectsList[obj.name] = 1;
            }
        }

        public override string GeneralInfo(out int usedLines)
        {
            usedLines = 0;
            if (!Enabled) return "";

            _objectsList.Clear();
            Update();

            if (_objectsList.Count == 0) return "";

            usedLines++;
            return $"{_farmCaveItemName} in the farm cave: {_objectsList.Sum(o => o.Value)}^";
        }

        public override string DetailedInfo(out int usedLines, out bool skipNextPage)
        {
            usedLines = 0;
            skipNextPage = false;
            if (!Enabled || _objectsList.Count == 0) return "";

            var stringBuilder = new StringBuilder();

            stringBuilder.Append($"{_farmCaveItemName} in the farm cave:^");
            usedLines++;

            foreach (var pair in _objectsList)
            {
                var name = Pluralize(pair.Key, pair.Value);
                stringBuilder.Append($"{pair.Value} {name}^");
                usedLines++;
            }

            return stringBuilder.ToString();
        }

        public override void Clear()
        {
            Enabled = _config.FarmCave && (Game1.player.caveChoice.Value != 0 || Game1.player.totalMoneyEarned >= 25000);
            _farmCaveItemName = Game1.player.caveChoice.Value == 1 ? "Fruits" : "Mushrooms";
            _objectsList.Clear();
        }

        private static string Pluralize(string name, int number)
        {
            if (number < 2)
                return name;
            if (name.EndsWith("y", StringComparison.CurrentCulture))
                return name.Substring(0, name.Length - 1) + "ies";
            if (name == "Peach")
                return name + "es";
            return name + "s";
        }
    }
}

#pragma warning restore CC0021 // Use nameof