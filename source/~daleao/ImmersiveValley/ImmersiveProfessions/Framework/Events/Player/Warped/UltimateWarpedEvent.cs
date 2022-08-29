/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.Player;

#region using directives

using Common.Events;
using Display;
using Extensions;
using StardewModdingAPI.Events;
using Ultimates;
using VirtualProperties;

#endregion using directives

[UsedImplicitly, UltimateEvent]
internal sealed class UltimateWarpedEvent : WarpedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal UltimateWarpedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (e.NewLocation.GetType() == e.OldLocation.GetType()) return;

        if (e.NewLocation.IsDungeon())
        {
            Manager.Enable<UltimateMeterRenderingHudEvent>();
        }
        else
        {
            e.Player.get_Ultimate()!.ChargeValue = 0.0;
            Manager.Disable<UltimateMeterRenderingHudEvent>();
        }
    }
}