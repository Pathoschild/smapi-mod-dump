/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hikari-BS/StardewMods
**
*************************************************/

using GenericModConfigMenu;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Reflection;

namespace FixedWeaponsDamage
{
    /// <summary>The mod entry point.</summary>
    internal class ModEntry : Mod
    {
        private ModConfig config;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<ModConfig>();

            BeginHarmonyPatch();

            helper.Events.Content.AssetRequested += Content_AssetRequested;
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            helper.Events.Player.InventoryChanged += Player_InventoryChanged;
            helper.Events.Content.LocaleChanged += Content_LocaleChanged;
        }

        private void Player_InventoryChanged(object sender, InventoryChangedEventArgs e)
        {
            if (!config.EnableMeleeFixedDamage) return;

            foreach (Item item in e.Added)
            {
                Monitor.Log($"Looping check for Player_InventoryChanged()", LogLevel.Trace);
                if (item is not null and MeleeWeapon weapon)
                    if (weapon.minDamage != weapon.maxDamage) RefreshInventory();
            }
        }

        private void Content_AssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (!config.EnableMeleeFixedDamage) return;

            float multiplierFloat = config.PercentMeleeDamageMultiplier / 100f;
            string damageLabel = Helper.Translation.Get("ui.damage-label");

            if (e.NameWithoutLocale.IsEquivalentTo("Data/weapons"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<int, string>().Data;

                    foreach ((int itemID, string itemData) in data)
                    {
                        string[] fields = itemData.Split('/');
                        string finalDamageString = Math.Ceiling(int.Parse(fields[3]) * multiplierFloat).ToString();

                        fields[2] = finalDamageString;
                        fields[3] = finalDamageString;

                        data[itemID] = string.Join('/', fields);
                    }
                }, AssetEditPriority.Late);

                Monitor.Log($"Successfully edited melee weapons data with damage multiplier of {config.PercentMeleeDamageMultiplier}%.", LogLevel.Trace);
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Strings/UI"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, string>().Data;
                    data["ItemHover_Damage"] = damageLabel;
                }, AssetEditPriority.Late);
            }
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (config.EnableMeleeFixedDamage) RefreshInventory();
        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            InitializeConfigMenu();
        }

        private void Content_LocaleChanged(object sender, LocaleChangedEventArgs e)
        {
            // make sure cache properly invalidated after changing the game language
            _ = Helper.GameContent.InvalidateCache(asset => asset.Name.IsEquivalentTo("Data/weapons"));
            _ = Helper.GameContent.InvalidateCache(asset => asset.Name.IsEquivalentTo("Strings/UI"));
        }

        internal static void RefreshInventory()
        {
            // update weapons data
            for (int i = 0; i < Game1.player.Items.Count; i++)
            {
                if (Game1.player.Items[i] is not null and MeleeWeapon weapon)
                    Game1.player.Items[i] = new MeleeWeapon(weapon.InitialParentTileIndex);
            }
        }

        internal void BeginHarmonyPatch()
        {
            SlingshotPatches.Initialize(Monitor, Helper, config.PercentSlingshotDamageMultiplier / 100f);
            var harmony = new Harmony(ModManifest.UniqueID);

            // let's dive into madness >:)
            MethodInfo original = AccessTools.Method(typeof(Slingshot), nameof(Slingshot.PerformFire));

            harmony.Unpatch(original, HarmonyPatchType.Prefix);
            Monitor.Log($"Unpatched Slingshot.PerformFire()", LogLevel.Trace);

            if (config.EnableSlingshotFixedDamage)
            {
                harmony.Patch(
                    original,
                    prefix: new HarmonyMethod(typeof(SlingshotPatches), nameof(SlingshotPatches.PerformFire_Prefix))
                    );
                Monitor.Log($"Patched Slingshot.PerformFire()", LogLevel.Trace);
            }
        }

        private void InitializeConfigMenu()
        {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) return;

            configMenu.Register(
                mod: ModManifest,
                reset: () => config = new ModConfig(),
                save: () =>
                {
                    Helper.WriteConfig(config);

                    _ = Helper.GameContent.InvalidateCache(asset => asset.Name.IsEquivalentTo("Data/weapons"));
                    _ = Helper.GameContent.InvalidateCache(asset => asset.Name.IsEquivalentTo("Strings/UI"));

                    RefreshInventory(); BeginHarmonyPatch();
                });

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => "Melee Weapon"
                );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Enable for melee weapon",
                tooltip: () => "Whether fixed damage is applied for melee weapons.",
                getValue: () => config.EnableMeleeFixedDamage,
                setValue: value => config.EnableMeleeFixedDamage = value
                );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Melee damage multiplier%",
                tooltip: () => "Damage multiplier for melee weapons relative to vanilla max damage (in percentage).\n\nI.e. a multiplier of 50% means 0.5 * damage, you know the math ;).",
                getValue: () => config.PercentMeleeDamageMultiplier,
                setValue: value => config.PercentMeleeDamageMultiplier = value
                );

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => "Slingshot"
                );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Enable for slingshot",
                tooltip: () => "Whether fixed damage is applied for slingshot.",
                getValue: () => config.EnableSlingshotFixedDamage,
                setValue: value => config.EnableSlingshotFixedDamage = value
                );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Slingshot damage multiplier%",
                tooltip: () => "Damage multiplier for slingshot relative to vanilla max damage (in percentage).\n\nI.e. a multiplier of 50% means 0.5 * damage, you know the math ;).",
                getValue: () => config.PercentSlingshotDamageMultiplier,
                setValue: value => config.PercentSlingshotDamageMultiplier = value
                );
        }
    }
}
