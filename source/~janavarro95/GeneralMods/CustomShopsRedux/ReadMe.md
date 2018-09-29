**Custom Shop Redux GUI** is a [Stardew Valley](http://stardewvalley.net/) mod which lets you
create custom shops by editing text files.

Compatible with Stardew Valley 1.2+ on Linux, Mac, and Windows.

## Installation
1. [Install the latest version of SMAPI](https://github.com/Pathoschild/SMAPI/releases).
2. <s>Install this mod from Nexus mods</s> (not yet released).
3. Run the game using SMAPI.

## Usage
Make a custom shop by copying & editing one of the included custom shop templates, or use the
[Custom Shops Creation Tool](https://myscccd-my.sharepoint.com/personal/0703280_my_scccd_edu/_layouts/15/guestaccess.aspx?guestaccesstoken=ZYxG9Cs8S0q%2bxCVV3fEnc8MI4SfVfe07919rhFUhRiA%3d&docid=0e51dae1da2eb43988f77f5c54ec3ee58)
(see [demo video](https://youtu.be/bSvNTZmgeZE)).

You can access the custom shops by pressing `U` in-game (configurable via `config.json`).

Supported item types:

* inventory items;
* furniture (windows, tables, chairs, etc);
* swords (swish, swish);
* gats (got to look cool);
* boots (lace up for adventure);
* wallpapers (make your house look nice);
* carpets & flooring (like Animal Crossing);
* rings (as long as they aren't evil);
* lamps (light up the world);
* craftables (note that there are some... issues with craftables. They all act like torches when
  you interact with them. It's kind of hilarious and I don't think I'll change it anytime soon. You
  can still have objects like the furnace function like normal, by right-clicking it with copper.
  In order to get the smelted copper bar however, you would have to destroy it, as would go for all
  machines that behave this way. Sorry. On the plus side your scarecrows can be on fire forever.)

## Goals
* [x] Get as many different types of items available for selling.
* [x] Create a nice GUI for creating custom shops so that modders don't have to deal with my icky
      formatting rules. The GUI will take care of that for modders.
* [x] Make my code compatible with other mods for modders, so that they can call my
      shop_command_code and be able to open up a shop from text file with just path information,
      and file names.

## Versions
1.0:
* Initial release.

1.0.1:
* Corrected price display to reflect markup.

1.0.2:
* Fixed issues where unintended items were bought.

1.1:
* Updated to Stardew Valley 1.1 and SMAPI 0.40 1.1-3.
* Fixed mouse not appearing in menu.
* Fixed bug where you sometimes couldn't buy an item even if you had enough money.

1.3:
* Updated to Stardew Valley 1.2 and SMAPI 1.12.

1.4:
* Updated for SMAPI 2.0.
* Switched to standard JSON config file.
* Internal refactoring.

1.4.1:
* Enabled update checks in SMAPI 2.0+.
