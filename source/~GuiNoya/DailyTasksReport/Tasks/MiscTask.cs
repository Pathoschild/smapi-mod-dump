using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DailyTasksReport.Tasks
{
    public class MiscTask : Task
    {
        private readonly ModConfig _config;

        private readonly Dictionary<string, string> _tvRecipes =
            Game1.content.Load<Dictionary<string, string>>("Data\\TV\\CookingChannel");

        private string _recipeOfTheDay;

        private NPC _birthdayNpc;

        private bool _isTravelingMerchantOpen;
        private bool _wasTravelingMerchantVisited;

        internal MiscTask(ModConfig config)
        {
            _config = config;
        }

        protected override void FirstScan()
        {
            if (Game1.locations.OfType<Forest>().First().travelingMerchantDay)
            {
                ModEntry.EventsHelper.Display.MenuChanged -= Display_MenuChanged;
                ModEntry.EventsHelper.Display.MenuChanged += Display_MenuChanged;
            }

            if ((Game1.dayOfMonth % 7 == 0 || Game1.dayOfMonth % 7 == 3) && Game1.stats.DaysPlayed > 5)
            {
                var recipeId = (int)(Game1.stats.DaysPlayed % 224 / 7);

                if (Game1.dayOfMonth % 7 == 3)
                    recipeId = Math.Max(1,
                        1 + new Random((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2).
                        Next((int)Game1.stats.DaysPlayed % 224) / 7);

                if (_tvRecipes.TryGetValue(recipeId.ToString(), out var value))
                {
                    var key = value.Split('/')[0];
                    if (CraftingRecipe.cookingRecipes.ContainsKey(key) && !Game1.player.knowsRecipe(key))
                        _recipeOfTheDay = key;
                }
            }

            foreach (var location in Game1.locations)
                foreach (var npc in location.characters)
                    if (npc.isBirthday(Game1.currentSeason, Game1.dayOfMonth))
                    {
                        _birthdayNpc = npc;
                        return;
                    }
        }

        private void Display_MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is ShopMenu && Game1.currentLocation is Forest &&
                Game1.player.GetGrabTile() == new Vector2(27, 11))
            {
                ModEntry.EventsHelper.Display.MenuChanged -= Display_MenuChanged;
                _wasTravelingMerchantVisited = true;
            }
        }

        private void Update()
        {
            if (Game1.locations.OfType<Forest>().First().travelingMerchantDay)
                _isTravelingMerchantOpen = Game1.timeOfDay < 2000;

            if (_birthdayNpc != null && Game1.player.friendshipData.TryGetValue(_birthdayNpc.Name, out var friendship) &&
                friendship.GiftsToday > 0)
                _birthdayNpc = null;

            if (_recipeOfTheDay.Length > 0 && Game1.player.knowsRecipe(_recipeOfTheDay))
                _recipeOfTheDay = "";
        }

        public override void Clear()
        {
            _isTravelingMerchantOpen = false;
            _wasTravelingMerchantVisited = false;
            ModEntry.EventsHelper.Display.MenuChanged -= Display_MenuChanged;

            _birthdayNpc = null;

            _recipeOfTheDay = "";
        }

        public override string GeneralInfo(out int usedLines)
        {
            usedLines = 0;

            if (!Enabled) return "";

            Update();
            var stringBuilder = new StringBuilder();

            if (_config.NewRecipeOnTv && _recipeOfTheDay.Length > 0)
            {
                usedLines++;
                stringBuilder.Append($"There's a new recipe on The Queen of Sauce.^");
            }

            if (_config.Birthdays && _birthdayNpc != null)
            {
                usedLines++;
                stringBuilder.Append($"It's {_birthdayNpc.displayName} birthday!^");
            }

            if (_config.TravelingMerchant && _isTravelingMerchantOpen && !_wasTravelingMerchantVisited)
            {
                usedLines++;
                stringBuilder.Append("The traveling merchant is in town.^");
            }

            return stringBuilder.ToString();
        }

        public override string DetailedInfo(out int usedLines, out bool skipNextPage)
        {
            usedLines = 0;
            skipNextPage = false;
            return "";
        }
    }
}