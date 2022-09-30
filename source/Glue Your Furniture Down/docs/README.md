**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/elizabethcd/PreventFurniturePickup**

----

# Glue Your Furniture Down
This mod adds options to prevent picking up furniture by type.

## How to use this mod

There are two ways to use this mod: by manually editing the config.json file, and by using the Generic Mod Config Menu (GMCM) integration. I highly recommend the GMCM integration, since otherwise there's no way to change settings in the middle of a day. Please refer to GMCM documentation for instructions on how to use that mod. 

The config options are:
  * `PreventAllPickup`: A global toggle that turns off picking up all furniture. Overrides all other config options when true. 
  * `FurniturePlacementKey`: A keybind to bypass this mod. When this key combination is held down, all of this mod functionality is off. 
  * `CanPickUpBed`
  * `CanPickUpChair`
  * `CanPickUpTable`
  * `CanPickUpDresser`
  * `CanPickUpDecoration`
  * `CanPickUpTV`
  * `CanPickUpLamp`
  * `CanPickUpRug`
  * `CanPickUpWindow`
  * `CanPickUpFireplace`
  * `CanPickUpTorch`
  * `CanPickUpSconce`
  * `CanPickUpFishTank`

Everything except the first two is just a boolean (either true or false), so you can turn on/off the ability to pick up each type of furniture, as long as `PreventAllPickup` is false. 

## Questions?

If you have any questions, you can find me on the Stardew Valley discord in #making-mods.
