**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/andyruwruw/stardew-valley-water-bot**

----

 <p align="center">
  <img src="https://raw.githubusercontent.com/andyruwruw/stardew-valley-water-bot/main/documentation/cover-image.gif">
 </p>
 
 <p align="center">
  <a href="https://www.nexusmods.com/stardewvalley/mods/8167">Download on Nexus</a>
  ·
  <a href="https://github.com/andyruwruw/stardew-valley-water-bot/issues/new/choose">Submit an Issue</a>
  ·
  <a href="https://www.youtube.com/watch?v=_8lDC51DbRs&feature=youtu.be">Youtube Video</a>
</p>
 
 <p align="center">
  For my fellow lazy farmers without sprinklers
 </p>

# WaterBot

**WaterBot** is a [Stardew Valley](https://www.stardewvalley.net/) mod that helps you water your crops while staying as vanilla as possible.

Unlike other mods, watering your crops will still cost you the same amount of time and energy as if you had done it yourself, just a little less tedious.

When you **right-click** a not fully grown plant with your **Watering Can**, the bot will take control of your character, **watering all your crops** and **refilling the water** can at the nearest water source whenever necessary.

**Press any button** at any point to stop the bot. The bot will automatically stop if you have low stamina.

# Contents

- [Install](#install)
- [How to Use](#how-to-use)
- [Implementation](#implementation)
  1. [Trigger](#1-trigger)
  2. [Loading the Player's Farm](##2-loading-the-players-farm)
  3. [Find Grouped Crops](#3-find-grouped-crops)
  4. [Cost of Traveling Between Groups](#4-cost-of-traveling-between-groups)
  5. [The Traveling Water Man](#5-the-traveling-water-man)
  6. [Watering the Group](#6-watering-the-group)
  7. [Out of Water!](#7-out-of-water)

# Install

1. Install the latest version of [SMAPI](https://smapi.io/).
2. Download this mod and unzip the contents.
3. Place the mod in your Mods folder.
4. Run the game using SMAPI.

# How to Use

Good morning!

Time to water your crops.

Pull out your **Watering Can**, and **left-click** on any of your beautiful crops as if you were watering them.

By a miracle of love for your budding crops, you're carried away into watering the rest of them without pushing so much as a button.

You automatically get more water to refill your watering can from instinct, still in a lucid state.

Snap out of it early by pressing any button.

Don't worry, you won't knock yourself out from watering, you will stop before you run out of stamina

# Implementation

### 1. Trigger

To begin the mod listens for whenever the player `right-clicks`. If the player is clicking a crop with their watering can, the bot starts.

### 2. Loading the Player's Farm

The bot first looks through the farm map data, going tile by tile and marking the following traits:

| Trait     | Description                                         |
|-----------|-----------------------------------------------------|
| Waterable | Does the tile need to be watered                    |
| Block     | Does the tile allow the player to walk on top of it |
| Water     | Can the player refill the Watering Can here         |

All the tiles are placed in a 2D array. Any waterable crops are also placed in their own array.

<img src="https://raw.githubusercontent.com/andyruwruw/stardew-valley-water-bot/main/documentation/implementation/load_map.gif">

For the image above, the tiles are colors accordingly:

| Color      | Waterable | Block | Water |
|------------|-----------|-------|-------|
| Green      | True      | False | False |
| Dark Green | True      | True  | False |
| Turquoise  | False     | True  | True  |
| Dark Blue  | False     | False | True  |
| Red        | False     | True  | False |

### 3. Find Grouped Crops

Tiles with crops that need watering are then grouped based on adjacency using depth-first search.

<img src="https://raw.githubusercontent.com/andyruwruw/stardew-valley-water-bot/main/documentation/implementation/find_groups.gif">

### 4. Cost of Traveling Between Groups

The bot then uses A* pathfinding to determine the cost of traveling from one group to another.

The algorithm starts at the tile closest to the centroid of each grouping.

The cost of traveling to each group is also done from the player's current position.

<img src="https://raw.githubusercontent.com/andyruwruw/stardew-valley-water-bot/main/documentation/implementation/cost_matrix.gif">

This gives us a nice cost matrix!

|        | Player | Purple | Yellow | Blue |
|--------|--------|--------|--------|------|
| Player | -1     | 11     | 5      | 5    |
| Purple | 11     | -1     | 6      | 8    |
| Yellow | 5      | 6      | -1     | 6    |
| Blue   | 5      | 8      | 6      | -1   |

At this point any unreachable groups are disregarded.

### 5. The Traveling Water Man

We need to find the shortest path through all the groups, starting at the player's current position.

The bot runs a greedy approach to solving the travelling salesman problem.

The starting point is the player's position.

### 6. Watering the Group

For each group, depth first search is applied to fill in the tiles.

At each tile, all adjacent (now including diagonals) are watered as well. This means we can skip walking to every block and simply water anything around us.

If a block cannot be stood on, the bot chooses the next best option and waters it from there.

<img src="https://raw.githubusercontent.com/andyruwruw/stardew-valley-water-bot/main/documentation/implementation/fill_group.gif">

### 7. Out of Water!

When the watering can is low, the bot will go to the nearest source of water to refill.

The closest refillable spot is found using breadth-first-search from the player's position.

Once the spot is found, the bot navigates the player to the closest spot to the water, refills, then returns to watering.

<img src="https://raw.githubusercontent.com/andyruwruw/stardew-valley-water-bot/main/documentation/implementation/refill_water.gif">
