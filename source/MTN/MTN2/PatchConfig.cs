using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MTN2
{
    public class PatchConfig
    {
        public string Version { get; set; }
        public Dictionary<string, bool> EventPatch { get; set; }
        public Dictionary<string, bool> FarmPatch { get; set; }
        public Dictionary<string, bool> FarmHousePatch { get; set; }
        public Dictionary<string, bool> Game1Patch { get; set; }
        public Dictionary<string, bool> GameLocationPatch { get; set; }
        public Dictionary<string, bool> NPCPatch { get; set; }
        public Dictionary<string, bool> ObjectPatch { get; set; }
        public Dictionary<string, bool> PetPatch { get; set; }
        public Dictionary<string, bool> SaveGamePatch { get; set; }
        public Dictionary<string, bool> TitleMenuPatch { get; set; }
        public Dictionary<string, bool> WandPatch { get; set; }
        public Dictionary<string, bool> WorldChangeEventPatch { get; set; }

        public static PatchConfig Default {
            get {
                PatchConfig newConfig = new PatchConfig();
                newConfig.EventPatch = new Dictionary<string, bool> {
                    { "SetExitLocation", true }
                };
                newConfig.FarmPatch = new Dictionary<string, bool> {
                    { "CheckAction", true },
                    { "Constructor", true },
                    { "Draw", true },
                    { "GetFrontDoorPositionForFarmer", true},
                    { "LeftClick", true },
                    { "ResetLocalState", true },
                    { "UpdateWhenCurrentLocation", true},
                };
                newConfig.FarmHousePatch = new Dictionary<string, bool> {
                    { "Constructor", true },
                    { "GetPorchStandingSpot", false},
                    { "UpdateMap", true }
                };
                newConfig.Game1Patch = new Dictionary<string, bool> {
                    { "LoadForNewGame", true }
                };
                newConfig.GameLocationPatch = new Dictionary<string, bool> {
                    { "LoadObjects", true },
                    { "PerformAction", true },
                    { "StartEvent", true }
                };
                newConfig.NPCPatch = new Dictionary<string, bool> {
                    { "UpdateConstructionAnimation", true }
                };
                newConfig.ObjectPatch = new Dictionary<string, bool> {
                    { "TotemWarpForReal", true }
                };
                newConfig.PetPatch = new Dictionary<string, bool> {
                    { "DayUpdate", true },
                    { "SetAtFarmPosition", true }
                };
                newConfig.SaveGamePatch = new Dictionary<string, bool> {
                    { "LoadDataForLocations", false }
                };
                newConfig.TitleMenuPatch = new Dictionary<string, bool> {
                    { "SetUpIcons", true }
                };
                newConfig.WandPatch = new Dictionary<string, bool> {
                    { "WandWarpForReal", true }
                };
                newConfig.WorldChangeEventPatch = new Dictionary<string, bool> {
                    { "SetUp", true }
                };
                newConfig.Version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
                return newConfig;
            }
        }
    }
}
