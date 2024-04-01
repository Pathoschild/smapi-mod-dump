/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/DailyTaskReportPlus
**
*************************************************/

using StardewValley.Locations;
using StardewValley.Menus;
using StardewModdingAPI.Events;
using DailyTasksReport.UI;



namespace DailyTasksReport.TaskEngines
{
    class MiscTaskEngine : TaskEngine
    {

        private readonly Dictionary<string, string> _tvRecipes = DataLoader.Tv_CookingChannel(Game1.content);
        private string _recipeOfTheDay = "";

        private NPC? _birthdayNpc;

        private bool _isTravelingMerchantOpen;
        public bool _wasTravelingMerchantVisited;

        internal MiscTaskEngine(TaskReportConfig config)
        {
            _config = config;
            TaskClass = "Misc";
            DailyTaskHelper.Helper.Events.Display.MenuChanged -= Display_MenuChanged;

            SetEnabled();
        }


        public override void Clear()
        {
            _isTravelingMerchantOpen = false;
            _wasTravelingMerchantVisited = false;
            DailyTaskHelper.Helper.Events.Display.MenuChanged -= Display_MenuChanged;

            _birthdayNpc = null;

            _recipeOfTheDay = "";
        }

        public override List<ReportReturnItem> DetailedInfo()
        {
            return new List<ReportReturnItem> { };
        }
        public List<ReportReturnItem> GeneralInfo(bool useHTML)
        {
            List<ReportReturnItem> prItem = new List<ReportReturnItem> { };

            if (!Enabled) return prItem;

            Update();

            if (_config.NewRecipeOnTv && _recipeOfTheDay.Length > 0)
            {

                prItem.Add(new ReportReturnItem { Label = I18n.Tasks_Misc_Queen() });
            }

            if (_config.Birthdays && _birthdayNpc != null)
            {
                if (useHTML)
                {
                    prItem.Add(new ReportReturnItem { Label = I18n.Tasks_Misc_Birthday("<a href='/Friends?id=" + _birthdayNpc.Name + "'>" + _birthdayNpc.displayName + "</a>") });
                }
                else
                {
                    prItem.Add(new ReportReturnItem { Label = I18n.Tasks_Misc_Birthday(_birthdayNpc.displayName) });
                }
            }

            if (_config.TravelingMerchant && _isTravelingMerchantOpen && !_wasTravelingMerchantVisited)
            {
                prItem.Add(new ReportReturnItem { Label = I18n.Tasks_Misc_Merchant() });
            }

            if (Game1.dayOfMonth == 28 && _config.FlowerReportLastDay)
            {
                prItem.Add(new ReportReturnItem { Label = I18n.Tasks_LastDay() });
            }

            if (_config.SiloThreshold > 0 && Game1.getFarm().buildings.Any(b => b.buildingType.Value == "Silo") && Game1.getFarm().piecesOfHay.Value < _config.SiloThreshold)
            {
                if (Game1.getFarm().piecesOfHay.Value == 0)
                {
                    prItem.Add(new ReportReturnItem { Label = I18n.Tasks_NoHay() });
                }
                else
                {
                    prItem.Add(new ReportReturnItem { Label = I18n.Tasks_Hay(Game1.getFarm().piecesOfHay.Value) });
                }
            }



            return prItem;
        }
        public override List<ReportReturnItem> GeneralInfo()
        {
            return GeneralInfo(true);
        }

        internal override void FirstScan()
        {
            if (Game1.locations.OfType<Forest>().First().travelingMerchantDay)
            {
                 DailyTaskHelper.Helper.Events.Display.MenuChanged += Display_MenuChanged;
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

            foreach (GameLocation? location in Game1.locations)
                foreach (var npc in location.characters)

                    if (npc.isBirthday())
                    {
                        _birthdayNpc = npc;
                        return;
                    }
        }


        private void Display_MenuChanged(object? sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is ShopMenu && Game1.currentLocation is Forest &&
                Game1.player.GetGrabTile() == new Vector2(27, 11))
            {
                DailyTaskHelper.Helper.Events.Display.MenuChanged -= Display_MenuChanged;
                _wasTravelingMerchantVisited = true;
            }
        }
        public override void SetEnabled()
        {
            Enabled = _config.TravelingMerchant || _config.Birthdays || _config.NewRecipeOnTv || _config.SiloThreshold > 0 || _config.FlowerReportLastDay;
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
    }
}
