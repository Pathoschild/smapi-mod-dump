/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using AchtuurCore.Extensions;
using HoverLabels.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;
using StardewValley.Objects;
using HoverLabels.Drawing;

namespace HoverLabels.Labels.Objects;
internal class ObjectLabel : BaseLabel
{
    protected SObject hoverObject;

    /// <summary>
    /// Top tile of big-craftable
    /// </summary>
    protected Vector2? CursorTileTop;

    public ObjectLabel(int? priority = null) : base(priority)
    {
    }

    /// <inheritdoc/>
    public override void GenerateLabel()
    {
        if (hoverObject is null)
            return;

        AddBorder(new TitleLabelText(hoverObject.DisplayName));
    }

    /// <summary>
    /// Returns whether a new label should be created based on <paramref name="cursorTile"/>
    /// </summary>
    /// <param name="cursorTile"></param>
    /// <returns></returns>
    public override bool ShouldGenerateLabel(Vector2 cursorTile)
    {
        SObject sobj = GetCursorObject(cursorTile);
        return sobj is not null;
    }

    public override void SetCursorTile(Vector2 cursorTile)
    {
        hoverObject = GetCursorObject(cursorTile);

        if (hoverObject is null)
            return;

        // big craftables have 2 tiles, so we need to check if the cursor is on the top or bottom tile
        if (hoverObject.TileLocation == cursorTile)
        {
            CursorTile = cursorTile;
            CursorTileTop = cursorTile - Vector2.UnitY;
        }
        else
        {
            CursorTileTop = cursorTile;
            CursorTile = cursorTile + Vector2.UnitY;
        }
    }

    protected static SObject GetCursorObject(Vector2 cursorTile)
    {

        if (Game1.currentLocation.isObjectAtTile(cursorTile))
            return Game1.currentLocation.getObjectAtTile(cursorTile);


        // if pointing at top of big-craftable, return it
        if (Game1.currentLocation.isObjectAtTile(cursorTile + Vector2.UnitY))
        {
            SObject sobj = Game1.currentLocation.getObjectAtTile(cursorTile + Vector2.UnitY);
            if (sobj.bigCraftable.Value)
                return sobj;
        }
        return null;
    }

    internal static SObject GetObjectWithId(string item_id)
    {
        return new SObject(item_id, 1);
    }
}
