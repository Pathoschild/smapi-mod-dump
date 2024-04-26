**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Leroymilo/MZG**

----

# How to add you own Zen Garden

You need to make 2 sprites : the base and the feature. The base is the sand pattern that will appear under/behind the border, the feature is anything that will be drawn over the borders (a tree, a rock, a statue, anything).</br>
Both of these can be made seasonal : base_spring through base_winter and feature_spring through feature_winter. Note that the mod will first search for a seasonal variant, then for a general sprite (base.png and feature.png), then default to a blank base and a fully transparent feature if nothing is found. It is possible to define a general sprite and special sprites for certain seasons, for example the [Tree Zen Garden](https://github.com/Leroymilo/MZG/tree/main/ModularZenGarden/assets/gardens/tree) only has a base_winter.png and a base.png, the latter being used for spring through fall.</br>
For the rest of this tutorial, we'll call N and M the width and depth of your custom Zen Garden in tiles. The size of your textures should be 16\*Nx32\*M. For reference, all the sprites in the Tree Zen Garden (linked above) are 48x96 pixels, so they are for a 3x3 graden.


## The base

The base should fit in 16\*Nx16\*M pixels, and should be made from [this base](https://raw.githubusercontent.com/Leroymilo/MZG/main/ModularZenGarden/assets/default_base_3x3.png) so that the lines are aligned with other Gardens.</br>
Even though the texture needs to be 16\*Nx16\*M pixels to properly connect with other gardens when borders are removed, keep in mind that the borders will hide a few pixels of your sprite most of the time, so the best practice is to avoid putting your base pattern too close to the borders, or completely avoid the areas marked in red and purple on the gif below, leaving a (16\*N-4)x(16\*M-7) area to work with (so 46x41 if working on a 3x3 garden).</br>
This gif will also show you that when placing your base in the 16\*Nx32\*M final texture, you need to leave 2 pixels at the bottomn to leave space to place the border.</br>
![example gif](https://github.com/Leroymilo/MZG/blob/main/images/example.gif)

To make the winter variant of your base, you can base it of from [this sprite](https://raw.githubusercontent.com/Leroymilo/MZG/main/ModularZenGarden/assets/default_base_3x3_winter.png), if you have the right tools you can make the same colors by applying an overlay layer at 50% opacity with the color #66cdff to your base for other seasons, this is what I did with GIMP.


## The feature

The feature follows simpler rules : the sprite must be 16\*Nx32\*M pixels and you need to remember that anything you draw in the seven lowest pixels of the texture will be drawn **OVER** the border this time, so if you want to hide something behind the border it needs to be in the base, not the feature.</br>
The only other thing you need to be careful of is to properly line-up your base and your feature.</br>
I'm not 100% certain of how semi-transparent pixels will behave in certain situations, so if you see your base dissapearing in certain spots, it's probably because there are semi-transparent pixels in your feature.


## The config

To bring your creation to life, you'll need to add it to the config. Here are the steps :
- pick a new unique name for your garden
- create a new folder with this name in [assets/gardens](https://github.com/Leroymilo/MZG/tree/main/ModularZenGarden/assets/gardens)
- put all your sprites in this new folder, here's a list of the names your sprites should have (you don't need all of them) :
  - `base.png`, `base_spring.png`, `base_summer.png`, `base_fall.png`, `base_winter.png`
  - `feature.png`, `feature_spring.png`, `feature_summer.png`, `feature_fall.png`, `feature_winter.png`
- open [assets/types.json](https://github.com/Leroymilo/MZG/blob/main/ModularZenGarden/assets/types.json) in any text editor
- create a new entry in the file with this stucture:
```json
{
  "stones": {
    ... // previous entries
  },
  "[your garden name]": {
    "width": [the width of your garden in tiles],
    "height": [the height of your garden in tiles],
    "author": [your name (optional)],
    "use_default_base": [whether or not to use the default blank base (optional)],
    "use_default_feature": [whether or not to use the default empty feature (optional)],
  }
}
```
The fields "width" and "height" must be numbers, "author" must be text (surrounded by quotes `"`), "use_default_base" and "use_default_feature" can be either `true` or `false`.
All fields marked with (optional) can be completely ommited, "author" defaulting to a blank name and the "use_default" options defaulting to `false`.


## Testing

Once you did every step listed above, you can start Stardew Valley to test your new garden, it should show up in the Zen Garden Catalogue. If you're having any issue making it work, you can ask me for help on the [mod page](https://www.nexusmods.com/stardewvalley/mods/22140?tab=posts) or ping me on the Stardew Valley discord server for a quicker response.</br>
If you'd like to share your work with other players, I'll gladly add it to the mod (with your name credited in the mod and on the mod page of course), because the mod is a bit lacking content since I'm not an artist...
