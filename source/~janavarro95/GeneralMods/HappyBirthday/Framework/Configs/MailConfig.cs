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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omegasis.HappyBirthday.Framework.Configs
{
    public class MailConfig
    {
        public int dadBirthdayYear1MoneyGivenAmount;
        public int dadBirthdayMoneyGivenAmount;

        public int momBirthdayItemGive;
        public int momBirthdayItemGiveStackSize;


        public MailConfig()
        {
            this.dadBirthdayYear1MoneyGivenAmount = 2000;
            this.dadBirthdayMoneyGivenAmount = 5000;
            this.momBirthdayItemGive = 221; //Pink cake.
            this.momBirthdayItemGiveStackSize = 1;
        }

        public virtual void reAdjustForYear1()
        {
            this.dadBirthdayMoneyGivenAmount = 2000;
        }


        /// <summary>
        /// Initializes the config for the blacksmith shop prices.
        /// </summary>
        /// <returns></returns>
        public static MailConfig InitializeConfig()
        {
            if (HappyBirthdayModCore.Configs.doesConfigExist("MailConfig.json"))
            {
                return HappyBirthdayModCore.Configs.ReadConfig<MailConfig>("MailConfig.json");
            }
            else
            {
                MailConfig Config = new MailConfig();
                HappyBirthdayModCore.Configs.WriteConfig("MailConfig.json", Config);
                return Config;
            }
        }
    }
}
