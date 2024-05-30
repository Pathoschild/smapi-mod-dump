**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/MolsonCAD/DeluxeJournal**

----

# Deluxe Journal
Deluxe Journal is a Stardew Valley mod that upgrades the in-game journal, adding new features for
keeping track of the day-to-day. Create a to-do list of tasks with a variety of different
auto-completion and renewal conditions, or jot down what you need on the notes page.

## Install
- Install [the latest version of SMAPI](https://smapi.io)
- Install [this mod from Nexus mods](https://www.nexusmods.com/stardewvalley/mods/11436)
- Unzip into the `StardewValley/Mods` folder
- Run the game using SMAPI

## Requirements
- Stardew Valley 1.6
- SMAPI version 4.0.0 or newer

## Features

### Tasks Page
The tasks page provides a checklist for you to keep track of daily tasks. Tasks can be marked as completed by pressing the checkbox on the left-hand side, or you can choose from various auto-completion conditions that will automatically track your progress - these are categorized into *task types*.

Task types can be applied automatically to "well-formed" task names that use the relevant keywords, and enough supplementary information (item/npc/building names), to create a task. Additionally, the task options menu can be used to fine-tune the task settings directly. Either click the wrench icon in the add task menu or click the name of an existing task to open the options menu.

At the end of each day, all completed tasks are removed unless given a *renewal period*. Choose between daily, weekly, monthly, annual, and *custom* renewals to automatically reactivate tasks after completion. A custom renewal period allows you to set the number of days to wait **after task completion**. Renewal periods must be set via the options menu.

#### Task Types
| Name | Description | Example |
| ---- | ----------- | -------- |
**Basic** | With no fancy bells and whistles, this task type requires manual completion. A basic task is always created when pressing "OK" instead of the "Smart Add" button when adding a new task.
**Collect** | Collect an item, with an optional count. This broadly covers any item pickups you'd like to track. | "Collect 300 wood"
**Craft** | Craft an item, with an optional count. This task focuses on items created via the crafting menu. | "Make a solar panel"
**Upgrade Tool**\*\* | Upgrade a tool at Clint's blacksmith shop. Completes upon picking up the upgraded tool. | "Upgrade my axe"
**Build**\*\* | Construct a farm building, or multiple farm buildings. | "Build a barn"
**Farm Animal**\*\* | Purchase farm animals from Marnie's Ranch. | "Buy 4 chickens"
**Gift** | Give a villager a gift, and optionally specify an item. | "Give a poppy to Penny"
**Buy**\*\* | Buy an item from a shop, with an optional count. | "Buy 48 pumpkin seeds"
**Sell**\*\* | Sell an item to a shop or overnight via the shipping bin, with an optional count. | "Sell 10 tea saplings"

\*\* *These tasks provide cost tracking, meaning the total amount to pay/gain after completing the
task(s) will show up in the money box at the bottom of the tasks page.*

#### Tips
- *Shortcuts*: Press the spacebar while on the tasks page to quickly open up the "Add Task" menu.
- *Task Order*: Click and drag to reorder tasks. Completed and inactive (waiting for renewal) tasks will always be grouped at the end of the task list for readability.
- *Money Box*: Pressing the "G" symbol on the money box will toggle between "total amount to pay/gain" and "net wealth."

### Notes Page
The notes page provides a section for writing down anything beyond the scope of a task.
*The gamepad is not currently supported for editing the notes.*

## Configuration
After this mod is run for the first time, a `config.json` file is created in the mod folder with the following configuration settings:
| Setting | Default | Description |
| ------- | ------- | ----------- |
| `PushRenewedTasksToTheTop` | `false` | Set to `true` to push renewed tasks to the top of the tasks list instead of keeping them at the bottom. |
| `EnableDefaultSmartAdd` | `true` | Enable to have the "Smart Add" button be the default when creating a task (if applicable). Set to `false` to always create a "Basic" task instead. |
| `EnableVisualTaskCompleteIndicator` | `false` | Set to `true` to enable a visual indicator, in addition to the audio cue, notifying you that a task has been completed. |
| `ShowSmartAddTip` | `true` | Show the "Smart Add" info box in the "Add Task" menu. *This is automatically set to `false` when pressing the red "X" in-game.* |
| `ShowAddTaskHelpMessage` | `true` | Show the help message when the task page is empty. *This is automatically set to `false` upon creating a new task.* |
| `MoneyViewNetWealth` | `false` | Toggle between "Net Wealth" and "Total Amount to Pay/Gain" display modes. *This is automatically toggled by pressing the "G" icon on the Money Box.* |

## Mod Integration
There is some rudimentary support for adding custom pages and there's groundwork done for custom tasks.
If there's interest, I can add some more information on this and implement support for it.

## See Also
- [Source code](https://github.com/MolsonCAD/DeluxeJournal)
