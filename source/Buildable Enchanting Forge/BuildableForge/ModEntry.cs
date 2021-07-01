/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SlivaStari/BuildableForge
**
*************************************************/

using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using SObject = StardewValley.Object;
using BuildableForge;
using Harmony;

namespace BuildableForge
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod, IAssetEditor, IAssetLoader
    {
        private const string buildingType = "Enchanting Forge";

        private readonly string assetPath = Path.Combine("Buildings", buildingType);
        private readonly string blueprintsPath = Path.Combine("Data", "Blueprints");

        private readonly string defaultBuildingTexturePath = Path.Combine("assets", "EnchantingForge.png");
        private readonly string blueprintDataPath = Path.Combine("assets", "EnchantingForgeBlueprint.json");

        private BlueprintData blueprintData;

        private static IMonitor ModMonitor;
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            blueprintData = helper.Data.ReadJsonFile<BlueprintData>(blueprintDataPath);

            helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }
        /// <summary>The event called after an active menu is opened or closed.</summary>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            // Add the blueprint
            if (e.NewMenu is CarpenterMenu carpenterMenu && Game1.MasterPlayer.hasOrWillReceiveMail("reachedCaldera"))
            {
                bool isMagicalMenu = Helper.Reflection.GetField<bool>(carpenterMenu, "magicalConstruction").GetValue();

                if (!isMagicalMenu) return;

                IList<BluePrint> blueprints = Helper.Reflection
                    .GetField<List<BluePrint>>(carpenterMenu, "blueprints")
                    .GetValue();

                // Add furnace blueprint, and tag it uniquely based on how many have been built
                blueprints.Add(new BluePrint(buildingType){});
            }
        }
        private void OnGameLaunched(object sender, EventArgs e)
        {
            var harmony = HarmonyInstance.Create("Stari.BuildableForge");
            ModMonitor = Monitor;

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Buildings.Building), nameof(StardewValley.Buildings.Building.doAction)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.Building_doAction_Prefix))
            );
        }

        // Could be a onButtonPressed Event Handler instead, not much benefit to hooking into Building doAction, as it gets called on _every_ building on the farm when one is clicked.
        [HarmonyPostfix]
        public static bool Building_doAction_Prefix(StardewValley.Buildings.Building __instance, Vector2 tileLocation, Farmer who, ref bool __result)
        {
            try
            {
                if (__instance.buildingType.Value != buildingType)
                {
                    return true; // run original logic
                }
                ModMonitor.Log($"{nameof(Building_doAction_Prefix)}: {__instance.buildingType.Value} right clicked.", LogLevel.Trace);
                if (!(tileLocation.X >= (float)(int)__instance.tileX && tileLocation.X < (float)((int)__instance.tileX + (int)__instance.tilesWide) && tileLocation.Y >= (float)(int)__instance.tileY && tileLocation.Y < (float)((int)__instance.tileY + (int)__instance.tilesHigh)))
                {
                    return true; // run original logic
                }
                if (!who.IsLocalPlayer || (int)__instance.daysOfConstructionLeft > 0 || who.isRidingHorse())
                {
                    return true; // run original logic
                }
                // Just open the forge menu
                Game1.activeClickableMenu = new ForgeMenu();
                __result = true;
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                ModMonitor.Log($"Failed in {nameof(Building_doAction_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
        
        /// <summary>Get whether this instance can load the initial version of the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanLoad<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals(assetPath))
            {
                return true;
            }
            return false;
        }
        /// <summary>Load a matched asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public T Load<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals(assetPath))
            {
                return Helper.Content.Load<T>(defaultBuildingTexturePath, ContentSource.ModFolder);
            }

            throw new InvalidOperationException($"Unexpected asset '{asset.AssetName}'.");
        }
        /// <summary>Get whether this instance can edit the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being edit.</param>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals(blueprintsPath))
            {
                return true;
            }

            return false;
        }
        /// <summary>Edit a matched asset.</summary>
        /// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals(blueprintsPath))
            {
                var editor = asset.AsDictionary<string, string>();
                editor.Data[buildingType] = blueprintData.ToBlueprintString();
                return;
            }

            throw new InvalidOperationException($"Unexpected asset '{asset.AssetName}'.");
        }
    }
}