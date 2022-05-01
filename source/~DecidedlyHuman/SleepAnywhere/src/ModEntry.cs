/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System.Linq;
using HarmonyLib;
using Microsoft.Build.Utilities;
using Microsoft.Xna.Framework;
using SleepAnywhere.Helpers;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using Patches = SleepAnywhere.HarmonyPatches.Patches;
using SObject = StardewValley.Object;

namespace SleepAnywhere
{
	public class ModEntry : Mod
	{
		private IModHelper helper;
        private ModConfig config;

        public override void Entry(IModHelper helper)
		{
			this.helper = helper;
            config = helper.ReadConfig<ModConfig>();
            
			Patches.Initialise(Monitor);
			Harmony harmony = new Harmony(ModManifest.UniqueID);
            
			harmony.Patch(
				original: AccessTools.Method(typeof(BedFurniture), nameof(BedFurniture.placementAction)),
				postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.BedFurniture_PlacementAction_Postfix)));
            
            harmony.Patch(
                original: AccessTools.Method(typeof(Utility), nameof(Utility.playerCanPlaceItemHere)),
                postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.Utility_PlayerCanPlaceItemHere_Postfix)));
			
			this.helper.Events.Input.ButtonsChanged += ButtonsChanged;
            this.helper.Events.GameLoop.GameLaunched += (sender, args) =>
            {
                RegisterWithGmcm();
                
            }; 
			
            Monitor.Log("Stardew Teleporter Network initialised.");
		}

		private void ButtonsChanged(object? sender, ButtonsChangedEventArgs e)
		{
            if (e.Pressed.Contains(SButton.OemTilde))
            {
                Monitor.Log("Placing our portable bed.");

                Vector2 targetTile = Game1.player.GetGrabTile();
                BedFurniture bed = Utility.fuzzyItemSearch("DecidedlyHuman.SleepAnywhereItems/Portable Bed") as BedFurniture;

                bed.TileLocation = targetTile;
                Game1.currentLocation.furniture.Add(bed);
            }
		}
        
        private void RegisterWithGmcm()
        {
            IGenericModConfigMenuApi configMenuApi =
                Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (configMenuApi == null)
            {
                Monitor.Log("The user doesn't have GMCM installed. This is not an error.", LogLevel.Info);

                return;
            }

            // configMenuApi.Register(ModManifest,
            //     () => config = new ModConfig(),
            //     () => Helper.WriteConfig(config));
            //
            // configMenuApi.AddSectionTitle(
            //     mod: ModManifest,
            //     text: () => "Keybinds"
            // );
            //
            // configMenuApi.AddParagraph(
            //     mod: ModManifest,
            //     text: () => "GMCM currently doesn't support adding mouse keybinds in its config menus. In the meantime, refer to the second page for advice on editing the config.json file to add them manually."
            // );
            //
            // configMenuApi.AddKeybindList(
            //     mod: ModManifest,
            //     name: () => "Engage build mode",
            //     getValue: () => config.EngageBuildMode,
            //     setValue: value => config.EngageBuildMode = value);
            //
            // configMenuApi.AddKeybindList(
            //     mod: ModManifest,
            //     name: () => "Hold to draw",
            //     getValue: () => config.HoldToDraw,
            //     setValue: value => config.HoldToDraw = value);
            //
            // configMenuApi.AddKeybindList(
            //     mod: ModManifest,
            //     name: () => "Hold to erase",
            //     getValue: () => config.HoldToErase,
            //     setValue: value => config.HoldToErase = value);
            //
            // configMenuApi.AddKeybindList(
            //     mod: ModManifest,
            //     name: () => "Hold to insert item",
            //     getValue: () => config.HoldToInsert,
            //     setValue: value => config.HoldToInsert = value);
            //
            // configMenuApi.AddKeybindList(
            //     mod: ModManifest,
            //     name: () => "Confirm build",
            //     getValue: () => config.ConfirmBuild,
            //     setValue: value => config.ConfirmBuild = value);
            //
            // configMenuApi.AddKeybindList(
            //     mod: ModManifest,
            //     name: () => "Pick up object",
            //     getValue: () => config.PickUpObject,
            //     setValue: value => config.PickUpObject = value);
            //
            // configMenuApi.AddKeybindList(
            //     mod: ModManifest,
            //     name: () => "Pick up floor",
            //     getValue: () => config.PickUpFloor,
            //     setValue: value => config.PickUpFloor = value);
            //
            // configMenuApi.AddKeybindList(
            //     mod: ModManifest,
            //     name: () => "Pick up furniture",
            //     getValue: () => config.PickUpFurniture,
            //     setValue: value => config.PickUpFurniture = value);
            //
            // configMenuApi.AddSectionTitle(
            //     mod: ModManifest,
            //     text: () => "Optional Toggles"
            // );
            //
            // configMenuApi.AddBoolOption(
            //     mod: ModManifest,
            //     name: () => "Show build queue",
            //     getValue: () => config.ShowBuildQueue,
            //     setValue: value => config.ShowBuildQueue = value
            // );
            //
            // configMenuApi.AddBoolOption(
            //     mod: ModManifest,
            //     name: () => "Can pick up chests",
            //     tooltip: () => "WARNING: This will drop all contained items on the ground.",
            //     getValue: () => config.CanDestroyChests,
            //     setValue: value => config.CanDestroyChests = value
            // );
            //
            // configMenuApi.AddBoolOption(
            //     mod: ModManifest,
            //     name: () => "More lax floor placement",
            //     tooltip: () => "Allows you to place floors essentially anywhere, including UNREACHABLE AREAS. BE CAREFUL WITH THIS.",
            //     getValue: () => config.LessRestrictiveFloorPlacement,
            //     setValue: value => config.LessRestrictiveFloorPlacement = value
            // );
            //
            // configMenuApi.AddBoolOption(
            //     mod: ModManifest,
            //     name: () => "More lax furniture placement",
            //     tooltip: () => "Allows you to place furniture essentially anywhere, including UNREACHABLE AREAS. BE CAREFUL WITH THIS.",
            //     getValue: () => config.LessRestrictiveFurniturePlacement,
            //     setValue: value => config.LessRestrictiveFurniturePlacement = value
            // );
            //
            // configMenuApi.AddBoolOption(
            //     mod: ModManifest,
            //     name: () => "More lax bed placement",
            //     tooltip: () => "Allows you to place beds essentially anywhere, allowing you to sleep in places you shouldn't be able to sleep in. BE CAREFUL WITH THIS.",
            //     getValue: () => config.LessRestrictiveBedPlacement,
            //     setValue: value => config.LessRestrictiveBedPlacement = value
            // );
            //
            // configMenuApi.AddBoolOption(
            //     mod: ModManifest,
            //     name: () => "Replaceable floors",
            //     tooltip: () => "Allows you to replace an existing floor/path with another. Note that you will not get the existing floor back (yet).",
            //     getValue: () => config.EnableReplacingFloors,
            //     setValue: value => config.EnableReplacingFloors = value
            // );
            //
            // configMenuApi.AddBoolOption(
            //     mod: ModManifest,
            //     name: () => "Replaceable fences",
            //     tooltip: () => "Allows you to replace an existing fence with another. Note that you will not get the existing fence back.",
            //     getValue: () => config.EnableReplacingFences,
            //     setValue: value => config.EnableReplacingFences = value
            // );
            //
            // configMenuApi.AddSectionTitle(
            //     mod: ModManifest,
            //     text: () => "The Slightly Cheaty Zone"
            // );
            //
            // configMenuApi.AddBoolOption(
            //     mod: ModManifest,
            //     name: () => "Place crab pots in any water tile",
            //     getValue: () => config.CrabPotsInAnyWaterTile,
            //     setValue: b => config.CrabPotsInAnyWaterTile = b
            // );
            //
            // configMenuApi.AddBoolOption(
            //     mod: ModManifest,
            //     name: () => "Allow planting crops",
            //     getValue: () => config.EnablePlantingCrops,
            //     setValue: b => config.EnablePlantingCrops = b
            // );
            //
            // configMenuApi.AddBoolOption(
            //     mod: ModManifest,
            //     name: () => "Allow fertilizing crops",
            //     getValue: () => config.EnableCropFertilizers,
            //     setValue: b => config.EnableCropFertilizers = b
            // );
            //
            // configMenuApi.AddBoolOption(
            //     mod: ModManifest,
            //     name: () => "Allow fertilizing trees",
            //     getValue: () => config.EnableTreeFertilizers,
            //     setValue: b => config.EnableTreeFertilizers = b
            // );
            //
            // configMenuApi.AddBoolOption(
            //     mod: ModManifest,
            //     name: () => "Allow tree tappers",
            //     getValue: () => config.EnableTreeTappers,
            //     setValue: b => config.EnableTreeTappers = b
            // );
            //
            // configMenuApi.AddBoolOption(
            //     mod: ModManifest,
            //     name: () => "Enable placing items into machines",
            //     getValue: () => config.EnableInsertingItemsIntoMachines,
            //     setValue: b => config.EnableInsertingItemsIntoMachines = b
            // );
            //
            // configMenuApi.AddSectionTitle(
            //     mod: ModManifest,
            //     text: () => "Debug"
            // );
            //
            // configMenuApi.AddBoolOption(
            //     mod: ModManifest,
            //     name: () => "Enable debug command",
            //     getValue: () => config.EnableDebugCommand,
            //     setValue: b => config.EnableDebugCommand = b
            // );
            //
            // configMenuApi.AddBoolOption(
            //     mod: ModManifest,
            //     name: () => "Enable debug keybinds",
            //     getValue: () => config.EnableDebugControls,
            //     setValue: b => config.EnableDebugControls = b
            // );
            //
            // configMenuApi.AddKeybindList(
            //     mod: ModManifest,
            //     name: () => "Identify producer to console",
            //     getValue: () => config.IdentifyProducer,
            //     setValue: value => config.IdentifyProducer = value);
            //
            // configMenuApi.AddKeybindList(
            //     mod: ModManifest,
            //     name: () => "Identify held item to console",
            //     getValue: () => config.IdentifyItem,
            //     setValue: value => config.IdentifyItem = value);
            //
            // configMenuApi.AddSectionTitle(
            //     mod: ModManifest,
            //     text: () => "THIS NEXT OPTION IS POTENTIALLY DANGEROUS."
            // );
            //
            // configMenuApi.AddParagraph(
            //     mod: ModManifest,
            //     text: () => "You shouldn't, but you might lose items inside your dressers/other storage furniture. If you do, please let me know."
            // );
            //
            // configMenuApi.AddBoolOption(
            //     mod: ModManifest,
            //     name: () => "Enable placing storage furniture",
            //     tooltip: () => "WARNING: PLACING STORAGE FURNITURE WITH SMART BUILDING IS RISKY. Your items should transfer over just fine, but it's your risk to take.",
            //     getValue: () => config.EnablePlacingStorageFurniture,
            //     setValue: value => config.EnablePlacingStorageFurniture = value
            // );
            //
            // configMenuApi.AddPageLink(
            //     mod: ModManifest,
            //     pageId: "JsonGuide",
            //     text: () => "(Click me!) A short guide on adding mouse bindings."
            // );
            //
            // configMenuApi.AddPage(
            //     mod: ModManifest,
            //     pageId: "JsonGuide",
            //     pageTitle: () => "Mouse Key Bindings"
            // );
            //
            // configMenuApi.AddParagraph(
            //     mod: ModManifest,
            //     text: () => "From: https://stardewvalleywiki.com/Modding:Player_Guide/Key_Bindings#Multi-key_bindings"
            // );
            //
            // configMenuApi.AddParagraph(
            //     mod: ModManifest,
            //     text: () => "Mods using SMAPI 3.9+ features can support multi-key bindings. That lets you combine multiple button codes into a combo keybind, and list alternate keybinds. For example, \"LeftShoulder, LeftControl + S\" will apply if LeftShoulder is pressed, or if both LeftControl and S are pressed."
            // );
            //
            // configMenuApi.AddParagraph(
            //     mod: ModManifest,
            //     text: () => "Some things to keep in mind:"
            // );
            //
            // configMenuApi.AddParagraph(
            //     mod: ModManifest,
            //     text: () => "The order doesn't matter, so \"LeftControl + S\" and \"S + LeftControl\" are equivalent."
            // );
            //
            // configMenuApi.AddParagraph(
            //     mod: ModManifest,
            //     text: () => "SMAPI doesn't prevent mods from using overlapping hotkeys. For example, if one mod uses \"S\" and the other mod uses \"LeftControl + S\", pressing LeftControl and S will activate both."
            // );
        }
	}
}