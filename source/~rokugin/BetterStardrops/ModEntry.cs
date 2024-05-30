/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/rokugin/StardewMods
**
*************************************************/

using GenericModConfigMenu;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace BetterStardrops {
    internal class ModEntry : Mod {
        //void SetUpInts() {
        //    attackIncreaseAmount = config.AttackIncreaseAmount;
        //    defenseIncreaseAmount = config.DefenseIncreaseAmount;
        //    immunityIncreaseAmount = config.ImmunityIncreaseAmount;
        //    healthIncreaseAmount = config.HealthIncreaseAmount;
        //    staminaIncreaseAmount = config.StaminaIncreaseAmount;
        //    combatLevelIncreaseAmount = config.CombatLevelIncreaseAmount;
        //    farminglevelIncreaseAmount = config.FarmingLevelIncreaseAmount;
        //    fishingLevelIncreaseAmount = config.FishingLevelIncreaseAmount;
        //    foragingLevelIncreaseAmount = config.ForagingLevelIncreaseAmount;
        //    luckLevelIncreaseAmount = config.LuckLevelIncreaseAmount;
        //    miningLevelIncreaseAmount = config.MiningLevelIncreaseAmount;
        //    magneticIncreaseAmount = config.MagneticIncreaseAmount;
        //}

        int attackIncreaseAmount => config.AttackIncreaseAmount;
        int newAttackIncreaseAmount;

        int defenseIncreaseAmount => config.DefenseIncreaseAmount;
        int newDefenseIncreaseAmount;

        int immunityIncreaseAmount => config.ImmunityIncreaseAmount;
        int newImmunityIncreaseAmount;

        int healthIncreaseAmount => config.HealthIncreaseAmount;
        int newHealthIncreaseAmount;

        int staminaIncreaseAmount => config.StaminaIncreaseAmount;
        int newStaminaIncreaseAmount;

        int combatLevelIncreaseAmount => config.CombatLevelIncreaseAmount;
        int newCombatLevelIncreaseAmount;

        int farminglevelIncreaseAmount => config.FarmingLevelIncreaseAmount;
        int newFarmingLevelIncreaseAmount;

        int fishingLevelIncreaseAmount => config.FishingLevelIncreaseAmount;
        int newFishingLevelIncreaseAmount;

        int foragingLevelIncreaseAmount => config.ForagingLevelIncreaseAmount;
        int newForagingLevelIncreaseAmount;

        int luckLevelIncreaseAmount => config.LuckLevelIncreaseAmount;
        int newLuckLevelIncreaseAmount;

        int miningLevelIncreaseAmount => config.MiningLevelIncreaseAmount;
        int newMiningLevelIncreaseAmount;

        int magneticIncreaseAmount => config.MagneticIncreaseAmount;
        int newMagneticIncreaseAmount;

        int stardropsFound;

        ModConfig config = new();
        BuffMaker buffMaker = new BuffMaker();

        public override void Entry(IModHelper helper) {
            config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.DayEnding += OnDayEnding;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        private void OnGameLaunched(object? sender, StardewModdingAPI.Events.GameLaunchedEventArgs e) {
            SetUpGMCM();
        }

        private void OnDayEnding(object? sender, StardewModdingAPI.Events.DayEndingEventArgs e) {
            if (!config.EnableHealth) return;
            if (stardropsFound < 1) return;

            Game1.player.maxHealth = Game1.player.maxHealth - newHealthIncreaseAmount;

            if (config.ShowLogging && stardropsFound > 0) {
                Monitor.Log("", LogLevel.Debug);
                Monitor.Log("Reducing max health to prepare for next day calculations", LogLevel.Info);
            }
        }

        private void OnDayStarted(object? sender, StardewModdingAPI.Events.DayStartedEventArgs e) {
            if (config.ResetMaxHealth) {
                LevelUpMenu.RevalidateHealth(Game1.player);
                config.ResetMaxHealth = false;
            }

            //SetUpInts();
            stardropsFound = Utility.numStardropsFound(Game1.player);

            if (config.ShowLogging && stardropsFound < 1) {
                Monitor.Log("", LogLevel.Debug);
                Monitor.Log("No stardrops found, do nothing", LogLevel.Info);
                return;
            }

            if (config.ShowLogging) {
                Monitor.Log("", LogLevel.Debug);
                Monitor.Log($"Player has found {stardropsFound} Stardrops", LogLevel.Info);
            }

            if (stardropsFound > 0) {
                List<Buff> buffsToApply = new();

                if (config.EnableAttack) {
                    newAttackIncreaseAmount = attackIncreaseAmount * stardropsFound;
                    Buff buff = buffMaker.CreateAttackBuff(newAttackIncreaseAmount);
                    buffsToApply.Add(buff);
                    if (config.ShowLogging) Monitor.Log($"Attack buff: +{newAttackIncreaseAmount}", LogLevel.Info);
                }
                if (config.EnableDefense) {
                    newDefenseIncreaseAmount = defenseIncreaseAmount * stardropsFound;
                    Buff buff = buffMaker.CreateDefenseBuff(newDefenseIncreaseAmount);
                    buffsToApply.Add(buff);
                    if (config.ShowLogging) Monitor.Log($"Defense buff: +{newDefenseIncreaseAmount}", LogLevel.Info);
                }
                if (config.EnableImmunity) {
                    newImmunityIncreaseAmount = immunityIncreaseAmount * stardropsFound;
                    Buff buff = buffMaker.CreateImmunityBuff(newImmunityIncreaseAmount);
                    buffsToApply.Add(buff);
                    if (config.ShowLogging) Monitor.Log($"Immunity buff: +{newImmunityIncreaseAmount}", LogLevel.Info);
                }
                if (config.EnableHealth) {
                    newHealthIncreaseAmount = healthIncreaseAmount * stardropsFound;
                    Game1.player.maxHealth += newHealthIncreaseAmount;
                    Game1.player.health = Game1.player.maxHealth;
                    if (config.ShowLogging) Monitor.Log($"Health buff: +{newHealthIncreaseAmount}", LogLevel.Info);
                }
                if (config.EnableStamina) {
                    newStaminaIncreaseAmount = staminaIncreaseAmount * stardropsFound;
                    Buff buff = buffMaker.CreateStaminaBuff(newStaminaIncreaseAmount);
                    buffsToApply.Add(buff);
                    if (config.ShowLogging) Monitor.Log($"Stamina buff: +{newStaminaIncreaseAmount}", LogLevel.Info);
                }
                if (config.EnableCombatLevel) {
                    newCombatLevelIncreaseAmount = combatLevelIncreaseAmount * stardropsFound;
                    Buff buff = buffMaker.CreateCombatLevelBuff(newCombatLevelIncreaseAmount);
                    buffsToApply.Add(buff);
                    if (config.ShowLogging) Monitor.Log($"Combat level buff: +{newCombatLevelIncreaseAmount}", LogLevel.Info);
                }
                if (config.EnableFarmingLevel) {
                    newFarmingLevelIncreaseAmount = farminglevelIncreaseAmount * stardropsFound;
                    Buff buff = buffMaker.CreateFarmingLevelBuff(newFarmingLevelIncreaseAmount);
                    buffsToApply.Add(buff);
                    if (config.ShowLogging) Monitor.Log($"Farming level buff: +{newFarmingLevelIncreaseAmount}", LogLevel.Info);
                }
                if (config.EnableFishingLevel) {
                    newFishingLevelIncreaseAmount = fishingLevelIncreaseAmount * stardropsFound;
                    Buff buff = buffMaker.CreateFishingLevelBuff(newFishingLevelIncreaseAmount);
                    buffsToApply.Add(buff);
                    if (config.ShowLogging) Monitor.Log($"Fishing level buff: +{newFishingLevelIncreaseAmount}", LogLevel.Info);
                }
                if (config.EnableForagingLevel) {
                    newForagingLevelIncreaseAmount = foragingLevelIncreaseAmount * stardropsFound;
                    Buff buff = buffMaker.CreateForagingLevelBuff(newForagingLevelIncreaseAmount);
                    buffsToApply.Add(buff);
                    if (config.ShowLogging) Monitor.Log($"Foraging level buff: +{newForagingLevelIncreaseAmount}", LogLevel.Info);
                }
                if (config.EnableLuckLevel) {
                    newLuckLevelIncreaseAmount = luckLevelIncreaseAmount * stardropsFound;
                    Buff buff = buffMaker.CreateLuckLevelBuff(newLuckLevelIncreaseAmount);
                    buffsToApply.Add(buff);
                    if (config.ShowLogging) Monitor.Log($"Luck level buff: +{newLuckLevelIncreaseAmount}", LogLevel.Info);
                }
                if (config.EnableMiningLevel) {
                    newMiningLevelIncreaseAmount = miningLevelIncreaseAmount * stardropsFound;
                    Buff buff = buffMaker.CreateMiningLevelBuff(newMiningLevelIncreaseAmount);
                    buffsToApply.Add(buff);
                    if (config.ShowLogging) Monitor.Log($"Mining level buff: +{newMiningLevelIncreaseAmount}", LogLevel.Info);
                }
                if (config.EnableMagnetic) {
                    newMagneticIncreaseAmount = magneticIncreaseAmount * stardropsFound;
                    Buff buff = buffMaker.CreateMagneticBuff(newMagneticIncreaseAmount);
                    buffsToApply.Add(buff);
                    if (config.ShowLogging) Monitor.Log($"Magnetic radius buff: +{newMagneticIncreaseAmount}", LogLevel.Info);
                }

                foreach (Buff buffToApply in buffsToApply) {
                    Game1.player.applyBuff(buffToApply);
                    buffToApply.visible = false;
                }
            }
        }

        void SetUpGMCM() {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) return;

            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => config = new ModConfig(),
                save: () => Helper.WriteConfig(config)
            );

            configMenu.SetTitleScreenOnlyForNextOptions(mod: this.ModManifest, true);
            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("attack-options.label")
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => config.EnableAttack,
                setValue: value => config.EnableAttack = value,
                name: () => Helper.Translation.Get("enabled.label")
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => config.AttackIncreaseAmount,
                setValue: value => config.AttackIncreaseAmount = value,
                name: () => Helper.Translation.Get("buff-power.label"),
                tooltip: () => Helper.Translation.Get("attack-buff.tooltip"),
                min: 0,
                max: null
            );

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("defense-options.label")
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => config.EnableDefense,
                setValue: value => config.EnableDefense = value,
                name: () => Helper.Translation.Get("enabled.label")
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => config.DefenseIncreaseAmount,
                setValue: value => config.DefenseIncreaseAmount = value,
                name: () => Helper.Translation.Get("buff-power.label"),
                tooltip: () => Helper.Translation.Get("defense-buff.tooltip"),
                min: 0,
                max: null
            );

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("immunity-options.label")
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => config.EnableImmunity,
                setValue: value => config.EnableImmunity = value,
                name: () => Helper.Translation.Get("enabled.label")
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => config.ImmunityIncreaseAmount,
                setValue: value => config.ImmunityIncreaseAmount = value,
                name: () => Helper.Translation.Get("buff-power.label"),
                tooltip: () => Helper.Translation.Get("immunity-buff.tooltip"),
                min: 0,
                max: null
            );

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("health-options.label")
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => config.EnableHealth,
                setValue: value => config.EnableHealth = value,
                name: () => Helper.Translation.Get("enabled.label")
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => config.HealthIncreaseAmount,
                setValue: value => config.HealthIncreaseAmount = value,
                name: () => Helper.Translation.Get("buff-power.label"),
                tooltip: () => Helper.Translation.Get("health-buff.tooltip"),
                min: 0,
                max: null
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => config.ResetMaxHealth,
                setValue: value => config.ResetMaxHealth = value,
                name: () => Helper.Translation.Get("health-reset.label"),
                tooltip: () => Helper.Translation.Get("health-reset.tooltip")
            );

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("stamina-options.label")
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => config.EnableStamina,
                setValue: value => config.EnableStamina = value,
                name: () => Helper.Translation.Get("enabled.label")
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => config.StaminaIncreaseAmount,
                setValue: value => config.StaminaIncreaseAmount = value,
                name: () => Helper.Translation.Get("buff-power.label"),
                tooltip: () => Helper.Translation.Get("stamina-buff.tooltip"),
                min: 0,
                max: null
            );

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("magnetic-options.label")
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => config.EnableMagnetic,
                setValue: value => config.EnableMagnetic = value,
                name: () => Helper.Translation.Get("enabled.label")
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => config.MagneticIncreaseAmount,
                setValue: value => config.MagneticIncreaseAmount = value,
                name: () => Helper.Translation.Get("buff-power.label"),
                tooltip: () => Helper.Translation.Get("magnetic-buff.tooltip"),
                min: 0,
                max: null
            );

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("combat-options.label")
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => config.EnableCombatLevel,
                setValue: value => config.EnableCombatLevel = value,
                name: () => Helper.Translation.Get("enabled.label")
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => config.CombatLevelIncreaseAmount,
                setValue: value => config.CombatLevelIncreaseAmount = value,
                name: () => Helper.Translation.Get("buff-power.label"),
                tooltip: () => Helper.Translation.Get("combat-buff.tooltip"),
                min: 0,
                max: null
            );

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("farming-options.label")
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => config.EnableFarmingLevel,
                setValue: value => config.EnableFarmingLevel = value,
                name: () => Helper.Translation.Get("enabled.label")
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => config.FarmingLevelIncreaseAmount,
                setValue: value => config.FarmingLevelIncreaseAmount = value,
                name: () => Helper.Translation.Get("buff-power.label"),
                tooltip: () => Helper.Translation.Get("farming-buff.tooltip"),
                min: 0,
                max: null
            );

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("fishing-options.label")
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => config.EnableFishingLevel,
                setValue: value => config.EnableFishingLevel = value,
                name: () => Helper.Translation.Get("enabled.label")
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => config.FishingLevelIncreaseAmount,
                setValue: value => config.FishingLevelIncreaseAmount = value,
                name: () => Helper.Translation.Get("buff-power.label"),
                tooltip: () => Helper.Translation.Get("fishing-buff.tooltip"),
                min: 0,
                max: null
            );

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("foraging-options.label")
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => config.EnableForagingLevel,
                setValue: value => config.EnableForagingLevel = value,
                name: () => Helper.Translation.Get("enabled.label")
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => config.ForagingLevelIncreaseAmount,
                setValue: value => config.ForagingLevelIncreaseAmount = value,
                name: () => Helper.Translation.Get("buff-power.label"),
                tooltip: () => Helper.Translation.Get("foraging-buff.tooltip"),
                min: 0,
                max: null
            );

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("luck-options.label")
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => config.EnableLuckLevel,
                setValue: value => config.EnableLuckLevel = value,
                name: () => Helper.Translation.Get("enabled.label")
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => config.LuckLevelIncreaseAmount,
                setValue: value => config.LuckLevelIncreaseAmount = value,
                name: () => Helper.Translation.Get("buff-power.label"),
                tooltip: () => Helper.Translation.Get("luck-buff.tooltip"),
                min: 0,
                max: null
            );

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("mining-options.label")
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => config.EnableMiningLevel,
                setValue: value => config.EnableMiningLevel = value,
                name: () => Helper.Translation.Get("enabled.label")
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => config.MiningLevelIncreaseAmount,
                setValue: value => config.MiningLevelIncreaseAmount = value,
                name: () => Helper.Translation.Get("buff-power.label"),
                tooltip: () => Helper.Translation.Get("mining-buff.tooltip"),
                min: 0,
                max: null
            );

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("debugging.label")
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => config.ShowLogging,
                setValue: value => config.ShowLogging = value,
                name: () => Helper.Translation.Get("smapi.label"),
                tooltip: () => Helper.Translation.Get("smapi.tooltip")
            );
        }
    }
}
