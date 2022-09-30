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

namespace Omegasis.HappyBirthday.Framework.Constants
{
    public class EventIds
    {
        public static string PREFIX = "HappyBirthday_Events_";

        //Dating bachelorette birthdays.
        public static string BirthdayDatingAbigailSeedShop = CreateEventId("BirthdayParty_Dating_Abigail_Seedshop");
        public static string BirthdayDatingAbigailMines = CreateEventId("BirthdayParty_Dating_Abigail_Mines");
        public static string BirthdayDatingEmily = CreateEventId("BirthdayParty_Dating_Emily");
        public static string BirthdayDatingHaley = CreateEventId("BirthdayParty_Dating_Haley");
        public static string BirthdayDatingLeah = CreateEventId("BirthdayParty_Dating_Leah");
        public static string BirthdayDatingMaru = CreateEventId("BirthdayParty_Dating_Maru");
        public static string BirthdayDatingPennyTrailer = CreateEventId("BirthdayParty_Dating_Penny_Trailer");
        public static string BirthdayDatingPennyHouse = CreateEventId("BirthdayParty_Dating_Penny_House");

        //Dating bacehelor birthdays.
        public static string BirthdayDatingAlex = CreateEventId("BirthdayParty_Dating_Alex");
        public static string BirthdayDatingHarvey = CreateEventId("BirthdayParty_Dating_Harvey");
        public static string BirthdayDatingElliott = CreateEventId("BirthdayParty_Dating_Elliott");
        public static string BirthdayDatingSebastian = CreateEventId("BirthdayParty_Dating_Sebastian");
        public static string BirthdayDatingSam = CreateEventId("BirthdayParty_Dating_Sam");
        public static string BirthdayDatingShane = CreateEventId("BirthdayParty_Dating_Shane");


        //Birthday setup events.
        public static string AskPlayerForBirthday = CreateEventId("AskPlayerForBirthday");

        //Npc birthday events.
        public static string JojaMartBirthday = CreateEventId("JojaMartBirthday");
        public static string JunimoCommunityCenterBirthday = CreateEventId("JunimoCommunityCenterBirthday");

        public static string SaloonBirthdayParty_Year1 = CreateEventId("SaloonBirthdayParty_Year1");
        public static string SaloonBirthdayParty_Year2 = CreateEventId("SaloonBirthdayParty_Year2");


        //Married birthday events.
        public static string Married_BirthdayParty_Farmhouse_1 = CreateEventId("Married_BirthdayParty_Farmhouse_1");
        public static string Married_BirthdayParty_Farmhouse_2 = CreateEventId("Married_BirthdayParty_Farmhouse_2");
        public static string AskPlayerForFavoriteGift_Farmhouse_1= CreateEventId("AskPlayerForFavoriteGift_Farmhouse_1");
        public static string AskPlayerForFavoriteGift_Farmhouse_2 = CreateEventId("AskPlayerForFavoriteGift_Farmhouse_2");


        public static string CreateEventId(string EventId)
        {
            return PREFIX + EventId;
        }

    }
}
