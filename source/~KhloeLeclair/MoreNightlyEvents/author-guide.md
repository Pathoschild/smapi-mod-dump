**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/KhloeLeclair/StardewMods**

----

← [README](README.md)

Sadly, I haven't even started on this yet. Please check out the sample content pack
for now to see what event data looks like. There are comments and sample events
you can trigger using the `mne_test` command.


## Contents

* [Getting Started](#getting-started)
  * [Create a Content Pack](#create-a-content-pack)
  * [Using Content Patcher](#using-content-patcher)
* [Nightly Events](#nightly-events)
  * [Conditions](#conditions)
  * [Side Effects](#side-effects)
* [Event Types](#event-types)
  * [`Placement`](#placement)
	* [Output](#output)
  * [`Script`](#script)
* [Commands](#commands)
* [Mod Data](#mod-data)
* * [Triggers](#triggers)


## Getting Started

### Create a Content Pack

1. Install [SMAPI](https://smapi.io/) and [More Nightly Events](https://www.nexusmods.com/stardewvalley/mods/22612)
   if you haven't yet. (If you haven't, how did you even find this?)
2. Create an empty folder in your `Stardew Valley\Mods` folder and name it
   `[MNE] Your Mod's Name`. Replace `Your Mod's Name` with your mod's
   unique name, of course.
3. Create a `manifest.json` file inside the folder with this content:
   ```json
   {
	   "Name": "Your Mod's Name",
	   "Author": "Your Name",
	   "Version": "1.0.0",
	   "Description": "Something short about your mod.",
	   "UniqueId": "YourName.YourModName",
	   "ContentPackFor": {
		   // Do not change this when changing UniqueID
		   "UniqueID": "leclair.morenightlyevents"
	   },
	   "UpdateKeys": [
		   // When you get ready to release your mod, you will populate
		   // this section as according to the instructions at:
		   // https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Update_checks
	   ]
   }
   ```
4. Change the `Name`, `Author`, `Description`, and `UniqueID` values to
   describe your mod. Later, don't forget to set your UpdateKeys before
   uploading your mod.
5. Create an `events.json` file inside the folder, with a list of your
   custom events:
   ```js
   [
	   // your events go here
   ]
   ```


### Using Content Patcher

If you want to use Content Patcher rather than a content pack, you can!
Just use the `EditData` action with the target:
`Mods/leclair.morenightlyevents/Events`

> Note: It is currently untested what happens when players get different
> events from this mod in Multiplayer, so be careful with dynamic tokens
> and the like. Or don't, and let me know how it goes!


## Nightly Events

A nightly event is something that can happen... at night! The base game
makes use of several different ones, including but not limited to:
- The Junimos repairing things around the valley
- Noises in the night, like an earthquake
- An owl statue appearing on your farm

This mod lets you add your own! They have game state queries to control
when they happen, and many different ways to control their behaviors.

Each event has the following properties:

<table>
<tr>
<th>Field</th>
<th>Description</th>
</tr>
<tr><th colspan=2>Identity</th></tr>
<tr>
<td><code>Id</code></td>
<td>

**Required.** The event's unique Id. This must be unique, and is used
to identify your specific event.

</td>
</tr>
<tr>
<td><code>Type</code></td>
<td>

**Required>** The [event type](#event-types) of this event.

</td>
</tr>
<tr><th colspan=2>Selection</th></tr>
<tr>
<td><code>Priority</code></td>
<td>

*Optional.* What priority should this event have, compared to other events?
This is used to sort the event list, where events with a higher priority
come first.

Default: `0`

</td>
</tr>
<tr>
<td><code>Conditions</code></td>
<td>

*Optional.* A list of [Conditions](#conditions) that will cause
this event to happen. If your event is going to be manually triggered,
then you can leave this empty, otherwise you should have at least one.

</td>
</tr>
<tr><th colspan=2>Shared Properties</th></tr>
<tr>
<td><code>SideEffects</code></td>
<td>

*Optional.* A list of [side effects](#side-effects) that will happen
after this event takes place.

</td>
</tr>
<tr>
<td><code>TargetMap</code></td>
<td>

*Optional.* The map where this event should take place.

Default: `"Farm"`

</td>
</tr>
<tr>
<td><code>OverrideWeather</code></td>
<td>

*Optional.* If this is set, the weather at the target location will be
overridden while the event plays using this weather condition. This
can be any weather condition in the game: `"Sun"`, `"Rain"`, `"Storm"`, etc.

</td>
</tr>
<tr>
<td><code>TimeOfDay</code></td>
<td>

*Optional.* The time of day that should be set during the event. This should
be a four digit string, where the first two digits are the hour (in 24-hours)
and the second two digits are the minutes. Example: `2200` for 22:00 / 10:00 pm.

Default: `"2400"`

</td>
</tr>
<tr>
<td><code>AmbientLight</code></td>
<td>

*Optional.* A specific ambient light color to use during the event.

</td>
</tr>
</table>


### Conditions

Each of an event's conditions has a game state query that needs to pass for
the condition to match, as well as a numeric `Chance` that gets re-used by
each of a specific event's conditions in a given night.

<table>
<tr>
<th>Field</th>
<th>Description</th>
</tr>
<tr><th colspan=2>Identity</th></tr>
<tr>
<td><code>Condition</code></td>
<td>

**Required.** A game state query that must evaluate to true for this
condition to pass, and for this event to happen.

</td>
</tr>
<tr>
<td><code>Chance</code></td>
<td>

*Optional.* The chance that this event will happen. This is checked
after `Condition`, and it is checked against the same number for
each of an event's conditions in a given night. Set this to `0` for
a 0% chance to happen and `1.0` for a 100% chance to happen.

Default: `1.0`

</td>
</tr>
<tr>
<td><code>IsExclusive</code></td>
<td>

*Optional.* If this is set to true, this condition passing will mark
the event as exclusive and cause it to be chosen immediately,
disregarding any other possible nightly events.

</td>
</tr>
<tr>
<td><code>Weight</code></td>
<td>

*Optional.* If more than one event can happen in a night, and none of
them are flagged as exclusive, then one of them is picked at random to
happen. Increase this value to increase the chance that this event is
picked, or decrease it to decrease the chance.

Default: `1.0`

</td>
</tr>
</table>


### Side Effects

Each event can have one or more side effects, which cause
[trigger actions](https://stardewvalleywiki.com/Modding:Trigger_actions) to run.

<table>
<tr>
<th>Field</th>
<th>Description</th>
</tr>
<tr><th colspan=2>Identity</th></tr>
<tr>
<td><code>Condition</code></td>
<td>

*Optional.* A game state query that, if set and not true, will cause this
side effect to be skipped.

</td>
</tr>
<tr>
<td><code>HostOnly</code></td>
<td>

*Optional.* Whether this side effect can only run for the main player. If set to
true, the side effect will be ignored for farmhands in multiplayer.

</td>
</tr>
<tr>
<td><code>Actions</code></td>
<td>

**Required.** The actions to perform, as a list of strings matching the
[action format](https://stardewvalleywiki.com/Modding:Trigger_actions#Actions).

</td>
</tr>
</table>


## Event Types

The following event types can be used:

* [`Placement`](#placement)
* [`Script`](#script)


### `Placement`

Performs an event that places an object, building, crop, etc. into the world alongside
playing a sound effect or showing a message to the player.

<table>
<tr>
<th>Field</th>
<th>Description</th>
</tr>
<tr>
<td><code>Message</code></td>
<td>

*Optional.* A string to show to the player. This supports [tokenizable strings](https://stardewvalleywiki.com/Modding:Tokenizable_strings)
including localization with the `[LocalizedText <key>]` token.

</td>
</tr>
<tr>
<td><code>MessageDelay</code></td>
<td>

*Optional.* How long to wait before displaying `Message` to the player,
in milliseconds.

Default: `7000`

</td>
</tr>
<tr>
<td><code>SoundName</code></td>
<td>

*Optional.* The name of an [audio cue](https://stardewvalleywiki.com/Modding:Audio#Track_list)
to play while this event is happening. If not set, no sound will play.

</td>
</tr>
<tr>
<td><code>Output</code></td>
<td>

**Required.** A list of [placement output](#output) one or more entries. Only
the first output to have its condition match will be placed.

</td>
</tr>
</table>


### Output

Each `Placement` event must have one or more output entry. These entries
determine exactly what is placed by the placement event.

<table>
<tr>
<th>Field</th>
<th>Description</th>
</tr>
<tr><th colspan=2>Shared</th></tr>
<tr>
<td><code>Condition</code></td>
<td>

*Optional.* A game state query that, if set and not true, will cause this
output to be skipped.

</td>
</tr>
<tr>
<td><code>Type</code></td>
<td>

**Required.** What type of placeable thing should we spawn?

* `Building`: Construct a building. The target map must support
  placing buildings.
* `Crop`: Plant a crop. The target map must have diggable areas.
* `FruitTree` Place a fruit tree.
* `GiantCrop`: Place a giant crop.
* `Item`: An item. Either leave it on the ground to be picked up, or place it
  if it can be placed.
* `ResourceClump` Place a resource clump, like a stump or a meteorite.
* `WildTree`: Place a non-fruit tree.

</td>
</tr>
<tr>
<td><code>SpawnAreas</code></td>
<td>

*Optional.* A list of rectangles that constitute valid spawning spaces
for this event. Each entry in the list should have an `X`, `Y`, `Width`,
and `Height`. Example:

`{"X": 10, "Y": 15, "Width": 20, "Height": 5}`

If this is set, things will **only** be placed within these areas.

</td>
</tr>
<tr>
<td><code>RequireMinimumSpots</code></td>
<td>

*Optional.* When set to true, if we cannot find at least `MinStack` valid
locations, then the event will not run.

</td>
</tr>
<tr>
<td><code>ItemId</code></td>
<td>

**Required.** The Id of the thing to place. This behaves differently depending
on what `Type` of thing we're placing. For items, it should be a qualified
item Id. For buildings, crops, fruit trees, giant crops, and wild trees, it
should be an Id from the relevant data file.

</td>
</tr>
<tr>
<td><code>RandomItemId</code></td>
<td>

*Optional.* A list of strings. If this is set, `ItemId` will be ignored.
Each thing to be placed will pick a random Id from this list.

</td>
</tr>
<tr>
<td><code>MinStack</code></td>
<td>

*Optional.* The minimum number of things to place. Defaults to `1`. This
event may actually place fewer than this many things, if there are issues
finding enough valid places to spawn them.

Default: `1`

</td>
</tr>
<tr>
<td><code>MaxStack</code></td>
<td>

*Optional.* The maximum number of things to place. Defaults to being the
same as `MinStack`. If this is set, and higher than `MinStack`, a variable
number of things may be placed.

</td>
</tr>
<tr><th colspan=2>Item Fields</th></tr>
<tr>
<td><code>Contents</code></td>
<td>

*Optional.* A list of [item spawn fields](https://stardewvalleywiki.com/Modding:Item_queries#Item_spawn_fields)
that will be used to generate items to fill the inventory of the placed
thing, if the placed thing has an inventory that can contain items.
(So... chests, basically.)

</td>
</tr>
<tr><th colspan=2>Building Fields</th></tr>
</table>

> TODO: Sorry! I'm not done writing this yet. Still need to cover buildings,
> trees, crops, and resource clumps. Please look at the sample content pack for more!


### `Script`

Run an [event script](https://stardewvalleywiki.com/Modding:Event_data#Event_scripts)
as a nightly event.

<table>
<tr>
<th>Field</th>
<th>Description</th>
</tr>
<tr>
<td><code>Script</code></td>
<td>

**Required.** The [event script](https://stardewvalleywiki.com/Modding:Event_data#Event_scripts) to run.
This should support all standard event script features. Please contact me if you discover something
that doesn't run or that breaks things.

</td>
</tr>
</table>


## Commands

### `mne_interrupt`

Immediately stop the current event from processing, which may save
you if you get stuck in a bad event.

### `mne_invalidate`

Invalidate the event cache, so it will be reloaded the next time an
event is called for. Use this to reload your event data files when
editing events for testing.

### `mne_list`

List all the available events with their IDs.

### `mne_pick [day]`

Check to see which event would run on the given day, or today if no
day is specified. The day should be a number, where Spring 1, Year 1
is 1 and Summer 1, Year 1 is 29, etc.

### `mne_test [event]`

This allows you to force the next nightly event to be the event with
the given ID. This bypasses the conditions on the event, but the event
may fail to happen if other conditions aren't met (like a spawn event
not being able to spawn something).

You can use `mne_test clear` to clear the forced event.


## Mod Data

### `leclair.morenightlyevents/AlwaysInSeason`

This modData is supported on crop and fruit tree instances. Setting
it to `true` will cause the crop or tree to always be considered
in season no matter the location and season.

This is set by the Placement action when `IgnoreSeasons` is set
to `Always`.


## Triggers

### `leclair.morenightlyevents_ForceEvent [event]`

Works like the `mne_test` command, but as a trigger action. Lets you
either clear or set an event override for the coming night.
