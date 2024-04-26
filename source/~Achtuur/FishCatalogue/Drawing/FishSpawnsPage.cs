/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using AchtuurCore.Framework.Borders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishCatalogue.Drawing;
internal class FishSpawnsPage
{
    BorderDrawer borderDrawer;

    public FishSpawnsPage()
    {
        borderDrawer = new BorderDrawer();
    }


    public void Draw(SpriteBatch sb)
    {
        borderDrawer.Reset();
        borderDrawer.AddBorder(CreateBorders());
        Vector2 offset = borderDrawer.BorderSize() / 2;
        Vector2 screenDim = new Vector2(Game1.viewport.Size.Width, Game1.viewport.Size.Height) / 2;
        //Vector2 screenDim = new Vector2(Game1.viewport.X, Game1.viewport.Y) / 2;
        borderDrawer.Draw(sb, screenDim - offset);
    }

    private IEnumerable<Border> CreateBorders()
    {
        IEnumerable<IEnumerable<Label>> labels = FishCatalogue.AllFishData.Values
            .OrderBy(fish => fish.Name)
            .Where(fish => fish.CanBeCaughtThisSeason())
            .Select(fish => fish.GenerateSpawnConditionLabel());

        // Make sure all labels have the same number of columns
        // To align the grid
        int max_columns = labels.Max(label => label.Count());
        labels = labels.Select(label => label
        .Concat(Enumerable.Repeat(new EmptyLabel(), max_columns - label.Count())));

        GridLabel grid_lab = new GridLabel(labels.SelectMany(label => label));
        grid_lab.SetNumberOfColumns(max_columns * 3);
        grid_lab.SetFixedHeight(true);

        yield return new Border(grid_lab);
    }
}
