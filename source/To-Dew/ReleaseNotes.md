**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/jltaylor-us/StardewToDew**

----


# Release Notes

## Version 1.14.2

* Whether to add an app to the Mobile Phone mod (if installed) is now
  configurable.  (Configuration change requires a restart of the game
  before it will take effect.)
* The icon used with Mobile Phone can now be replaced via Content Patcher.
  See example content pack.

## Version 1.14.1

* Add Portugese translation provided by nexusmods user ericofurtado

## Version 1.14.0

* Repeating items can now specify a number of days to delay before the item
  is visible again

## Version 1.13.0

* Add an API to allow other mods to use the overlay
* increase minimum SMAPI version required to 3.17

## Version 1.12.0

* Allow clicking in overlay to mark an item as done (configurable)

## Version 1.11.0

* Add on option to toggle whether the overlay is scaled with UI Scale or Zoom Level
* Add config UI for the overlay Offset-X and Offset-Y config options
* Fix overlay on map areas smaller than the screen (e.g., bus stop)
* Fix overlay drawn over Qi coin total in the casino
* Fix GMCM deprecation warning

## Version 1.10.2

* Overlay background and text color can now be configured in-game if both
  Generic Mod Config Menu and GMCM Options are installed
* Work correctly when the same key is used for the hotkey and secondary close button
  (previously it would immediately close the just-opened menu)

## Version 1.10.1

* Updated Spanish translation (thanks to GitHub user Yllelder)
* Update to use new Generic Mod Config Menu API.  I18n now works in the config menu.
* Update build environment to use ModTranslationClassBuilder v2.0.0

## Version 1.10.0

* Built for SDV 1.5.5 / SMAPI 3.13

## Version 1.9.4

* Updated Chinese translation. (Thanks to Thomas Chen, github user cwt9562.)

## Version 1.9.3

* Scroll indicators in the main list now also function as buttons.

## Version 1.9.2

* Updated French translation provided by GitHub user Tenebrosful.

## Version 1.9.1

* Add Korean translation provided by nexusmods user primarina0220.

## Version 1.9.0

* Add import and export console commands

* Add `offsetX` and `offsetY` config properties for the overlay.

## Version 1.8.4

* Make weeks start on Monday (not Sunday) to match the game calendar.

## Version 1.8.3

* Fix typo causing visiblity by week to not work

## Version 1.8.2

* Hide the overlay during events

## Version 1.8.1

* Updates to Russian translation from nexusmods user Newrotd

## Version 1.8.0

* Now includes translations for several languages.  The translations
  are all via Google Translate, so they're probably not very good.

## Version 1.7.0

* Add `hotkeyList` and `overlay.hotkeyList` config options.  These do
  the same thing as `hotkey` and `overlay.hotkey`, but support the
  multi-key bindings added in SMAPI 3.9.  Increase required SMAPI
  version to 3.9.  I did not just update the existing config options
  because I think it would be rude to overwrite a multi-key binding
  when edited in the Generic Mod Config Menu (which doesn't support
  multi-key bindings).

* Add configuration of item visibility based on week.

* Add configuration option to hide the overlay while at festivals.

## Version 1.6.1

* Suppress overlay during in-game screenshots

## Version 1.6.0

* Add configuration of item visibility based on season.

* Make "move item" buttons bigger; move them to be side-by-side
  rather than stacked vertically.

* Attempt to fix some of the split-screen issues (untested).
  There should no longer be cross-talk between different players'
  copies of the list.  The overlay toggle should be independent.

## Version 1.5.0

* Major new functionality
  * Mark items done rather than deleting immediately
  * Per-item configuration
    * Repeating items
    * Control visibility based on weather
    * Control visibility based on day of week
    * Hide in overlay
    * Bold text
    * set as header (can't be marked done)
    * move item to top or bottom of list


* Fix overlay not moving down in some areas with a floor level


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
