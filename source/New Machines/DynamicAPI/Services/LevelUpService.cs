using System;
using System.Collections.Generic;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Extensions;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace Igorious.StardewValley.DynamicAPI.Services
{
    public sealed class LevelUpService
    {
        private static LevelUpService _instance;

        private LevelUpService()
        {
            MenuEvents.MenuChanged += (s, e) =>
            {
                var levelUpMenu = e.NewMenu as LevelUpMenu;
                if (levelUpMenu == null) return;
                OnLevelUp(levelUpMenu);
            };
        }

        public static LevelUpService Instance => _instance ?? (_instance = new LevelUpService());

        public void Register() { }

        private void OnLevelUp(LevelUpMenu levelUpMenu)
        {
            var overHeight = (levelUpMenu.height + Game1.tileSize) - Game1.viewport.Height;
            if (overHeight <= 0) return;

            var newCraftingRecipes = levelUpMenu.GetField<List<CraftingRecipe>>("newCraftingRecipes");

            var leftRecipes = newCraftingRecipes.Count;
            for (var i = leftRecipes - 1; i >= 0; --i)
            {
                overHeight -= (newCraftingRecipes[i].bigCraftable? 2 : 1) * Game1.tileSize;
                --leftRecipes;

                if (overHeight <= 0) break;
            }

            var copy = newCraftingRecipes.ToList();
            newCraftingRecipes.Clear();
            newCraftingRecipes.AddRange(copy.Take(leftRecipes));

            var textRecipes = copy.Skip(leftRecipes).Select(r => r.name).ToList();
            var extraInfo = $"New recipes for {string.Join(", ", textRecipes)}";
            var extraInfoForLevel = levelUpMenu.GetField<List<string>>("extraInfoForLevel");
            extraInfoForLevel.Add(extraInfo);
            overHeight += Game1.tileSize * 3 / 4;

            var textWidth = Game1.smallFont.MeasureString(extraInfo).X;
            var calcWidth = (int)Math.Min(Game1.viewport.Width - 3 * Game1.tileSize - 2 * IClickableMenu.borderWidth, textWidth + 2 * IClickableMenu.borderWidth);
            if (textWidth > levelUpMenu.width)
            {
                levelUpMenu.width = calcWidth;
            }

            levelUpMenu.height = Game1.viewport.Height - Game1.tileSize + overHeight;
            levelUpMenu.gameWindowSizeChanged(Rectangle.Empty, Rectangle.Empty);
        }
    }
}