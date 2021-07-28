**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://gitlab.com/kdau/predictivemods**

----

# Release notes

[Public Access TV](./)

## 2.0.0

*Released 2021 June 26.*

* Require, and fix compatibility with, Stardew 1.5 or higher
* Switch from PyTK to PlatoTK to work with all vanilla channels
* Remove `update_patv_channels` console command (no longer needed)
* Don't predict a train on the first day after a save is loaded
* Don't run the event at the railroad when a train is coming
* More natural format for dates thanks to Pathoschild
* Improvements to Korean translation by lando793

## 1.3.0

*Released 2020 April 23.*

* Put a real train in the intro event for the "Train Timetable" channel
* Show Claire as host of a certain channel when SVE is installed
* Fix rare error in introductory event for "In the Cans" channel
* Add support for Generic Mod Config Menu
* Add Korean translation by lando793
* Add Portuguese translation by Ertila007
* Add Simplified Chinese translation by caisijing

## 1.2.0

*Released 2020 April 8.*

* Support Android platform
* Fix issues with translated channel titles and days of the week
* Add French translation by Inu'tile
* Add Russian translation by Ghost3lboom

## 1.1.1

*Released 2020 March 23.*

* Really fix TV scenes not cleaning up this time

## 1.1.0

*Released 2020 March 23.*

* Fix TV scenes with special sounds sometimes not cleaning up after themselves
* Bump PyTK dependency to version that fixes second page of channel list
* Add `IncorrectPredictions` option (for entertainment purposes only)

## 1.0.1

*Released 2020 March 21.*

* Fix channels not running at all on some Windows versions
* Fix prediction about a certain game on the first visit to a certain place

## 1.0.0

*Released 2020 March 20.*

* Initial version
