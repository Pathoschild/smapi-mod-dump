**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/jltaylor-us/StardewRangeHighlight**

----


# Release Notes

## Version 1.4.0

### User-visible Changes

* Add Show*THING*Range configuration options to allow disabling
  highlighting of *THING* completely.
* Integration with the Prismatic Tools mod so that the range of
  the prismatic sprinkler matches the configured value in
  Prismatic Tools.
* Integration with the Better Junimos mod (if installed) to use
  its configured Junimo Hut range.
* Highlights now work during building placement.

### API Changes

* Change the function argument to `AddItemRangeHighlighter` to
  take the `Item` object and its ID as arguments in addition to
  the lower-cased item name.  (And by change I mean added a new
  signature and deprecated the old one.)
* Add a signature of `AddBuildingRangeHighlighter` that allows
  matching on a `BluePrint` during building placement.

-----

## Version 1.3.0

* Add the `hotkeysToggle` configuration property (off by default).
  If this property is changed to `true` then the hotkeys will act
  as a toggle switch rather than needing to be held down.
* Fix a NullReferenceException that was sometimes
  being printed to the console when moving between locations
  (only observed as a farm hand in multiplayer).

-----

## Version 1.2.0

* Bombs now show multiple areas of effect
  * The innermost area is where the bomb can cause dirt to turn
    into hoed ground.  The default configuration is to not show
    this area.
  * The "normal" range is the area where rocks and many items are
    destroyed.  This is the range shown in previous versions.
  * The outermost area is where crops are destroyed, flooring is
    disrupted, and the player takes damage.  (Note that this area
    is not centered on the bomb.)  The default configuration does
    show this range, but it can be disabled.
* Bombs that have been placed on the ground now show their ranges.
  This can be disabled in configuration.

_A note on the API:_

Unlike all of the other highlights, the bomb highlighting does not
use the public API.  Bombs are weird, and I didn't want to make
a messy API to support them without some other use case for it.

-----

## Version 1.1.1

* Change the default key bindings so that they do not conflict
  with the default movement keys.  (Keep in mind that if the
  `config.json` has already been generated with the old defaults
  then they won't be overwritten with the new defaults.)

-----

## Version 1.1.0

### User-visible Changes

* Bomb ranges are now highlighted when a bomb is equipped.
  (Ranges of bombs already on the ground are not shown.)

### API Changes

* Two new functions have been added to the API:
  * `GetCartesianCircleWithTruncate` truncates a tile's distance from
    the origin to an integer before comparing it against the radius.
    This is the same as the existing `GetCartesianCircle`.
  * `GetCartesianCircleWithRound` rounds a tile's distance from
    the origin to the nearest integer before comparing it against the radius.
    (This is the shape of a bomb's blast.)
* The `GetCartesianCircle` function is now deprecated.

-----

## Version 1.0.0

Initial release
