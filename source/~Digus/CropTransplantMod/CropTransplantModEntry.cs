using System.Reflection;
using Harmony;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace CropTransplantMod
{
    public class CropTransplantModEntry : Mod
    {
        public static IMonitor ModMonitor;

        public override void Entry(IModHelper helper)
        {
            ModMonitor = Monitor;
            new DataLoader(helper);

            var harmony = HarmonyInstance.Create("Digus.CustomCrystalariumMod");

            var utilityTryToPlaceItem = typeof(Utility).GetMethod("tryToPlaceItem");
            var objectOverridesTryToPlaceItem = typeof(TransplantOverrides).GetMethod("TryToPlaceItem");
            harmony.Patch(utilityTryToPlaceItem, new HarmonyMethod(objectOverridesTryToPlaceItem), null);

            var utilityCanGrabSomethingFromHere = typeof(Utility).GetMethod("canGrabSomethingFromHere");
            var objectOverridesCanGrabSomethingFromHere = typeof(TransplantOverrides).GetMethod("CanGrabSomethingFromHere");
            harmony.Patch(utilityCanGrabSomethingFromHere, new HarmonyMethod(objectOverridesCanGrabSomethingFromHere), null);

            var utilityPlayerCanPlaceItemHere = typeof(Utility).GetMethod("playerCanPlaceItemHere");
            var objectOverridesPlayerCanPlaceItemHere = typeof(TransplantOverrides).GetMethod("PlayerCanPlaceItemHere");
            harmony.Patch(utilityPlayerCanPlaceItemHere, new HarmonyMethod(objectOverridesPlayerCanPlaceItemHere), null);

            var hoeDirtPerformUseAction = typeof(HoeDirt).GetMethod("performUseAction");
            var objectOverridesPerformUseAction = typeof(TransplantOverrides).GetMethod("PerformUseAction");
            harmony.Patch(hoeDirtPerformUseAction, new HarmonyMethod(objectOverridesPerformUseAction), null);

            var fruitTreePerformUseAction = typeof(FruitTree).GetMethod("performUseAction");
            var transplantOverridesFruitTreePerformUseAction = typeof(TransplantOverrides).GetMethod("FruitTreePerformUseAction");
            harmony.Patch(fruitTreePerformUseAction, null, new HarmonyMethod(transplantOverridesFruitTreePerformUseAction));

            var game1PressUseToolButton = typeof(Game1).GetMethod("pressUseToolButton");
            var objectOverridesPressUseToolButton = typeof(TransplantOverrides).GetMethod("PressUseToolButton");
            harmony.Patch(game1PressUseToolButton, new HarmonyMethod(objectOverridesPressUseToolButton), null);

            if (DataLoader.ModConfig.EnableSoilTileUnderTrees)
            {
                var treeDraw = typeof(Tree).GetMethod("draw");
                var transplantOverridesPreTreeDraw = typeof(TransplantOverrides).GetMethod("PreTreeDraw");
                harmony.Patch(treeDraw, new HarmonyMethod(transplantOverridesPreTreeDraw), null);
            }

            SaveEvents.BeforeSave += (x, y) =>
            {
                if (Game1.player.ActiveObject is HeldIndoorPot pot)
                {
                    Game1.player.ActiveObject = TransplantOverrides.RegularPotObject;
                    TransplantOverrides.CurrentHeldIndoorPot = null;
                }
            };
        }
    }
}
