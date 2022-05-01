/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley.SDKs;

namespace SmartBuilding
{
    public class ModConfig
    {
        // TODO: Add an "instantly build" toggle.
        public KeybindList EngageBuildMode = KeybindList.Parse("LeftShift+B");
        public KeybindList HoldToDraw = KeybindList.Parse("MouseRight");
        public KeybindList HoldToErase = KeybindList.Parse("LeftShift");
        public KeybindList HoldToInsert = KeybindList.Parse("LeftControl");
        public KeybindList ConfirmBuild = KeybindList.Parse("MouseLeft");
        public KeybindList PickUpObject = KeybindList.Parse("Delete");
        public KeybindList PickUpFloor = KeybindList.Parse("End");
        public KeybindList PickUpFurniture = KeybindList.Parse("Home");
        public bool ShowBuildQueue = true;
        public bool CanDestroyChests = false;
        public bool CrabPotsInAnyWaterTile = false;
        public bool EnableReplacingFences = false;
        public bool EnableReplacingFloors = false;
        public bool LessRestrictiveFloorPlacement = false;
        public bool LessRestrictiveFurniturePlacement = false;
        public bool LessRestrictiveBedPlacement = false;
        // THE DANGER ZONE.
        public bool EnablePlacingStorageFurniture = false;
        // The cheesy zone.
        public bool EnablePlantingCrops = false;
        public bool EnableCropFertilizers = false;
        public bool EnableTreeFertilizers = false;
        public bool EnableTreeTappers = false;
        public bool EnableInsertingItemsIntoMachines = false;
		// Debug zone.
        public bool EnableDebugCommand = false;
		public bool EnableDebugControls = false;
        public KeybindList IdentifyItem = KeybindList.Parse("LeftControl+LeftShift+J");
		public KeybindList IdentifyProducer = KeybindList.Parse("LeftControl+LeftShift+K");
	}
}