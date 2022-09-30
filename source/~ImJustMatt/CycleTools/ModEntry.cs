/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.CycleTools;

using System;
using System.Reflection;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI.Events;
using StardewMods.Common.Helpers;
using StardewMods.CycleTools.Framework;

/// <inheritdoc />
public sealed class ModEntry : Mod
{
    private static readonly FieldInfo MouseStateField = typeof(InputState).GetField(
        "_currentMouseState",
        BindingFlags.Instance | BindingFlags.NonPublic)!;

    private static readonly FieldInfo MouseWheelScrolledEventArgsOldValueField =
        typeof(MouseWheelScrolledEventArgs).GetField(
            "<OldValue>k__BackingField",
            BindingFlags.Instance | BindingFlags.NonPublic)!;

    private static readonly FieldInfo ScrollWheelValueField = typeof(MouseState).GetField(
        "_scrollWheelValue",
        BindingFlags.Instance | BindingFlags.NonPublic)!;

    private ModConfig? _config;

    private ModConfig Config => this._config ??= CommonHelpers.GetConfig<ModConfig>(this.Helper);

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        Log.Monitor = this.Monitor;
        I18n.Init(this.Helper.Translation);

        // Events
        this.Helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        this.Helper.Events.Input.MouseWheelScrolled += this.OnMouseWheelScrolled;
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        if (!Integrations.GMCM.IsLoaded)
        {
            return;
        }

        Integrations.GMCM.Register(
            this.ModManifest,
            () => this._config = new(),
            () => this.Helper.WriteConfig(this.Config));

        Integrations.GMCM.API.AddKeybindList(
            this.ModManifest,
            () => this.Config.ModifierKey,
            value => this.Config.ModifierKey = value,
            I18n.Config_ModifierKey_Name,
            I18n.Config_ModifierKey_Tooltip);
    }

    private void OnMouseWheelScrolled(object? sender, MouseWheelScrolledEventArgs e)
    {
        if (!this.Config.ModifierKey.IsDown())
        {
            return;
        }

        // Cycle Tool from active object
        Item? firstItem = null;
        var firstIndex = -1;
        var lastIndex = -1;
        var start = e.Delta < 0 ? 0 : Math.Min(Game1.player.MaxItems, Game1.player.Items.Count) - 1;
        var end = e.Delta < 0 ? Math.Min(Game1.player.MaxItems, Game1.player.Items.Count) : -1;
        var delta = e.Delta < 0 ? 1 : -1;
        for (var i = start; i != end; i += delta)
        {
            if (i != Game1.player.CurrentToolIndex && Game1.player.Items[i] is not Tool)
            {
                continue;
            }

            if (firstIndex == -1)
            {
                firstIndex = i;
                firstItem = Game1.player.Items[i];
                lastIndex = i;
                continue;
            }

            Game1.player.Items[lastIndex] = Game1.player.Items[i];
            lastIndex = i;
        }

        if (lastIndex != firstIndex && lastIndex != -1)
        {
            Game1.player.Items[lastIndex] = firstItem;
        }

        // Suppress Mouse State
        Game1.oldMouseState = Game1.input.GetMouseState();

        // Suppress Input from subsequent SMAPI events
        ModEntry.MouseWheelScrolledEventArgsOldValueField.SetValue(e, e.NewValue);
    }
}