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
using Omegasis.StardustCore.Events.Preconditions;

namespace Omegasis.HappyBirthday.Framework.Events.EventPreconditions
{
    public class HasChosenFavoriteGiftPrecondition : EventPrecondition
    {

        public const string EventPreconditionId = "Omegasis.HappyBirthday.Framework.EventPreconditions.HasChosenFavoriteGiftPrecondition";

        public bool hasChosenFavoriteGift;

        public HasChosenFavoriteGiftPrecondition()
        {
            this.hasChosenFavoriteGift = true;
        }

        public HasChosenFavoriteGiftPrecondition(bool ShouldHaveChosenBirthday)
        {
            this.hasChosenFavoriteGift = ShouldHaveChosenBirthday;
        }


        public override string ToString()
        {
            return EventPreconditionId + " " + this.hasChosenFavoriteGift.ToString();
        }

        public override bool meetsCondition()
        {
            return HappyBirthdayModCore.Instance.birthdayManager.hasChoosenFavoriteGift() == this.hasChosenFavoriteGift;
        }
    }
}
