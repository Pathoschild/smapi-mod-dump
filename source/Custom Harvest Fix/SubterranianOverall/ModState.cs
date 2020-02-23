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
        private const String SAVE_KEY = "so_data";
        public static HashSet<String> visitedMineshafts = new HashSet<String>();
        public static Dictionary<String, Dictionary<Vector2, VoidshroomtreeSaveData>> voidshroomTreeLocations = new Dictionary<String, Dictionary<Vector2, VoidshroomtreeSaveData>>();
        //public static Dictionary<String, Dictionary<Vector2, CaveCarrotCropSaveData>> caveCarrotsPlanted = new Dictionary<String, Dictionary<Vector2, CaveCarrotCropSaveData>>();
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
            ModEntry.GetMonitor().Log("Attempting to save mod data");
            ModEntry.GetHelper().Data.WriteSaveData(SAVE_KEY, ModState.getModState());
        }

        public static void LoadMod()
        {
            if (!Game1.IsMasterGame)
            {
                return;
            }

            
            try
            {
                ModState state = ModEntry.GetHelper().Data.ReadSaveData<ModState>(SAVE_KEY);

                if (state == null)
                {
                    //need to find folks who have the old data model and convert them to the new one.
                    //since the save key used doesn't match the current save key, lets try the old method.
                    String data = ModEntry.GetHelper().Data.ReadSaveData<String>("data");
                    ModEntry.GetMonitor().Log("Attempting to load mod data");
                    state = JsonConvert.DeserializeObject<ModState>(data);

                    if (state != null)
                    {
                        //this user has the old save format.  Save in new format real quick and blank out the old key.
                        ModEntry.GetHelper().Data.WriteSaveData(SAVE_KEY, ModState.getModState());
                        ModEntry.GetHelper().Data.WriteSaveData("data", "");
                    }

                }
            } catch (Exception e)
            {
                ModEntry.GetMonitor().Log(e.Message);
            }
            
            
        }
    }
}
