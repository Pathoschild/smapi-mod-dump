# Craft Anything

By default, Stardew Valley only lets you craft certain kinds of items. One major category this excludes is *furniture.*
Craft Anything lets you augment the crafting system to craft literally any kind of object you can hold in your inventory.

Craft Anything is a SMAPI mod. It requires the SMAPI 2.6 beta and Stardew Valley 1.3 beta.

## How It Works

Craft Anything lets you specify transformations between an item represented on the cooking or crafting interfaces, and
a final product which is the object you actually want. This can be used to alter the default crafts, or you can create
your own proxy items that become your crafted furniture or other custom item!

## The format

Craft Anything reads a file with lines in the following format:

```
Fried Egg creates 5 of item 388 # creates 5 wood instead whenever you craft "Fried Egg"
Omelette creates item 714 as furniture # creates a Birch Chest instead whenever you craft "Omelette"
Pancakes creates item Garden Pot # creates a Garden Pot instead whenever you craft "Pancakes"
Sprinkler creates 2 of item CustomSprinkler # creates a CustomSprinkler whenever you craft "Sprinkler"
```

Each line sets up a transformation rule between an item name and some crafting result. Craft Anything replacements are
compatible with right-click stacking for stackable objects, and with Shift-left-click crafting. Craft Anything does not
interfere with any crafts that are unspecified in the replacements file.

## The API

Craft Anything exposes an API for SMAPI's `GetApi()`:

```cs
public interface ICraftAnythingAPI
{
    bool RegisterCustomType(Type t, string packName);
    void LoadReplacements(string file, string packName);
}
```

where `packName` is the `UniqueID` field from your mod's manifest.

If you want Craft Anything to be able to create custom types for you, you must register then using `RegisterCustomType` before you load
a replacements file, for example:
```cs
public class ModEntry : Mod
{
        private ICraftAnythingAPI CraftAnything;

	    public void GameEvents_FirstUpdateTick(object sender, EventArgs e)
        {
            this.CraftAnything = this.Helper.ModRegistry.GetApi<ICraftAnythingAPI>("Sabera.CraftAnything");
            if (this.CraftAnything == null)
            {
                // handle a load failure
            }

			// register the custom sprinkler type
            this.CraftAnything.RegisterCustomType(typeof(CustomSprinkler), this.ModManifest.UniqueID);
			this.CraftAnything.LoadReplacements(Path.Combine(this.Helper.DirectoryPath,"replacements.conf"), this.ModManifest.UniqueID);
		}
}
```

Once you've successfully loaded your replacements, crafting replacements will happen on both the cooking and main crafting menus.

You can `LoadReplacements` more than once, from any file you like, at any point in your mod. If you load a replacement file that transforms an item
specified in a prior replacement file, the old transform will be overwritten. So if you have two different files:

`replacements1.conf`:
```
Fried Egg creates 5 of item 388 # creates 5 wood instead whenever you craft "Fried Egg"
Omelette creates item 714 as furniture # creates a Birch Chest instead whenever you craft "Omelette"
Pancakes creates item Garden Pot # creates a Garden Pot instead whenever you craft "Pancakes"
Sprinkler creates 2 of item CustomSprinkler # creates a CustomSprinkler whenever you craft "Sprinkler"
```

`replacements2.conf`:
```
Fried Egg creates 2 of item CustomSprinkler # creates a CustomSprinkler whenever you craft "Fried Egg"
```

and you load `replacements1.conf` first, then load `replacements2.conf`, crafting a Fried Egg will produce a `CustomSprinkler`.

## Pull Requests

Pull Requests are welcome, however please make sure you test the following cases:

- Left-click crafting
  - For a stackable item
	  - Not being replaced
		- on the cooking menu
		- on the crafting tab of the main menu
	  - Being replaced
		- on the cooking menu
		- on the crafting tab of the main menu
  - For a non-stackable item
	  - Not being replaced
		- on the cooking menu
		- on the crafting tab of the main menu
	  - Being replaced
		- on the cooking menu
		- on the crafting tab of the main menu

- Shift-left-click crafting:
  - For a stackable item
	  - Not being replaced
		- on the cooking menu
		- on the crafting tab of the main menu
	  - Being replaced
		- on the cooking menu
		- on the crafting tab of the main menu
  - For a non-stackable item
	  - Not being replaced
		- on the cooking menu
		- on the crafting tab of the main menu
	  - Being replaced
		- on the cooking menu
		- on the crafting tab of the main menu

- Right-click crafting
  - For a stackable item
	  - Not being replaced
		- on the cooking menu
		- on the crafting tab of the main menu
	  - Being replaced
		- on the cooking menu
		- on the crafting tab of the main menu
  - For a non-stackable item
	  - Not being replaced
		- on the cooking menu
		- on the crafting tab of the main menu
	  - Being replaced
		- on the cooking menu
		- on the crafting tab of the main menu

- Unintended transformation of a grabbed inventory item
  - In the cooking menu
  - In the crafting tab of the main menu