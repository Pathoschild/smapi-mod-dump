/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/hawkfalcon/Stardew-Mods
**
*************************************************/

using System.Collections.Generic;
using BetterJunimos.Abilities;
using BetterJunimos.Utils;
using StardewValley.Buildings;

namespace BetterJunimos {
    public class BetterJunimosApi {

        public int GetJunimoHutMaxRadius() {
            return Util.Config.JunimoHuts.MaxRadius;
        }

        public int GetJunimoHutMaxJunimos() {
            return Util.Config.JunimoHuts.MaxJunimos;
        }

        public Dictionary<string, bool> GetJunimoAbilities() {
            return Util.Config.JunimoAbilites;
        }

        public void RegisterJunimoAbility(IJunimoAbility junimoAbility) {
            Util.Abilities.RegisterJunimoAbility(junimoAbility);
        }

        public bool GetWereJunimosPaidToday() {
            return Util.Payments.WereJunimosPaidToday;
        }
    }
}