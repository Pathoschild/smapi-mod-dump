/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using System.Linq;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using TheLion.Stardew.Common.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events;

[UsedImplicitly]
internal class DebugButtonsChangedEvent : ButtonsChangedEvent
{
    /// <inheritdoc />
    public override void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
    {
        if (!ModEntry.Config.DebugKey.IsDown() ||
            !e.Pressed.Any(b => b is SButton.MouseRight or SButton.MouseLeft)) return;
        ModEntry.Log($"{e.Cursor.GetScaledScreenPixels()}", LogLevel.Debug);

        if (DebugRenderedActiveMenuEvent.FocusedComponent is null) return;
        var component = DebugRenderedActiveMenuEvent.FocusedComponent;
        var name = string.IsNullOrEmpty(component.name) ? "Anon" : component.name;
        var message = $"{component.myID} : {name} ({component.GetType().Name})";
        message = component.GetType().GetFields().Where(f => !f.Name.IsAnyOf("myID", "name")).Aggregate(message,
            (current, field) => current + $"\n\t- {field.Name}: {field.GetValue(component)}");
        ModEntry.Log(message, LogLevel.Debug);
    }
}