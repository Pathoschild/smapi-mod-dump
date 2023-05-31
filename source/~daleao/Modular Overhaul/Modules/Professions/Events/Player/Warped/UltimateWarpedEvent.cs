/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Events.Player;

#region using directives

using DaLion.Overhaul.Modules.Professions.Events.Display;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.Ultimates;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UltimateEvent]
[UsedImplicitly]
internal sealed class UltimateWarpedEvent : WarpedEvent
{
    /// <summary>Initializes a new instance of the <see cref="UltimateWarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal UltimateWarpedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (!ProfessionsModule.Config.EnableLimitBreaks)
        {
            this.Disable();
            return;
        }

        if (e.NewLocation.GetType() == e.OldLocation.GetType())
        {
            return;
        }

        if (e.NewLocation.IsDungeon())
        {
            this.Manager.Enable<UltimateMeterRenderingHudEvent>();
        }
        else
        {
            e.Player.Get_Ultimate()!.ChargeValue = 0.0;
            this.Manager.Disable<UltimateMeterRenderingHudEvent>();
        }
    }
}
