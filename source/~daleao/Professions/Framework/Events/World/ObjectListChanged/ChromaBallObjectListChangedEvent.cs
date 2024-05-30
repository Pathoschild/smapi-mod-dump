/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Events.World.ObjectListChanged;

#region using directives

using DaLion.Core;
using DaLion.Core.Framework;
using DaLion.Professions.Framework.Chroma;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ChromaBallObjectListChangedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class ChromaBallObjectListChangedEvent(EventManager? manager = null)
    : ObjectListChangedEvent(manager ?? ProfessionsMod.EventManager)
{
    protected override void OnEnabled()
    {
        CoreMod.EventManager.Disable<Core.Framework.Events.SlimeBallObjectListChangedEvent>();
    }

    protected override void OnDisabled()
    {
        CoreMod.EventManager.Enable<Core.Framework.Events.SlimeBallObjectListChangedEvent>();
    }

    /// <inheritdoc />
    protected override void OnObjectListChangedImpl(object? sender, ObjectListChangedEventArgs e)
    {
        if (!e.IsCurrentLocation || e.Location is not SlimeHutch hutch)
        {
            return;
        }

        foreach (var (key, value) in e.Removed)
        {
            if (value.QualifiedItemId != QualifiedBigCraftableIds.SlimeBall)
            {
                continue;
            }

            var drops = hutch.GetContainingBuilding().GetOwner().HasProfessionOrLax(Profession.Piper, true)
                ? new ChromaBall(value, key).GetDrops()
                : new SlimeBall(value, key).GetDrops();
            foreach (var (id, stack) in drops)
            {
                Game1.createMultipleObjectDebris(
                    id,
                    (int)key.X,
                    (int)key.Y,
                    stack,
                    1f + (Game1.player.FacingDirection == 2 ? 0f : (float)Game1.random.NextDouble()));
            }
        }
    }
}
