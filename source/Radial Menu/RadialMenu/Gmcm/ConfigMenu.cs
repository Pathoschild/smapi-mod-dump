/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/focustense/StardewRadialMenu
**
*************************************************/

using RadialMenu.Config;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace RadialMenu.Gmcm;

internal class ConfigMenu(
    IGenericModMenuConfigApi gmcm,
    IGMCMOptionsAPI? gmcmOptions,
    GenericModConfigKeybindings? gmcmBindings,
    GenericModConfigSync? gmcmSync,
    IManifest mod,
    ITranslationHelper translations,
    IModContentHelper modContent,
    TextureHelper textureHelper,
    IGameLoopEvents gameLoopEvents,
    Func<Configuration> getConfig)
{
    protected Configuration Config => getConfig();

    // Primary ctor properties can't be read-only and we're OCD.
    private readonly IGenericModMenuConfigApi gmcm = gmcm;
    private readonly IGMCMOptionsAPI? gmcmOptions = gmcmOptions;
    private readonly IManifest mod = mod;
    private readonly ITranslationHelper translations = translations;
    private readonly Func<Configuration> getConfig = getConfig;

    // Sub-pages
    private readonly CustomMenuPage customMenuPage = new(
        gmcm, gmcmBindings, gmcmSync, mod, translations, textureHelper, gameLoopEvents,
        getConfig);
    private readonly StylePage stylePage =
        new(gmcm, gmcmOptions, mod, modContent, translations, () => getConfig().Styles);

    public void Setup()
    {
        AddMainOptions();
        customMenuPage.Setup();
        stylePage.Setup();
    }

    private void AddMainOptions()
    {
        gmcm.AddSectionTitle(
            mod,
            text: () => translations.Get("gmcm.controls"));
        gmcm.AddNumberOption(
            mod,
            name: () => translations.Get("gmcm.controls.trigger.deadzone"),
            tooltip: () => translations.Get("gmcm.controls.trigger.deadzone.tooltip"),
            getValue: () => Config.TriggerDeadZone,
            setValue: value => Config.TriggerDeadZone = value,
            min: 0.0f,
            max: 1.0f);
        gmcm.AddBoolOption(
            mod,
            name: () => translations.Get("gmcm.controls.trigger.swap"),
            tooltip: () => translations.Get("gmcm.controls.trigger.swap.tooltip"),
            getValue: () => Config.SwapTriggers,
            setValue: value => Config.SwapTriggers = value);
        AddEnumOption(
            "gmcm.controls.thumbstick.preference",
            getValue: () => Config.ThumbStickPreference,
            setValue: value => Config.ThumbStickPreference = value);
        gmcm.AddNumberOption(
            mod,
            name: () => translations.Get("gmcm.controls.thumbstick.deadzone"),
            tooltip: () => translations.Get("gmcm.controls.thumbstick.deadzone.tooltip"),
            getValue: () => Config.ThumbStickDeadZone,
            setValue: value => Config.ThumbStickDeadZone = value,
            min: 0.0f,
            max: 1.0f);
        AddEnumOption(
            "gmcm.controls.activation",
            getValue: () => Config.PrimaryActivation,
            setValue: value => Config.PrimaryActivation = value);
        AddEnumOption(
            "gmcm.controls.action.primary",
            "gmcm.controls.action.type",
            () => Config.PrimaryAction,
            value => Config.PrimaryAction = value);
        gmcm.AddKeybind(
            mod,
            name: () => translations.Get("gmcm.controls.action.secondary.button"),
            tooltip: () => translations.Get("gmcm.controls.action.secondary.button.tooltip"),
            getValue: () => Config.SecondaryActionButton,
            setValue: value => Config.SecondaryActionButton = value);
        AddEnumOption(
            "gmcm.controls.action.secondary",
            "gmcm.controls.action.type",
            () => Config.SecondaryAction,
            value => Config.SecondaryAction = value);
        gmcm.AddNumberOption(
            mod,
            name: () => translations.Get("gmcm.controls.activation.delay"),
            tooltip: () => translations.Get("gmcm.controls.activation.delay.tooltip"),
            getValue: () => Config.ActivationDelayMs,
            setValue: value => Config.ActivationDelayMs = value,
            formatValue: value => translations.Get(
                "gmcm.controls.activation.delay.value",
                new { value }),
            min: 0,
            max: 500);
        AddEnumOption(
            "gmcm.controls.activation.delay.actions",
            getValue: () => Config.DelayedActions,
            setValue: value => Config.DelayedActions = value);

        gmcm.AddSectionTitle(
            mod,
            text: () => translations.Get("gmcm.inventory"));
        gmcm.AddNumberOption(
            mod,
            name: () => translations.Get("gmcm.inventory.max"),
            tooltip: () => translations.Get("gmcm.inventory.max.tooltip"),
            getValue: () => Config.MaxInventoryItems,
            setValue: value => Config.MaxInventoryItems = value,
            // Any less than the size of a single backpack row (12) and some items become
            // effectively inaccessible without rearranging the inventory. We don't want that.
            min: 12,
            // Limiting this is less about balance and more about preventing overlap or crashes due
            // to the math not working out. If players really want to exceed the limit, they can
            // edit the config.json, but we won't encourage that in the CM.
            max: 24);

        gmcm.AddPageLink(
            mod,
            pageId: CustomMenuPage.ID,
            text: () => translations.Get("gmcm.custom.link"),
            tooltip: () => translations.Get("gmcm.custom.link.tooltip"));
        gmcm.AddPageLink(
            mod,
            pageId: StylePage.ID,
            text: () => translations.Get("gmcm.style.link"),
            tooltip: () => translations.Get("gmcm.style.link.tooltip"));
    }

    private void AddEnumOption<T>(
        string messageId,
        Func<T> getValue,
        Action<T> setValue)
    where T : struct, Enum
    {
        AddEnumOption(messageId, messageId, getValue, setValue);
    }

    private void AddEnumOption<T>(
        string messageId,
        string choiceIdPrefix,
        Func<T> getValue,
        Action<T> setValue)
    where T : struct, Enum
    {
        gmcm.AddTextOption(
            mod,
            name: () => translations.Get(messageId),
            tooltip: () => translations.Get($"{messageId}.tooltip"),
            getValue: () => getValue().ToString().ToLowerInvariant(),
            setValue: value => setValue(Enum.Parse<T>(value, true)),
            allowedValues: Enum.GetValues<T>()
                .Select(e => e.ToString().ToLowerInvariant())
                .ToArray(),
            formatAllowedValue: value => translations.Get($"{choiceIdPrefix}.{value}"));
    }
}