/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using Common;
using Common.Data;
using Common.Events;
using Common.Extensions.Collections;
using Extensions;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using System.Linq;
using Ultimates;

#endregion using directives

[UsedImplicitly]
internal sealed class StaticSaveLoadedEvent : SaveLoadedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal StaticSaveLoadedEvent(ProfessionEventManager manager)
        : base(manager)
    {
        AlwaysHooked = true;
    }

    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object? sender, SaveLoadedEventArgs e)
    {
        // enable events
        Manager.HookForLocalPlayer();

        // load and initialize Ultimate index
        Log.T("Initializing Ultimate...");

        var superModeIndex = ModDataIO.ReadFrom(Game1.player, "UltimateIndex", UltimateIndex.None);
        switch (superModeIndex)
        {
            case UltimateIndex.None when Game1.player.professions.Any(p => p is >= 26 and < 30):
                Log.W($"{Game1.player.Name} is eligible for an Ultimate but is not currently registered to any. A default one will be chosen.");
                superModeIndex = (UltimateIndex)Game1.player.professions.First(p => p is >= 26 and < 30);
                ModDataIO.WriteTo(Game1.player, "UltimateIndex", superModeIndex.ToString());
                Log.W($"{Game1.player.Name}'s Ultimate was set to {superModeIndex}.");

                break;

            case > UltimateIndex.None when !Game1.player.professions.Contains((int)superModeIndex):
                Log.W($"Missing corresponding profession for {superModeIndex} Ultimate. Resetting to a default value.");
                if (Game1.player.professions.Any(p => p is >= 26 and < 30))
                {
                    superModeIndex = (UltimateIndex)Game1.player.professions.First(p => p is >= 26 and < 30);
                    ModDataIO.WriteTo(Game1.player, "UltimateIndex", superModeIndex.ToString());
                }
                else
                {
                    superModeIndex = UltimateIndex.None;
                    ModDataIO.WriteTo(Game1.player, "UltimateIndex", null);
                }

                break;
        }

        if (superModeIndex > UltimateIndex.None)
        {
#pragma warning disable CS8509
            ModEntry.PlayerState.RegisteredUltimate = superModeIndex switch
#pragma warning restore CS8509
            {
                UltimateIndex.BruteFrenzy => new UndyingFrenzy(),
                UltimateIndex.PoacherAmbush => new Ambush(),
                UltimateIndex.PiperPandemic => new Pandemic(),
                UltimateIndex.DesperadoBlossom => new DeathBlossom()
            };
        }

        // revalidate levels
        Game1.player.RevalidateLevels();

        // revalidate fish pond populations
        Game1.getFarm().buildings.OfType<FishPond>()
            .Where(p => (p.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer) &&
                        !p.isUnderConstruction()).ForEach(p => p.UpdateMaximumOccupancy());

        // prepare to check for prestige achievement
        Manager.Hook<PrestigeAchievementOneSecondUpdateTickedEvent>();
    }
}