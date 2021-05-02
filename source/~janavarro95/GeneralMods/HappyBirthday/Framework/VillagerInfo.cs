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

namespace Omegasis.HappyBirthday.Framework
{
    public class VillagerInfo
    {
        public bool hasGivenBirthdayWish;
        public bool hasGivenBirthdayGift;

        public VillagerInfo()
        {
            this.hasGivenBirthdayGift = false;
            this.hasGivenBirthdayWish = false;
        }

        public void reset()
        {
            this.hasGivenBirthdayGift = false;
            this.hasGivenBirthdayWish = false;
        }

    }
}
