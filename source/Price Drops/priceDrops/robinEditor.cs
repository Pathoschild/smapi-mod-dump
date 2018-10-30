using System;
using StardewModdingAPI;
using StardewValley;

namespace priceDrops
{
    internal class robinEditor : IAssetEditor
    {

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals(@"Data\Blueprints");
        }

        public void Edit<T>(IAssetData asset)
        {
            asset
                .AsDictionary<string, string>()
                .Set((id, data) =>
                {
                    string[] fields = data.Split('/');

                    if (!fields[0].Equals("animal"))
                    {
                        int buildingPrice = -1;
                        string resources = "";

                        if (fields.Length >= 18 && fields[17] != null)
                        {
                            buildingPrice = Int32.Parse(fields[17]);
                            resources = fields[0];

                            // Check for friendship with Robin
                            int hearts = Game1.player.getFriendshipHeartLevelForNPC("Robin");

                            int percentage = 0;

                            if (hearts >= ModEntry.HEART_LEVEL_3)
                                percentage = ModEntry.DISC_3;
                            else if (hearts >= ModEntry.HEART_LEVEL_2)
                                percentage = ModEntry.DISC_2;
                            else if (hearts >= ModEntry.HEART_LEVEL_1)
                                percentage = ModEntry.DISC_1;

                            // Bonus discount for marriage to Maru/Sebastian
                            if ((Game1.getCharacterFromName("Maru", true) != null && Game1.getCharacterFromName("Maru", true).isMarried()) || (Game1.getCharacterFromName("Sebastian", true) != null && Game1.getCharacterFromName("Sebastian", true).isMarried()))
                            {
                                percentage +=  ModEntry.BONUS_DISC;
                            }

                            //this.Monitor.Log($"Player has " + hearts + " hearts with Robin and receives " + percentage + "% off.");

                            // Blueprints also contains prices for Wizard buildings, so we need him too
                            int hearts_wiz = Game1.player.getFriendshipHeartLevelForNPC("Wizard");

                            int percentage_wiz = 0;

                            if (hearts_wiz >= ModEntry.HEART_LEVEL_3)
                                percentage_wiz = ModEntry.DISC_3;
                            else if (hearts_wiz >= ModEntry.HEART_LEVEL_2)
                                percentage_wiz = ModEntry.DISC_2;
                            else if (hearts_wiz >= ModEntry.HEART_LEVEL_1)
                                percentage_wiz = ModEntry.DISC_1;

                            //this.Monitor.Log($"Player has " + hearts_wiz + " hearts with the Wizard and receives " + percentage_wiz + "% off.");

                            // Beware evil hardcoding of doom
                            if (fields[8].Equals("Silo"))
                            {
                                // Update price and resource cost
                                fields[17] = ModEntry.getPercentage(buildingPrice, percentage).ToString();
                                fields[0] = "390 " + ModEntry.getPercentage(100, percentage).ToString() + " 330 " + ModEntry.getPercentage(10, percentage).ToString() + " 334 " + ModEntry.getPercentage(5, percentage).ToString();
                            }
                            else if (fields[8].Equals("Mill"))
                            {
                                fields[17] = ModEntry.getPercentage(buildingPrice, percentage).ToString();
                                fields[0] = "388 " + ModEntry.getPercentage(150, percentage).ToString() + " 390 " + ModEntry.getPercentage(50, percentage).ToString() + " 428 " + ModEntry.getPercentage(4, percentage).ToString();
                            }
                            else if (fields[8].Equals("Stable"))
                            {
                                fields[17] = ModEntry.getPercentage(buildingPrice, percentage).ToString();
                                fields[0] = "709 " + ModEntry.getPercentage(100, percentage).ToString() + " 335 " + ModEntry.getPercentage(5, percentage).ToString();
                            }
                            else if (fields[8].Equals("Well"))
                            {
                                fields[17] = ModEntry.getPercentage(buildingPrice, percentage).ToString();
                                fields[0] = "390 " + ModEntry.getPercentage(75, percentage).ToString();
                            }
                            else if (fields[8].Equals("Coop"))
                            {
                                fields[17] = ModEntry.getPercentage(buildingPrice, percentage).ToString();
                                fields[0] = "388 " + ModEntry.getPercentage(300, percentage).ToString() + " 390 " + ModEntry.getPercentage(100, percentage).ToString();
                            }
                            else if (fields[8].Equals("Big Coop"))
                            {
                                fields[17] = ModEntry.getPercentage(buildingPrice, percentage).ToString();
                                fields[0] = "388 " + ModEntry.getPercentage(400, percentage).ToString() + " 390 " + ModEntry.getPercentage(150, percentage).ToString();
                            }
                            else if (fields[8].Equals("Deluxe Coop"))
                            {
                                fields[17] = ModEntry.getPercentage(buildingPrice, percentage).ToString();
                                fields[0] = "388 " + ModEntry.getPercentage(500, percentage).ToString() + " 390 " + ModEntry.getPercentage(200, percentage).ToString();
                            }
                            else if (fields[8].Equals("Barn"))
                            {
                                fields[17] = ModEntry.getPercentage(buildingPrice, percentage).ToString();
                                fields[0] = "388 " + ModEntry.getPercentage(350, percentage).ToString() + " 390 " + ModEntry.getPercentage(150, percentage).ToString();
                            }
                            else if (fields[8].Equals("Big Barn"))
                            {
                                fields[17] = ModEntry.getPercentage(buildingPrice, percentage).ToString();
                                fields[0] = "388 " + ModEntry.getPercentage(450, percentage).ToString() + " 390 " + ModEntry.getPercentage(200, percentage).ToString();
                            }
                            else if (fields[8].Equals("Deluxe Barn"))
                            {
                                fields[17] = ModEntry.getPercentage(buildingPrice, percentage).ToString();
                                fields[0] = "388 " + ModEntry.getPercentage(550, percentage).ToString() + " 390 " + ModEntry.getPercentage(300, percentage).ToString();
                            }
                            else if (fields[8].Equals("Slime Hutch"))
                            {
                                fields[17] = ModEntry.getPercentage(buildingPrice, percentage).ToString();
                                fields[0] = "390 " + ModEntry.getPercentage(500, percentage).ToString() + " 338 " + ModEntry.getPercentage(10, percentage).ToString() + " 337 1";
                            }
                            else if (fields[8].Equals("Shed"))
                            {
                                fields[17] = ModEntry.getPercentage(buildingPrice, percentage).ToString();
                                fields[0] = "388 " + ModEntry.getPercentage(300, percentage).ToString();
                            }
                            else if (fields[8].Equals("Stone Cabin"))
                            {
                                fields[17] = ModEntry.getPercentage(buildingPrice, percentage).ToString();
                                fields[0] = "390 " + ModEntry.getPercentage(10, percentage).ToString();
                            }
                            else if (fields[8].Equals("Plank Cabin"))
                            {
                                fields[17] = ModEntry.getPercentage(buildingPrice, percentage).ToString();
                                fields[0] = "388 " + ModEntry.getPercentage(5, percentage).ToString() + " 771 " + ModEntry.getPercentage(10, percentage).ToString();
                            }
                            else if (fields[8].Equals("Log Cabin"))
                            {
                                fields[17] = ModEntry.getPercentage(buildingPrice, percentage).ToString();
                                fields[0] = "388 " + ModEntry.getPercentage(10, percentage).ToString();
                            }
                            else if (fields[8].Equals("Shipping Bin"))
                            {
                                fields[17] = ModEntry.getPercentage(buildingPrice, percentage).ToString();
                                fields[0] = "388 " + ModEntry.getPercentage(150, percentage).ToString();
                            }
                            // Wizard buildings
                            else if (fields[8].Equals("Earth Obelisk"))
                            {
                                fields[17] = ModEntry.getPercentage(buildingPrice, percentage_wiz).ToString();
                                fields[0] = "337 " + ModEntry.getPercentage(10, percentage_wiz).ToString() + " 86 " + ModEntry.getPercentage(10, percentage_wiz).ToString();
                            }
                            else if (fields[8].Equals("Water Obelisk"))
                            {
                                fields[17] = ModEntry.getPercentage(buildingPrice, percentage_wiz).ToString();
                                fields[0] = "337 " + ModEntry.getPercentage(5, percentage_wiz).ToString() + " 372 " + ModEntry.getPercentage(10, percentage_wiz).ToString() + " 393 " + ModEntry.getPercentage(10, percentage_wiz).ToString();
                            }
                            else if (fields[8].Equals("Junimo Hut"))
                            {
                                fields[17] = ModEntry.getPercentage(buildingPrice, percentage_wiz).ToString();
                                fields[0] = "390 " + ModEntry.getPercentage(200, percentage_wiz).ToString() + " 268 " + ModEntry.getPercentage(9, percentage_wiz).ToString() + " 771 " + ModEntry.getPercentage(100, percentage_wiz).ToString();
                            }
                            else if (fields[8].Equals("Gold Clock"))
                            {
                                fields[17] = ModEntry.getPercentage(buildingPrice, percentage_wiz).ToString();
                            }
                        }
                    }

                    return string.Join("/", fields);
                });
        }
    }
}