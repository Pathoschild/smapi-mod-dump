/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/greyivy/OrnithologistsGuild
**
*************************************************/

const { promises: fs } = require("fs");
const path = require("path");

const UNIQUE_ID = "Ivy.OrnithologistsGuild";
const SHOP_ID = "STF.Ivy_OrnithologistsGuild";

(async () => {
    console.log('Building content.json... ')

    const baths = JSON.parse(await fs.readFile("baths.json", "utf-8"));
    const feeders = JSON.parse(await fs.readFile("feeders.json", "utf-8"));
    const foods = JSON.parse(await fs.readFile("foods.json", "utf-8"));

    const output = [
        {
            "$ItemType": "Object",
            "ID": "LifeList",
            "Texture": "LifeList.png:0",
            "SellPrice": null,
            "CanTrash": false,
            "IsGiftable": false,
            "Category": "Junk",
            "CategoryColorOverride": "255, 255, 0, 255"
        },
        {
            "$ItemType": "Object",
            "ID": "HulledSunflowerSeeds",
            "Texture": "HulledSunflowerSeeds.png:0",
            "SellPrice": 30,
            "Category": "Seeds"
        },
        {
            "$ItemType": "ShopEntry",
            "Item": {
                "Type": "DGAItem",
                "Value": `${UNIQUE_ID}/HulledSunflowerSeeds`
            },
            "ShopId": SHOP_ID,
            "MaxSold": 25,
            "Cost": 300
        },
        {
            "$ItemType": "Object",
            "ID": "JojaBinoculars",
            "Texture": "Binoculars.png:0",
            "SellPrice": 250,
            "Category": "Junk",
            "CategoryColorOverride": "255, 255, 0, 255"
        },
        {
            "$ItemType": "ShopEntry",
            "Item": {
                "Type": "Custom",
                "Value": "OrnithologistsGuild.Game.Items.JojaBinoculars/arg"
            },
            "ShopId": "Joja",
            "Cost": 1500
        },
        {
            "$ItemType": "ShopEntry",
            "Item": {
                "Type": "Custom",
                "Value": "OrnithologistsGuild.Game.Items.JojaBinoculars/arg"
            },
            "ShopId": SHOP_ID,
            "Cost": 2500
        },
        {
            "$ItemType": "Object",
            "ID": "AntiqueBinoculars",
            "Texture": "Binoculars.png:1",
            "SellPrice": 50,
            "Category": "Junk",
            "CategoryColorOverride": "255, 255, 0, 255"
        },
        {
            "$ItemType": "ShopEntry",
            "Item": {
                "Type": "Custom",
                "Value": "OrnithologistsGuild.Game.Items.AntiqueBinoculars/arg"
            },
            "ShopId": "TravelingMerchant",
            "MaxSold": 1,
            "Cost": 500,
            "EnableConditions": {
                "Query: {{DailyLuck}} > 0": true // spirits happy today
            }
        },
        {
            "$ItemType": "ShopEntry",
            "Item": {
                "Type": "Custom",
                "Value": "OrnithologistsGuild.Game.Items.AntiqueBinoculars/arg"
            },
            "ShopId": SHOP_ID,
            "Cost": 5000
        },
        {
            "$ItemType": "Object",
            "ID": "ProBinoculars",
            "Texture": "Binoculars.png:2",
            "SellPrice": 2500,
            "Category": "Junk",
            "CategoryColorOverride": "255, 255, 0, 255"
        },
        {
            "$ItemType": "ShopEntry",
            "Item": {
                "Type": "Custom",
                "Value": "OrnithologistsGuild.Game.Items.ProBinoculars/arg"
            },
            "ShopId": SHOP_ID,
            "Cost": 25000
        },
        {
            "$ItemType": "BigCraftable",
            "ID": "SeedHuller",
            "Texture": "SeedHuller.png:0",
            "SellPrice": 500
        },
        {
            "$ItemType": "ShopEntry",
            "Item": {
                "Type": "DGAItem",
                "Value": `${UNIQUE_ID}/SeedHuller`
            },
            "ShopId": SHOP_ID,
            "MaxSold": 1,
            "Cost": 2500
        },
        {
            "$ItemType": "MachineRecipe",
            "MachineId": `${UNIQUE_ID}/SeedHuller`,
            "MinutesToProcess": 30,
            "MachineWorkingTextureOverride": "SeedHuller.png:1",
            "MachinePulseWhileWorking": true,
            "Ingredients": [
                {
                    "Type": "VanillaObject",
                    "Value": "Sunflower Seeds",
                    "Quantity": 1
                }
            ],
            "Result": [
                {
                    "Weight": 1,
                    "Value": {
                        "Type": "DGAItem",
                        "Value": `${UNIQUE_ID}/HulledSunflowerSeeds`
                    }
                }
            ]
        },
    ];

    for (let bath of baths) {
        // BigCraftable
        const bigCraftable = {
            "$ItemType": "BigCraftable",
            "ID": bath.ID,
            "Texture": `${bath.Texture}:0`,
            "SellPrice": bath.SellPrice
        }
        if (!bath.Heated) {
            bigCraftable.DynamicFields = [
                {
                    "Conditions": { "Season": "winter" },
                    "Texture": `${bath.Texture}:1`, // Frozen texture
                }
            ]
        }
        output.push(bigCraftable)

        // ShopEntry
        output.push({
            "$ItemType": "ShopEntry",
            "Item": {
                "Type": "DGAItem",
                "Value": `${UNIQUE_ID}/${bath.ID}`
            },
            "ShopId": SHOP_ID,
            "MaxSold": 1,
            "Cost": bath.Cost
        })

        if (bath.RecipeIngredients && bath.RecipePrice) {
            // Recipe
            output.push({
                "$ItemType": "CraftingRecipe",
                "ID": `Ivy_OrnithologistsGuild_Recipe_${bath.ID}`,
                "KnownByDefault": true,
                "Result": {
                    "Type": "DGAItem",
                    "Value": `${UNIQUE_ID}/${bath.ID}`
                },
                "Ingredients": bath.RecipeIngredients
            })

            // TODO not compatible with ShopTileFramework
            // Recipe ShopEntry
            // output.push({
            //     "$ItemType": "ShopEntry",
            //     "Item": {
            //         "Type": "DGARecipe",
            //         "Value": `${UNIQUE_ID}/Ivy_OrnithologistsGuild_Recipe_${feeder.ID}`
            //     },
            //     "ShopId": SHOP_ID,
            //     "MaxSold": 1,
            //     "Cost": feeder.RecipePrice
            // })
        }
    }

    for (let feeder of feeders) {
        // BigCraftable
        output.push({
            "$ItemType": "BigCraftable",
            "ID": feeder.ID,
            "Texture": `${feeder.Texture}:0`,
            "SellPrice": feeder.SellPrice
        })

        // ShopEntry
        output.push({
            "$ItemType": "ShopEntry",
            "Item": {
                "Type": "DGAItem",
                "Value": `${UNIQUE_ID}/${feeder.ID}`
            },
            "ShopId": SHOP_ID,
            "MaxSold": 1,
            "Cost": feeder.Cost
        })

        if (feeder.RecipeIngredients && feeder.RecipePrice) {
            // Recipe
            output.push({
                "$ItemType": "CraftingRecipe",
                "ID": `Ivy_OrnithologistsGuild_Recipe_${feeder.ID}`,
                "KnownByDefault": true,
                "Result": {
                    "Type": "DGAItem",
                    "Value": `${UNIQUE_ID}/${feeder.ID}`
                },
                "Ingredients": feeder.RecipeIngredients
            })

            // TODO not compatible with ShopTileFramework
            // Recipe ShopEntry
            // output.push({
            //     "$ItemType": "ShopEntry",
            //     "Item": {
            //         "Type": "DGARecipe",
            //         "Value": `${UNIQUE_ID}/Ivy_OrnithologistsGuild_Recipe_${feeder.ID}`
            //     },
            //     "ShopId": SHOP_ID,
            //     "MaxSold": 1,
            //     "Cost": feeder.RecipePrice
            // })
        }

        // MachineRecipe
        for (let food of foods) {
            if (food.FeedersTypes.includes(feeder.Type)) {
                for (let item of food.Items) {
                    output.push({
                        "$ItemType": "MachineRecipe",
                        "MachineId": `${UNIQUE_ID}/${feeder.ID}`,
                        "MinutesToProcess": feeder.CapacityHrs * 60,
                        "MachineWorkingTextureOverride": `${feeder.Texture}:${food.FeederAssetIndex}`,
                        "MachinePulseWhileWorking": false,
                        "StartWorkingSound": null,
                        "Ingredients": [
                            {
                                "Type": item.startsWith("DGA:") ? "DGAItem" : "VanillaObject",
                                "Value": item.startsWith("DGA:") ? item.split(":")[1] : item,
                                "Quantity": 1
                            }
                        ],
                        "Result": [
                            {
                                "Weight": 25,
                                "Value": {
                                    "Type": "VanillaObject",
                                    "Value": "Rotten Plant"
                                }
                            },
                            {
                                "Weight": 3,
                                "Value": {
                                    "Type": "VanillaObject",
                                    "Value": "Iron Ore"
                                }
                            },
                            {
                                "Weight": 1,
                                "Value": {
                                    "Type": "VanillaObject",
                                    "Value": "Gold Ore"
                                }
                            }
                        ]
                    })
                }
            }
        }
    }

    await fs.writeFile(path.join('assets', 'dga', 'content.json'), JSON.stringify(output, null, 2));

    console.log('Done!')
})()
