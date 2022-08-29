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

using Common;
using Common.Events;
using Extensions;
using GameLoop;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.Locations;
using StardewValley.Monsters;
using System;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class PiperWarpedEvent : WarpedEvent
{
    private readonly Func<int, double> _pipeChance = x => 19f / (x + 18f);

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal PiperWarpedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        var isDungeon = e.NewLocation.IsDungeon();
        var hasMonsters = e.NewLocation.HasMonsters();
        if (!isDungeon && !hasMonsters)
        {
            Manager.Enable<PiperUpdateTickedEvent>();
            return;
        }

        if (!isDungeon || e.NewLocation is MineShaft shaft1 && shaft1.isLevelSlimeArea())
            return;

        var enemyCount = Game1.player.currentLocation.characters.OfType<Monster>().Count(m => !m.IsSlime());
        if (enemyCount <= 0) return;

        var r = new Random(Guid.NewGuid().GetHashCode());
        var raisedSlimes = e.Player.GetRaisedSlimes().ToArray();
        var chance = _pipeChance(raisedSlimes.Length);
        var pipedCount = 0;
        foreach (var tamedSlime in raisedSlimes)
        {
            if (r.NextDouble() > chance) continue;

            // choose spawn tile
            var x = r.Next(e.NewLocation.Map.Layers[0].LayerWidth);
            var y = r.Next(e.NewLocation.Map.Layers[0].LayerHeight);
            var spawnTile = new Vector2(x, y);

            if (!e.NewLocation.isTileOnMap(spawnTile) ||
                !e.NewLocation.isTileLocationTotallyClearAndPlaceable(spawnTile)) continue;

            if (e.NewLocation is MineShaft shaft2)
            {
                shaft2.checkForMapAlterations(x, y);
                if (!shaft2.isTileClearForMineObjects(spawnTile)) continue;
            }

            // choose slime variation
            GreenSlime pipedSlime;
            switch (e.NewLocation)
            {
                case MineShaft shaft:
                    {
                        pipedSlime = new(Vector2.Zero, shaft.mineLevel);
                        if (shaft.GetAdditionalDifficulty() > 0 &&
                            r.NextDouble() < Math.Min(shaft.GetAdditionalDifficulty() * 0.1f, 0.5f))
                            pipedSlime.stackedSlimes.Value = r.NextDouble() < 0.0099999997764825821 ? 4 : 2;

                        shaft.BuffMonsterIfNecessary(pipedSlime);
                        break;
                    }
                case Woods:
                    {
                        pipedSlime = Game1.currentSeason switch
                        {
                            "fall" => new(Vector2.Zero, r.NextDouble() < 0.5 ? 40 : 0),
                            "winter" => new(Vector2.Zero, 40),
                            _ => new(Vector2.Zero, 0)
                        };
                        break;
                    }
                case VolcanoDungeon:
                    {
                        pipedSlime = new(Vector2.Zero, 0);
                        pipedSlime.makeTigerSlime();
                        break;
                    }
                default:
                    {
                        pipedSlime = new(Vector2.Zero, 121);
                        break;
                    }
            }

            // adjust color
            if (tamedSlime.Name == "Tiger Slime" && pipedSlime.Name != tamedSlime.Name)
            {
                pipedSlime.makeTigerSlime();
            }
            else
            {
                pipedSlime.color.R = (byte)(tamedSlime.color.R + r.Next(-20, 21));
                pipedSlime.color.G = (byte)(tamedSlime.color.G + r.Next(-20, 21));
                pipedSlime.color.B = (byte)(tamedSlime.color.B + r.Next(-20, 21));
            }

            // spawn
            pipedSlime.setTileLocation(spawnTile);
            e.NewLocation.characters.Add(pipedSlime);
            if (++pipedCount >= enemyCount) break;
        }

        Log.D($"Spawned {pipedCount} Slimes after {raisedSlimes.Length} attempts.");

        if (pipedCount > 0 || e.NewLocation.characters.Any(npc => npc is GreenSlime))
            Manager.Enable<PiperUpdateTickedEvent>();
    }
}