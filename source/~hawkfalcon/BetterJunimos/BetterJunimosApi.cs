using BetterJunimos.Utils;

namespace BetterJunimos {
    public class BetterJunimosApi {
        public int GetJunimoHutMaxRadius() {
            return Util.Config.JunimoHuts.MaxRadius;
        }

        public int GetJunimoHutMaxJunimos() {
            return Util.Config.JunimoHuts.MaxJunimos;
        }

        public bool GetWereJunimosPaidToday() {
            return Util.Payments.WereJunimosPaidToday;
        }
    }
}
