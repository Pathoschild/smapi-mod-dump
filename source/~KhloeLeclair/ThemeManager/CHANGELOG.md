**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/KhloeLeclair/StardewMods**

----

# Changelog

## 0.7.0
Released on April 14th, 2024.

### New Features
* SpriteText colors can be changed. There is a new type of
  entry in game themes called `SpriteTextColorSets`, which
  let you override any specific color of SpriteText you would
  want to override. The global `*` color set applies to
  all calls, but it is possible to craft patches for methods
  to make them use alternative color sets.
* When defining a color, you can prefix the color with
  `premultiply:` and Theme Manager will premultiply the color's
  alpha, which can improve rendering in some situations. This
  can be used in combination with variables.

### Changes
* `SpriteTextColors` in game themes have been renamed to
  `IndexedSpriteTextColors`.
* The `tm_method_genpatch` method has slightly nicer output.
* Update the built-in patches for better coverage of the
  existing game classes.

### Fixed
* The managed asset loader would incorrectly match on some
  asset names and log a useless error.

### API Changes
* The C# API now functions! That's probably important.
* Removed direct references to BmFont types from the API, allowing
  mods that don't reference BmFont to use the API.


## 0.6.0
Released on April 13th, 2024.

It's been a while. This is the initial 1.6 release. There are a lot
of behind the scenes changes. The C# API is not ready, though, so
it doesn't matter, really. Hopefully we get that fixed soon.

Content pack authors can go nuts, though.


## 0.5.0
Released on November 18th, 2022.

* This is the initial public release.
