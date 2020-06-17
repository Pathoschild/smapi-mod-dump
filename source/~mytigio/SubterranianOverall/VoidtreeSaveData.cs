using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubterranianOverhaul
{   
    public class VoidshroomtreeSaveData
    {

        public VoidshroomtreeSaveData()
        {

        }

        public VoidshroomtreeSaveData(VoidshroomTree tree)
        {
            this.growthStage = tree.growthStage.Value;
            this.flipped = tree.flipped.Value;
            this.health = tree.health.Value;
            this.stump = tree.stump.Value;
            this.tapped = tree.tapped.Value;
            this.hasSeed = tree.hasSeed.Value;
        }

        public int growthStage;
        public bool flipped;
        public float health;
        public bool stump;
        public bool tapped;
        public bool hasSeed;
    }
}
