/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dunc4nNT/StardewMods
**
*************************************************/

using HarmonyLib;
using NeverToxic.StardewMods.YetAnotherFishingMod.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Objects;
using StardewValley.Menus;
using StardewValley.Tools;
using System.Collections.Generic;
using Patches = NeverToxic.StardewMods.YetAnotherFishingMod.Framework.Patches;
using SObject = StardewValley.Object;

namespace NeverToxic.StardewMods.YetAnotherFishingMod
{
    internal sealed class ModEntry : Mod
    {
        private ModConfig Config { get; set; }

        private ModConfigKeys Keys => this.Config.Keys;

        private FishHelper FishHelper { get; set; }

        private readonly List<string> _baitList = [];

        private readonly List<string> _tackleList = [];

        public override void Entry(IModHelper helper)
        {
            I18n.Init(helper.Translation);

            this.Config = helper.ReadConfig<ModConfig>();
            Harmony harmony = new(this.ModManifest.UniqueID);
            Patches.Initialise(harmony, this.Monitor, () => this.Config, this.Helper.Reflection);
            this.FishHelper = new(() => this.Config, this.Monitor);

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
            helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            foreach (KeyValuePair<string, ObjectData> item in Game1.objectData)
            {
                if (item.Value.Category == SObject.baitCategory)
                    this._baitList.Add(ItemRegistry.QualifyItemId(item.Key));
                else if (item.Value.Category == SObject.tackleCategory)
                    this._tackleList.Add(ItemRegistry.QualifyItemId(item.Key));
            }

            new GenericModConfigMenu(this.Helper.ModRegistry, this.ModManifest, this.Monitor, () => this.Config, () => this.Config = new ModConfig(), () => this.Helper.WriteConfig(this.Config), this._baitList, this._tackleList).Register();
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (Game1.player.CurrentTool is FishingRod fishingRod)
                this.FishHelper.OnFishingRodEquipped(fishingRod);
            else if (Game1.player.CurrentTool is not FishingRod)
                this.FishHelper.OnFishingRodNotEquipped();

            if (this.FishHelper.IsInFishingMiniGame.Value)
                this.FishHelper.ApplyFishingMiniGameBuffs();
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is BobberBar bobberBar)
                this.FishHelper.OnFishingMiniGameStart(bobberBar);
            else if (e.OldMenu is BobberBar)
                this.FishHelper.OnFishingMiniGameEnd();

            if (e.NewMenu is ItemGrabMenu itemGrabMenu && itemGrabMenu.source == ItemGrabMenu.source_fishingChest)
                this.FishHelper.OnTreasureMenuOpen(itemGrabMenu);
        }

        private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (this.Keys.ReloadConfig.JustPressed())
                this.ReloadConfig();
        }

        private void ReloadConfig()
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            this.Monitor.Log(I18n.Message_ConfigReloaded(), LogLevel.Info);
        }
    }
}
