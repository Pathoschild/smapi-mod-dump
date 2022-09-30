/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omegasis.StardustCore.Events.Preconditions.NPCSpecific;
using Omegasis.StardustCore.Events.Preconditions.PlayerSpecific;
using Omegasis.StardustCore.Events.Preconditions.TimeSpecific;

namespace Omegasis.StardustCore.Events.Preconditions
{
    public static class PreconditionParsingMethods
    {

        public static DatingNPCEventPrecondition ParseDatingNpcEventPrecondition(string[] data)
        {
            string npcName = data[1];
            return new DatingNPCEventPrecondition(npcName);
        }

        public static CanReadJunimoEventPrecondition ParseCanReadJunimoEventPrecondition(string[] data)
        {
            return new CanReadJunimoEventPrecondition();
        }

        public static CommunityCenterCompletedEventPreconditon ParseCommunityCenterCompletedPrecondition(string[] data)
        {
            bool needsToBeCompleted = Convert.ToBoolean(data[1]);
            return new CommunityCenterCompletedEventPreconditon(needsToBeCompleted);
        }

        public static IsJojaMemberEventPrecondition ParseIsJojaMemeberPrecondition(string[] data)
        {
            bool isJojaMember = Convert.ToBoolean(data[1]);
            return new IsJojaMemberEventPrecondition(isJojaMember);
        }

        public static DayOfWeekPrecondition ParseDayOfWeekPrecondition(string[] data)
        {
            List<string> daysOfWeek = data.ToList();

            return new DayOfWeekPrecondition(
                daysOfWeek.Contains("Sunday"),
                daysOfWeek.Contains("Monday"),
                daysOfWeek.Contains("Tuesday"),
                daysOfWeek.Contains("Wednesday"),
                daysOfWeek.Contains("Thursday"),
                daysOfWeek.Contains("Friday"),
                daysOfWeek.Contains("Saturday")
                );

        }

        public static TimeOfDayPrecondition ParseTimeOfDayPrecondition(string[] data)
        {
            return new TimeOfDayPrecondition(Convert.ToInt32(data[1]), Convert.ToInt32(data[2]));
        }

        public static GameLocationPrecondition ParseGameLocationPrecondition(string[] data)
        {
            return new GameLocationPrecondition(data[1]);
        }

    }
}
