/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/loe2run/ChildToNPC
**
*************************************************/

namespace ChildToNPC
{
    //This takes just the information I need from the Friendship class
    //(There's a Netcode error if I try to directly save Date information)
    class NPCFriendshipData
    {
        public int Points { get; set; }
        public int GiftsThisWeek { get; set; }
        public string LastGiftDate { get; set; }

        public NPCFriendshipData(int points, int giftsThisWeek, string lastGiftDate)
        {
            Points = points;
            GiftsThisWeek = giftsThisWeek;
            LastGiftDate = lastGiftDate;
            //string lastGiftDate = lastGiftWorldDate.DayOfMonth + " " + lastGiftWorldDate.Season + " " + lastGiftWorldDate.Year;
            //Example: "1 spring 1"
        }

        public int GetYear()
        {
            //Format "1 spring 1"
            int index = LastGiftDate.LastIndexOf(" ");
            string yearString = LastGiftDate.Substring(index + 1, LastGiftDate.Length);
            return int.Parse(yearString);
        }

        public string GetSeason()
        {
            //Format "1 spring 1"
            int index = LastGiftDate.IndexOf(" ");
            int index2 = LastGiftDate.LastIndexOf(" ");
            string seasonString = LastGiftDate.Substring(index + 1, index2);
            return seasonString;
        }

        public int GetDay()
        {
            //Format "1 spring 1"
            int index = LastGiftDate.IndexOf(" ");
            string dayString = LastGiftDate.Substring(0, index);
            return int.Parse(dayString);
        }
    }
}