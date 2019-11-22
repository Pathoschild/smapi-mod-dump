using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using StardewValley.Locations;

namespace SubterranianOverhaul
{
    [Serializable]
    class ModState
    {
        public static HashSet<String> visitedMineshafts = new HashSet<String>();
        public static Dictionary<String, Dictionary<Vector2, VoidshroomtreeSaveData>> voidshroomTreeLocations = new Dictionary<String, Dictionary<Vector2, VoidshroomtreeSaveData>>();
        public static ModState thisModState;

        public HashSet<String> mineShaftSaveData
        {
            get {
                return ModState.visitedMineshafts;
            }

            set {
                ModState.visitedMineshafts = value;
            }
        }

        public Dictionary<String, Dictionary<Vector2, VoidshroomtreeSaveData>> voidshroomTreeLocationsSaveData
        {
            get {
                return ModState.voidshroomTreeLocations;
            }

            set {
                ModState.voidshroomTreeLocations = value;
            }
        }

        private ModState()
        {
           
        }

        public static ModState getModState()
        {
            if(thisModState == null)
            {
                thisModState = new ModState();
            }

            return thisModState;
        }

        public static void SaveMod()
        {
            if (!Game1.IsMasterGame)
                return;

            // save data
            string json = JsonConvert.SerializeObject(ModState.getModState());

            ModEntry.GetMonitor().Log("Attempting to save mod data");

            ModEntry.GetHelper().Data.WriteSaveData("data", json);
        }

        public static void LoadMod()
        {
            if (!Game1.IsMasterGame)
            {
                return;
            }

            
            try
            {
                String data = ModEntry.GetHelper().Data.ReadSaveData<String>("data");
                ModEntry.GetMonitor().Log("Attempting to load mod data");
                ModState state = JsonConvert.DeserializeObject<ModState>(data);
            } catch (Exception e)
            {
                ModEntry.GetMonitor().Log(e.Message);
            }
            
            
        }
    }
}
