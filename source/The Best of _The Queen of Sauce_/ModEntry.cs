/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Bpendragon/Best-of-Queen-of-Sauce
**
*************************************************/

using System.Collections.Generic;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Delegates;
using StardewValley.GameData.Shops;

namespace Bpendragon.BestOfQueenOfSauce
{
    internal partial class ModEntry : Mod
    {
        private readonly Dictionary<string, (int day, string Id)> FirstAirDate = new();
        private ModConfig Config;
        private bool MailChangesMade = false;

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();
            I18n.Init(helper.Translation);

            FirstAirDate["Stir Fry"] = (7, "(O)606");
            FirstAirDate["Coleslaw"] = (14, "(O)648");
            FirstAirDate["Radish Salad"] = (21, "(O)609");
            FirstAirDate["Baked Fish"] = (35, "(O)198");
            FirstAirDate["Trout Soup"] = (70, "(O)219");
            FirstAirDate["Glazed Yams"] = (77, "(O)208");
            FirstAirDate["Artichoke Dip"] = (84, "(O)605");
            FirstAirDate["Plum Pudding"] = (91, "(O)604");
            FirstAirDate["Chocolate Cake"] = (98, "(O)220");
            FirstAirDate["Pumpkin Pie"] = (105, "(O)608");
            FirstAirDate["Cranberry Candy"] = (112, "(O)612");
            FirstAirDate["Complete Breakfast"] = (133, "(O)201");
            FirstAirDate["Lucky Lunch"] = (140, "(O)204");
            FirstAirDate["Carp Surprise"] = (147, "(O)209");
            FirstAirDate["Maple Bar"] = (154, "(O)731");
            FirstAirDate["Pink Cake"] = (161, "(O)221");
            FirstAirDate["Roasted Hazelnuts"] = (168, "(O)607");
            FirstAirDate["Fruit Salad"] = (175, "(O)610");
            FirstAirDate["Blackberry Cobbler"] = (182, "(O)611");
            FirstAirDate["Crab Cakes"] = (189, "(O)732");
            FirstAirDate["Fiddlehead Risotto"] = (196, "(O)649");
            FirstAirDate["Poppyseed Muffin"] = (203, "(O)651");
            FirstAirDate["Bruschetta"] = (217, "(O)618");
            FirstAirDate["Shrimp Cocktail"] = (224, "(O)733");

            helper.Events.Content.AssetRequested += OnAssetRequested;
            GameStateQuery.Register("BestOfQOS.RecipeCondition", CheckRecipe);
        }

        internal bool CheckRecipe(string[] query, GameStateQueryContext context)
        {
            //Naming is hard. This is the most recent date that a recipe can be available. 
            //Example, it's Y2,S27 (Day 167) using the default config setting of 28 days (making this variable 139) Complete Breakfast (aired day 133) would be available, but Lucky Lunch (day 140) would not.
            int latestRecipeDate = Game1.Date.TotalDays - Config.DaysAfterAiring;
            return latestRecipeDate >= FirstAirDate[query[^1]].day && !Game1.player.cookingRecipes.Keys.Contains(query[^1]);
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (!MailChangesMade && e.NameWithoutLocale.IsEquivalentTo(@"Data\mail"))
            {
                e.Edit(asset => {
                    var data = asset.AsDictionary<string, string>().Data;

                    data["BestOfQOS.Letter1"] = I18n.BestOfQOS_Letter1().Replace("[days]", Config.DaysAfterAiring.ToString()).Replace("[price]", Config.Price.ToString());
                });
                MailChangesMade = true;
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/Shops"))
            {
                e.Edit(delegate (IAssetData data) {
                    var dict = data.AsDictionary<string, ShopData>();
                    foreach (var kvp in FirstAirDate)
                    {
                        dict.Data["Saloon"].Items.Add(new ShopItemData() {
                            Id = kvp.Key,
                            ItemId = kvp.Value.Id,
                            Price = Config.Price,
                            IsRecipe = true,
                            AvoidRepeat = true,
                            Condition = $"BestOfQOS.RecipeCondition \"{kvp.Key}\""
                        });
                    }
                });
            } 
        
        }

        private void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            if (Game1.Date.TotalDays >= 7 + Config.DaysAfterAiring + 1)
            {
                Game1.addMailForTomorrow("BestOfQOS.Letter1");
            }
        }
    }
}
