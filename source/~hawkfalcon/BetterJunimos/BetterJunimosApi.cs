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