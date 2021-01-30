**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/jltaylor-us/StardewToDew**

----


# Release Notes

## Version 1.4.3

* Fix a couple of bugs that could result in a System.NullReferenceException
  in ToDew.ModEntry.OnButtonPressed.

## Version 1.4.2

* Fix crash with `MissingMethodException` for a `SpriteBatch.Draw` signature
  in some environments.

## Version 1.4.1

* Fix accidental deletion of items when clicking above or below the visible
  scrolling area.

## Version 1.4.0

* Items can now be reordered.

* Right-clicking on an item will copy its text to the textbox (replacing
  whatever is currently there).

* When the overlay is enabled, its visibility can be toggled with a
  hotkey, if so configured.  The default configuration does not have
  a hotkey for the overlay.

* Fixed a bug with the Generic Mod Config integration that would result
  in the overlay portion of the configuration getting "de-synced" after
  resetting to defaults (until game restart).

## Version 1.3.1

* Move overlay below the mine level indicator when in mines.

## Version 1.3.0

* Add an overlay that displays the list all the time (when enabled
  in the configuration).

* Fix rendering of long items.  (More specifically, fix the rendering
  of the subsequent items so they don't overlap the additional line(s)
  of the long item.)  As part of this, change the highlight style from
  being a different background color to being a rectangular border instead.

### Known Issue

* The overlay will be partially (or fully) hidden by the black bars drawn
  on the left and right sides of the screen on maps that are narrower than
  the screen (which depends on the screen, but most likely e.g. is the bus
  stop).

## Version 1.2.0

* Tighten up the spacing between items in the list

* Add the "secondary close button" configuration option, check
  for controller inputs as well as keyboard inputs to tell when
  to close the list

## Version 1.1.0

* If the Mobile Phone mod is installed, add a To-Dew "app" to it.
  (This is really just an icon that brings up the same interface
  as the hotkey.)


## Version 1.0.0

Initial release
