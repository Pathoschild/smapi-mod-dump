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
using FishCatalogue.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace FishCatalogue.Drawing;
internal class FishHUD
{
    static Color UndiscoveredColor = new Color(0, 0, 0, 50);
    static Color UncaughtColor = new Color(72, 72, 72, 150);

    public bool Enabled;
    BorderDrawer borderDrawer;
    public FishHUD()
    {
        borderDrawer = new();
    }

    internal void Enable()
    {
        Enabled = true;
    }

    internal void Disable()
    {
        Enabled = false;
    }

    internal void Toggle()
    {
        Enabled = !Enabled;
    }

    public void Draw(SpriteBatch sb, Vector2 position, float alpha)
    {
        if (!Enabled)
            return;
        borderDrawer.Draw(sb, position);
    }

    public void Reset()
    {
        borderDrawer.Reset();
    }

    public void AddAvailableFishBorder(IEnumerable<FishData> AvailableFish)
    {
        IEnumerable<ItemLabel> itemLabels = AvailableFish
            .OrderBy(fish => fish.FishItem.Name)
            .Select(fish => GenerateItemLabel(fish));

        if (itemLabels.Count() == 0)
            return;

        GridLabel grid_label = new(itemLabels);
        grid_label.SetNumberOfColumns(ModEntry.Instance.Config.HUD_Columns);
        borderDrawer.AddBorder(new Border(grid_label));
    }

    private void AddUnavailableFishBorder(IEnumerable<FishData> AvailableFish, IEnumerable<FishData> CurrentLocationFish)
    {
        IEnumerable<IEnumerable<Label>> itemLabels = CurrentLocationFish
            .Except(AvailableFish)
            .Where(fish => fish.CanBeCaughtThisSeason())
            .OrderBy(fish => fish.FishItem.Name)
            .Select(fish => fish.GenerateUnfulfilledConditionLabel());

        if (itemLabels.Count() == 0)
            return;
        // Make sure all labels have the same number of columns
        // To align the grid
        int max_columns = itemLabels.Max(label => label.Count());
        itemLabels = itemLabels.Select(label => label
        .Concat(Enumerable.Repeat(new EmptyLabel(), max_columns - label.Count())));

        GridLabel grid_label = new(itemLabels.SelectMany(l => l));
        grid_label.SetNumberOfColumns(max_columns);
        borderDrawer.AddBorder(new Border(grid_label));
    }

    private ItemLabel GenerateItemLabel(FishData fish)
    {
        ItemLabel itemLabel = new ItemLabel(fish.FishItem);

        // fish caught -> dont do anything special
        if (fish.IsCaughtByPlayer()) 
            return itemLabel;

        if (ModEntry.Instance.Config.HideUncaughtFish)
        {
            itemLabel.SetText("???");
            itemLabel.SetColor(UndiscoveredColor);
        } 
        else
        {
            itemLabel.SetColor(UncaughtColor);
        }
        if (!ModEntry.Instance.Config.ShowFishNames)
            itemLabel.HideDescription();

        return itemLabel;
    }

    
}
