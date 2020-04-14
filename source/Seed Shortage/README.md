# Seed-Shortage

**[NEXUS PAGE](https://www.nexusmods.com/stardewvalley/mods/5614)**<br/>


## Contents
* [Description](#description)

* [Installation](#installation)

* [Configurations](#configurations)
  * [Vendors](#vendors)

* [Compatibility](#compatibility)
  * [Mod Compatibility](#mod-compatibility)
  * [Better Mixed Seeds Config compatibility](#better-mixed-seeds-config-compatibility)

## Description

Seed Shortage is a mod for Stardew Valley that removes the seeds from the shops. Instead, players should clean up the farm, go out in the forest and the mines in order to find some new varieties.<br/>

It has a Content Pack that replaces the recipe of the Seed Maker to be available in the beginning. It requires some copper ore so the player should explore the mine when it's open to. By that time, the first parsnip seeds should be grown and can be used to make some other seeds.<br/>

The mod also has an optional config file that changes the seed drop rate. It is highly recommended to use it for a balanced playthrough. It aims to add some RNG to the game, and make some seeds rare than others.<br/>

Visit the [Nexus page](https://www.nexusmods.com/stardewvalley/mods/5614) for more informations.<br/>

## Installation

* Install [SMAPI](https://smapi.io/), [Content Patcher](https://www.nexusmods.com/stardewvalley/mods/1915) and [Json Assets](https://www.nexusmods.com/stardewvalley/mods/1720)
* Install [Better Mixed Seeds](https://www.nexusmods.com/stardewvalley/mods/3012) to be ableto get the seeds
* If you want to add any custom crop mods, do so.
* Download and install [Seed Shortage](https://www.nexusmods.com/stardewvalley/mods/5614)<br/>

If you do not know how to install mods, [visit this link](https://stardewvalleywiki.com/Modding:Player_Guide/Getting_Started)<br/>

## Configurations

When launching the game, the mod will create a `config.json` file where you can configure the mod to your liking.
Here are the different configurations for the mod.

<table>
<tr>
<th>Setting</th>
<th>What it does</th>
<th>Available values</th>
</tr>
<tr>
<td> 
  <code>VendorsWithoutSeeds</code>
</td>
<td>
  
  The vendors whose names are present in this config will not be selling seeds anymore. You must use the exact names of the vendors, and write it in quotation marks and commas in between except for the last one, just like this:
```js
"VendorsWithoutSeeds": [
"name1",
"name2",
"name3"
]
```
</td>
<td>
  
  _String value._<br/>
  See the different [available vendor names](#vendors) below.
</td>
</tr>
<tr>
<td>
  <code>VendorsPrice</code>
</td>
<td>
  Same as the vendors without seeds, the shop owners whose names are listed here will have the price of their seeds increased by a value defined in the next config. It follows the same formatting as the vendors without seeds, so the exact name, in quotation marks, and commas in between each name. Keep in mind that <code>VendorsWithoutSeeds</code> will take priority over this, so you if you have the same name in both <code>VendorsWithoutSeeds</code> and <code>VendorsPrice</code>, the seeds will be removed.
</td>
<td>
  
  _String value._<br/>
  See the different [available vendor names](#vendors) below.
</td>
</tr>
<td>
  <code>PriceIncrease</code>
</td>
<td>
  
  This is a bit special. The mod accept two different types of increments: a percentage and a multiplier. Supposing that "P" is your price in as an **integer** number, you need to either write "P%" or Px" as it detects which one it ends with. For example:<br/>
  
  <code>
  PriceIncrease = "10x";
  </code><br/>
  or<br/>
  <code>
  PriceIncrease = "45%";
  </code>
</td>
<td>
  
  _String value._<br/>
  This only supports an **INTEGER** value. So if you want to multiply the price by 1.5, you can't write "1.5x", you will have to write "150%" instead.
</td>
</tr>
<tr>
<td>
  <code>CropExceptions</code>
</td>
<td>
  
  The seeds listed here will be added as an exception to the seed removal process. You need to specify the **exact** _English_ name of the seed you want to add. For example:
```js
"CropExceptions": [
"Parsnip Seeds",
"Pink Cat Seeds",
"Coffee Bean"
]
```
</td>
<td>
  
  _String value._<br/>
  I suggest you look at [Mousey's Database](https://mouseypounds.github.io/ppja-ref/index.html) to know the exact name of the seeds.
</td>
</tr>
</table>

### Vendors

Here is the list of the different available names you can put in the config file:

<table>
<tr>
<th>Name</th>
<th>Shop</th>
</tr>
<tr>
<td><code>Pierre</td>
<td>
  
  [Pierre's General Store](https://stardewvalleywiki.com/Pierre%27s_General_Store)</td>
</tr>
<tr>
  <td><code>Marnie</code></td>
<td>
  
  [Marnie's Ranch](https://stardewvalleywiki.com/Marnie%27s_Ranch)</td>
</tr>
<tr>
  <td><code>Sandy</code></td>
<td>
  
  [The Oasis](https://stardewvalleywiki.com/Oasis)</td>
</tr>
<tr>
  <td><code>Clint</code></td>
<td>
  
  [Blacksmith](https://stardewvalleywiki.com/Blacksmith)</td>
</tr>
<tr>
  <td><code>Marlon</code></td>
<td>
  
  [The Adventurer's Guild](https://stardewvalleywiki.com/Adventurer%27s_Guild)</td>
</tr>
<tr>
  <td><code>Dwarf</code></td>
<td>
  
  [The Mines](https://stardewvalleywiki.com/Dwarf)</td>
</tr>
<tr>
  <td><code>Gus</code></td>
<td>
  
  [The Stardrop Saloon](https://stardewvalleywiki.com/The_Stardrop_Saloon)</td>
</tr>
<tr>
  <td><code>Krobus</code></td>
<td>
  
  [The Sewers](https://stardewvalleywiki.com/Krobus)</td>
</tr>
<tr>
  <td><code>Robin</code></td>
<td>
  
  [Capenter's Shop](https://stardewvalleywiki.com/Carpenter%27s_Shop)</td>
</tr>
<tr>
  <td><code>Willy</code></td>
<td>
  
  [The Fish Shop](https://stardewvalleywiki.com/Fish_Shop)</td>
</tr>
<tr>
  <td><code>Harvey</code></td>
<td>
  
  [Harvey's Clinic](https://stardewvalleywiki.com/Harvey%27s_Clinic)</td>
</tr>
<tr>
  <td><code>Joja</code></td>
<td>
  
  [JojaMart](https://stardewvalleywiki.com/JojaMart)</td>
</tr>
<tr>
  <td><code>Travelling Merchant</code></td>
<td>
  
  [Travelling Cart](https://stardewvalleywiki.com/Traveling_Cart)</td>
</tr>
<tr>
  <td><code>Wizard</code></td>
<td>
  
  [The Wizard's Tower](https://stardewvalleywiki.com/Wizard%27s_Tower)</td>
</tr>
<tr>
  <td><code>Magic Boat</code></td>
<td>
  
  [The Magic Boat at the Night Market](https://stardewvalleywiki.com/Night_Market)</td>
</tr>
<tr>
  <td><code>Seed Catalogue</code></td>
<td>
  
  [The Seed Catalogue from the "Seed Catalogue" mod](https://www.nexusmods.com/stardewvalley/mods/1640)</td>
</tr>
</table>

## Compatibility

### Mod compatibility

Seed Shortage has been designed with compatibility in mind. Thus, it is compatible with every other crop mods out there as well as all the custom shops mods.<br/>

For the crop mods, players will simply need to add the crop's seed's name in the <code>CropExceptions</code> config for it to be excluded from the removal, as it uses Json Assets to get modded crops IDs.<br/>

For the custom shops, the mod will simply not affect these. For it to be included in the mod, I need to add this manually add a way to detect it. If you want me to add a custom shop, ask in the comments section in the [mod's Nexus page](https://www.nexusmods.com/stardewvalley/mods/5614).

### Better Mixed Seeds Config compatibility

The BMS config supports the following mods:<br/>

* [Fruits and Veggies](https://www.nexusmods.com/stardewvalley/mods/1598)
* [Mizu's Flowers](https://www.nexusmods.com/stardewvalley/mods/2028)
* [Farmer to Florist](https://www.nexusmods.com/stardewvalley/mods/2075)

You can see the drop chances of the Vanilla and JA configs [here](https://docs.google.com/spreadsheets/d/19NQOwh7jsPuDfbe_DhlLuAqT31xA5CnKkMWyS94TSko/edit?usp=sharing) and [here](https://docs.google.com/spreadsheets/d/113_m9x1nYK6qcdhJjijLgbjmj6STz8ZHwP3yXxBcS04/edit?usp=sharing) respectively.
