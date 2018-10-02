using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN {
    public struct PlayerMod {
        public string name;
        public string version;
        public string uniqueId;
        public string author;
        public bool isContentPack;
        public string ContentPackFor;
        public Dictionary<string, int> updatekeys;

        public PlayerMod(string name, string version, string uniqueId, string author, bool isContentPack, string ContentPackFor, Dictionary<string, int> updatekeys) {
            this.name = name;
            this.version = version;
            this.uniqueId = uniqueId;
            this.author = author;
            this.isContentPack = isContentPack;
            this.ContentPackFor = ContentPackFor;
            this.updatekeys = updatekeys;
        }

        public string getDetails() {
            return (name + " " + version + " by " + author + ". UniqueID: " + uniqueId);
        }
    }
}
