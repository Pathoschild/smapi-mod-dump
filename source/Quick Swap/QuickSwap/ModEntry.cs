/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AllanWang/Stardew-Mods
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;


namespace QuickSwap
{

    public class ModConfig
    {
        public SButton ActivationKey { get; set; } = SButton.Q;
    }

    /// https://github.com/spacechase0/StardewValleyMods/tree/develop/GenericModConfigMenu
    public interface IGenericModConfigMenuApi
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);
        void AddSectionTitle(IManifest mod, Func<string> text, Func<string>? tooltip = null);
        void AddKeybind(IManifest mod, Func<SButton> getValue, Action<SButton> setValue, Func<string> name, Func<string>? tooltip = null, string? fieldId = null);
    }


    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            config = Helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        private ModConfig config = new ModConfig();

        /**
          * Last known tool held
          */
        private Item? currTool;

        /**
          * Prev tool held
          */
        private Item? prevTool;



        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            if (!Context.IsPlayerFree) return;

            CheckPlayerActiveTool();
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            if (!Context.IsPlayerFree) return;
            // Not correct key; ignore
            if (e.Button != this.config.ActivationKey) return;

            // Swapping while animating freezes game until another swap occurs
            if (Game1.player.FarmerSprite.isOnToolAnimation()) return;
            var fishingRod = Game1.player.CurrentTool as FishingRod;
            if (fishingRod?.inUse() == true)
            {
                // Swapping mid use makes the fishing rod sounds persistent
                return;
            }

            QuickSwap();
        }


        /// <summary>Maintain backstack (size 2) of last used tools</summary>
        private void CheckPlayerActiveTool()
        {
            Item? tool = Game1.player.CurrentTool;
            if (tool == null) return;
            if (tool == currTool) return;
            prevTool = currTool;
            currTool = tool;
            // this.Monitor.Log($"QuickSwap tools {currTool?.Name} prev {prevTool?.Name}");
        }

        private void QuickSwap()
        {
            Tool? tool = Game1.player.CurrentTool;
            // If current item is still the same, swap to previous and reorder
            // Otherwise, we've swapped to a non tool, so go back to the last (curr) tool and keep ordering.
            if (tool == currTool)
            {
                var toSwap = prevTool;
                prevTool = currTool;
                currTool = toSwap;
                SelectTool(toSwap);
            }
            else
            {
                SelectTool(currTool);
            }
        }

        private void SelectTool(Item? item)
        {
            if (item == null) return;
            int i = Game1.player.getIndexOfInventoryItem(item);
            if (i == -1) return;
            Game1.player.CurrentToolIndex = i;
        }

        /// Mod Config
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.config)
            );

            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Quick Swap",
                tooltip: () => "Swap between last used tools."
            );

            configMenu.AddKeybind(
                mod: this.ModManifest,
                name: () => "Activation Key",
                getValue: () => this.config.ActivationKey,
                setValue: value => this.config.ActivationKey = value
            );
        }

    }
}