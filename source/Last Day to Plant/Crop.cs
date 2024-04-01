/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/dmcrider/LastDayToPlant
**
*************************************************/

using StardewModdingAPI;
using Newtonsoft.Json.Linq;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LastDayToPlant
{
    public class Crop
    {
        public string Name { get; set; }
        public int DaysToGrow { get; set; }
        public List<Season> Seasons { get; set; }
        public int DaysToGrowIrrigated { get; set; } = 0;
        public int AvailableYear { get; set; } = 1;
        public bool GingerIsland { get; set; } = false;

        public string Message { get; set; }
        public string MessageSpeedGro { get; set; }
        public string MessageDelxueSpeedGro { get; set; }
        public string MessageHyperSpeedGro { get; set; }


        public Crop(string name, int daysToGrow)
        {
            Name = name;
            DaysToGrow = daysToGrow;
        }

        public Crop() { }

        public bool IsLastGrowSeason(Season season)
        {
            var seasons = Seasons.OrderByDescending(x => x);
            return seasons.First() == season;
        }

        public void Localize(IModHelper helper, string baseName)
        {
            // This one can't be handled by I18n because it's dynamic
            Name = helper.Translation.Get($"crop.{baseName.Replace(" ", "")}");
            // The rest of the messages can though
            Message = I18n.Notification_Crop_NoFertilizer(Name);
            MessageSpeedGro = I18n.Notification_Crop_SpeedGro(Name);
            MessageDelxueSpeedGro = I18n.Notification_Crop_DeluxeSpeedGro(Name);
            MessageHyperSpeedGro = I18n.Notification_Crop_HyperSpeedGro(Name);
        }

        public static Crop FromModFile(string cropFilePath)
        {
            var jsonObject = JObject.Parse(File.ReadAllText(cropFilePath));
            var crop = new Crop()
            {
                Name = jsonObject["Name"].ToString()
            };

            // Seasons
            var seasons = jsonObject["Seasons"].ToObject<string[]>();
            crop.Seasons = new List<Season>();
            foreach (var season in seasons)
            {
                crop.Seasons.Add(GetSeasonEnum(season));
            }

            // Days to Grow
            var desc = jsonObject["SeedDescription"].ToString();
            var startWord = "Takes";
            var endWord = "mature";
            if (!desc.Contains(startWord))
            {
                startWord = "in";
                endWord = "days";
            }
            var start = desc.IndexOf(startWord);
            var end = desc.IndexOf(endWord);
            if(start == -1 || end == -1)
            {
                crop.DaysToGrow = 0;
                return crop;
            }
            var splits = desc.Substring(start, end - start).Split(' ');
            foreach(var split in splits)
            {
                var isNumber = int.TryParse(split, out int days);
                if (isNumber)
                {
                    crop.DaysToGrow = days;
                }
            }

            return crop;
        }

        private static Season GetSeasonEnum(string season)
        {
            return (Season)Enum.Parse(typeof(Season), season);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
