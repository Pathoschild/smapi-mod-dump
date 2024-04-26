/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

// namespace StardewMods.BetterChests.Framework.Services;
//
// using Microsoft.Xna.Framework;
// using StardewMods.Common.Services;
// using StardewMods.Common.Services.Integrations.Automate;
// using StardewMods.Common.Services.Integrations.BetterChests.Enums;
// using StardewMods.Common.Services.Integrations.BetterCrafting;
// using StardewMods.Common.Services.Integrations.FauxCore;
// using StardewMods.Common.Services.Integrations.GenericModConfigMenu;
// using StardewMods.Common.Services.Integrations.ToolbarIcons;
// using StardewValley.Buildings;
// using StardewValley.Extensions;
// using StardewValley.Locations;
// using StardewValley.Objects;
//
// /// <summary>Handles integrations with other mods.</summary>
// internal sealed class IntegrationsManager : BaseService<IntegrationsManager>
// {
//     private const string ExpandedFridgeId = "Uwazouri.ExpandedFridge";
//     private const string HorseOverhaulId = "Goldenrevolver.HorseOverhaul";
//     private const string WearMoreRingsId = "bcmpinc.WearMoreRings";
//
//     private static IntegrationsManager instance = null!;
//
//     private readonly AutomateIntegration automateIntegration;
//     private readonly BetterCraftingIntegration betterCraftingIntegration;
//     private readonly GenericModConfigMenuIntegration genericModConfigMenuIntegration;
//     private readonly Dictionary<string, HashSet<string>> incompatibilities;
//     private readonly IModRegistry modRegistry;
//     private readonly ToolbarIconsIntegration toolbarIconsIntegration;
//
//     /// <summary>Initializes a new instance of the <see cref="IntegrationsManager" /> class.</summary>
//     /// <param name="log">Dependency used for logging debug information to the console.</param>
//     /// <param name="manifest">Dependency for accessing mod manifest.</param>
//     /// <param name="modContent">Dependency for loading mod assets.</param>
//     /// <param name="modRegistry">Dependency for fetching metadata about loaded mods.</param>
//     /// <param name="automateIntegration">Dependency for Automate integration.</param>
//     /// <param name="betterCraftingIntegration">Dependency for Better Crafting integration.</param>
//     /// <param name="genericModConfigMenuIntegration">Dependency for Generic Mod Config Menu integration.</param>
//     /// <param name="toolbarIconsIntegration">Dependency for Toolbar Icons integration.</param>
//     public IntegrationsManager(
//         AutomateIntegration automateIntegration,
//         BetterCraftingIntegration betterCraftingIntegration,
//         GenericModConfigMenuIntegration genericModConfigMenuIntegration,
//         ILog log,
//         IManifest manifest,
//         IModContentHelper modContent,
//         IModRegistry modRegistry,
//         ToolbarIconsIntegration toolbarIconsIntegration)
//         : base(log, manifest)
//     {
//         IntegrationsManager.instance = this;
//         this.modRegistry = modRegistry;
//         this.automateIntegration = automateIntegration;
//         this.betterCraftingIntegration = betterCraftingIntegration;
//         this.genericModConfigMenuIntegration = genericModConfigMenuIntegration;
//         this.toolbarIconsIntegration = toolbarIconsIntegration;
//         this.incompatibilities = modContent.Load<Dictionary<string, HashSet<string>>>("assets/incompatibilities.json");
//
//         // Defaults
//         if (this.modRegistry.IsLoaded(IntegrationsManager.HorseOverhaulId))
//         {
//             this.config.VanillaStorages.TryAdd(
//                 "SaddleBag",
//                 new StorageData { CustomColorPicker = FeatureOption.Disabled });
//         }
//     }
//
//     private static Dictionary<string, HashSet<string>> Incompatibilities =>
//         IntegrationsManager.instance.incompatibilities;
//
//     /// <summary>Gets all storages placed in a particular location.</summary>
//     /// <param name="location">The location to get storages from.</param>
//     /// <param name="excluded">A list of storage contexts to exclude to prevent iterating over the same object.</param>
//     /// <returns>An enumerable of all placed storages at the location.</returns>
//     public static IEnumerable<Storage> FromLocation(GameLocation location, ISet<object>? excluded = null)
//     {
//         excluded ??= new HashSet<object>();
//
//         foreach (var storage in IntegrationsManager.ExpandedFridge_FromLocation(location, excluded))
//         {
//             yield return storage;
//         }
//
//         foreach (var storage in IntegrationsManager.HorseOverhaul_FromLocation(location, excluded))
//         {
//             yield return storage;
//         }
//
//         if (IntegrationsManager.instance.modRegistry.IsLoaded(IntegrationsManager.WearMoreRingsId) && location is Farm
//             && location.Objects.TryGetValue(new Vector2(0, -50), out var obj))
//         {
//             excluded.Add(obj);
//         }
//     }
//
//     /// <summary>Gets all storages placed in a particular farmer's inventory.</summary>
//     /// <param name="player">The farmer to get storages from.</param>
//     /// <param name="excluded">A list of storage contexts to exclude to prevent iterating over the same object.</param>
//     /// <returns>An enumerable of all held storages in the farmer's inventory.</returns>
//     public static IEnumerable<Storage> FromPlayer(Farmer player, ISet<object>? excluded = null)
//     {
//         excluded ??= new HashSet<object>();
//
//         foreach (var storage in IntegrationsManager.HorseOverhaul_FromPlayer(player, excluded))
//         {
//             yield return storage;
//         }
//     }
//
//     /// <summary>Checks if any known incompatibilities.</summary>
//     /// <param name="featureName">The feature to check.</param>
//     /// <param name="mods">The list of incompatible mods.</param>
//     /// <returns>Returns true if there is an incompatibility.</returns>
//     public static bool TestConflicts(string featureName, [NotNullWhen(true)] out List<IModInfo?>? mods)
//     {
//         if (!IntegrationsManager.Incompatibilities.TryGetValue(featureName, out var modIds))
//         {
//             mods = null;
//             return false;
//         }
//
//         mods = modIds.Where(IntegrationsManager.instance.modRegistry.IsLoaded)
//             .Select(IntegrationsManager.instance.modRegistry.Get).ToList();
//
//         return mods.Any();
//     }
//
//     /// <summary>Attempts to retrieve a storage based on a context object.</summary>
//     /// <param name="context">The context object.</param>
//     /// <param name="storage">The storage object.</param>
//     /// <returns>Returns true if a storage could be found for the context object.</returns>
//     public static bool TryGetOne(object? context, [NotNullWhen(true)] out Storage? storage)
//     {
//         if (IntegrationsManager.HorseOverhaul_TryGetOne(context, out storage))
//         {
//             return true;
//         }
//
//         storage = default;
//         return false;
//     }
//
//     private static IEnumerable<Storage> ExpandedFridge_FromLocation(GameLocation location, ISet<object> excluded)
//     {
//         if (!IntegrationsManager.instance.modRegistry.IsLoaded(IntegrationsManager.ExpandedFridgeId) || location is not
//                 FarmHouse
//                 {
//                     upgradeLevel: > 0
//                 })
//         {
//             yield break;
//         }
//
//         foreach (var (pos, obj) in location.Objects.Pairs)
//         {
//             if ((int)pos.Y == -300 && obj is Chest chest && obj.HasTypeBigCraftable() && obj.ParentSheetIndex == 216)
//             {
//                 excluded.Add(chest);
//             }
//         }
//     }
//
//     private static IEnumerable<Storage> HorseOverhaul_FromLocation(GameLocation location, ISet<object> excluded)
//     {
//         if (!IntegrationsManager.instance.modRegistry.IsLoaded(IntegrationsManager.HorseOverhaulId))
//         {
//             yield break;
//         }
//
//         var farm = Game1.getFarm();
//         foreach (var stable in farm.buildings.OfType<Stable>())
//         {
//             if (!stable.modData.TryGetValue($"{IntegrationsManager.HorseOverhaulId}/stableID", out var stableId)
//                 || !int.TryParse(stableId, out var x) || !farm.Objects.TryGetValue(new Vector2(x, 0), out var obj)
//                 || obj is not Chest chest
//                 || !chest.modData.ContainsKey($"{IntegrationsManager.HorseOverhaulId}/isSaddleBag"))
//             {
//                 continue;
//             }
//
//             var horse = Game1.player.mount;
//             if (horse?.HorseId == stable.HorseId && Game1.player.currentLocation.Equals(location))
//             {
//                 excluded.Add(chest);
//                 yield return new ChestStorage(chest, horse, Game1.player.Tile);
//             }
//
//             horse = stable.getStableHorse();
//             if (horse?.getOwner() != Game1.player || !horse.currentLocation.Equals(location))
//             {
//                 continue;
//             }
//
//             excluded.Add(chest);
//             yield return new ChestStorage(chest, horse, horse.Tile);
//         }
//     }
//
//     private static IEnumerable<Storage> HorseOverhaul_FromPlayer(Farmer player, ISet<object> excluded)
//     {
//         if (!IntegrationsManager.instance.modRegistry.IsLoaded(IntegrationsManager.HorseOverhaulId))
//         {
//             yield break;
//         }
//
//         if (player.mount is null)
//         {
//             yield break;
//         }
//
//         var farm = Game1.getFarm();
//         var stable = farm.buildings.OfType<Stable>().FirstOrDefault(stable => stable.HorseId == player.mount.HorseId);
//         if (stable is null
//             || !stable.modData.TryGetValue($"{IntegrationsManager.HorseOverhaulId}/stableID", out var stableId)
//             || !int.TryParse(stableId, out var x) || !farm.Objects.TryGetValue(new Vector2(x, 0), out var obj)
//             || obj is not Chest chest
//             || !chest.modData.ContainsKey($"{IntegrationsManager.HorseOverhaulId}/isSaddleBag"))
//         {
//             yield break;
//         }
//
//         excluded.Add(chest);
//         yield return new ChestStorage(chest, Game1.player, player.Tile);
//     }
//
//     private static bool HorseOverhaul_TryGetOne(object? context, [NotNullWhen(true)] out Storage? storage)
//     {
//         if (!IntegrationsManager.instance.modRegistry.IsLoaded(IntegrationsManager.HorseOverhaulId)
//             || context is not Chest chest
//             || !chest.modData.ContainsKey($"{IntegrationsManager.HorseOverhaulId}/isSaddleBag"))
//         {
//             storage = default;
//             return false;
//         }
//
//         var farm = Game1.getFarm();
//         foreach (var stable in farm.buildings.OfType<Stable>())
//         {
//             if (!stable.modData.TryGetValue($"{IntegrationsManager.HorseOverhaulId}/stableID", out var stableId)
//                 || !int.TryParse(stableId, out var x) || !farm.Objects.TryGetValue(new Vector2(x, 0), out var obj)
//                 || chest != obj)
//             {
//                 continue;
//             }
//
//             var horse = Game1.player.mount;
//             if (horse?.HorseId == stable.HorseId)
//             {
//                 storage = new ChestStorage(chest, Game1.player, horse.Tile);
//                 return true;
//             }
//
//             horse = stable.getStableHorse();
//             if (horse?.getOwner() != Game1.player)
//             {
//                 continue;
//             }
//
//             storage = new ChestStorage(chest, horse, horse.Tile);
//             return true;
//         }
//
//         storage = default;
//         return false;
//     }
//
//     private void OnStorageTypeRequested(object? sender, IStorageTypeRequestedEventArgs e)
//     {
//         switch (e.Context)
//         {
//             case Chest chest when this.modRegistry.IsLoaded(IntegrationsManager.HorseOverhaulId)
//                                   && chest.modData.ContainsKey($"{IntegrationsManager.HorseOverhaulId}/isSaddleBag")
//                                   && this.config.VanillaStorages.TryGetValue("SaddleBag", out var saddleBagData):
//                 e.Load(saddleBagData, -1);
//                 return;
//         }
//     }
// }
//
//

