/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;
using BirbShared.Mod;
using BirbShared.APIs;
using StardewModdingAPI;
using System.Collections.Generic;
using BirbShared;

namespace RealtimeFramework
{
    public class ModEntry : Mod
    {
        [SmapiInstance]
        internal static ModEntry Instance;
        [SmapiAsset]
        internal static Assets Assets;
        [SmapiApi(UniqueID = "Pathoschild.ContentPatcher", IsRequired = false)]
        internal static IContentPatcherApi ContentPatcher;

        internal ITranslationHelper I18n => this.Helper.Translation;

        internal static IRealtimeAPI API = new RealtimeAPI();

        public override void Entry(IModHelper helper)
        {
            ModClass mod = new ModClass();
            mod.Parse(this);

            helper.Events.GameLoop.GameLaunched += this.GameLoop_GameLaunched;
        }

        public override object GetApi()
        {
            return API;
        }

        private void GameLoop_GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            if (ContentPatcher == null)
            {
                Log.Info("Content Patcher is not installed, will skip adding tokens");
                return;
            }
            ContentPatcher.RegisterToken(Instance.ModManifest, "Hour", RegisterHourToken);
            ContentPatcher.RegisterToken(Instance.ModManifest, "DayOfMonth", RegisterDayOfMonth);
            ContentPatcher.RegisterToken(Instance.ModManifest, "DayOfWeek", RegisterDayOfWeek);
            ContentPatcher.RegisterToken(Instance.ModManifest, "DayOfYear", RegisterDayOfYear);
            ContentPatcher.RegisterToken(Instance.ModManifest, "Month", RegisterMonth);
            ContentPatcher.RegisterToken(Instance.ModManifest, "Year", RegisterYear);
            ContentPatcher.RegisterToken(Instance.ModManifest, "WeekdayLocal", RegisterWeekdayLocal);
            ContentPatcher.RegisterToken(Instance.ModManifest, "MonthLocal", RegisterMonthLocal);

            ContentPatcher.RegisterToken(Instance.ModManifest, "AllHolidays", RegisterAllHolidays);
            ContentPatcher.RegisterToken(Instance.ModManifest, "ComingHolidays", RegisterComingHolidays);
            ContentPatcher.RegisterToken(Instance.ModManifest, "CurrentHolidays", RegisterCurrentHolidays);
            ContentPatcher.RegisterToken(Instance.ModManifest, "PassingHolidays", RegisterPassingHolidays);
            ContentPatcher.RegisterToken(Instance.ModManifest, "AllHolidaysLocal", RegisterAllHolidaysLocal);
            ContentPatcher.RegisterToken(Instance.ModManifest, "ComingHolidaysLocal", RegisterComingHolidaysLocal);
            ContentPatcher.RegisterToken(Instance.ModManifest, "CurrentHolidaysLocal", RegisterCurrentHolidaysLocal);
            ContentPatcher.RegisterToken(Instance.ModManifest, "PassingHolidaysLocal", RegisterPassingHolidaysLocal);
        }

        private IEnumerable<string> RegisterHourToken()
        {
            yield return "" + DateTime.Now.Hour;
        }

        private IEnumerable<string> RegisterDayOfMonth()
        {
            yield return "" + DateTime.Today.Day;
        }

        private IEnumerable<string> RegisterDayOfWeek()
        {
            yield return "" + ((int)DateTime.Today.DayOfWeek + 1);
        }

        private IEnumerable<string> RegisterDayOfYear()
        {
            yield return "" + DateTime.Today.DayOfYear;
        }

        private IEnumerable<string> RegisterMonth()
        {
            yield return "" + DateTime.Today.Month;
        }

        private IEnumerable<string> RegisterYear()
        {
            yield return "" + DateTime.Today.Year;
        }

        private IEnumerable<string> RegisterWeekdayLocal()
        {
            yield return Instance.I18n.Get("time.weekday." + DateTime.Today.DayOfWeek);
        }

        private IEnumerable<string> RegisterMonthLocal()
        {
            yield return Instance.I18n.Get("time.month." + DateTime.Today.Month);
        }

        // Because Content Patcher treats null and empty arrays as an unready token, we need to return
        // a single empty string iff we would otherwise return an empty set of values.
        private IEnumerable<string> RegisterAllHolidays()
        {
            bool empty = true;
            foreach (string holiday in API.GetAllHolidays())
            {
                empty = false;
                yield return holiday;
            }
            if (empty)
            {
                yield return "";
            }
        }

        private IEnumerable<string> RegisterComingHolidays()
        {
            bool empty = true;
            foreach (string holiday in API.GetComingHolidays())
            {
                empty = false;
                yield return holiday;
            }
            if (empty)
            {
                yield return "";
            }
        }

        private IEnumerable<string> RegisterCurrentHolidays()
        {
            bool empty = true;
            foreach (string holiday in API.GetCurrentHolidays())
            {
                empty = false;
                yield return holiday;
            }
            if (empty)
            {
                yield return "";
            }
        }

        private IEnumerable<string> RegisterPassingHolidays()
        {
            bool empty = true;
            foreach (string holiday in API.GetPassingHolidays())
            {
                empty = false;
                yield return holiday;
            }
            if (empty)
            {
                yield return "";
            }
        }

        private IEnumerable<string> RegisterAllHolidaysLocal()
        {
            bool empty = true;
            foreach (string holiday in API.GetAllHolidays())
            {
                empty = false;
                yield return API.GetLocalName(holiday);
            }
            if (empty)
            {
                yield return "";
            }
        }


        private IEnumerable<string> RegisterComingHolidaysLocal()
        {
            bool empty = true;
            foreach (string holiday in API.GetComingHolidays())
            {
                empty = false;
                yield return API.GetLocalName(holiday);
            }
            if (empty)
            {
                yield return "";
            }
        }

        private IEnumerable<string> RegisterCurrentHolidaysLocal()
        {
            bool empty = true;
            foreach (string holiday in API.GetCurrentHolidays())
            {
                empty = false;
                yield return API.GetLocalName(holiday);
            }
            if (empty)
            {
                yield return "";
            }
        }

        private IEnumerable<string> RegisterPassingHolidaysLocal()
        {
            bool empty = true;
            foreach (string holiday in API.GetPassingHolidays())
            {
                empty = false;
                yield return API.GetLocalName(holiday);
            }
            if (empty)
            {
                yield return "";
            }
        }
    }
}
