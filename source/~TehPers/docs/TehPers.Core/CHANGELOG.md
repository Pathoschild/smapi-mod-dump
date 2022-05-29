**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/TehPers/StardewValleyMods**

----

# Changelog

Changelog for [TehCore].

## 1.0.2 - 2022-05-20

### Added

- Add ModDrop update keys.
- Add special handling for creating Caroline's necklace.
- Add `Patcher` base class for services that apply Harmony patches.

### Changed

- Update to new SMAPI content API.
- Change `BindingExtensions.BindForeignModApi<TApi>` to give a nullable binding because the mod
  could not have an API.
- Descriptive JSON serialization now checks doc comments on members if no `[Description]` attribute
  is present. A `[Description("")]` attribute opts out of this behavior.

### Removed

- Remove asset tracking because SMAPI supports asset loading events now. This also cleans up the
  console output quite a bit.

### Fixed

- The descriptive JSON serialization now uses Harmony to patch Newtonsoft.Json. This fixes any
  issues with comments in nested objects.

## 1.0.1 - 2021-12-23

### Added

- Improved documentation for public API members.

### Removed

- Removed useless `DataStore` class from the API. Services are not expected to be stateless anyway.

[tehcore]: https://www.nexusmods.com/stardewvalley/mods/3256
