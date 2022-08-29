/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace SmartBuilding
{
    public class ModConfig
    {
        public KeybindList CancelBuild = KeybindList.Parse("");
        public bool CanDestroyChests = false;
        public KeybindList CommitBuild = KeybindList.Parse("");
        public bool CrabPotsInAnyWaterTile = false;

        // Optional layer hotkeys
        public KeybindList DrawnLayer = KeybindList.Parse("");

        // Optional tool hotkeys
        public KeybindList DrawTool = KeybindList.Parse("");

        // Debug zone.
        public bool EnableDebugCommand = false;
        public bool EnableDebugControls = false;
        public bool EnableFertilizers = false;
        public bool EnableInsertingItemsIntoMachines = false;

        // THE DANGER ZONE.
        public bool EnablePlacingStorageFurniture = false;

        // The cheesy zone.
        public bool EnablePlantingCrops = false;
        public bool EnableReplacingFences = false;
        public bool EnableReplacingFloors = false;
        public bool EnableTreeTappers = false;
        // Main keybinds
        public KeybindList EngageBuildMode = KeybindList.Parse("LeftShift+B");
        public KeybindList EraseTool = KeybindList.Parse("");
        public KeybindList FilledRectangleTool = KeybindList.Parse("");
        public KeybindList FloorLayer = KeybindList.Parse("");
        public KeybindList FurnitureLayer = KeybindList.Parse("");
        public KeybindList HoldToDraw = KeybindList.Parse("MouseLeft");
        public KeybindList HoldToMoveMenu = KeybindList.Parse("MouseMiddle");
        public KeybindList IdentifyItem = KeybindList.Parse("LeftControl+LeftShift+J");
        public KeybindList IdentifyProducer = KeybindList.Parse("LeftControl+LeftShift+K");
        public KeybindList InsertTool = KeybindList.Parse("");
        // public KeybindList HoldToDrawRectangle = KeybindList.Parse("LeftAlt");
        // public KeybindList HoldToErase = KeybindList.Parse("LeftShift");
        // public KeybindList HoldToInsert = KeybindList.Parse("LeftControl");
        // public KeybindList ConfirmBuild = KeybindList.Parse("Enter");
        // public KeybindList PickUpObject = KeybindList.Parse("Delete");
        // public KeybindList PickUpFloor = KeybindList.Parse("End");
        // public KeybindList PickUpFurniture = KeybindList.Parse("Home");

        // Toggles
        public bool InstantlyBuild = false;
        public bool LessRestrictiveBedPlacement = false;
        public bool LessRestrictiveFloorPlacement = false;
        public bool LessRestrictiveFurniturePlacement = false;
        public bool LessRestrictiveObjectPlacement = false;
        public KeybindList ObjectLayer = KeybindList.Parse("");
        public bool ShowBuildQueue = true;
    }
}
