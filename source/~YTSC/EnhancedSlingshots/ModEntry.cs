/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/YTSC/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using StardewValley.Menus;
using System.Reflection;
using StardewValley.Locations;
using Microsoft.Xna.Framework.Graphics;
using EnhancedSlingshots.Framework.Patch;
using Microsoft.Xna.Framework;
using System;
using EnhancedSlingshots.Framework;
using System.Collections.Generic;
using EnhancedSlingshots.Framework.Integrations;

namespace EnhancedSlingshots
{
    public class ModEntry : Mod
    {
        internal ITranslationHelper i18n => Helper.Translation;
        internal Config config;
        private Harmony harmony;
        private Texture2D InfinitySlingTexture;
        private string EnchantmentsKey => $"{ModManifest.UniqueID}_ToolEnchantments"; 
        public static ModEntry Instance;

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            harmony = new Harmony(ModManifest.UniqueID);
            config = Helper.ReadConfig<Config>();
            InfinitySlingTexture = helper.ModContent.Load<Texture2D>("assets/InfinitySlingshot.png");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            InitializeMonitors();

            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            Helper.Events.Display.MenuChanged += OnMenuChanged;
            Helper.Events.GameLoop.Saving += OnSaving;
            Helper.Events.GameLoop.Saved += OnSaved;
            Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            Helper.Events.Content.AssetRequested += OnAssetRequested;
        }
        public void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
            {
                Monitor.Log($"spacechase0.GenericModConfigMenu not found, integration will not be applied", LogLevel.Debug);
                return;
            }
            ApplyConfigOptions(configMenu);              
        }
        public void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (Game1.currentLocation is AdventureGuild && e?.NewMenu is ShopMenu shopMenu)
            {
                if (config.EnableGalaxySligshot && Game1.player.hasSkullKey)
                {
                    Tool slingshot = new Slingshot(Slingshot.galaxySlingshot);
                    shopMenu.itemPriceAndStock.Add(slingshot, new int[2] { config.GalaxySlingshotPrice, 1 });
                    shopMenu.forSale.Insert(0, slingshot);
                }
            }
        }
        public void OnSaved(object sender, SavedEventArgs e)
        {
            Utility.iterateAllItems(item =>
            {
                if (item is Slingshot sling)
                    LoadEnchantments(sling);                
            });
        }
        public void OnSaving(object sender, SavingEventArgs e)
        {
            Utility.iterateAllItems(item =>
            {
                if (item is Slingshot sling)                
                    SaveEnchantments(sling);  
            });
        }
        public void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {            
            Utility.iterateAllItems(item =>
            {
                if (item is Slingshot sling && sling.modData.ContainsKey(EnchantmentsKey) && !String.IsNullOrEmpty(sling.modData[EnchantmentsKey]))
                    LoadEnchantments(sling);
            });
        }
        private void LoadEnchantments(Slingshot sling)
        {
            string enchantments = sling.modData[EnchantmentsKey];
            Monitor.Log($"Loading Enchantments from: {sling.Name} - {enchantments}", LogLevel.Debug);
            foreach (string enchantmentKey in enchantments.Split(','))
            {
                if (Enum.TryParse(typeof(EnchantmentKey), enchantmentKey, out object key) && (EnchantmentKey)key != EnchantmentKey.Invalid)
                {
                    Monitor.Log($"Loading Enchantment: {key}", LogLevel.Debug);
                    SlingshotEnchantment.Enchantments.TryGetValue((EnchantmentKey)key, out BaseEnchantment enchantment);
                    if (enchantment != null)
                    {
                        sling.enchantments.Add(enchantment);
                        enchantment.ApplyTo(sling, sling.getLastFarmerToUse());
                    }
                }
            }
        }
        private void SaveEnchantments(Slingshot sling)
        {
            Monitor.Log($"Saving Enchantments from: {sling.Name}", LogLevel.Debug);
            List<EnchantmentKey> enchantsKeys = new();
            for (int x = sling.enchantments.Count - 1; x >= 0; x--)
            {
                var key = SlingshotEnchantment.GetKeyByEnchantmentType(sling.enchantments[x]);
                Monitor.Log($"Saving Enchantment: {key}", LogLevel.Debug);
                if (key != EnchantmentKey.Invalid)
                    enchantsKeys.Add(key);

                sling.enchantments[x].UnapplyTo(sling, sling.getLastFarmerToUse());
                sling.enchantments.RemoveAt(x);
            }
            sling.modData[EnchantmentsKey] = string.Join(',', enchantsKeys);
            Monitor.Log($"Enchantments Saved: {sling.modData[EnchantmentsKey]}", LogLevel.Debug);
        }
        public void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Weapons"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<int, string>().Data;
                    if (!data.ContainsKey(config.InfinitySlingshotId))
                        data[config.InfinitySlingshotId] = GetInfinitySlingshotData();
                    else 
                        throw new ArgumentException($"The ID for the Infinity Slinghot ({config.InfinitySlingshotId}) is beeing used! Please choose another ID.");
                });
            }

            if (e.Name.IsEquivalentTo("TileSheets/weapons"))
            {
                e.Edit(asset =>
                {
                    var weapons = asset.AsImage();
                    var area = GetTargetArea(config.InfinitySlingshotId);
                    if (weapons.Data.Height < area.Y + 16)
                        weapons.ExtendImage(minWidth: weapons.Data.Width, minHeight: area.Y + 16);

                    weapons.PatchImage(InfinitySlingTexture, targetArea: area);
                });
            }
        }
        private void ApplyConfigOptions(IGenericModConfigMenuApi configMenu)
        {
            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => config = Helper.ReadConfig<Config>(),
                save: () => Helper.WriteConfig(config)
            );

            configMenu.AddSectionTitle(
                  mod: ModManifest,
                  text: () => "Slingshot options"
                );

            // add some config options
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Enable Galaxy Sligshot",
                tooltip: () => "If the Galaxy Slingshot is enabled to sell at the Adventure Guild Shop",
                getValue: () => config.EnableGalaxySligshot,
                setValue: value => config.EnableGalaxySligshot = value
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Galaxy Slingshot Price",
                tooltip: () => "The price of the Galaxy Slingshot being selled on Adventure Guild Shop",
                getValue: () => config.GalaxySlingshotPrice,
                setValue: value => config.GalaxySlingshotPrice = value
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Infinity Slingshot Id",
                tooltip: () => "The ID for the Infinity Slingshot (if changed restart the game)",
                getValue: () => config.InfinitySlingshotId,
                setValue: value => config.InfinitySlingshotId = value
            );

            configMenu.AddSectionTitle(
                 mod: ModManifest,
                 text: () => "Enchantment options"
               );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Automated - Auto Fire Rate",
                tooltip: () => "The frequency in seconds which a new projectile is shoot with the Automated enchantment",
                min: 0,
                getValue: () => config.SlingshotAutoFireRate,
                setValue: value => config.SlingshotAutoFireRate = value
            );

            configMenu.AddNumberOption(
               mod: ModManifest,
               name: () => "Bug Killer - Extra Damage",
               tooltip: () => "The extra damage of Bug Killer enchantment",
               min: 1,
               getValue: () => config.BugKillerEnchantment_Damage,
               setValue: value => config.BugKillerEnchantment_Damage = value
           );

            configMenu.AddNumberOption(
               mod: ModManifest,
               name: () => "Precise - Extra Damage",
               tooltip: () => "The extra damage of Precise enchantment",
               min: 1,
               getValue: () => config.PreciseEnchantment_Damage,
               setValue: value => config.PreciseEnchantment_Damage = value
           );

            configMenu.AddNumberOption(
               mod: ModManifest,
               name: () => "Hunter - Extra Drop Amount",
               tooltip: () => "The amount of monsters extra drops of Hunter enchantment",
               min: 1,
               getValue: () => config.HunterEnchantment_ExtraDropsAmount,
               setValue: value => config.HunterEnchantment_ExtraDropsAmount = value
           );

            configMenu.AddNumberOption(
               mod: ModManifest,
               name: () => "Miner - Extra Drop Amount",
               tooltip: () => "The amount of ore extra drops of Miner enchantment",
               min: 1,
               getValue: () => config.MinerEnchantment_ExtraDropsAmount,
               setValue: value => config.MinerEnchantment_ExtraDropsAmount = value
           );

            configMenu.AddNumberOption(
              mod: ModManifest,
              name: () => "Swift - Times Faster",
              tooltip: () => "The projectile extra speed of Swift enchantment",
              min: 1,
              getValue: () => config.SwiftEnchantment_TimesFaster,
              setValue: value => config.SwiftEnchantment_TimesFaster = value
          );
            configMenu.AddNumberOption(
               mod: ModManifest,
               name: () => "Preserving - Preserve Chance",
               tooltip: () => "The chance of not using ammo of Preserving enchantment",
               min: 0,
               max: 1,
               getValue: () => config.PreservingEnchantment_PreserveChance,
               setValue: value => config.PreservingEnchantment_PreserveChance = value
           );
           
            configMenu.AddNumberOption(
               mod: ModManifest,
               name: () => "Vampiric - Recovery Chance",
               tooltip: () => "The chance of health recover on monster kill of Vampiric enchantment",
               min: 0,
               max: 1,
               getValue: () => config.VampiricEnchantment_RecoveryChance,
               setValue: value => config.VampiricEnchantment_RecoveryChance = value
           );

            configMenu.AddParagraph(
                mod: ModManifest,
                text: () => "Since Generic Mod Config Menu doesn't support lists, those options can only be modified on the config.json:\n" +
                            "- Magnetic Enchantment Affected Stones\n" +
                            "- Items That Can Be Used As Ammo"
           );
        }
        private Rectangle GetTargetArea(int id)
        {
            return new ((id % 8) * 16, (id / 8) * 16, InfinitySlingTexture.Width, InfinitySlingTexture.Height);
        }
        private string GetInfinitySlingshotData()
        {
            return $"Infinity Slingshot/{i18n.Get("InfinitySlingshotDescription")}/1/3/1/308/0/0/4/-1/-1/0/.02/3/{i18n.Get("InfinitySlingshotName")}";
        }
        private void InitializeMonitors()
        {
            BaseEnchantmentPatchs.Initialize(Monitor);           
            GameLocationPatchs.Initialize(Monitor);
            ProjectilePatchs.Initialize(Monitor);
            SlingshotPatchs.Initialize(Monitor);
            ToolPatchs.Initialize(Monitor);
        }
    }
}
