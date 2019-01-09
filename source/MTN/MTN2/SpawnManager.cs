using MTN2.MapData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2 {
    public class SpawnManager {
        private readonly CustomFarmManager farmManager;
        private int Attempts = 10;

        public SpawnManager(CustomFarmManager farmManager) {
            this.farmManager = farmManager;
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
            if (farmManager.Canon) return;
            if (farmManager.LoadedFarm.Foraging == null) return;
            farmManager.LoadedFarm.Foraging.SpawnAll(Attempts);
        }

        public void ManageOre() {
            if (farmManager.Canon) return;
            if (farmManager.LoadedFarm.Ores == null) return;
            farmManager.LoadedFarm.Ores.SpawnAll(Attempts);
        }

        public void ManageLargeDebris() {
            if (farmManager.Canon) return;
            if (farmManager.LoadedFarm.ResourceClumps == null) return;
            farmManager.LoadedFarm.ResourceClumps.SpawnAll(Attempts);
        }
    }
}
