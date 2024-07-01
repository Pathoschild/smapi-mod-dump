/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/1Avalon/Avas-Stardew-Mods
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendshipStreaks
{
    public class FriendshipStreak
    {
        public string NpcName;

        public int CurrentTalkingStreak;

        public int CurrentGiftStreak;

        public int HighestTalkingStreak;

        public int HighestGiftStreak;

        public float Multiplier = 0f;

        public int LastDayTalked = 0;

        public int LastWeekGiftGiven = 0;

        public FriendshipStreak(string npcName, int currentTalkingStreak, int currentGiftStreak, int highestTalkingStreak, int highestGiftStreak)
        {
            NpcName = npcName;
            CurrentTalkingStreak = currentTalkingStreak;
            CurrentGiftStreak = currentGiftStreak;
            HighestTalkingStreak = highestTalkingStreak;
            HighestGiftStreak = highestGiftStreak;
        }

        public void ResetStreaksIfMissed()
        {
            int day = Game1.Date.TotalDays;
            int week = Game1.Date.TotalWeeks;

            int yesterday = day - 1;
            int lastWeek = week - 1;

            if (yesterday > LastDayTalked)
            {
                CurrentTalkingStreak = 0;
            }

            if (lastWeek > LastWeekGiftGiven)
            {
                CurrentGiftStreak = 0;
            }
        }

        public void UpdateGiftStreak()
        {
            CurrentGiftStreak++;
            int week = Game1.Date.TotalWeeks;
            LastWeekGiftGiven = week;
            if (CurrentGiftStreak > HighestGiftStreak)
                HighestGiftStreak = CurrentGiftStreak;
        }
        public void UpdateTalkingStreak()
        {
            CurrentTalkingStreak++;
            LastDayTalked = Game1.Date.TotalDays;
            if (CurrentTalkingStreak > HighestTalkingStreak)
            {
                HighestTalkingStreak = CurrentTalkingStreak;
            }
        }
        public float EvaluateFriendshipBonus()
        {
            float result = 0.2f * CurrentTalkingStreak + 0.35f * CurrentGiftStreak;
            Multiplier = (float) Math.Round(result, 2);
            return result;
        }
    }
}
