/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dunc4nNT/StardewMods
**
*************************************************/

using NeverToxic.StardewMods.Common;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Enchantments;
using StardewValley.Menus;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeverToxic.StardewMods.YetAnotherFishingMod.Framework
{
    internal class FishHelper(Func<ModConfig> config, IMonitor monitor)
    {
        private readonly PerScreen<BobberBar> _bobberBar = new();
        private readonly PerScreen<SFishingRod> _fishingRod = new();
        public readonly PerScreen<bool> IsInFishingMiniGame = new();

        private void ApplyFishingRodBuffs()
        {
            ModConfig config_ = config();
            SFishingRod fishingRod = this._fishingRod.Value;

            if (config_.FishQuality != Quality.Any && fishingRod.Instance.fishSize > 0)
                fishingRod.Instance.fishQuality = (int)config_.FishQuality;

            if (config_.InfiniteBait)
                fishingRod.InfiniteBait();

            if (config_.InfiniteTackle)
                fishingRod.InfiniteTackle();

            if (config_.AlwaysMaxCastingPower)
                fishingRod.Instance.castingPower = 1.01f;

            if (config_.InstantBite)
                fishingRod.InstantBite();

            if (config_.AutoHook)
                fishingRod.AutoHook();

            if (config_.AlwaysCatchDouble)
                fishingRod.Instance.caughtDoubleFish = true;
        }

        public void OnTreasureMenuOpen(ItemGrabMenu itemGrabMenu)
        {
            ModConfig config_ = config();

            if (config_.AutoLootTreasure)
            {
                IList<Item> actualInventory = itemGrabMenu.ItemsToGrabMenu.actualInventory;
                for (int i = actualInventory.Count - 1; i >= 0; i--)
                    if (Game1.player.addItemToInventoryBool(actualInventory.ElementAt(i)))
                        actualInventory.RemoveAt(i);

                if (actualInventory.Count == 0)
                    itemGrabMenu.exitThisMenu();
                else
                    Notifier.DisplayHudNotification(I18n.Message_InventoryFull(), logLevel: LogLevel.Warn);
            }
        }

        public void ApplyFishingMiniGameBuffs()
        {
            BobberBar bobberBar = this._bobberBar.Value;

            if (bobberBar is null)
                return;

            if (config().AlwaysPerfect)
                bobberBar.perfect = true;
        }

        private void CreateFishingRod(FishingRod fishingRod)
        {
            ModConfig config_ = config();
            this._fishingRod.Value = new(fishingRod);

            if (config_.OverrideAttachmentLimit)
                this._fishingRod.Value.Instance.AttachmentSlotsCount = 2;

            if (config_.SpawnBaitWhenEquipped)
                this._fishingRod.Value.SpawnBait(config_.SpawnWhichBait);
            if (config_.SpawnTackleWhenEquipped)
                this._fishingRod.Value.SpawnTackle(config_.SpawnWhichTackle);

            if (config_.DoAddEnchantments)
            {
                if (!fishingRod.hasEnchantmentOfType<AutoHookEnchantment>() && (config_.AddAllEnchantments || config_.AddAutoHookEnchantment))
                    this._fishingRod.Value.AddEnchantment(new AutoHookEnchantment());
                if (!fishingRod.hasEnchantmentOfType<EfficientToolEnchantment>() && (config_.AddAllEnchantments || config_.AddEfficientToolEnchantment))
                    this._fishingRod.Value.AddEnchantment(new EfficientToolEnchantment());
                if (!fishingRod.hasEnchantmentOfType<MasterEnchantment>() && (config_.AddAllEnchantments || config_.AddMasterEnchantment))
                    this._fishingRod.Value.AddEnchantment(new MasterEnchantment());
                if (!fishingRod.hasEnchantmentOfType<PreservingEnchantment>() && (config_.AddAllEnchantments || config_.AddPreservingEnchantment))
                    this._fishingRod.Value.AddEnchantment(new PreservingEnchantment());
            }
        }

        public void OnFishingRodEquipped(FishingRod fishingRod)
        {
            if (this._fishingRod.Value is null)
                this.CreateFishingRod(fishingRod);
            else if (this._fishingRod.Value.Instance != fishingRod)
            {
                this.OnFishingRodNotEquipped();
                this.CreateFishingRod(fishingRod);
            }

            this.ApplyFishingRodBuffs();
        }

        public void OnFishingRodNotEquipped()
        {
            SFishingRod fishingRod = this._fishingRod.Value;

            if (fishingRod is not null)
            {
                ModConfig config_ = config();

                if (config_.ResetAttachmentsWhenNotEquipped)
                    fishingRod.ResetAttachments();

                if (config_.ResetEnchantmentsWhenNotEquipped)
                    fishingRod.ResetEnchantments();

                this._fishingRod.Value = null;
            }
        }

        public void OnFishingMiniGameStart(BobberBar bobberBar)
        {
            this.IsInFishingMiniGame.Value = true;
            this._bobberBar.Value = bobberBar;

            this.ApplyBobberBarBuffs();
        }

        private void ApplyBobberBarBuffs()
        {
            BobberBar bobberBar = this._bobberBar.Value;
            ModConfig config_ = config();

            bobberBar.difficulty *= config_.DifficultyMultiplier;

            if (bobberBar.fishQuality < (int)config_.MinimumFishQuality)
                bobberBar.fishQuality = (int)config_.MinimumFishQuality;

            if (config_.TreasureAppearence is TreasureAppearanceSettings.Never)
            {
                bobberBar.treasure = false;
                bobberBar.treasureCaught = false;
            }
            else if ((config_.InstantCatchTreasure && bobberBar.treasure && config_.TreasureAppearence is TreasureAppearanceSettings.Vanilla) || config_.TreasureAppearence is TreasureAppearanceSettings.Always)
                bobberBar.treasureCaught = true;

            if (config_.InstantCatchFish)
            {
                this._fishingRod.Value.Instance.pullFishFromWater(bobberBar.whichFish, bobberBar.fishSize, bobberBar.fishQuality, (int)bobberBar.difficulty, bobberBar.treasureCaught, bobberBar.perfect, bobberBar.fromFishPond, bobberBar.setFlagOnCatch, bobberBar.bossFish, this._fishingRod.Value.Instance.caughtDoubleFish);
                if (Game1.activeClickableMenu is BobberBar)
                    Game1.exitActiveMenu();
            }
        }

        public void OnFishingMiniGameEnd()
        {
            this.IsInFishingMiniGame.Value = false;
            this._bobberBar.Value = null;
        }
    }
}
