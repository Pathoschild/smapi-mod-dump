/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace BetterChests.Services
{
    using System.Diagnostics.CodeAnalysis;
    using Common.Services;
    using CommonHarmony.Enums;
    using CommonHarmony.Services;
    using HarmonyLib;
    using StardewValley.Objects;

    internal class ResizeChestService : BaseService, IFeatureService
    {
        private static ManagedChestService ManagedChestService;
        private HarmonyService _harmony;

        private ResizeChestService(ServiceManager serviceManager)
            : base("ResizeChest")
        {
            // Dependencies
            this.AddDependency<ManagedChestService>(service => ResizeChestService.ManagedChestService = service as ManagedChestService);
            this.AddDependency<HarmonyService>(
                service =>
                {
                    // Init
                    this._harmony = service as HarmonyService;

                    // Patches
                    this._harmony?.AddPatch(
                        this.ServiceName,
                        AccessTools.Method(typeof(Chest), nameof(Chest.GetActualCapacity)),
                        typeof(ResizeChestService),
                        nameof(ResizeChestService.Chest_GetActualCapacity_postfix),
                        PatchType.Postfix);
                });
        }

        /// <inheritdoc />
        public void Activate()
        {
            // Patches
            this._harmony.ApplyPatches(this.ServiceName);
        }

        /// <inheritdoc />
        public void Deactivate()
        {
            // Patches
            this._harmony.UnapplyPatches(this.ServiceName);
        }

        [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Naming is determined by Harmony.")]
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "Type is determined by Harmony.")]
        private static void Chest_GetActualCapacity_postfix(Chest __instance, ref int __result)
        {
            if (!ResizeChestService.ManagedChestService.TryGetChestConfig(__instance, out var chestConfig) || chestConfig.Capacity == 0)
            {
                return;
            }

            __result = chestConfig.Capacity switch
            {
                -1 => int.MaxValue,
                > 0 => chestConfig.Capacity,
                _ => __result,
            };
        }
    }
}