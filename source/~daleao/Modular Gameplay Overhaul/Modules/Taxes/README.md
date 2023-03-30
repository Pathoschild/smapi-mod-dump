**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/daleao/sdv-mods**

----

<div align="center">

# Modular Overhaul :: Taxes

</div>

## Overview

This mod implements a "simple" (*citation needed*) taxation system to the game, because surely a nation at war would be on top of your juicy pile of gold.

### Income Tax

The Ferngill Revenue Service (FRS) will calculate your due federal obligations based on your Shipping Bin income. This adds a new gameplay element and a reason to prefer doing business locally, and also affords the player a choice to become a tax evader.

By default, the FRS defines 7 tax brackets for the individual taxpayer, modeled after the United States Of America, with 37% as the highest bracket for income tax. But these brackets and their respective thresholds are all configurable.

Federal obligations are due on the first of the month, end-of-day, and will be deducted automatically from the farmer's balance overnight (i.e., on the morning of day 2). This means that the farmer has one day to reinvest the closing season's profits into new seeds or livestock for the current season, before ~~being robbed~~ making their contribution to the Federal Government. Reinvestments (e.g., tool upgrades, animal or seed purchases, building comissions, etc.) may count as business expenses, which are deductible up to 100% under the Ferngill Revenue Code. However, if the farmer does not have enough funds remaining at the end of the day to cover their duties, the farmer will be fined and seized of all Shipping Bin income until the outstanding amount is settled. All debts will also accrue daily interest (configurable, default 11% **per annum**).

If the [Professions](../Professions) module is enabled and the player has the Conservationist profession, the professions' tax deduction perk will change from a % value increase to all items, to a more immersive % deduction of taxable income. Environmentalist activities can be used to deduct taxable income up to 100%. This means that farmers can be tax-exempt by collecting enough trash from oceans or rivers (it is recommended to decrease the default Professions setting for TrashNeededPerTaxBonusPct, as the TaxBonusCeiling setting will be overridden to 100%).

### (NEW) Property Tax

In addition to federal obligations, the farmer is also obliged to contribute Property Taxes to their local government.

The total value of the farmer's property will be appraised twice every season based on a Use-Value Assessment (UVA) program, which basically means that farmers are more liable for unproductive land; in other words, the taxation rate applied to land which is actively used for agriculture, livestock or forestry is generally **less** than that applied to land which is not actively used.

During the assessment, the total value of the farm's agriculture activities, livestock and real-estate all will be weighted. At the start of each year, Mayor Lewis will collect due property taxes from the **host** farmer, based on the average UVA value of the entire property throughout the previous year, except Winter, when the agriculture component is ignored.

Farming activities in Ginger Island and other properties will not be charged.

Property taxes are not eligible for deductions.

Lateness fines are generally higher for property taxes (configurable). But since all debts are purchased by the same bank (canonically the Bank of Stardew), interest rates are always the same.

## Compatibility

Should be compatible with anything (including [Ferngill Revenue Service](https://www.nexusmods.com/stardewvalley/mods/7566, but please don't try using both together).
