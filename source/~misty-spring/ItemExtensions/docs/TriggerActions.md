**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

# Trigger actions

The mod adds some new trigger actions.

# Contents

* [Description](#description)
* Triggers
  * [On item held](#when-an-item-is-held)
  * [When item stops being held](#when-an-item-stops-being-held)
  * [On item dropped](#when-an-item-is-dropped)
  * [On equipping](#on-equipping-items)
  * [On UNequipping](#on-equipping-items)

--------------------

## Description

Trigger actions are a new modding feature added in 1.6: with them, you can make changes to the game after something happens.
To see how they work, read [the wiki page on them](https://stardewvalleywiki.com/Modding:Trigger_actions).

## When an item is held
`mistyspring.ItemExtensions_OnBeingHeld`

When your farmer begins holding any item (usually shown above head), this action will be called.

## When an item stops being held
`mistyspring.ItemExtensions_OnStopHolding`

When your farmer stops holding any item, this action will be called.

## When an item is dropped
`mistyspring.ItemExtensions_OnItemDropped`

This action is called when you drop an item out of your inventory.

## On equipping items
`mistyspring.ItemExtensions_OnEquip`
This action is called when you equip something on your farmer (be it a hat, ring, shoes, etc.)

## On unequipping items
`mistyspring.ItemExtensions_OnUnequip`
Similarly, this action is called when you take something off your farmer.

## Adding to an object's stack
`mistyspring.ItemExtensions_AddedToStack`

(Specialized) when you add to an existing object stack, e.g mix two hay stacks to have a single one.