/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NeroYuki/StardewSurvivalProject
**
*************************************************/

using System;
using StardewModdingAPI;
using Microsoft.Xna.Framework;

namespace StardewSurvivalProject.source.api
{
    public class ConfigMenu
    {
        public static void Init(Mod context)
        {
            var api = context.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api is null)
                return;

            // register mod configuration
            api.RegisterModConfig(
                mod: context.ModManifest,
                revertToDefault: () => ModConfig.GetInstance().SetConfig(new ModConfig()),
                saveToFile: () => context.Helper.WriteConfig(ModConfig.GetInstance())
            );

            // let players configure your mod in-game (instead of just from the title screen)
            api.SetDefaultIngameOptinValue(context.ModManifest, true);

            api.RegisterParagraph(context.ModManifest, "There are A LOT of options available for this mod. Therefore please read how each option affect the game by hover above said option");
            api.RegisterPageLabel(
                context.ModManifest,
                "Toggle Feature",
                "Options to enable / disable certain mod feature",
                "Toggle Feature"
            );
            api.RegisterPageLabel(
                context.ModManifest,
                "UI Configuration",
                "Options to adjust how the mod's UI is displayed",
                "UI Configuration"
            );
            api.RegisterPageLabel(
                context.ModManifest,
                "Difficulty Setting",
                "Options to tweak various aspect of the mod's mechanic for a harder or easier experience, some options is fairly advanced",
                "Difficulty Setting"
            );

            api.StartNewPage(context.ModManifest, "Toggle Feature");
            api.RegisterParagraph(context.ModManifest, "Options to enable / disable certain mod feature");

            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Passive drain",
                optionDesc: "Enable hunger and thirst drain overtime - trigger every 10 minutes in-game (Default: Checked)",
                optionGet: () => ModConfig.GetInstance().UsePassiveDrain,
                optionSet: value => ModConfig.GetInstance().UsePassiveDrain = value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Drain on running",
                optionDesc: "Drain hunger and thirst on running (Default: Checked)",
                optionGet: () => ModConfig.GetInstance().UseOnRunningDrain,
                optionSet: value => ModConfig.GetInstance().UseOnRunningDrain = value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Drain on tools used",
                optionDesc: "Drain hunger and thirst after using tools - Further adjustment can be made in Difficulty Setting (Default: Checked)",
                optionGet: () => ModConfig.GetInstance().UseOnToolUseDrain,
                optionSet: value => ModConfig.GetInstance().UseOnToolUseDrain = value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Use Temperature Module",
                optionDesc: "Enable Environment and Body Temperature mechanic (Default: Checked)",
                optionGet: () => ModConfig.GetInstance().UseTemperatureModule,
                optionSet: value => ModConfig.GetInstance().UseTemperatureModule = value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Disable Item HP Regen.",
                optionDesc: "Disable HP Regeneration from all in-game item except item from modded and some whitelisted items (Default: Checked)",
                optionGet: () => ModConfig.GetInstance().DisableHPHealingOnEatingFood,
                optionSet: value => ModConfig.GetInstance().DisableHPHealingOnEatingFood = value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Overnight drain",
                optionDesc: "Drain hunger and thirst when sleeping - Equivalent to 4 Hours of passive drain (Default: Checked)",
                optionGet: () => ModConfig.GetInstance().UsePassiveDrain,
                optionSet: value => ModConfig.GetInstance().UsePassiveDrain = value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Friendship penalty on not feeding spouse",
                optionDesc: "Define how much friendship is lost when not feeding your spouse each day, set to 0 to disable this feature entirely (Default: 50)",
                optionGet: () => ModConfig.GetInstance().FriendshipPenaltyOnNotFeedingSpouse,
                optionSet: value => ModConfig.GetInstance().FriendshipPenaltyOnNotFeedingSpouse = value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Experimental - Saturation Stat.",
                optionDesc: "Saturation is scaled with hunger restored to penalize low quality food, also make stomachache more severe",
                optionGet: () => ModConfig.GetInstance().ScaleHungerRestoredWithTimeFromLastMeal,
                optionSet: value => ModConfig.GetInstance().ScaleHungerRestoredWithTimeFromLastMeal = value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Experimental - Stamina rework",
                optionDesc: "Stamina will drain at an accelarated pace, but also regen overtime, and you have access to sprinting",
                optionGet: () => ModConfig.GetInstance().UseStaminaRework,
                optionSet: value => ModConfig.GetInstance().UseStaminaRework = value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Experimental - Sanity Meter",
                optionDesc: "Hahahaha let's not talk about this one!",
                optionGet: () => ModConfig.GetInstance().UseSanityModule,
                optionSet: value => ModConfig.GetInstance().UseSanityModule = value
            );

            api.StartNewPage(context.ModManifest, "UI Configuration");
            api.RegisterParagraph(context.ModManifest, "Options to adjust how the mod's UI is displayed");

            api.RegisterChoiceOption(
                mod: context.ModManifest,
                optionName: "Re-texture Preset",
                optionDesc: "Change UI Texture of the mod to fit better with popular re-texture mods (Default: Auto - Auto-detect re-texture mods in the game and pick the appropriate preset) - Save and restart the game to apply this change",
                optionGet: () => ModConfig.GetInstance().RetexturePreset,
                optionSet: value => ModConfig.GetInstance().RetexturePreset = value,
                choices: new string[] { "auto", "default", "vintage2", "overgrown", "earthy", "legacy", "alternative"}
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "UI Offset (X-axis)",
                optionDesc: "Offset of the mod vitality UI by X-axis (Default: 10)",
                optionGet: () => ModConfig.GetInstance().UIOffsetX,
                optionSet: value => ModConfig.GetInstance().UIOffsetX = value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "UI Offset (Y-axis)",
                optionDesc: "Offset of the mod vitality UI by Y-axis (Default: 10)",
                optionGet: () => ModConfig.GetInstance().UIOffsetY,
                optionSet: value => ModConfig.GetInstance().UIOffsetY = value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "UI Scale",
                optionDesc: "Scale of the mod vitality UI (Default: 4.0)",
                optionGet: () => ModConfig.GetInstance().UIScale,
                optionSet: value => ModConfig.GetInstance().UIScale = value
            );
            api.RegisterChoiceOption(
                mod: context.ModManifest,
                optionName: "Temperature Unit",
                optionDesc: "Change the temperature Unit (Default: Celcius)",
                optionGet: () => ModConfig.GetInstance().TemperatureUnit,
                optionSet: value => ModConfig.GetInstance().TemperatureUnit = value,
                choices: new string[] { "Celcius", "Kelvin", "Fahrenheit" }
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Env. Temperature Lower Bound",
                optionDesc: "Lowest Temperature value in the environment temperature bar (Default: -10C)",
                optionGet: () => (float)ModConfig.GetInstance().EnvironmentTemperatureDisplayLowerBound,
                optionSet: value => ModConfig.GetInstance().EnvironmentTemperatureDisplayLowerBound = (double)value
            ) ;
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Env. Temperature Higher Bound",
                optionDesc: "Highest Temperature value in the environment temperature bar (Default: 50C)",
                optionGet: () => (float)ModConfig.GetInstance().EnvironmentTemperatureDisplayHigherBound,
                optionSet: value => ModConfig.GetInstance().EnvironmentTemperatureDisplayHigherBound = (double)value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Body Temperature Lower Bound",
                optionDesc: "Lowest Temperature value in the body temperature bar (Default: 28C)",
                optionGet: () => (float)ModConfig.GetInstance().BodyTemperatureDisplayLowerBound,
                optionSet: value => ModConfig.GetInstance().BodyTemperatureDisplayLowerBound = (double)value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Body Temperature Higher Bound",
                optionDesc: "Lowest Temperature value in the body temperature bar (Default: 45C)",
                optionGet: () => (float)ModConfig.GetInstance().BodyTemperatureDisplayHigherBound,
                optionSet: value => ModConfig.GetInstance().BodyTemperatureDisplayHigherBound = (double)value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Disable Hover Info",
                optionDesc: "Disable All Mod-related info when hover on items. Not recommended (Default: Unchecked)",
                optionGet: () => ModConfig.GetInstance().DisableModItemInfo,
                optionSet: value => ModConfig.GetInstance().DisableModItemInfo = value
            );

            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Overlay Comfortable Temperature",
                optionDesc: "Player can see the comfortable temperature range in the environment temperature meter (Default: Checked)",
                optionGet: () => ModConfig.GetInstance().IndicateComfortableTemperatureRange,
                optionSet: value => ModConfig.GetInstance().IndicateComfortableTemperatureRange = value
            );


            api.StartNewPage(context.ModManifest, "Difficulty Setting");
            api.RegisterParagraph(context.ModManifest, "Options to tweak various aspect of the mod's mechanic for a harder or easier experience, some options is fairly advanced");

            api.RegisterPageLabel(
                context.ModManifest,
                "Thirst and Hunger",
                "Options for Thirst and Hunger mechanic",
                "Thirst and Hunger"
            );
            api.RegisterPageLabel(
                context.ModManifest,
                "Environmental Temperature",
                "Options to tweak how environmental temperature is calculated",
                "Environmental Temperature"
            );
            api.RegisterPageLabel(
                context.ModManifest,
                "Body Temperature",
                "Options to tweak how body temperature is calculated",
                "Body Temperature"
            );
            api.RegisterPageLabel(
                context.ModManifest,
                "Custom Buff / Debuff",
                "Options for mod's custom buff and debuff conditions and mechanics",
                "Custom Buff / Debuff"
            );
            api.RegisterPageLabel(
                context.ModManifest,
                "Stamina / HP Rework",
                "Options for mod's stamina and HP rework",
                "Stamina / HP Rework"
            );

            api.StartNewPage(context.ModManifest, "Thirst and Hunger");
            api.RegisterParagraph(context.ModManifest, "Options for Thirst and Hunger mechanic");
            api.RegisterLabel(context.ModManifest, "Hunger / Thirst Capacity", "This section decide how much hunger or thirst player possesed as well as conditions for some buffs and debuffs");
            //Capacity
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Max Hunger",
                optionDesc: "Maximum capacity for hunger (Default: 100)",
                optionGet: () => (float)ModConfig.GetInstance().MaxHunger,
                optionSet: value => ModConfig.GetInstance().MaxHunger = (double)value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Max Thirst",
                optionDesc: "Maximum capacity for thirst (Default: 100)",
                optionGet: () => (float)ModConfig.GetInstance().MaxThirst,
                optionSet: value => ModConfig.GetInstance().MaxThirst = (double)value
            );
            //Hunger / Thirst debuff
            api.RegisterClampedOption(
                mod: context.ModManifest,
                optionName: "% threshold for Hunger",
                optionDesc: "Hunger effect will be applied if Hunger value drop below this threshold (Default: 25, Effect is disabled if value below 0)",
                optionGet: () => (float)ModConfig.GetInstance().HungerEffectPercentageThreshold,
                optionSet: value => ModConfig.GetInstance().HungerEffectPercentageThreshold = (double)value,
                min: -1f,
                max: 100f,
                interval: 0.1f
            );
            api.RegisterClampedOption(
                mod: context.ModManifest,
                optionName: "% threshold for Thirst",
                optionDesc: "Thirst effect will be applied if Thirst value drop below this threshold (Default: 25, Effect is disabled if value below 0)",
                optionGet: () => (float)ModConfig.GetInstance().HungerEffectPercentageThreshold,
                optionSet: value => ModConfig.GetInstance().HungerEffectPercentageThreshold = (double)value,
                min: -1f,
                max: 100f,
                interval: 0.1f
            );
            //Well Fed Buff
            api.RegisterClampedOption(
                mod: context.ModManifest,
                optionName: "% Hunger threshold for Well Fed",
                optionDesc: "Well Fed effect will be applied if Hunger value exceed this threshold, as well as Thirst value decided below (Default: 25, Effect is disabled if value above 100)",
                optionGet: () => (float)ModConfig.GetInstance().HungerWellFedEffectPercentageThreshold,
                optionSet: value => ModConfig.GetInstance().HungerWellFedEffectPercentageThreshold = (double)value,
                min: 0f,
                max: 101f,
                interval: 0.1f
            );
            api.RegisterClampedOption(
                mod: context.ModManifest,
                optionName: "% Thirst threshold for Well Fed",
                optionDesc: "Well Fed effect will be applied if Thirst value exceed this threshold, as well as Hunger value decided above (Default: 25, Effect is disabled if value above 100)",
                optionGet: () => (float)ModConfig.GetInstance().ThirstWellFedEffectPercentageThreshold,
                optionSet: value => ModConfig.GetInstance().ThirstWellFedEffectPercentageThreshold = (double)value,
                min: 0f,
                max: 101f,
                interval: 0.1f
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Health Loss on Dehydration",
                optionDesc: "Health value loss in place of hydration value consumption if Dehydration is in effect (Default: 10)",
                optionGet: () => ModConfig.GetInstance().HealthPenaltyOnDehydration,
                optionSet: value => ModConfig.GetInstance().HealthPenaltyOnDehydration = value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Energy Loss on Dehydration",
                optionDesc: "Energy value loss in place of hunger value consumption if Starvation is in effect (Default: 10)",
                optionGet: () => ModConfig.GetInstance().StaminaPenaltyOnStarvation,
                optionSet: value => ModConfig.GetInstance().StaminaPenaltyOnStarvation = value
            );
            //Foods and Drinks 
            api.RegisterLabel(context.ModManifest, "Foods / Drinks Setting", "This section decide how foods and drinks effect Hunger and Thirst stat");
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Hydration on Env. Drinking",
                optionDesc: "Hydration value restored for player by drinking river, lake water or water from watering can (Default: 5)",
                optionGet: () => (float)ModConfig.GetInstance().HydrationGainOnEnvironmentWaterDrinking,
                optionSet: value => ModConfig.GetInstance().HydrationGainOnEnvironmentWaterDrinking = (double)value
            );
            api.RegisterChoiceOption(
                mod: context.ModManifest,
                optionName: "Environment Hydration Mode",
                optionDesc: "Change how to drink water from water tile and watering can (default = right click, strict = <secondary layer button> + right click, disable = no trigger)",
                optionGet: () => ModConfig.GetInstance().EnvironmentHydrationMode,
                optionSet: value => ModConfig.GetInstance().EnvironmentHydrationMode = value,
                choices: new string[] { "default", "strict", "disable" }
            );
            api.AddKeybind(
                mod: context.ModManifest,
                name: () => "Secondary Layer Button",
                tooltip: () => "Keybind to enable secondary layer action",
                getValue: () => ModConfig.GetInstance().SecondaryLayerButton,
                setValue: value => ModConfig.GetInstance().SecondaryLayerButton = value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Hydration on Item Drinking",
                optionDesc: "Default hydration value restored for player by drinking any items, items with custom hydration value also scale with this (Default: 10)",
                optionGet: () => (float)ModConfig.GetInstance().DefaultHydrationGainOnDrinkableItems,
                optionSet: value => ModConfig.GetInstance().DefaultHydrationGainOnDrinkableItems = (double)value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Hunger Multiplier",
                optionDesc: "Multiplier value to decide hunger restore value from item's edibility (Default: 1)",
                optionGet: () => (float)ModConfig.GetInstance().HungerGainMultiplierFromItemEdibility,
                optionSet: value => ModConfig.GetInstance().HungerGainMultiplierFromItemEdibility = (double)value
            );
            api.RegisterLabel(context.ModManifest, "Drain overtime", "This section decide how much hunger or thirst is consume overtime");
            //Passive
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Passive - Hunger",
                optionDesc: "Hunger drain happen each 10-minute in-game time (Default: 0.2)",
                optionGet: () => (float)ModConfig.GetInstance().PassiveHungerDrainRate,
                optionSet: value => ModConfig.GetInstance().PassiveHungerDrainRate = (double)value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Passive - Thirst",
                optionDesc: "Thirst drain happen each 10-minute in-game time (Default: 0.3)",
                optionGet: () => (float)ModConfig.GetInstance().PassiveThirstDrainRate,
                optionSet: value => ModConfig.GetInstance().PassiveThirstDrainRate = (double)value
            );
            //Running
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Running - Hunger",
                optionDesc: "Hunger drain happen each tick if running - there are 60 ticks in 1 second (Default: 0.001)",
                optionGet: () => (float)ModConfig.GetInstance().RunningHungerDrainRate,
                optionSet: value => ModConfig.GetInstance().RunningHungerDrainRate = (double)value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Running - Thirst",
                optionDesc: "Thirst drain happen each tick if running - there are 60 ticks in 1 second (Default: 0.002)",
                optionGet: () => (float)ModConfig.GetInstance().RunningThirstDrainRate,
                optionSet: value => ModConfig.GetInstance().RunningThirstDrainRate = (double)value
            );
            api.RegisterLabel(context.ModManifest, "Drain on tools used", "This section decide how much hunger or thirst is consume on one tool usage");
            //Axe
            //api.RegisterImage(context.ModManifest, "TileSheets\\tools", new Rectangle(80, 160, 16, 16));
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Axe - Hunger",
                optionDesc: "Hunger drain each time the axe is used (Default: 0.5)",
                optionGet: () => (float)ModConfig.GetInstance().AxeHungerDrain,
                optionSet: value => ModConfig.GetInstance().AxeHungerDrain = (double)value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Axe - Thirst",
                optionDesc: "Thirst drain each time the axe is used (Default: 0.2)",
                optionGet: () => (float)ModConfig.GetInstance().AxeThirstDrain,
                optionSet: value => ModConfig.GetInstance().AxeThirstDrain = (double)value
            );
            //Pickaxe
            //api.RegisterImage(context.ModManifest, "TileSheets\\tools", new Rectangle(80, 96, 16, 16));
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Pickaxe - Hunger",
                optionDesc: "Hunger drain each time the pickaxe is used (Default: 0.5)",
                optionGet: () => (float)ModConfig.GetInstance().PickaxeHungerDrain,
                optionSet: value => ModConfig.GetInstance().PickaxeHungerDrain = (double)value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Pickaxe - Thirst",
                optionDesc: "Thirst drain each time the pickaxe is used (Default: 0.2)",
                optionGet: () => (float)ModConfig.GetInstance().PickaxeThirstDrain,
                optionSet: value => ModConfig.GetInstance().PickaxeThirstDrain = (double)value
            );
            //Hoe
            //api.RegisterImage(context.ModManifest, "TileSheets\\tools", new Rectangle(80, 32, 16, 16));
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Hoe - Hunger",
                optionDesc: "Hunger drain each time the hoe is used (Default: 0.5)",
                optionGet: () => (float)ModConfig.GetInstance().HoeHungerDrain,
                optionSet: value => ModConfig.GetInstance().HoeHungerDrain = (double)value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Hoe - Thirst",
                optionDesc: "Thirst drain each time the hoe is used (Default: 0.2)",
                optionGet: () => (float)ModConfig.GetInstance().HoeThirstDrain,
                optionSet: value => ModConfig.GetInstance().HoeThirstDrain = (double)value
            );
            //Melee Weapon
            //api.RegisterImage(context.ModManifest, "TileSheets\\weapons", new Rectangle(16, 0, 16, 16));
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Melee Weapons - Hunger",
                optionDesc: "Hunger drain each time a melee weapon is used (Default: 0.5)",
                optionGet: () => (float)ModConfig.GetInstance().MeleeWeaponHungerDrain,
                optionSet: value => ModConfig.GetInstance().MeleeWeaponHungerDrain = (double)value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Melee Weapons - Thirst",
                optionDesc: "Thirst drain each time a melee weapon is used (Default: 0.2)",
                optionGet: () => (float)ModConfig.GetInstance().MeleeWeaponThirstDrain,
                optionSet: value => ModConfig.GetInstance().MeleeWeaponThirstDrain = (double)value
            );
            //Slingshot
            //api.RegisterImage(context.ModManifest, "TileSheets\\weapons", new Rectangle(16, 64, 16, 16));
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Slingshot - Hunger",
                optionDesc: "Hunger drain each time a slingshot is used (Default: 0.5)",
                optionGet: () => (float)ModConfig.GetInstance().SlingshotHungerDrain,
                optionSet: value => ModConfig.GetInstance().SlingshotHungerDrain = (double)value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Slingshot - Thirst",
                optionDesc: "Thirst drain each time a slingshot is used (Default: 0.2)",
                optionGet: () => (float)ModConfig.GetInstance().SlingshotThirstDrain,
                optionSet: value => ModConfig.GetInstance().SlingshotThirstDrain = (double)value
            );
            //Watering Can
            //api.RegisterImage(context.ModManifest, "TileSheets\\tools", new Rectangle(32, 224, 16, 16));
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Watering can - Hunger",
                optionDesc: "Hunger drain each time the watering can is used (Default: 0.5)",
                optionGet: () => (float)ModConfig.GetInstance().WateringCanHungerDrain,
                optionSet: value => ModConfig.GetInstance().WateringCanHungerDrain = (double)value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Watering can - Thirst",
                optionDesc: "Thirst drain each time a watering can is used (Default: 0.2)",
                optionGet: () => (float)ModConfig.GetInstance().WateringCanThirstDrain,
                optionSet: value => ModConfig.GetInstance().WateringCanThirstDrain = (double)value
            );
            //Fishing Pole
            //api.RegisterImage(context.ModManifest, "TileSheets\\tools", new Rectangle(128, 0, 16, 16));
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Fishing pole - Hunger",
                optionDesc: "Hunger drain each time a fishing pole is used (Default: 2)",
                optionGet: () => (float)ModConfig.GetInstance().FishingPoleHungerDrain,
                optionSet: value => ModConfig.GetInstance().FishingPoleHungerDrain = (double)value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Fishing pole - Thirst",
                optionDesc: "Thirst drain each time a fishing pole is used (Default: 0.6)",
                optionGet: () => (float)ModConfig.GetInstance().FishingPoleThirstDrain,
                optionSet: value => ModConfig.GetInstance().FishingPoleThirstDrain = (double)value
            );
            //Milk Pail
            //api.RegisterImage(context.ModManifest, "TileSheets\\tools", new Rectangle(96, 0, 16, 16));
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Milk pail - Hunger",
                optionDesc: "Hunger drain each time the milk pail is used (Default: 0.5)",
                optionGet: () => (float)ModConfig.GetInstance().MilkPailHungerDrain,
                optionSet: value => ModConfig.GetInstance().MilkPailHungerDrain = (double)value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Milk pail - Thirst",
                optionDesc: "Thirst drain each time the milk pail is used (Default: 0.2)",
                optionGet: () => (float)ModConfig.GetInstance().MilkPailThirstDrain,
                optionSet: value => ModConfig.GetInstance().MilkPailThirstDrain = (double)value
            );
            //Shear
            //api.RegisterImage(context.ModManifest, "TileSheets\\tools", new Rectangle(112, 0, 16, 16));
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Shear - Hunger",
                optionDesc: "Hunger drain each time the shear is used (Default: 0.5)",
                optionGet: () => (float)ModConfig.GetInstance().ShearHungerDrain,
                optionSet: value => ModConfig.GetInstance().ShearHungerDrain = (double)value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Shear - Thirst",
                optionDesc: "Thirst drain each time the shear is used (Default: 0.2)",
                optionGet: () => (float)ModConfig.GetInstance().ShearThirstDrain,
                optionSet: value => ModConfig.GetInstance().ShearThirstDrain = (double)value
            );

            api.StartNewPage(context.ModManifest, "Environmental Temperature");
            api.RegisterParagraph(context.ModManifest, "Options to tweak how environmental temperature is calculated");
            ////Environmental Temperature
            api.RegisterLabel(context.ModManifest, "General Values", "This section contains values use in most temperature calculation");
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Enviroment Temperature Base",
                optionDesc: "Base Temperature used for most calculation (Default: 25)",
                optionGet: () => (float)ModConfig.GetInstance().EnvironmentBaseTemperature,
                optionSet: value => ModConfig.GetInstance().EnvironmentBaseTemperature = (double)value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Day/Night Cycle Scale",
                optionDesc: "Default Day/Night Cycle Temperature Difference Scale (Default: 3)",
                optionGet: () => (float)ModConfig.GetInstance().DefaultDayNightCycleTemperatureDiffScale,
                optionSet: value => ModConfig.GetInstance().DefaultDayNightCycleTemperatureDiffScale = (double)value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Fluctuation Scale",
                optionDesc: "Default Temperature Fluctuation Scale (Default: 1)",
                optionGet: () => (float)ModConfig.GetInstance().DefaultTemperatureFluctuationScale,
                optionSet: value => ModConfig.GetInstance().DefaultTemperatureFluctuationScale = (double)value
            );
            ////seasonal multiplier
            api.RegisterLabel(context.ModManifest, "Seasonal Multiplier", "This section contains values use to modify temperature by season");
            api.RegisterImage(context.ModManifest, "LooseSprites\\Cursors", new Rectangle(406, 441, 12, 8));
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Spring",
                optionDesc: "Spring Temperature Multiplier (Default: 0.9)",
                optionGet: () => (float)ModConfig.GetInstance().SpringSeasonTemperatureMultiplier,
                optionSet: value => ModConfig.GetInstance().SpringSeasonTemperatureMultiplier = (double)value
            );
            api.RegisterImage(context.ModManifest, "LooseSprites\\Cursors", new Rectangle(406, 449, 12, 8));
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Summer",
                optionDesc: "Summer Temperature Multiplier (Default: 1.1)",
                optionGet: () => (float)ModConfig.GetInstance().SummerSeasonTemperatureMultiplier,
                optionSet: value => ModConfig.GetInstance().SummerSeasonTemperatureMultiplier = (double)value
            );
            api.RegisterImage(context.ModManifest, "LooseSprites\\Cursors", new Rectangle(406, 457, 12, 8));
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Fall",
                optionDesc: "Fall Temperature Multiplier (Default: 0.9)",
                optionGet: () => (float)ModConfig.GetInstance().FallSeasonTemperatureMultiplier,
                optionSet: value => ModConfig.GetInstance().FallSeasonTemperatureMultiplier = (double)value
            );
            api.RegisterImage(context.ModManifest, "LooseSprites\\Cursors", new Rectangle(406, 465, 12, 8));
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Winter",
                optionDesc: "Winter Temperature Multiplier (Default: 0.2)",
                optionGet: () => (float)ModConfig.GetInstance().WinterSeasonTemperatureMultiplier,
                optionSet: value => ModConfig.GetInstance().WinterSeasonTemperatureMultiplier = (double)value
            );
            ////weather multiplier
            api.RegisterLabel(context.ModManifest, "Weather Multiplier", "This section contains values use to modify temperature by weather");
            api.RegisterImage(context.ModManifest, "LooseSprites\\Cursors", new Rectangle(341, 421, 12, 8));
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Sunny",
                optionDesc: "Sunny Temperature Multiplier (Default: 1.2)",
                optionGet: () => (float)ModConfig.GetInstance().SunnyWeatherTemperatureMultiplier,
                optionSet: value => ModConfig.GetInstance().SunnyWeatherTemperatureMultiplier = (double)value
            );
            api.RegisterImage(context.ModManifest, "LooseSprites\\Cursors", new Rectangle(329, 421, 12, 8));
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Festival",
                optionDesc: "Festival Temperature Multiplier (Default: 1.2)",
                optionGet: () => (float)ModConfig.GetInstance().FestivalWeatherTemperatureMultiplier,
                optionSet: value => ModConfig.GetInstance().FestivalWeatherTemperatureMultiplier = (double)value
            );
            api.RegisterImage(context.ModManifest, "LooseSprites\\Cursors", new Rectangle(317, 421, 12, 8));
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Wedding",
                optionDesc: "Wedding Temperature Multiplier (Default: 1.2)",
                optionGet: () => (float)ModConfig.GetInstance().WeddingWeatherTemperatureMultiplier,
                optionSet: value => ModConfig.GetInstance().WeddingWeatherTemperatureMultiplier = (double)value
            );
            api.RegisterImage(context.ModManifest, "LooseSprites\\Cursors", new Rectangle(377, 421, 12, 8));
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Storm",
                optionDesc: "Storm Temperature Multiplier (Default: 0.8)",
                optionGet: () => (float)ModConfig.GetInstance().StormWeatherTemperatureMultiplier,
                optionSet: value => ModConfig.GetInstance().StormWeatherTemperatureMultiplier = (double)value
            );
            api.RegisterImage(context.ModManifest, "LooseSprites\\Cursors", new Rectangle(365, 421, 12, 8));
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Rain",
                optionDesc: "Rain Temperature Multiplier (Default: 0.8)",
                optionGet: () => (float)ModConfig.GetInstance().RainWeatherTemperatureMultiplier,
                optionSet: value => ModConfig.GetInstance().RainWeatherTemperatureMultiplier = (double)value
            );
            api.RegisterImage(context.ModManifest, "LooseSprites\\Cursors", new Rectangle(353, 421, 12, 8));
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Windy - Spring",
                optionDesc: "Windy in spring Temperature Multiplier (Default: 0.9)",
                optionGet: () => (float)ModConfig.GetInstance().WindySpringWeatherTemperatureMultiplier,
                optionSet: value => ModConfig.GetInstance().WindySpringWeatherTemperatureMultiplier = (double)value
            );
            api.RegisterImage(context.ModManifest, "LooseSprites\\Cursors", new Rectangle(389, 421, 12, 8));
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Windy - Fall",
                optionDesc: "Windy in fall Temperature Multiplier (Default: 0.9)",
                optionGet: () => (float)ModConfig.GetInstance().WindyFallWeatherTemperatureMultiplier,
                optionSet: value => ModConfig.GetInstance().WindyFallWeatherTemperatureMultiplier = (double)value
            );
            api.RegisterImage(context.ModManifest, "LooseSprites\\Cursors", new Rectangle(401, 421, 12, 8));
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Snow",
                optionDesc: "Snow Temperature Multiplier (Default: -2.0)",
                optionGet: () => (float)ModConfig.GetInstance().SnowWeatherTemperatureMultiplier,
                optionSet: value => ModConfig.GetInstance().SnowWeatherTemperatureMultiplier = (double)value
            );

            ////location setting
            api.RegisterLabel(context.ModManifest, "Location Settings", "This section contains values use to modify temperature by location");
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Custom Location Data",
                optionDesc: "Enable using custom location temperature data for temperature calculation (Default: checked)",
                optionGet: () => ModConfig.GetInstance().UseCustomLocationTemperatureData,
                optionSet: value => ModConfig.GetInstance().UseCustomLocationTemperatureData = value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Default Indoor Modifier",
                optionDesc: "Enable using default indoor temperature modifier (Default: checked)",
                optionGet: () => ModConfig.GetInstance().UseDefaultIndoorTemperatureModifier,
                optionSet: value => ModConfig.GetInstance().UseDefaultIndoorTemperatureModifier = value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Default Cave Modifier",
                optionDesc: "Enable using default cave temperature modifier - support cave level (Default: checked)",
                optionGet: () => ModConfig.GetInstance().UseDefaultCaveTemperatureModifier,
                optionSet: value => ModConfig.GetInstance().UseDefaultCaveTemperatureModifier = value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Default Skull Cavern Modifier",
                optionDesc: "Enable using default skull cavern temperature modifier - support cave level (Default: checked)",
                optionGet: () => ModConfig.GetInstance().UseDefaultSkullCavernTemperatureModifier,
                optionSet: value => ModConfig.GetInstance().UseDefaultSkullCavernTemperatureModifier= value
            );

            api.StartNewPage(context.ModManifest, "Body Temperature");
            api.RegisterParagraph(context.ModManifest, "Options to tweak how body temperature is calculated (All value is in Celcius degree)");
            api.RegisterLabel(context.ModManifest, "Default Values", "This section contains values use to calculate player body temperature");
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Default Body Temp.",
                optionDesc: "Default player body temperature (Default: 37.5)",
                optionGet: () => (float)ModConfig.GetInstance().DefaultBodyTemperature,
                optionSet: value => ModConfig.GetInstance().DefaultBodyTemperature = (double)value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Default Min. Comfort Temp.",
                optionDesc: "Default minimum comfortable temperature for player, body temperature will drop beyond this point (Default: 16.0)",
                optionGet: () => (float)ModConfig.GetInstance().DefaultMinComfortableTemperature,
                optionSet: value => ModConfig.GetInstance().DefaultMinComfortableTemperature = (double)value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Default Max. Comfort Temp.",
                optionDesc: "Default maximum comfortable temperature for player, body temperature will rise beyond this point (Default: 24.0)",
                optionGet: () => (float)ModConfig.GetInstance().DefaultMaxComfortableTemperature,
                optionSet: value => ModConfig.GetInstance().DefaultMaxComfortableTemperature = (double)value
            );
            api.RegisterLabel(context.ModManifest, "Threshold Values", "This section contains values use to for threshold of various status effect caused by body temperature");
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Hypothermia Threshold",
                optionDesc: "Player get hypothermia if their body temperature drop beyond this value (Default: 35.0)",
                optionGet: () => (float)ModConfig.GetInstance().HypothermiaBodyTempThreshold,
                optionSet: value => ModConfig.GetInstance().HypothermiaBodyTempThreshold = (double)value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Frostbite Threshold",
                optionDesc: "Player get frostbite if their body temperature drop beyond this value (Default: 30.0)",
                optionGet: () => (float)ModConfig.GetInstance().FrostbiteBodyTempThreshold,
                optionSet: value => ModConfig.GetInstance().FrostbiteBodyTempThreshold = (double)value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Heatstroke Threshold",
                optionDesc: "Player get heatstroke if their body temperature rise beyond this value (Default: 38.5)",
                optionGet: () => (float)ModConfig.GetInstance().HeatstrokeBodyTempThreshold,
                optionSet: value => ModConfig.GetInstance().HeatstrokeBodyTempThreshold = (double)value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Burn Threshold",
                optionDesc: "Player get burned if their body temperature rise beyond this value (Default: 41.0)",
                optionGet: () => (float)ModConfig.GetInstance().BurnBodyTempThreshold,
                optionSet: value => ModConfig.GetInstance().BurnBodyTempThreshold = (double)value
            );
            api.RegisterLabel(context.ModManifest, "Temperature Change", "[Advanced] This section contains values use to calculate how body temperature change");
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Low Temp. Slope",
                optionDesc: "Indicate how target body temperature change when environment temperature drop below minimum comfortable temperature (Default: -0.17)",
                optionGet: () => (float)ModConfig.GetInstance().LowTemperatureSlope,
                optionSet: value => ModConfig.GetInstance().LowTemperatureSlope = (double)value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "High Temp. Slope",
                optionDesc: "Indicate how target body temperature change when environment temperature rise above maximum comfortable temperature (Default: 0.09)",
                optionGet: () => (float)ModConfig.GetInstance().HighTemperatureSlope,
                optionSet: value => ModConfig.GetInstance().HighTemperatureSlope = (double)value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Temp. Change Easing",
                optionDesc: "Indicate how fast player body temperature change to environment temperature, the lower the slower, should be between 0 and 1 (Default: 0.5)",
                optionGet: () => (float)ModConfig.GetInstance().TemperatureChangeEasing,
                optionSet: value => ModConfig.GetInstance().TemperatureChangeEasing = (double)value
            );

            api.StartNewPage(context.ModManifest, "Custom Buff / Debuff");
            api.RegisterParagraph(context.ModManifest, "Options for mod's custom buff and debuff conditions and mechanics");
            api.RegisterLabel(context.ModManifest, "Effect Chance", "This section contains values use to calculate chance of certain effect apply to player");
            api.RegisterClampedOption(
                mod: context.ModManifest,
                optionName: "% Fever Chance",
                optionDesc: "Define the chance of player catching a fever each new day (Default: 2)",
                optionGet: () => (float)ModConfig.GetInstance().PercentageChanceGettingFever,
                optionSet: value => ModConfig.GetInstance().PercentageChanceGettingFever = (double)value,
                min: 0f,
                max: 100f,
                interval: 0.1f
            );
            api.RegisterClampedOption(
                mod: context.ModManifest,
                optionName: "% Fever Additional Chance",
                optionDesc: "Define the additional chance of player catching a fever each new day by how little stamina they have left at the end of the day (Default: 8)",
                optionGet: () => (float)ModConfig.GetInstance().AdditionalPercentageChanceGettingFever,
                optionSet: value => ModConfig.GetInstance().AdditionalPercentageChanceGettingFever = (double)value,
                min: 0f,
                max: 100f,
                interval: 0.1f
            );
            api.RegisterClampedOption(
                mod: context.ModManifest,
                optionName: "% Stomachache Chance",
                optionDesc: "Define the additional chance of player haivng a stomachache after eating any non-cooked, non-procduct foods (Default: 3)",
                optionGet: () => (float)ModConfig.GetInstance().PercentageChanceGettingStomachache,
                optionSet: value => ModConfig.GetInstance().PercentageChanceGettingStomachache = (double)value,
                min: 0f,
                max: 100f,
                interval: 0.1f
            );

            api.RegisterLabel(context.ModManifest, "Effect Values", "This section contains values use to decide how effects should be applied to player");
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Heatstroke Thirst Drain",
                optionDesc: "Indicate how much hydration value player loss when they have a heatstroke every second (Default: 0.8)",
                optionGet: () => (float)ModConfig.GetInstance().HeatstrokeThirstDrainPerSecond,
                optionSet: value => ModConfig.GetInstance().HeatstrokeThirstDrainPerSecond = (double)value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "% Additional Energy Use if Fever",
                optionDesc: "Define how much more energy spent for actions if player is having a Fever (Default: 200)",
                optionGet: () => (float)ModConfig.GetInstance().AdditionalPercentageStaminaDrainOnFever,
                optionSet: value => ModConfig.GetInstance().AdditionalPercentageStaminaDrainOnFever = (double)value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "% Hunger Drain on Stomachache",
                optionDesc: "Indicate how much hunger value player loss when they have a stomachache every second (Default: 1)",
                optionGet: () => (float)ModConfig.GetInstance().StomachacheHungerPercentageDrainPerSecond,
                optionSet: value => ModConfig.GetInstance().StomachacheHungerPercentageDrainPerSecond = (double)value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Health Drain on Burn",
                optionDesc: "How much health player lose every second if they are on Burn effect (Default: 3)",
                optionGet: () => ModConfig.GetInstance().HealthDrainOnBurnPerSecond,
                optionSet: value => ModConfig.GetInstance().HealthDrainOnBurnPerSecond = value
            );
            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Health Drain on Frostbite",
                optionDesc: "How much health player lose every second if they are on Frostbite effect (Default: 3)",
                optionGet: () => ModConfig.GetInstance().HealthDrainOnFrostbitePerSecond,
                optionSet: value => ModConfig.GetInstance().HealthDrainOnFrostbitePerSecond = value
            );

            api.StartNewPage(context.ModManifest, "Stamina / HP Rework");
            api.RegisterParagraph(context.ModManifest, "Options for mod's stamina rework");
            api.AddKeybind(
                mod: context.ModManifest,
                name: () => "Sprint Button",
                tooltip: () => "Keybind to sprint - Only available with stamina rework option enabled",
                getValue: () => ModConfig.GetInstance().SprintButton,
                setValue: value => ModConfig.GetInstance().SprintButton = value
            );

            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Health Restore on Sleep",
                optionDesc: "How much health player restore when they sleep (Default: 20)",
                optionGet: () => ModConfig.GetInstance().HealthRestoreOnSleep,
                optionSet: value => ModConfig.GetInstance().HealthRestoreOnSleep = value
            );

            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Stamina Regen. on Not Moving",
                optionDesc: "How much stamina player restore every second when they are not moving (Default: 2)",
                optionGet: () => ModConfig.GetInstance().StaminaRegenOnNotMovingPerSecond,
                optionSet: value => ModConfig.GetInstance().StaminaRegenOnNotMovingPerSecond = value
            );

            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Stamina Regen. on Sitting",
                optionDesc: "How much extra stamina player restore every second when they are sitting (Default: 1)",
                optionGet: () => ModConfig.GetInstance().StaminaExtraRegenOnSittingPerSecond,
                optionSet: value => ModConfig.GetInstance().StaminaExtraRegenOnSittingPerSecond = value
            );

            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Stamina Regen. on Napping",
                optionDesc: "How much extra stamina player restore every second when they are napping (Default: 2)",
                optionGet: () => ModConfig.GetInstance().StaminaExtraRegenOnNappingPerSecond,
                optionSet: value => ModConfig.GetInstance().StaminaExtraRegenOnNappingPerSecond = value
            );

            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Additional % Stamina Drain on Tool Use",
                optionDesc: "How much extra stamina player lose in percentage when they use a tool (Default: 200 - 3x)",
                optionGet: () => (float)ModConfig.GetInstance().AdditionalDrainOnToolUse,
                optionSet: value => ModConfig.GetInstance().AdditionalDrainOnToolUse = (double)value
            );

            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Stamina Drain on Running",
                optionDesc: "How much stamina player lose every tick when they are running (Default: 0.01)",
                optionGet: () => ModConfig.GetInstance().StaminaDrainOnRunningPerTick,
                optionSet: value => ModConfig.GetInstance().StaminaDrainOnRunningPerTick = value
            );

            api.RegisterSimpleOption(
                mod: context.ModManifest,
                optionName: "Stamina Drain on Sprinting",
                optionDesc: "How much stamina player lose every tick when they are sprinting (Default: 0.05)",
                optionGet: () => ModConfig.GetInstance().StaminaDrainOnSprintingPerTick,
                optionSet: value => ModConfig.GetInstance().StaminaDrainOnSprintingPerTick = value
            );
        }
    }
}
