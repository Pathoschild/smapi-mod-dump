/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MissCoriel/Event-Repeater
**
*************************************************/

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace EventRepeater.Integrations;
internal sealed class GMCMHelper : IntegrationHelper
{
    private const string MINVERSION = "1.8.0";
    private const string APIID = "spacechase0.GenericModConfigMenu";

    private readonly IManifest manifest;

    private IGenericModConfigMenuApi? modMenuApi;

    /// <summary>
    /// Initializes a new instance of the <see cref="GMCMHelper"/> class.
    /// </summary>
    /// <param name="monitor">Logger.</param>
    /// <param name="translation">Translation helper.</param>
    /// <param name="modRegistry">Mod registry helper.</param>
    /// <param name="manifest">Mod's manifest.</param>
    public GMCMHelper(IMonitor monitor, ITranslationHelper translation, IModRegistry modRegistry, IManifest manifest)
        : base(monitor, translation, modRegistry)
    {
        this.manifest = manifest;
    }

    /// <summary>
    /// Tries to grab a copy of the API.
    /// </summary>
    /// <returns>True if successful, false otherwise.</returns>
    [MemberNotNullWhen(returnValue: true, members: nameof(modMenuApi))]
    public bool TryGetAPI() => this.TryGetAPI(APIID, MINVERSION, out this.modMenuApi);

    /// <summary>
    /// Register mod with GMCM.
    /// </summary>
    /// <param name="reset">Reset callback.</param>
    /// <param name="save">Save callback.</param>
    /// <param name="titleScreenOnly">Whether or not the config should only be availble from the title screen.</param>
    /// <returns>this.</returns>
    public GMCMHelper Register(Action reset, Action save, bool titleScreenOnly = false)
    {
        this.modMenuApi!.Register(
            mod: this.manifest,
            reset: reset,
            save: save,
            titleScreenOnly: titleScreenOnly);
        return this;
    }

    #region keybinds

    /// <summary>
    /// Adds a KeyBindList at this position in the form.
    /// </summary>
    /// <param name="name">Function to get the name.</param>
    /// <param name="getValue">GetValue callback.</param>
    /// <param name="setValue">SetValue callback.</param>
    /// <param name="tooltip">Function to get the tooltip.</param>
    /// <param name="fieldId">FieldID.</param>
    /// <returns>this.</returns>
    public GMCMHelper AddKeybindList(
        Func<string> name,
        Func<KeybindList> getValue,
        Action<KeybindList> setValue,
        Func<string>? tooltip = null,
        string? fieldId = null)
    {
        this.modMenuApi!.AddKeybindList(
            mod: this.manifest,
            name: name,
            getValue: getValue,
            setValue: setValue,
            tooltip: tooltip,
            fieldId: fieldId);
        return this;
    }

    /// <summary>
    /// Adds a keybindlist option at this point in the form.
    /// </summary>
    /// <typeparam name="TModConfig">ModConfig's type.</typeparam>
    /// <param name="property">Property to process.</param>
    /// <param name="getConfig">Function that gets the current config instance.</param>
    /// <param name="fieldID">fieldId.</param>
    /// <returns>this.</returns>
    public GMCMHelper AddKeybindList<TModConfig>(
        PropertyInfo property,
        Func<TModConfig> getConfig,
        string? fieldID = null)
    {
        if (property.GetGetMethod() is not MethodInfo getter || property.GetSetMethod() is not MethodInfo setter)
        {
            this.Monitor.Log($"{property.Name} appears to be a misconfigured option!", LogLevel.Warn);
        }
        else
        {
            Func<TModConfig, KeybindList> getterDelegate = getter.CreateDelegate<Func<TModConfig, KeybindList>>();
            Action<TModConfig, KeybindList> setterDelegate = setter.CreateDelegate<Action<TModConfig, KeybindList>>();
            this.AddKeybindList(
                name: () => this.Translation.Get($"{property.Name}.title"),
                tooltip: () => this.Translation.Get($"{property.Name}.description"),
                getValue: () => getterDelegate(getConfig()),
                setValue: value => setterDelegate(getConfig(), value),
                fieldId: fieldID);
        }
        return this;
    }
    #endregion
}
