**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/su226/StardewValleyMods**

----

# Field Ring

A powerful ring that keeps monsters away. (Even damage them!)

## Download

Sorry, currently you can only download source code and compile by yourself.

Requires [PyTK](https://www.nexusmods.com/stardewvalley/mods/1726).

## Usage

Currently you cannot get field ring normally, but you can use [CJB Item Spawner](https://www.nexusmods.com/stardewvalley/mods/93) or builtin command `fieldring_add`.

## Config

```jsonc
{
  "Index": 538603905, // Random item ID.
  "Range": 128, // Range of the field, 1 tile equals 64.
  "Damage": 10, // Damage to monsters.
  "PercentageDamage": true, // Makes damage relative to monster's health (percentage).
  "IgnoreResilience": true, // Ignore monster's resilience. (Deals true damage.)
  "IgnoreMissChance": true, // Ignore monster's miss chance.
  "IgnoreCooldown": false, // Ignore monster's hurt cooldown.
  "Knockback": 8.0 // Knockback power multiplier.
}
```

