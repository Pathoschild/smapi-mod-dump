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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2
{
    public class PatchManager
    {
        private readonly CustomFarmManager farmManager;
        private List<Patch> patches;

        public PatchManager(CustomFarmManager farmManager) {
            this.farmManager = farmManager;
            patches = new List<Patch>();
        }

        public void Initialize(IModHelper helper, IMonitor monitor) {
            ImpInitialize(helper, "Event", "setExitLocation", new setExitLocationPatch(farmManager), typeof(setExitLocationPatch));
            ImpInitialize(helper, "Farm", "checkAction", new checkActionPatch(farmManager), typeof(checkActionPatch));
            ImpInitialize(helper, "Farm", "", new ConstructorFarmPatch(farmManager), typeof(ConstructorFarmPatch), new Type[] { typeof(string), typeof(string) });
            ImpInitialize(helper, "Farm", "draw", new drawPatch(farmManager), typeof(drawPatch));
            ImpInitialize(helper, "Farm", "getFrontDoorPositionForFarmer", new getFrontDoorPositionForFarmerPatch(farmManager), typeof(getFrontDoorPositionForFarmerPatch));
            ImpInitialize(helper, "Farm", "leftClick", new leftClickPatch(farmManager), typeof(leftClickPatch));
            ImpInitialize(helper, "Farm", "resetLocalState", new resetLocalStatePatch(farmManager), typeof(resetLocalStatePatch));
            ImpInitialize(helper, "Farm", "UpdateWhenCurrentLocation", new UpdateWhenCurrentLocationPatch(farmManager), typeof(UpdateWhenCurrentLocationPatch));
            ImpInitialize(helper, "Locations.FarmHouse", "", new ConstructorFarmHousePatch(farmManager), typeof(ConstructorFarmHousePatch), new Type[] { typeof(string), typeof(string) });
            //patches.Add(new Patch(helper, "Locations.FarmHouse", "getPorchStandingSpot", PatchType.Method));
            ImpInitialize(helper, "Locations.FarmHouse", "updateMap", new updateMapPatch(farmManager), typeof(updateMapPatch));
            ImpInitialize(helper, "Game1", "loadForNewGame", new loadForNewGamePatch(farmManager, monitor), typeof(loadForNewGamePatch));
            ImpInitialize(helper, "GameLocation", "loadObjects", new loadObjectsPatch(farmManager), typeof(loadObjectsPatch));
            ImpInitialize(helper, "GameLocation", "startEvent", new startEventPatch(farmManager), typeof(startEventPatch));
            //ImpInitialize(helper, "Network.NetBuildingRef", "get_Value", new ValueGetterPatch(), typeof(ValueGetterPatch));
            ImpInitialize(helper, "NPC", "updateConstructionAnimation", new updateConstructionAnimationPatch(farmManager), typeof(updateConstructionAnimationPatch));
            ImpInitialize(helper, "Object", "totemWarpForReal", new totemWarpForRealPatch(farmManager), typeof(totemWarpForRealPatch));
            ImpInitialize(helper, "Characters.Pet", "dayUpdate", new dayUpdatePatch(farmManager), typeof(dayUpdatePatch));
            ImpInitialize(helper, "Characters.Pet", "setAtFarmPosition", new setAtFarmPositionPatch(farmManager), typeof(setAtFarmPositionPatch));
            //ImpInitialize(helper, "SaveGame", "loadDataToLocations", new loadDataToLocationsPatch(), typeof(loadDataToLocationsPatch));
            ImpInitialize(helper, "Menus.TitleMenu", "setUpIcons", new setUpIconsPatch(), typeof(setUpIconsPatch));
            ImpInitialize(helper, "Tools.Wand", "wandWarpForReal", new wandWarpForRealPatch(farmManager), typeof(wandWarpForRealPatch));
            ImpInitialize(helper, "Events.WorldChangeEvent", "setUp", new setUpPatch(farmManager), typeof(setUpPatch));
        }

        private void ImpInitialize(IModHelper helper, string targetClass, string routine, object obj, Type type, Type[] parameters = null) {
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
