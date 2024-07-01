/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Events.Display.RenderedHud;

#region using directives

using DaLion.Shared.Events;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.TerrainFeatures;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ScavengerRenderedHudEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class ScavengerRenderedHudEvent(EventManager? manager = null)
    : RenderedHudEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnRenderedHudImpl(object? sender, RenderedHudEventArgs e)
    {
        if (Config.DisableAlwaysTrack && !Config.ModKey.IsDown())
        {
            return;
        }

        var shouldHighlightOnScreen = Config.ModKey.IsDown();

        // track spawned objects
        foreach (var (tile, @object) in Game1.currentLocation.Objects.Pairs)
        {
            if (@object.ShouldBeTrackedBy(Profession.Scavenger))
            {
                tile.TrackWhenOffScreen(Color.Yellow);
                if (shouldHighlightOnScreen)
                {
                    tile.TrackWhenOnScreen(Color.Yellow);
                }
            }
            else if (@object.QualifiedItemId == QualifiedObjectIds.ArtifactSpot)
            {
                tile.TrackWhenOffScreen(Color.Lime);
                if (shouldHighlightOnScreen)
                {
                    tile.TrackWhenOnScreen(Color.Lime);
                }
            }
        }

        // track berries
        foreach (var feature in Game1.currentLocation.largeTerrainFeatures)
        {
            if (feature is not Bush bush || bush.townBush.Value || bush.tileSheetOffset.Value != 1 ||
                !bush.inBloom())
            {
                continue;
            }

            var tile = bush.Tile + new Vector2(0.5f, -1f);
            tile.TrackWhenOffScreen(Color.Yellow);
            if (shouldHighlightOnScreen)
            {
                tile.TrackWhenOnScreen(Color.Yellow);
            }
        }

        // track ginger
        foreach (var feature in Game1.currentLocation.terrainFeatures.Values)
        {
            if (feature is not HoeDirt { crop: { } crop } dirt || !crop.forageCrop.Value)
            {
                continue;
            }

            dirt.Tile.TrackWhenOffScreen(Color.Yellow);
            if (shouldHighlightOnScreen)
            {
                dirt.Tile.TrackWhenOnScreen(Color.Yellow);
            }
        }

        // track coconuts
        foreach (var feature in Game1.currentLocation.terrainFeatures.Values)
        {
            if (feature is not Tree tree || !tree.hasSeed.Value || tree.treeType.Value != Tree.palmTree)
            {
                continue;
            }

            tree.Tile.TrackWhenOffScreen(Color.Yellow);
            if (shouldHighlightOnScreen)
            {
                tree.Tile.TrackWhenOnScreen(Color.Yellow);
            }
        }
    }
}
