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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LastDayToPlant
{
    public class Crop
    {
        public string Name { get; set; }
        public int DaysToMature { get; set; }
        public string Message { get; set; }
        public string MessageSpeedGro { get; set; }
        public string MessageDelxueSpeedGro { get; set; }
        public string MessageHyperSpeedGro { get; set; }


        public Crop(string name, int daysToMature)
        {
            Name = name;
            DaysToMature = daysToMature;
        }

        public static Crop GetLocalizedCrop(string season, string name, int daysToMature, IModHelper helper)
        {
            var tagName = name.Replace(" ", "").Trim().ToLower();

            var crop = new Crop(helper.Translation.Get($"crop.{season}.{tagName}", new { cropName = name }), daysToMature)
            {
                Message = helper.Translation.Get("notification.crop.no-fertilizer", new { cropName = name }),
                MessageSpeedGro = helper.Translation.Get("notification.crop.speed-gro", new { cropName = name }),
                MessageDelxueSpeedGro = helper.Translation.Get("notification.crop.deluxe-speed-gro", new { cropName = name }),
                MessageHyperSpeedGro = helper.Translation.Get("notification.crop.hyper-speed-gro", new { cropName = name })
            };

            return crop;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
