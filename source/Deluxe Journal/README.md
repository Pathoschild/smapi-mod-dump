**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/MolsonCAD/DeluxeJournal**

----

# Deluxe Journal
Deluxe Journal is a Stardew Valley mod that upgrades the in-game journal, adding new features for
keeping track of the day-to-day. Create a to-do list of tasks with a variety of different
auto-completion conditions, or just jot down what you need on the notes page.

## Install
- Install [the latest version of SMAPI](https://smapi.io)
- Install [this mod from Nexus mods](https://www.nexusmods.com/stardewvalley/mods/11436)
- Unzip into the `StardewValley/Mods` folder
- Run the game using SMAPI

## Requirements
- Stardew Valley 1.5
- SMAPI version 3.9.0 or newer

## Features

### Tasks Page
The tasks page provides a checklist for you to keep track of daily tasks. Tasks can be marked as completed by pressing the checkbox on the left-hand side, or you can choose from a variety of different auto-completion conditions that will automatically track your progress.

At the end of each day, all completed tasks are removed unless they are given a renew period. Tasks can be given a renew period (daily, weekly, monthly, annually) to automatically reactivate themselves after completion.

#### Task Types:
- **Basic**: Requires manual completion. This type is always selected when pressing "OK" instead of the "Smart Add" button when adding a new task.
- **Collect**: Collect an item, with an optional count. e.g. "Collect 100 wood"; "Find a diamond"
- **Craft**: Craft an item, with an optional count. e.g. "Craft a solar panel"; "Make 5 wood lamp-post"
- **Build**\*\*: Construct a farm building. e.g. "Build a barn"; "Build a deluxe coop"
- **Upgrade Tool**\*\*: Upgrade a tool. e.g. "Upgrade my axe"
- **Gift**: Give someone a gift, with an optional item specification. e.g. "Give robin a gift"; "Give a poppy to Penny"
- **Buy**\*\*: Buy an item, with an optional count. e.g. "Buy potato seeds"; "Buy 100 coal"
- **Sell**\*\*: Sell an item, with an optional count. e.g. "Sell 10 tea sapling"; "Sell my lava katana"

\*\* *These tasks provide cost tracking, meaning the total amount to pay/gain after completing the task will show up in the money box at the bottom of the tasks page.*

#### Tips:
- When adding a task, the above task types can be applied automatically (simply by typing a name that matches the format of the desired type) or manually by opening the options menu. This can also be changed after task creation by pressing the name of the task.
- Click and drag to reorder tasks. Completed and inactive (waiting for renewal) tasks will always be grouped together, however, for readability.
- Pressing the "G" symbol on the money box will toggle between "total amount to pay/gain" and "net wealth."
- If the audio cue won't cut it, you can set the EnableVisualTaskCompleteIndicator setting in the config.json file to "true" to enable a visual indicator.

### Notes Page
The notes page provides a section for writing down anything that's beyond the scope of a task.
Fill it with anything you want!

## Mod Integration
There is some rudimentary support for adding custom pages and there's groundwork done for custom tasks.
If there's interest, I can add some more information on this and implement support for it.

## See Also
- [Source code](https://github.com/MolsonCAD/DeluxeJournal)
