/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using Common;
using Common.Events;
using Common.Extensions.Collections;
using Common.Extensions.Stardew;
using Extensions;
using StardewModdingAPI.Events;
using StardewValley.Buildings;
using System.Linq;
using Ultimates;
using VirtualProperties;

#endregion using directives

[UsedImplicitly]
internal sealed class StaticSaveLoadedEvent : SaveLoadedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal StaticSaveLoadedEvent(ProfessionEventManager manager)
        : base(manager)
    {
        AlwaysEnabled = true;
    }

    /// <inheritdoc />
    public override bool Enable() => false;

    /// <inheritdoc />
    public override bool Disable() => false;

    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object? sender, SaveLoadedEventArgs e)
    {
        var player = Game1.player;

        // enable events
        ModEntry.Events.EnableForLocalPlayer();

        // load and initialize Ultimate index
        Log.T("Initializing Ultimate...");

        var ultimateIndex = Game1.player.Read("UltimateIndex", UltimateIndex.None);
        switch (ultimateIndex)
        {
            case UltimateIndex.None when player.professions.Any(p => p is >= 26 and < 30):
                Log.W($"{player.Name} is eligible for an Ultimate but is not currently registered to any. A default one will be chosen.");
                ultimateIndex = (UltimateIndex)player.professions.First(p => p is >= 26 and < 30);
                Log.W($"{player.Name}'s Ultimate was set to {ultimateIndex}.");

                break;

            case > UltimateIndex.None when !player.professions.Contains((int)ultimateIndex):
                Log.W($"Missing corresponding profession for {ultimateIndex} Ultimate. Resetting to a default value.");
                if (player.professions.Any(p => p is >= 26 and < 30))
                    ultimateIndex = (UltimateIndex)player.professions.First(p => p is >= 26 and < 30);
                else
                    ultimateIndex = UltimateIndex.None;

                break;
        }

        if (ultimateIndex > UltimateIndex.None)
            Game1.player.set_Ultimate(Ultimate.FromIndex(ultimateIndex));

        // revalidate levels
        Game1.player.RevalidateLevels();

        // revalidate fish pond populations
        Game1.getFarm().buildings.OfType<FishPond>()
            .Where(p => (p.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer) &&
                        !p.isUnderConstruction()).ForEach(p => p.UpdateMaximumOccupancy());

        // prepare to check for prestige achievement
        Manager.Enable<PrestigeAchievementOneSecondUpdateTickedEvent>();
    }
}