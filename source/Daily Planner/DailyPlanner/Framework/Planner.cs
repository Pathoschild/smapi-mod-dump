/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/BuildABuddha/StardewDailyPlanner
**
*************************************************/

using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using DailyPlanner.Framework.Constants;
using StardewModdingAPI;

namespace DailyPlanner.Framework
{
    public class Planner
    {
        private readonly string Filename;
        private readonly string Filepath;
        private List<string> DailyPlan;

        private PlannerData Data;

        private readonly int Year;

        private readonly IMonitor Monitor;

        /// <summary>Provides translations for the mod.</summary>
        private readonly ITranslationHelper TranslationHelper;

        private readonly IModHelper ModHelper;

        public class PlannerData
        {
            public AllYearData AllYear { get; set; }
            public MonthData Spring { get; set; }
            public MonthData Summer { get; set; }
            public MonthData Fall { get; set; }
            public MonthData Winter { get; set; }
            public class AllYearData
            {
                public List<string> Daily { get; set; }
                public List<List<string>> Weekly { get; set; }

            }
            public class MonthData
            {
                public List<string> Daily { get; set; }
                public List<List<string>> Weekly { get; set; }
                public List<List<string>> OnDate { get; set; }

            }
        }

        public Planner(int year, IModHelper modHelper, IMonitor monitor)
        {
            this.Filepath = StardewModdingAPI.Constants.CurrentSavePath;
            this.Filename = $"year_{year}.json";
            this.ModHelper = modHelper;
            //this.Filepath = filepath;
            this.Year = year;
            this.Monitor = monitor;
            this.TranslationHelper = this.ModHelper.Translation;

            this.Monitor.Log($"Reading plan from {Path.Combine(this.Filepath, "DailyPlanner", this.Filename)}", LogLevel.Debug);
            this.ReadJson();
        }

        private void ReadJson()
        {
            // Check if the file {Constants.SaveFolderPath}\DailyPlanner\year_x.json doesn't exist yet
            if (!File.Exists(Path.Combine(this.Filepath, "DailyPlanner", this.Filename)))
            {   
                // Create the DailyPlanner directory in the save folder if it doesn't exist yet
                Directory.CreateDirectory(Path.Combine(this.Filepath, "DailyPlanner"));

                // Move file from {this.ModHelper.DirectoryPath}/Plans/year_x.json to new location if it exists.
                // We do this to move plan .jsons where they used to be stored in older versions of the mod.
                if (File.Exists(Path.Combine(this.ModHelper.DirectoryPath, "Plans", this.Filename)))
                {
                    this.Monitor.Log($"NOTICE: In this version of Daily Planner, the location of stored plans has moved. Your file is being moved from " +
                        $"{Path.Combine(this.ModHelper.DirectoryPath, "Plans", this.Filename)} to " +
                        $"{Path.Combine(this.Filepath, "DailyPlanner", this.Filename)}", LogLevel.Alert);
                    File.Move(
                        Path.Combine(this.ModHelper.DirectoryPath, "Plans", this.Filename),
                        Path.Combine(this.Filepath, "DailyPlanner", this.Filename));
                }
                // Else, create blank from Template.json
                else
                {
                    this.Monitor.Log($"File for current year not found, creating new one using Template.json", LogLevel.Debug);
                    File.Copy(
                        Path.Combine(this.ModHelper.DirectoryPath, "Plans", "Template.json"),
                        Path.Combine(this.Filepath, "DailyPlanner", this.Filename));
                }
            }

            this.Data = new();
            this.Data = JsonSerializer.Deserialize<PlannerData>(
                File.ReadAllText(Path.Combine(this.Filepath, "DailyPlanner", this.Filename)));
        }

        public override string ToString()
        {
            return Data.AllYear.Daily[0];
        }

        public void CreateDailyPlan()
        {
            int season = StardewModdingAPI.Utilities.SDate.Now().SeasonIndex+1;
            int day = StardewModdingAPI.Utilities.SDate.Now().Day;

            this.DailyPlan = GetTasksForDay(season, day);
        }

        public List<string> GetDailyPlan()
        {
            return this.DailyPlan;
        }

        public void CompleteTask(string task)
        {
            this.DailyPlan.Remove(task);
        }

        private List<string> GetTasksForDay(int season, int day)
        {
            if (day <= 0) { day = 1; }
            if (day >= 29) { day = 28; }
            int dayOfWeekIndex = DayToDayOfWeekIndex(day);

            List<string> returnList = new();
            returnList.Clear();

            // Add all year - daily tasks
            returnList = new(this.Data.AllYear.Daily);
            // Add all year - weekly tasks
            try
            {
                returnList.AddRange(this.Data.AllYear.Weekly[dayOfWeekIndex]);
            }
            catch (System.ArgumentOutOfRangeException) {  }

            PlannerData.MonthData PlanMonthData = season switch
            {
                1 => this.Data.Spring,
                2 => this.Data.Summer,
                3 => this.Data.Fall,
                4 => this.Data.Winter,
                _ => new(),
            };
            returnList.AddRange(PlanMonthData.Daily);
            returnList.AddRange(PlanMonthData.Weekly[dayOfWeekIndex]);
            returnList.AddRange(PlanMonthData.OnDate[day - 1]);
            return returnList;
        }

        public List<string> GetTasksBySeasonTypeAndDate(int season, TaskType type, int day = 0)
        {
            if (day <= 0) { day = 1; }
            if (day >= 29) { day = 28; }
            int dayOfWeekIndex = DayToDayOfWeekIndex(day);

            PlannerData.MonthData PlanMonthData = season switch
            {
                1 => this.Data.Spring,
                2 => this.Data.Summer,
                3 => this.Data.Fall,
                4 => this.Data.Winter,
                _ => new(),
            };

            if (season == 0 && type == TaskType.Daily) return new(this.Data.AllYear.Daily);
            else if (season == 0 && type == TaskType.Weekly) return new(this.Data.AllYear.Weekly[dayOfWeekIndex]);
            else if (type == TaskType.Daily) return new(PlanMonthData.Daily);
            else if (type == TaskType.Weekly) return new(PlanMonthData.Weekly[dayOfWeekIndex]);
            else if (type == TaskType.OnDate) return new(PlanMonthData.OnDate[day - 1]);
            else return new();
        }

        public List<string> CreateWeekList()
        {
            int seasonIndex = StardewModdingAPI.Utilities.SDate.Now().SeasonIndex + 1;
            int day = StardewModdingAPI.Utilities.SDate.Now().Day;

            if (day <= 0) { day = 1; }
            if (day >= 29) { day = 28; }

            List<string> ReturnList = new();
            ReturnList.Clear();

            for (int i = 1; i <= 7; i++)
            {                
                ReturnList.Add($"{this.DayToString(seasonIndex, day)}:");
                ReturnList.AddRange(this.GetTasksForDay(seasonIndex, day));
                ReturnList.Add(" ");  // Blank line

                day++;
                // TODO: Go to next month
                if (day == 29) break;
            }

            return ReturnList;
        }

        public List<string> CreateMonthList()
        {
            int seasonIndex = StardewModdingAPI.Utilities.SDate.Now().SeasonIndex + 1;

            List<string> ReturnList = new();
            ReturnList.Clear();

            for (int day = 1; day <= 28; day++)
            {
                ReturnList.Add($"{this.DayToString(seasonIndex, day)}:");
                ReturnList.AddRange(this.GetTasksForDay(seasonIndex, day));
                ReturnList.Add(" ");  // Blank line
            }

            return ReturnList;
        }

        public void AddTask(int season, TaskType type, int day, string taskName)
        {
            PlannerData TempData;

            int dayOfWeekIndex;
            if (type == TaskType.Weekly) dayOfWeekIndex = day;
            else {
                dayOfWeekIndex = DayToDayOfWeekIndex(day);
                if (day <= 0) { day = 1; }
                if (day >= 29) { day = 28; }
            }

            bool isToday = StardewModdingAPI.Utilities.SDate.Now().SeasonIndex + 1 == season
                && StardewModdingAPI.Utilities.SDate.Now().Day == day;
            bool isSameSeason = StardewModdingAPI.Utilities.SDate.Now().SeasonIndex +1 == season;
            bool isSameDayOfWeek = DayToDayOfWeekIndex(StardewModdingAPI.Utilities.SDate.Now().Day) == dayOfWeekIndex;

            string jsonString = File.ReadAllText(Path.Combine(this.Filepath, "DailyPlanner", this.Filename)); 

            TempData = JsonSerializer.Deserialize<PlannerData>(jsonString)!;

            if (season == 0)
            {
                PlannerData.AllYearData TempAllYearData = TempData.AllYear;
                PlannerData.AllYearData PlanAllYearData = this.Data.AllYear;

                switch (type)
                {
                    case TaskType.Daily:
                        TempAllYearData.Daily.Add(taskName);
                        PlanAllYearData.Daily.Add(taskName);
                        this.DailyPlan.Add(taskName);
                        break;
                    case TaskType.Weekly:
                        TempData.AllYear.Weekly[dayOfWeekIndex].Add(taskName);
                        PlanAllYearData.Weekly[dayOfWeekIndex].Add(taskName);
                        if (isSameDayOfWeek) this.DailyPlan.Add(taskName);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                PlannerData.MonthData TempMonthData = (season) switch
                {
                    1 => TempData.Spring,
                    2 => TempData.Summer,
                    3 => TempData.Fall,
                    4 => TempData.Winter,
                    _ => new(),
                };

                PlannerData.MonthData PlanMonthData = (season) switch
                {
                    1 => this.Data.Spring,
                    2 => this.Data.Summer,
                    3 => this.Data.Fall,
                    4 => this.Data.Winter,
                    _ => new(),
                };

                switch (type)
                {
                    case TaskType.Daily:
                        TempMonthData.Daily.Add(taskName);
                        PlanMonthData.Daily.Add(taskName);
                        if (isSameSeason) this.DailyPlan.Add(taskName);
                        break;
                    case TaskType.Weekly:
                        TempMonthData.Weekly[dayOfWeekIndex].Add(taskName);
                        PlanMonthData.Weekly[dayOfWeekIndex].Add(taskName);
                        if (isSameDayOfWeek && isSameSeason) this.DailyPlan.Add(taskName);
                        break;
                    case TaskType.OnDate:
                        TempMonthData.OnDate[day - 1].Add(taskName);
                        PlanMonthData.OnDate[day - 1].Add(taskName);
                        if (isToday) this.DailyPlan.Add(taskName);
                        break;
                    default:
                        break;
                }
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            jsonString = new(JsonSerializer.Serialize(TempData, options));
            File.WriteAllText(Path.Combine(this.Filepath, "DailyPlanner", this.Filename), jsonString);
        }

        public void RemoveTask(int season, TaskType type, int day, string taskName)
        {
            PlannerData TempData;

            if (day <= 0) { day = 1; }
            if (day >= 29) { day = 28; }

            bool isToday = StardewModdingAPI.Utilities.SDate.Now().SeasonIndex + 1 == season
                && StardewModdingAPI.Utilities.SDate.Now().Day == day;
            bool isSameSeason = StardewModdingAPI.Utilities.SDate.Now().SeasonIndex + 1 == season;
            bool isSameDayOfWeek = DayToDayOfWeekIndex(StardewModdingAPI.Utilities.SDate.Now().Day) == DayToDayOfWeekIndex(day);

            int dayOfWeekIndex = DayToDayOfWeekIndex(day);

            string jsonString = File.ReadAllText(Path.Combine(this.Filepath, "DailyPlanner", this.Filename));

            TempData = JsonSerializer.Deserialize<PlannerData>(jsonString)!;


            if (season == 0)
            {
                PlannerData.AllYearData TempAllYearData = TempData.AllYear;
                PlannerData.AllYearData PlanAllYearData = this.Data.AllYear;

                switch (type)
                {
                    case TaskType.Daily:
                        TempAllYearData.Daily.Remove(taskName);
                        PlanAllYearData.Daily.Remove(taskName);
                        this.DailyPlan.Remove(taskName);
                        break;
                    case TaskType.Weekly:
                        TempData.AllYear.Weekly[dayOfWeekIndex].Remove(taskName);
                        PlanAllYearData.Weekly[dayOfWeekIndex].Remove(taskName);
                        if (isSameDayOfWeek) this.DailyPlan.Remove(taskName);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                PlannerData.MonthData TempMonthData = (season) switch
                {
                    1 => TempData.Spring,
                    2 => TempData.Summer,
                    3 => TempData.Fall,
                    4 => TempData.Winter,
                    _ => new(),
                };

                PlannerData.MonthData PlanMonthData = (season) switch
                {
                    1 => this.Data.Spring,
                    2 => this.Data.Summer,
                    3 => this.Data.Fall,
                    4 => this.Data.Winter,
                    _ => new(),
                };

                switch (type)
                {
                    case TaskType.Daily:
                        TempMonthData.Daily.Remove(taskName);
                        PlanMonthData.Daily.Remove(taskName);
                        if (isSameSeason) this.DailyPlan.Remove(taskName);
                        break;
                    case TaskType.Weekly:
                        TempMonthData.Weekly[dayOfWeekIndex].Remove(taskName);
                        PlanMonthData.Weekly[dayOfWeekIndex].Remove(taskName);
                        if (isSameDayOfWeek && isSameSeason) this.DailyPlan.Remove(taskName);
                        break;
                    case TaskType.OnDate:
                        TempMonthData.OnDate[day - 1].Remove(taskName);
                        PlanMonthData.OnDate[day - 1].Remove(taskName);
                        if (isToday) this.DailyPlan.Remove(taskName);
                        break;
                    default:
                        break;
                }
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            jsonString = new(JsonSerializer.Serialize(TempData, options));
            File.WriteAllText(Path.Combine(this.Filepath, "DailyPlanner", this.Filename), jsonString);
        }

        public static int DayToDayOfWeekIndex(int day)
        {
            int DayOfWeekIndex = day % 7;
            if (DayOfWeekIndex == 0) DayOfWeekIndex = 7;
            return DayOfWeekIndex -1;
        }

        public string SeasonIndexToName(int seasonIndex)
        {
            return (seasonIndex) switch
            {
                0 => this.TranslationHelper.Get("seasons.all_year"),
                1 => this.TranslationHelper.Get("seasons.spring"),
                2 => this.TranslationHelper.Get("seasons.summer"),
                3 => this.TranslationHelper.Get("seasons.fall"),
                4 => this.TranslationHelper.Get("seasons.winter"),
                _ => "",
            };
        }

        public string DayToDayOfWeekName(int day)
        {
            return (DayToDayOfWeekIndex(day)) switch
            {
                0 => this.TranslationHelper.Get("week.monday"),
                1 => this.TranslationHelper.Get("week.tuesday"),
                2 => this.TranslationHelper.Get("week.wednesday"),
                3 => this.TranslationHelper.Get("week.thursday"),
                4 => this.TranslationHelper.Get("week.friday"),
                5 => this.TranslationHelper.Get("week.saturday"),
                6 => this.TranslationHelper.Get("week.sunday"),
                _ => "",
            };
        }

        public string TaskTypeToString(TaskType type)
        {
            return (type) switch
            {
                TaskType.Daily => this.TranslationHelper.Get("task.daily"),
                TaskType.Weekly => this.TranslationHelper.Get("task.weekly"),
                TaskType.OnDate => this.TranslationHelper.Get("task.on_date"),
                _ => "",
            };
        }

        public string DayToString(int seasonIndex, int day)
        {
            if (day <= 0) { day = 1; }
            if (day >= 29) { day = 28; }

            return $"{this.TranslationHelper.Get("year.year")} {this.Year}, {this.SeasonIndexToName(seasonIndex)} {day}, {this.DayToDayOfWeekName(day)}";
        }
    }
}
