**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/daleao/sdv-mods**

----

# Taxes Module Change Log

## 1.0.2

### Added

* In addition to interest, outstanding debt now suffers additional penalties at each season close. The penalty amount is the lesser of 100g or 5% of the outstanding amount. 

### Changed

* The default value for annual interest has been increased from 6% to 11% (the base corporate tax interest rate in the state of New York).

### Fixed

* Fixed the taxable amount displayed in the logs (should have no effect on gameplay).
* Taxes now apply to items sold via Mini-Shipping Bin.

## 0.9.7

### Added

* Added season name to console outputs.
* Added SetExpenses command.

### Fixed

* Business expenses will now correctly reset at the close of the season.
* Prevent business expenses from being greater than due taxes.

## 0.9.0 (Initial release)

### Added

* Added the option to deduct business expenses from taxable income. Business expenses can include building constructions, tool-related costs, animal-related costs and seed purchases. You may also specify individual objects by name that should be tax-deductible.
* Added new console commands for directly setting season income and outstanding debt.

### Changed

* Tax brackets now work progressively, as in real life; i.e., instead of applying only the highest applicable bracket, income is actually split into each applicable bracket. The result is less taxes paid overall.

### Fixed

* Fixed an issue where debited amount would not actually reduce the player's money when they didn't have any before sleeping. The debit now happens when the day starts, *after* the previous day's earnings have been added to the player's balance.
* Fixed a typo in introduction letter (english).
* Fixed a translation objects not being converted to strings.