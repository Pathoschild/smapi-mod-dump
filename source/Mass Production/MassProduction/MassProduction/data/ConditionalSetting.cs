/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JacquePott/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassProduction
{
    /// <summary>
    /// A conditional modification of the base mass production machine settings.
    /// </summary>
    public class ConditionalSetting
    {
        public int? InputQualityRequired { get; set; } = null;
        public string[] InputIdentifiersRequired { get; set; } = new string[0];

        public int InputStaticChange { get; set; } = 0;
        public int OutputStaticChange { get; set; } = 0;
        public double InputMultiplier { get; set; } = 0;
        public double OutputMultiplier { get; set; } = 0.0;
        public double OutputMultiplierMin { get; set; } = 0.0;
        public double OutputMultiplierMax { get; set; } = 0.0;
        public double TimeMultiplier { get; set; } = 0.0;

        private ConditionalSettingCheckCache RecentChecks = new ConditionalSettingCheckCache();

        /// <summary>
        /// Returns true if this conditional setting should modify the usual settings.
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public bool IsActive(params InputInfo[] inputs)
        {
            bool isActive = false;

            foreach (InputInfo input in inputs)
            {
                //Uses a cache since this can be called many times per frame if a player is holding an object and hovering cursor over machine
                bool? cachedValue = RecentChecks.Get(input.ID);

                if (cachedValue.HasValue && cachedValue.Value)
                {
                    isActive = true;
                    break;
                }
                else
                {
                    if (InputQualityRequired.HasValue && InputQualityRequired.Value != input.Quality)
                    {
                        continue;
                    }

                    bool matchesIdentifier = InputIdentifiersRequired.Length == 0;

                    foreach (string identifier in InputIdentifiersRequired)
                    {
                        if (input.Name.Equals(identifier) || input.ID.ToString().Equals(identifier) || input.ContextTags.Contains(identifier))
                        {
                            matchesIdentifier = true;
                            break;
                        }
                    }

                    if (matchesIdentifier)
                    {
                        isActive = true;
                        break;
                    }
                }
            }

            return isActive;
        }
    }
}
