# Paritee's Better Farm Animal Variety

Customize the types and species of farm animals you can raise without needing to replace the default farm animal types

## Contents

- [Get Started](#get-started)
- [Configure](#configure)
- [For Modders](#for-modders)

## Get Started

### Install

1. Install the latest version of [SMAPI](https://smapi.io/)
2. Install the latest version of [Content Patcher](https://www.nexusmods.com/stardewvalley/mods/1915)
3. Download the [Paritee's Better Farm Animal Variety](https://www.nexusmods.com/stardewvalley/mods/3273) (BFAV) mod files from Nexus Mods
4. Unzip the mod files into `Stardew Valley/Mods`
5. Run the game using SMAPI

### Add New Farm Animal

See [Farm Animals: Using with BFAV](https://github.com/paritee/Farm-Animals/blob/master/README.md#using-with-bfav)


### Add New Species

To add a new species, you must add a new entry to the `FarmAnimals` array in the BFAV's `config.json` file. See [Fields](#fields) for more detailed information. The structure of a new `FarmAnimals` entry is as follows:


```json
"<unique species name>": {
  "Types": [
    "<unique type #1>",
    "<optional unique type #2>"
  ],
  "Buildings": [
    "<building to live in #1>",
    "<optional building to live in #2>"
  ],
  "AnimalShop": {
    "Name": "<species display name or null if not in shop>",
    "Description": "<species description or null if not in shop>",
    "Price": "<price or null if not in shop>",
    "Icon": "<assets\\filename.png or null if not in shop>"
  }
}
```

## Configure

You can configure your mod at `Stardew Valley/Mods/Paritee's Better Farm Animal Variety/config.json`

### Fields

#### VoidFarmAnimalsInShop

| Value  | Description |
| ------------- | ------------- |
| `Never` | Void farm animals will never be available in [Marnie's animal shop](https://stardewvalleywiki.com/Marnie%27s_Ranch#Livestock) (default) |
| `QuestOnly` | Void farm animals will only be available in [Marnie's animal shop](https://stardewvalleywiki.com/Marnie%27s_Ranch#Livestock) if the player has completed the [Goblin Problem](https://stardewvalleywiki.com/Quests#Goblin_Problem) quest |
| `Always` | Void farm animals are always availabe in [Marnie's animal shop](https://stardewvalleywiki.com/Marnie%27s_Ranch#Livestock) |

#### FarmAnimals

##### Chicken, Dairy Cow, Dinosaur, Duck, Goat, Pig, Rabbit, Sheep (and many more!)

| Name | Type | Description |
| ------------- | ------------- | ------------- |
| `Types` | `string []` | The types of farm animals that exist in the game for this species. The types must exist in `Stardew Valley/Content/Data/FarmAnimals` (ex. `["White Cow", "Brown Cow"]`) |
| `Buildings` | `string []` | The buildings that these species can live in. The types must exist in `Stardew Valley/Content/Data/Blueprints` (ex. `["Barn", "Deluxe Coop"]`) |
| `AnimalShop` | `object` | Configures the animal shop menu (see below). Leave all values `null` if you do not want this species to show up in [Marnie's animal shop](https://stardewvalleywiki.com/Marnie%27s_Ranch#Livestock). |

###### AnimalShop

| Name | Type | Description |
| ------------- | ------------- | ------------- |
| `Name` | `string` | Display name of the species (ex. `"Dairy Cow"`) |
| `Description` | `string` | Description of the species (ex. `"Adults can be milked daily. A milk pail is required to harvest the milk."`) |
| `Price` | `string` | Purchase cost of one animal of this species |
| `Icon` | `string` | The filename of your file in `Stardew Valley/Mods/Paritee's Better Farm Animal Variety/assets`. These icons are used for the species options in [Marnie's animal shop](https://stardewvalleywiki.com/Marnie%27s_Ranch#Livestock) menu. The vanilla icons are saved in the `assets` directory for your use (ex. `"animal_shop_dairy cow.png"`) |


### Default

Here is a sample of a default `config.json` file:

```json
{
  "VoidFarmAnimalsInShop": "Never",
  "FarmAnimals": {
    "Chicken": {
      "Types": [
        "White Chicken",
        "Brown Chicken",
        "Blue Chicken",
        "Void Chicken"
      ],
      "Buildings": [
        "Coop",
        "Big Coop",
        "Deluxe Coop"
      ],
      "AnimalShop": {
        "Name": "Chicken",
        "Description": "Well cared-for adult chickens lay eggs every day.",
        "Price": "800",
        "Icon": "assets\\animal_shop_chicken.png"
      }
    },
    "Dairy Cow": {
      "Types": [
        "White Cow",
        "Brown Cow"
      ],
      "Buildings": [
        "Barn",
        "Big Barn",
        "Deluxe Barn"
      ],
      "AnimalShop": {
        "Name": "Dairy Cow",
        "Description": "Adults can be milked daily. A milk pail is required to harvest the milk.",
        "Price": "1500",
        "Icon": "assets\\animal_shop_dairy cow.png"
      }
    },
    "Goat": {
      "Types": [
        "Goat"
      ],
      "Buildings": [
        "Big Barn",
        "Deluxe Barn"
      ],
      "AnimalShop": {
        "Name": "Goat",
        "Description": "Happy adults provide goat milk every other day. A milk pail is required to harvest the milk.",
        "Price": "4000",
        "Icon": "assets\\animal_shop_goat.png"
      }
    },
    "Duck": {
      "Types": [
        "Duck"
      ],
      "Buildings": [
        "Big Coop",
        "Deluxe Coop"
      ],
      "AnimalShop": {
        "Name": "Duck",
        "Description": "Happy adults lay duck eggs every other day.",
        "Price": "4000",
        "Icon": "assets\\animal_shop_duck.png"
      }
    },
    "Sheep": {
      "Types": [
        "Sheep"
      ],
      "Buildings": [
        "Deluxe Barn"
      ],
      "AnimalShop": {
        "Name": "Sheep",
        "Description": "Adults can be shorn for wool. Sheep who form a close bond with their owners can grow wool faster. A pair of shears is required to harvest the wool.",
        "Price": "8000",
        "Icon": "assets\\animal_shop_sheep.png"
      }
    },
    "Rabbit": {
      "Types": [
        "Rabbit"
      ],
      "Buildings": [
        "Deluxe Coop"
      ],
      "AnimalShop": {
        "Name": "Rabbit",
        "Description": "These are wooly rabbits! They shed precious wool every few days.",
        "Price": "8000",
        "Icon": "assets\\animal_shop_rabbit.png"
      }
    },
    "Pig": {
      "Types": [
        "Pig"
      ],
      "Buildings": [
        "Deluxe Barn"
      ],
      "AnimalShop": {
        "Name": "Pig",
        "Description": "These pigs are trained to find truffles!",
        "Price": "16000",
        "Icon": "assets\\animal_shop_pig.png"
      }
    },
    "Dinosaur": {
      "Types": [
        "Dinosaur"
      ],
      "Buildings": [
        "Big Coop",
        "Deluxe Coop"
      ],
      "AnimalShop": {
        "Name": null,
        "Description": null,
        "Price": null,
        "Icon": null
      }
    }
  }
}
```

### For Modders

#### BetterFarmAnimalVarietyAPI

See [SMAPI Modder Guide](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Integrations#Using_an_API) for usage. Requires [Paritee.StardewValleyAPI](https://github.com/paritee/Paritee.StardewValleyAPI).

```c#

/// <returns>Returns Dictionary<string, string[]> (ex. { "Cows", [ "White Cow", "Brown Cow" ] }</returns>
public Dictionary<string, string[]> GetGroupedFarmAnimals();

/// <param name="player">Paritee.StardewValleyAPI.Players</param>
/// <returns>Returns Paritee.StardewValleyAPI.FarmAnimals.Variations.BlueVariation</returns>
public Blue GetBlueFarmAnimals(Player player);

/// <param name="player">Paritee.StardewValleyAPI.Players</param>
/// <returns>Returns Paritee.StardewValleyAPI.FarmAnimals.Variations.VoidVariation</returns>
public Void GetVoidFarmAnimals(Player player);

/// <param name="farm">StardewValley.Farm</param>
/// <param name="blueFarmAnimals">Paritee.StardewValleyAPI.FarmAnimals.Variations.BlueVariation</param>
/// <param name="voidFarmAnimals">Paritee.StardewValleyAPI.FarmAnimals.Variations.VoidVariation</param>
/// <returns>Returns Paritee.StardewValleyAPI.Buidlings.AnimalShop</returns>
public AnimalShop GetAnimalShop(Player player)
```

#### Complimentary Mods

- [Generate Farm Animal Data](https://paritee.github.io/#generate-data-farmanimals-entry)
- [Paritee's Gender-Neutral Farm Animals](https://www.nexusmods.com/stardewvalley/mods/3289)
