/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using ArsVenefici.Framework.Util;
using Newtonsoft.Json.Linq;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsVenefici.Framework.Skill
{
    public class ManaEfficiencyProfession : GenericProfession
    {
        private int manaCostReductionAmount;


        public ManaEfficiencyProfession(Skill skill, string theId, int manaCostReductionAmount) : base(skill, theId)
        {
            this.manaCostReductionAmount = manaCostReductionAmount;
        }

        public override void DoImmediateProfessionPerk()
        {
            Game1.player.GetSpellBook().SetManaCostReductionAmount(manaCostReductionAmount);
        }

        public override T GetValue<T>()
        {
            return (T)Convert.ChangeType(manaCostReductionAmount, typeof(T));
        }
    }
}
