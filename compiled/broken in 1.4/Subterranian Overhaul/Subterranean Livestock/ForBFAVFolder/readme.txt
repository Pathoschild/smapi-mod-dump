Step 1:

Copy the assets folder from [CP] Subterranian Livestock into the BFAV mod folder and merge the 2 asset folders.

Step 2:

Open the config.json file found in the BFAV mod folder.

At the bottom, find the final 2 closing brackets (which look like this:)

	}
}

replace them with the following:
    ,
    "Grubs": {
      "Types": [
        "Grub"
      ],
      "Buildings": [
        "Coop",
        "Big Coop",
        "Deluxe Coop"
      ],
      "AnimalShop": {
        "Name": "Grub",
        "Description": "Grubs have a short life span, but keep their population constant.  The result for the aspiring grub farmer is a steady supply of slightly revolting meat!",
        "Price": "400",
        "Icon": "assets\\animal_shop_grubs.png"
      }
    },
    "CopperCrabs": {
      "Types": [
        "CopperCrab"
      ],
      "Buildings": [
        "Coop",
        "Big Coop",
        "Deluxe Coop"
      ],
      "AnimalShop": {
        "Name": "Copper Crab",
        "Description": "Adults grow ore on their shell, which can be scrapped off with a set of shears. Happy rock crabs produce extreamly pure metal.",
        "Price": "800",
        "Icon": "assets\\animal_shop_rockcrab.png"
      }
    },
    "IronCrabs": {
      "Types": [
        "IronCrab"
      ],
      "Buildings": [
        "Big Coop",
        "Deluxe Coop"
      ],
      "AnimalShop": {
        "Name": "Iron Crab",
        "Description": "Adults grow ore on their shell, which can be scrapped off with a set of shears. Happy rock crabs produce extreamly pure metal.",
        "Price": "1200",
        "Icon": "assets\\animal_shop_ironcrab.png"
      }
    },
    "GoldCrabs": {
      "Types": [
        "GoldCrab"
      ],
      "Buildings": [
        "Deluxe Coop"
      ],
      "AnimalShop": {
        "Name": "Gold Crab",
        "Description": "Adults grow ore on their shell, which can be scrapped off with a set of shears. Happy rock crabs produce extreamly pure metal.",
        "Price": "1600",
        "Icon": "assets\\animal_shop_goldcrab.png"
      }
    },
    "IridiumCrabs": {
      "Types": [
        "IridiumCrab"
      ],
      "Buildings": [
        "Deluxe Coop"
      ],
      "AnimalShop": {
        "Name": "Iridium Crab",
        "Description": "Adults grow ore on their shell, which can be scrapped off with a set of shears. Happy rock crabs produce extreamly pure metal.",
        "Price": "3200",
        "Icon": "assets\\animal_shop_iridiumcrab.png"
      }
    },
    "NakedMoleRat": {
      "Types": [
        "NakedMoleRat"
      ],
      "Buildings": [
        "Barn",
        "Big Barn",
        "Deluxe Barn"
      ],
      "AnimalShop": {
        "Name": "Naked Mole Rat",
        "Description": "Giant Naked Mole Rats. Kept by the dwarves as a source of milk and meat.",
        "Price": "500",
        "Icon": "assets\\animal_shop_molerat.png"
      }
    }
  }
}
