using Harmony;
using MTN2.Patches.EventPatches;
using MTN2.Patches.FarmHousePatches;
using MTN2.Patches.FarmPatches;
using MTN2.Patches.Game1Patches;
using MTN2.Patches.GameLocationPatches;
using MTN2.Patches.NetBuildingRefPatches;
using MTN2.Patches.NPCPatches;
using MTN2.Patches.ObjectsPatches;
using MTN2.Patches.PetPatches;
using MTN2.Patches.SaveGamePatches;
using MTN2.Patches.TitleMenuPatches;
using MTN2.Patches.WandPatches;
using MTN2.Patches.WorldChangeEventPatches;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MTN2
{
    public class PatchManager
    {
        private readonly CustomManager customManager;
        private IModHelper helper;
        private List<Patch> patches;
        private PatchConfig patchConfig;

        public PatchManager(CustomManager customManager) {
            this.customManager = customManager;
            patches = new List<Patch>();
        }

        public void Initialize(IModHelper helper, IMonitor monitor) {
            this.helper = helper;
            patchConfig = helper.Data.ReadJsonFile<PatchConfig>("darkmagic.json");
            if (patchConfig == null || patchConfig.Version != FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion) {
                patchConfig = PatchConfig.Default;
                helper.Data.WriteJsonFile("darkmagic.json", patchConfig);
            }

            ImpInitialize(patchConfig.EventPatch["SetExitLocation"], "Event", "setExitLocation", new setExitLocationPatch(customManager), typeof(setExitLocationPatch));
            ImpInitialize(patchConfig.FarmPatch["CheckAction"], "Farm", "checkAction", new checkActionPatch(customManager), typeof(checkActionPatch));
            ImpInitialize(patchConfig.FarmPatch["Constructor"], "Farm", "", new ConstructorFarmPatch(customManager), typeof(ConstructorFarmPatch), new Type[] { typeof(string), typeof(string) });
            ImpInitialize(patchConfig.FarmPatch["Draw"], "Farm", "draw", new drawPatch(customManager), typeof(drawPatch));
            ImpInitialize(patchConfig.FarmPatch["GetFrontDoorPositionForFarmer"], "Farm", "getFrontDoorPositionForFarmer", new getFrontDoorPositionForFarmerPatch(customManager), typeof(getFrontDoorPositionForFarmerPatch));
            ImpInitialize(patchConfig.FarmPatch["LeftClick"], "Farm", "leftClick", new leftClickPatch(customManager), typeof(leftClickPatch));
            ImpInitialize(patchConfig.FarmPatch["ResetLocalState"], "Farm", "resetLocalState", new resetLocalStatePatch(customManager), typeof(resetLocalStatePatch));
            ImpInitialize(patchConfig.FarmPatch["UpdateWhenCurrentLocation"], "Farm", "UpdateWhenCurrentLocation", new UpdateWhenCurrentLocationPatch(customManager), typeof(UpdateWhenCurrentLocationPatch));
            ImpInitialize(patchConfig.FarmHousePatch["Constructor"], "Locations.FarmHouse", "", new ConstructorFarmHousePatch(customManager), typeof(ConstructorFarmHousePatch), new Type[] { typeof(string), typeof(string) });
            ImpInitialize(patchConfig.FarmHousePatch["GetPorchStandingSpot"], "Locations.FarmHouse", "GetPorchStandingSpot", new getPorchStandingSpotPatch(customManager), typeof(getPorchStandingSpotPatch));
            ImpInitialize(patchConfig.FarmHousePatch["UpdateMap"], "Locations.FarmHouse", "updateMap", new updateMapPatch(customManager), typeof(updateMapPatch));
            ImpInitialize(patchConfig.Game1Patch["LoadForNewGame"], "Game1", "loadForNewGame", new loadForNewGamePatch(customManager, monitor), typeof(loadForNewGamePatch));
            ImpInitialize(patchConfig.GameLocationPatch["LoadObjects"], "GameLocation", "loadObjects", new loadObjectsPatch(customManager), typeof(loadObjectsPatch));
            ImpInitialize(patchConfig.GameLocationPatch["PerformAction"], "GameLocation", "performAction", new performActionPatch(customManager), typeof(performActionPatch));
            ImpInitialize(patchConfig.GameLocationPatch["StartEvent"], "GameLocation", "startEvent", new startEventPatch(customManager), typeof(startEventPatch));
            ImpInitialize(patchConfig.NPCPatch["UpdateConstructionAnimation"], "NPC", "updateConstructionAnimation", new updateConstructionAnimationPatch(customManager), typeof(updateConstructionAnimationPatch));
            ImpInitialize(patchConfig.ObjectPatch["TotemWarpForReal"], "Object", "totemWarpForReal", new totemWarpForRealPatch(customManager), typeof(totemWarpForRealPatch));
            ImpInitialize(patchConfig.PetPatch["DayUpdate"], "Characters.Pet", "dayUpdate", new dayUpdatePatch(customManager), typeof(dayUpdatePatch));
            ImpInitialize(patchConfig.PetPatch["SetAtFarmPosition"], "Characters.Pet", "setAtFarmPosition", new setAtFarmPositionPatch(customManager), typeof(setAtFarmPositionPatch));
            ImpInitialize(patchConfig.SaveGamePatch["LoadDataForLocations"], "SaveGame", "loadDataToLocations", new loadDataToLocationsPatch(), typeof(loadDataToLocationsPatch));
            ImpInitialize(patchConfig.TitleMenuPatch["SetUpIcons"], "Menus.TitleMenu", "setUpIcons", new setUpIconsPatch(), typeof(setUpIconsPatch));
            ImpInitialize(patchConfig.WandPatch["WandWarpForReal"], "Tools.Wand", "wandWarpForReal", new wandWarpForRealPatch(customManager), typeof(wandWarpForRealPatch));
            ImpInitialize(patchConfig.WorldChangeEventPatch["SetUp"], "Events.WorldChangeEvent", "setUp", new setUpPatch(customManager), typeof(setUpPatch));

            //ImpInitialize(helper, "Network.NetBuildingRef", "get_Value", new ValueGetterPatch(), typeof(ValueGetterPatch));
        }

        private void ImpInitialize(bool enabled, string targetClass, string routine, object obj, Type type, Type[] parameters = null) {
            if (!enabled) return;

            bool prefix = false;
            bool transpiler = false;
            bool postfix = false;

            if (helper.Reflection.GetMethod(type, "Prefix", false) != null) {
                prefix = true;
            }
            if (helper.Reflection.GetMethod(type, "Transpiler", false) != null) {
                transpiler = true;
            }
            if (helper.Reflection.GetMethod(type, "Postfix", false) != null) {
                postfix = true;
            }

            patches.Add(new Patch(helper, targetClass, routine, (routine == "") ? PatchType.Constructor : PatchType.Method, obj));

            patches[patches.Count - 1].Initialize(prefix, postfix, transpiler, type, parameters);
        }

        public void Apply(HarmonyInstance harmonyInstance) {
            for (int i = 0; i < patches.Count; i++) patches[i].Apply(harmonyInstance);
        }
    }
}
