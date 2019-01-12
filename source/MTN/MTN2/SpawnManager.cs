using MTN2.MapData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2 {
    public class SpawnManager {
        private readonly CustomManager customManager;
        private int Attempts = 10;

        public SpawnManager(CustomManager customManager) {
            this.customManager = customManager;
        }

        public void ManageAll() {
            ManageForage();
            ManageOre();
            ManageLargeDebris();
        }

        public void ManageAll(object sender, EventArgs e) {
            ManageForage();
            ManageOre();
            ManageLargeDebris();
        }

        public void ManageForage() {
            if (customManager.Canon) return;
            if (customManager.LoadedFarm.Foraging == null) return;
            customManager.LoadedFarm.Foraging.SpawnAll(Attempts);
        }

        public void ManageOre() {
            if (customManager.Canon) return;
            if (customManager.LoadedFarm.Ores == null) return;
            customManager.LoadedFarm.Ores.SpawnAll(Attempts);
        }

        public void ManageLargeDebris() {
            if (customManager.Canon) return;
            if (customManager.LoadedFarm.ResourceClumps == null) return;
            customManager.LoadedFarm.ResourceClumps.SpawnAll(Attempts);
        }
    }
}
