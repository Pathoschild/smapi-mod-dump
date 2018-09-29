using Newtonsoft.Json;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AdjustArtisanPrices
{
    public class AdjustArtisanPrices : Mod
    {

        private const int ARTISAN_GOODS = -26;
        public Config ModConfig { get; set; }

        public override void Entry(params object[] objects)
        {
            PlayerEvents.InventoryChanged += Event_OnInventoryChanged;

            var configLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "config.json");

            if (!File.Exists(configLocation))
            {
                ModConfig = new Config();

                ModConfig.WineIncrease = 1.75;
                ModConfig.JellyIncrease = 1.25;
                ModConfig.PicklesIncrease = 1.25;
                ModConfig.JuiceIncrease = 1.50;
                ModConfig.BeerIncrease = 0.50;
                ModConfig.PaleAleIncrease = 0.50;
                ModConfig.CheeseIncrease = 0.75;
                ModConfig.GoatCheeseIncrease = 0.75;
                ModConfig.MayonnaiseIncrease = 0.75;
                ModConfig.DuckMayonnaiseIncrease = 0.80;
                ModConfig.ClothIncrease = 0.75;

                File.WriteAllBytes(configLocation, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(ModConfig, Formatting.Indented)));
            }
            else
            {
                ModConfig = JsonConvert.DeserializeObject<Config>(Encoding.UTF8.GetString(File.ReadAllBytes(configLocation)));
            }

        }
        public void Event_OnInventoryChanged(object sender, EventArgsInventoryChanged e)
        {
            if(Game1.hasLoadedGame)
            {
                var allObjects = Game1.objectInformation;

                Dictionary<string, int> parsedObjs = new Dictionary<string, int>();

                foreach (var item in allObjects)
                {
                    var stringObjs = item.Value.Split('/');

                    if (!parsedObjs.ContainsKey(stringObjs[0]) && stringObjs[3].Contains("Basic -75") || stringObjs[3].Contains("Basic -79") || stringObjs[3].Contains("Basic -26"))
                    {
                        parsedObjs.Add(stringObjs[0], Convert.ToInt32(stringObjs[1]));
                    }
                }

                for (int i = 0; i < Game1.player.items.Count; i++)
                {
                    if (Game1.player.items[i] != null)
                    {
                        var item = Game1.player.items[i];

                        if (item.category == ARTISAN_GOODS && item is StardewValley.Object)
                        {
                            var originalItemStringArray = item.Name.Split(' ');
                            var originalItemName = "";
                            var artisanType = "";

                            if (originalItemStringArray.Count() >= 1)
                            {
                                
                                if(originalItemStringArray.Count() == 1)
                                {
                                    artisanType = originalItemStringArray[0];
                                }

                                if (originalItemStringArray.Count() == 2)
                                {
                                    if(originalItemStringArray.Contains("Pale"))
                                    {
                                        artisanType = "Pale Ale";
                                    }

                                    else if (originalItemStringArray.Contains("Duck"))
                                    {
                                        artisanType = "Duck Mayonnaise";
                                    }

                                    else if (originalItemStringArray.Contains("Goat"))
                                    {
                                        artisanType = "Goat Cheese";
                                    }

                                    else
                                    {
                                        // E.g., Cranberries Wine
                                        originalItemName = originalItemStringArray[0];
                                        artisanType = originalItemStringArray[1];
                                    }
                                }

                                //E.g., Hot Pepper Jelly
                                if (originalItemStringArray.Count() == 3)
                                {
                                    originalItemName += originalItemStringArray[0];
                                    originalItemName += " ";
                                    originalItemName += originalItemStringArray[1];

                                    artisanType = originalItemStringArray[2];
                                }

                                KeyValuePair<string, int> priceFromObjects;

                                if(!originalItemName.Equals(""))
                                {
                                    priceFromObjects = parsedObjs.Select(x => x).Where(x => x.Key.Equals(originalItemName)).FirstOrDefault();
                                }

                                else
                                {
                                    priceFromObjects = parsedObjs.Select(x => x).Where(x => x.Key.Equals(artisanType)).FirstOrDefault();
                                }                                

                                var originalPrice = priceFromObjects.Value;

                                if (artisanType.Equals("Wine"))
                                {
                                    item.Cast<StardewValley.Object>().Price = (int)(originalPrice * ModConfig.WineIncrease);
                                }

                                if (artisanType.Equals("Juice"))
                                {
                                    item.Cast<StardewValley.Object>().Price = (int)(originalPrice * ModConfig.JuiceIncrease);
                                }

                                if (artisanType.Equals("Jelly"))
                                {
                                    item.Cast<StardewValley.Object>().Price = (int)(originalPrice * ModConfig.JellyIncrease);
                                }

                                if (artisanType.Equals("Pickles"))
                                {
                                    item.Cast<StardewValley.Object>().Price = (int)(originalPrice * ModConfig.PicklesIncrease);
                                }

                                if (artisanType.Equals("Pale Ale"))
                                {
                                    item.Cast<StardewValley.Object>().Price = (int)(originalPrice * ModConfig.PaleAleIncrease);
                                }

                                if (artisanType.Equals("Beer"))
                                {
                                    item.Cast<StardewValley.Object>().Price = (int)(originalPrice * ModConfig.BeerIncrease);
                                }

                                if (artisanType.Equals("Mayonnaise"))
                                {
                                    item.Cast<StardewValley.Object>().Price = (int)(originalPrice * ModConfig.MayonnaiseIncrease);
                                }

                                if (artisanType.Equals("Duck Mayonnaise"))
                                {
                                    item.Cast<StardewValley.Object>().Price = (int)(originalPrice * ModConfig.DuckMayonnaiseIncrease);
                                }

                                if (artisanType.Equals("Cheese"))
                                {
                                    item.Cast<StardewValley.Object>().Price = (int)(originalPrice * ModConfig.CheeseIncrease);
                                }

                                if (artisanType.Equals("Goat Cheese"))
                                {
                                    item.Cast<StardewValley.Object>().Price = (int)(originalPrice * ModConfig.GoatCheeseIncrease);
                                }

                                if (artisanType.Equals("Cloth"))
                                {
                                    item.Cast<StardewValley.Object>().Price = (int)(originalPrice * ModConfig.ClothIncrease);
                                }
                            }
                        }
                    }
                }
            }
        }

        public class Config
        {
            public double WineIncrease { get; set; }
            public double JuiceIncrease { get; set; }
            public double JellyIncrease { get; set; }
            public double PicklesIncrease { get; set; }
            public double PaleAleIncrease { get; set;  }
            public double BeerIncrease { get; set; }
            public double MayonnaiseIncrease { get; set; }
            public double DuckMayonnaiseIncrease { get; set; }
            public double CheeseIncrease { get; set; }
            public double GoatCheeseIncrease { get; set; }
            public double ClothIncrease { get; set; }
        }
    }
}

