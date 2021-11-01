/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace XSPlus.Features
{
    using System.Diagnostics.CodeAnalysis;
    using Common.Services;
    using CommonHarmony.Enums;
    using CommonHarmony.Services;
    using HarmonyLib;
    using Services;
    using StardewValley;
    using StardewValley.Objects;

    /// <inheritdoc />
    internal class CapacityFeature : FeatureWithParam<int>
    {
        private static CapacityFeature Instance;
        private ExpandedMenuFeature _expandedMenu;
        private HarmonyService _harmony;
        private ModConfigService _modConfig;

        private CapacityFeature(ServiceManager serviceManager)
            : base("Capacity", serviceManager)
        {
            CapacityFeature.Instance ??= this;

            // Dependencies
            this.AddDependency<ModConfigService>(service => this._modConfig = service as ModConfigService);
            this.AddDependency<ExpandedMenuFeature>(service => this._expandedMenu = service as ExpandedMenuFeature);
            this.AddDependency<HarmonyService>(
                service =>
                {
                    // Init
                    this._harmony = service as HarmonyService;

                    // Patches
                    this._harmony?.AddPatch(
                        this.ServiceName,
                        AccessTools.Method(typeof(Chest), nameof(Chest.GetActualCapacity)),
                        typeof(CapacityFeature),
                        nameof(CapacityFeature.Chest_GetActualCapacity_postfix),
                        PatchType.Postfix);
                });
        }

        /// <inheritdoc />
        public override void Activate()
        {
            // Patches
            this._harmony.ApplyPatches(this.ServiceName);
        }

        /// <inheritdoc />
        public override void Deactivate()
        {
            // Patches
            this._harmony.UnapplyPatches(this.ServiceName);
        }

        internal override bool IsEnabledForItem(Item item)
        {
            return base.IsEnabledForItem(item) && this._expandedMenu.IsEnabledForItem(item);
        }

        /// <inheritdoc />
        internal override bool TryGetValueForItem(Item item, out int param)
        {
            if (base.TryGetValueForItem(item, out param))
            {
                return true;
            }

            return (param = this._modConfig.ModConfig.Capacity) != 0;
        }

        [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Naming is determined by Harmony.")]
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "Type is determined by Harmony.")]
        private static void Chest_GetActualCapacity_postfix(Chest __instance, ref int __result)
        {
            if (!CapacityFeature.Instance.IsEnabledForItem(__instance) || !CapacityFeature.Instance.TryGetValueForItem(__instance, out var capacity))
            {
                return;
            }

            __result = capacity switch
            {
                -1 => int.MaxValue,
                > 0 => capacity,
                _ => __result,
            };
        }
    }
}