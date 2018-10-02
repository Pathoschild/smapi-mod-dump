using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN {
    public class Blacklist {
        public List<string> bannedID { get; set; } = new List<string>();

        public Blacklist() { }

        public bool searchForBannedMod(string uniqueId) {
            foreach (string s in bannedID) {
                if (uniqueId == s) {
                    return true;
                }
            }
            return false;
        }
    }
}
