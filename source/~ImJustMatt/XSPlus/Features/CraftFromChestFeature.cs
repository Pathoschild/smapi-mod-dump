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
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection.Emit;
    using HarmonyLib;
    using Microsoft.Xna.Framework;
    using Services;
    using StardewModdingAPI;
    using StardewModdingAPI.Events;
    using StardewModdingAPI.Utilities;
    using StardewValley;
    using StardewValley.Menus;
    using StardewValley.Network;
    using StardewValley.Objects;

    /// <inheritdoc />
    internal class CraftFromChestFeature : FeatureWithParam<string>
    {
        private readonly IGameLoopEvents _gameLoopEvents;
        private readonly IInputHelper _inputHelper;
        private readonly ModConfigService _modConfigService;
        private readonly PerScreen<List<Chest>?> _cachedEnabledChests = new();
        private readonly PerScreen<IList<Chest>?> _cachedPlayerChests = new();
        private readonly PerScreen<IList<Chest>?> _cachedGameChests = new();
        private readonly PerScreen<MultipleChestCraftingPage> _multipleChestCraftingPage = new();

        /// <summary>Initializes a new instance of the <see cref="CraftFromChestFeature"/> class.</summary>
        /// <param name="inputHelper">API for changing state of input.</param>
        /// <param name="gameLoopEvents">Events linked to the game's update loop.</param>
        /// <param name="modConfigService">Service to handle read/write to ModConfig.</param>
        public CraftFromChestFeature(IInputHelper inputHelper, IGameLoopEvents gameLoopEvents, ModConfigService modConfigService)
            : base("CraftFromChest")
        {
            this._inputHelper = inputHelper;
            this._gameLoopEvents = gameLoopEvents;
            this._modConfigService = modConfigService;
        }

        private List<Chest> EnabledChests
        {
            get
            {
                this._cachedPlayerChests.Value ??= Game1.player.Items.OfType<Chest>().Where(this.IsEnabledForItem).ToList();
                this._cachedGameChests.Value ??= XSPlus.AccessibleChests.Where(this.IsEnabledForItem).ToList();
                return this._cachedEnabledChests.Value ??= this._cachedPlayerChests.Value.Union(this._cachedGameChests.Value).ToList();
            }
        }

        /// <inheritdoc/>
        public override void Activate(IModEvents modEvents, Harmony harmony)
        {
            // Events
            modEvents.Player.InventoryChanged += this.OnInventoryChanged;
            modEvents.Player.Warped += this.OnWarped;
            modEvents.Input.ButtonsChanged += this.OnButtonsChanged;

            // Patches
            harmony.Patch(
                original: AccessTools.Method(typeof(CraftingPage), "getContainerContents"),
                postfix: new HarmonyMethod(typeof(CraftFromChestFeature), nameof(CraftFromChestFeature.CraftingPage_getContainerContents_postfix)));
            harmony.Patch(
                original: AccessTools.Method(typeof(CraftingRecipe), nameof(CraftingRecipe.consumeIngredients)),
                transpiler: new HarmonyMethod(typeof(CraftFromChestFeature), nameof(CraftFromChestFeature.CraftingRecipe_consumeIngredients_transpiler)));
        }

        /// <inheritdoc/>
        public override void Deactivate(IModEvents modEvents, Harmony harmony)
        {
            // Events
            modEvents.Player.InventoryChanged -= this.OnInventoryChanged;
            modEvents.Player.Warped -= this.OnWarped;
            modEvents.Input.ButtonsChanged -= this.OnButtonsChanged;

            // Patches
            harmony.Unpatch(
                original: AccessTools.Method(typeof(CraftingPage), "getContainerContents"),
                patch: AccessTools.Method(typeof(CraftFromChestFeature), nameof(CraftFromChestFeature.CraftingPage_getContainerContents_postfix)));
            harmony.Unpatch(
                original: AccessTools.Method(typeof(CraftingRecipe), nameof(CraftingRecipe.consumeIngredients)),
                patch: AccessTools.Method(typeof(CraftFromChestFeature), nameof(CraftFromChestFeature.CraftingRecipe_consumeIngredients_transpiler)));
        }

        /// <inheritdoc/>
        [SuppressMessage("ReSharper", "HeapView.BoxingAllocation", Justification = "Required for enumerating this collection.")]
        protected internal override bool IsEnabledForItem(Item item)
        {
            if (!base.IsEnabledForItem(item) || item is not Chest chest || !chest.playerChest.Value || !this.TryGetValueForItem(item, out string range))
            {
                return false;
            }

            return range switch
            {
                "Inventory" => Game1.player.Items.IndexOf(item) != -1,
                "Location" => Game1.currentLocation.Objects.Values.Contains(item),
                "World" => true,
                _ => false,
            };
        }

        /// <inheritdoc/>
        protected override bool TryGetValueForItem(Item item, out string param)
        {
            if (base.TryGetValueForItem(item, out param))
            {
                return true;
            }

            param = this._modConfigService.ModConfig.CraftingRange;
            return !string.IsNullOrWhiteSpace(param);
        }

        [SuppressMessage("ReSharper", "SA1313", Justification = "Naming is determined by Harmony.")]
        [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Naming is determined by Harmony.")]
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "Type is determined by Harmony.")]
        private static void CraftingPage_getContainerContents_postfix(CraftingPage __instance, ref IList<Item> __result)
        {
            if (__instance._materialContainers is null)
            {
                return;
            }

            __result.Clear();
            var items = new List<Item>();
            foreach (Chest chest in __instance._materialContainers)
            {
                items.AddRange(chest.GetItemsForPlayer(Game1.player.UniqueMultiplayerID));
            }

            __result = items;
        }

        [SuppressMessage("ReSharper", "SA1313", Justification = "Naming is determined by Harmony.")]
        [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Naming is determined by Harmony.")]
        private static IEnumerable<CodeInstruction> CraftingRecipe_consumeIngredients_transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldfld && instruction.operand.Equals(AccessTools.Field(typeof(Chest), nameof(Chest.items))))
                {
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Property(typeof(Game1), nameof(Game1.player)).GetGetMethod());
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Property(typeof(Farmer), nameof(Farmer.UniqueMultiplayerID)).GetGetMethod());
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(Chest), nameof(Chest.GetItemsForPlayer)));
                }
                else
                {
                    yield return instruction;
                }
            }
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (this._multipleChestCraftingPage.Value.Exited)
            {
                this._gameLoopEvents.UpdateTicked -= this.OnUpdateTicked;
                return;
            }

            this._multipleChestCraftingPage.Value.UpdateChests();
        }

        private void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
        {
            if (!e.IsLocalPlayer)
            {
                return;
            }

            this._cachedPlayerChests.Value = null;
            this._cachedEnabledChests.Value = null;
        }

        private void OnWarped(object sender, WarpedEventArgs e)
        {
            this._cachedGameChests.Value = null;
            this._cachedEnabledChests.Value = null;
        }

        /// <summary>Open crafting menu for all chests in inventory.</summary>
        private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            if (!Context.IsPlayerFree || !this._modConfigService.ModConfig.OpenCrafting.JustPressed() || !this.EnabledChests.Any())
            {
                return;
            }

            this._multipleChestCraftingPage.Value = new MultipleChestCraftingPage(this.EnabledChests);
            this._gameLoopEvents.UpdateTicked += this.OnUpdateTicked;
            this._inputHelper.SuppressActiveKeybinds(this._modConfigService.ModConfig.OpenCrafting);
        }

        private class MultipleChestCraftingPage
        {
            private const int TimeOut = 100;
            private readonly List<Chest> _chests;
            private readonly MultipleMutexRequest _multipleMutexRequest;
            private int _timeOut = TimeOut;

            public MultipleChestCraftingPage(List<Chest> chests)
            {
                this._chests = chests.Where(chest => !chest.mutex.IsLocked()).ToList();
                List<NetMutex> mutexes = this._chests.Select(chest => chest.mutex).ToList();
                this._multipleMutexRequest = new MultipleMutexRequest(
                    mutexes: mutexes,
                    success_callback: this.SuccessCallback,
                    failure_callback: this.FailureCallback);
            }

            public bool Exited { get; private set; }

            public void UpdateChests()
            {
                if (--this._timeOut <= 0)
                {
                    return;
                }

                foreach (Chest chest in this._chests)
                {
                    chest.mutex.Update(Game1.getOnlineFarmers());
                }
            }

            private void SuccessCallback()
            {
                int width = 800 + (IClickableMenu.borderWidth * 2);
                int height = 600 + (IClickableMenu.borderWidth * 2);
                Vector2 pos = Utility.getTopLeftPositionForCenteringOnScreen(width, height);
                Game1.activeClickableMenu = new CraftingPage((int)pos.X, (int)pos.Y, width, height, false, true, this._chests)
                {
                    exitFunction = this.ExitFunction,
                };
            }

            private void FailureCallback()
            {
                Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:Workbench_Chest_Warning"));
                this.Exited = true;
            }

            private void ExitFunction()
            {
                this._multipleMutexRequest.ReleaseLocks();
                this.Exited = true;
            }
        }
    }
}