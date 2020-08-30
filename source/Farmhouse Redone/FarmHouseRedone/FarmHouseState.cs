using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace FarmHouseRedone
{
    public class FarmHouseState
    {
        
        public string bedData;
        public string spouseRoomData;
        public string kitchenData;
        public string entryData;
        public string cellarData;
        public string levelThreeData;
        public bool isMarriage;
        public LevelNUpgrade pendingUpgrade;
        public LevelNUpgrade currentUpgrade;

        public int wallsAndFloorsSheet;
        public int interiorSheet;
        public int farmSheet;
        public int sewerSheet;

        public FarmHouseState()
        {
            interiorSheet = 0;
            farmSheet = 1;
            wallsAndFloorsSheet = 2;
            sewerSheet = -1;
            bedData = null;
            spouseRoomData = null;
            kitchenData = null;
            entryData = null;
            cellarData = null;
            isMarriage = false;
        }

        public void clear()
        {
            interiorSheet = 0;
            farmSheet = 1;
            wallsAndFloorsSheet = 2;
            sewerSheet = -1;
            bedData = null;
            spouseRoomData = null;
            kitchenData = null;
            entryData = null;
            cellarData = null;
            isMarriage = false;
        }
    }
}
