/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using AchtuurCore.Framework;
using AchtuurCore.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace HoverLabels.Labels.Objects;
internal class SprinklerLabel : ObjectLabel
{
    public SprinklerLabel(int? priority = null) : base(priority)
    {
    }

    public override bool ShouldGenerateLabel(Vector2 cursorTile)
    {
        SObject sobj = GetCursorObject(cursorTile);
        return sobj is not null
            && sobj.IsSprinkler();
    }

    public override void GenerateLabel()
    {
        base.GenerateLabel();

        Description.Add(I18n.LabelSprinklerTilesWatered(GetWateredTileCrops().Count()));
        Description.Add(I18n.LabelShowrange(ModEntry.GetShowDetailButtonName()));
    }

    private IEnumerable<Vector2> GetWateredTiles()
    {
        foreach (Vector2 tile in hoverObject.GetSprinklerTiles())
        {
            if (!Game1.currentLocation.terrainFeatures.ContainsKey(tile))
                continue;

            if (Game1.currentLocation.terrainFeatures[tile] is HoeDirt)
                yield return tile;
        }
    }

    private IEnumerable<Vector2> GetWateredTileCrops()
    {
        foreach (Vector2 tile in hoverObject.GetSprinklerTiles())
        {
            if (!Game1.currentLocation.terrainFeatures.ContainsKey(tile))
                continue;

            if (Game1.currentLocation.terrainFeatures[tile] is HoeDirt hoeDirt)
            {
                if (hoeDirt.crop is not null && !hoeDirt.crop.dead.Value)
                    yield return tile;
            }
        }
    }

    public override void DrawOnOverlay(SpriteBatch spriteBatch)
    {
        if (ModEntry.IsShowDetailButtonPressed())
        {
            IEnumerable<Vector2> sprinklerRange = hoverObject.GetSprinklerTiles();
            Overlay.DrawTiles(spriteBatch, sprinklerRange, tileTexture: Overlay.GreenTilePlacementTexture);
        }

        IEnumerable<Vector2> CropTiles = GetWateredTileCrops();
        Overlay.DrawTiles(spriteBatch, CropTiles, tileTexture: Overlay.GreenTilePlacementTexture);
    }
}
