**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/KhloeLeclair/StardewMods**

----

# Changelog

## 0.2.0
Released April 16th, 2024.

### Added
* The `mre_pick` command can be used to view what event would be
  selected tonight. Useful for testing conditions without actually
  going to bed to trigger an event.
* Placement events can now spawn buildings, optionally with animals.
* Placement events can now spawn crops.
* Placement event output entries can now accept a list of `SpawnAreas`,
  which are rectangles that limit the possible locations chosen to
  only be within the rectangles.
* Placement events have a new `RequireMinimumSpots` flag that will
  cause them to abort if they don't find `MinStack` valid locations.
* Events can now have a Priority, which changes how they're sorted
  in the event list. Events with a higher priority have a chance
  to happen first.
* Events can now be tagged as Exclusive. When determining which event
  should happen in a night, the first exclusive event to pass its
  conditions will be used. If no exclusive event passes, then all
  the remaining events with passing condition are selected between.
* Non-exclusive events now have a Weight on their condition, which
  can be used to adjust the likelyhood that that event is chosen
  when there are multiple matching events.

### Changed
* The placement event now displays an icon on screen when a sound is
  playing, like the vanilla SoundInTheNight event does.
* The sample content pack now has a 1% chance for any given event
  to happen, rather than 0%, so you don't *need* to trigger them
  manually. It's just as rare as a vanilla event.

### Fixed
* The farmer can now be used in `Script` events and will be visible
  and everything.


## 0.1.0
Released April 16th, 2024.

This is the initial release of More Nightly Events. Please look forward
to it, everyone! I look forward to seeing what the community comes up
with, and what we can create together.
