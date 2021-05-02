**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Pathoschild/StardewMods**

----

[← back to readme](README.md)

# Release notes
<!--


STOP RIGHT THERE.
When releasing a format change, don't forget to update the smapi.io/json schema!


-->
## Upcoming release
* Fixed patches not applied correctly in some cases when added by a conditional `Include` patch.

## 1.22
Released 17 April 2021. See the [release highlights](https://www.patreon.com/posts/50144071).

* Added a [conditions API](docs/conditions-api.md), which lets other mods parse and use Content Patcher conditions.
* Added new tokens:
  * [`FirstValidFile`](docs/author-tokens-guide.md#FirstValidFile) to enable fallback files without duplicating patches.
  * [`HasActiveQuest`](docs/author-tokens-guide.md#HasActiveQuest) to check a player's current quest list.
* Improved console commands:
  * Added `patch export` argument to optionally set the data type.
  * Tweaked console command handling.
  * Fixed `patch export` for an asset that's not already loaded causing the wrong data type to be cached.
* The latest `Format` version now always matches the main Content Patcher version. Previously it only changed if the format changed.
* Fixed default log names for patches with multiple `FromFile` or `Target` values.

## 1.21.2
Released 27 March 2021.

* Simplified 'unsupported format' message to avoid confusion when players need to update Content Patcher.
* When using [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098), you can now configure content packs in-game after loading the save.
* Fixed error when editing entries in `Data\RandomBundles`.
* Fixed misplaced warps when replacing some farm types.
* Fixed setting a map tile property to `null` not deleting it.
* Fixed compatibility with [unofficial 64-bit mode](https://stardewvalleywiki.com/Modding:Migrate_to_64-bit_on_Windows).

## 1.21.1
Released 07 March 2021.

* Fixed 'changes the save serializer' warning in 1.21.

## 1.21
Released 07 March 2021. See the [release highlights](https://www.patreon.com/posts/48471994).

* Added support for [creating custom locations](docs/author-guide.md#custom-locations).
* Added `AddWarps` field in [`EditMap` patches](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide.md#editmap).
* Added new tokens:
  * [`Render`](docs/author-tokens-guide.md#string-manipulation) to allow string comparison in `When` blocks.
  * `DailyLuck` to get a player's daily luck (thanks to Thom1729!).
* The `FarmhouseUpgrade` token can now check either the current player (default) or the host player.
* The `Enabled` field no longer allows tokens (in format version 1.21.0+).
* Improved default `LogName` for patches with multiple `Target` or `FromFile` values.
* Improved split-screen support.
* Fixed changes through [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) not correctly updating dynamic tokens and `Enabled` fields.
* Fixed `patch reload` command not reapplying format migrations to reloaded patches.
* Fixed error patching `Data\Concessions` using `EditData`.

**Update note for mod authors:**  
If you use tokens in the `Enabled` field, updating the `Format` field to `1.21.0` or later will
cause errors. See the [migration guide](docs/author-migration-guide.md) for more info.

## 1.20
Released 06 February 2021. See the [release highlights](https://www.patreon.com/posts/47213526).

* Improved tokens:
  * Added `LocationContext` (the world area recognized by the game like `Island` or `Valley`).
  * Added `LocationUniqueName` (the unique name for constructed building and cabin interiors).
  * `Weather` now returns weather for the current location context by default, instead of always returning the valley's weather.
  * You can now use an optional argument like `{{Weather: Valley}}` to get the weather for a specific context.
* You can now set translation token values through `i18n` token arguments.
* Added console commands:
  * `patch dump applied` shows all active patches grouped by target in their apply order, including whether each patch is applied.
  * `patch dump order` shows the global definition order for all loaded patches.
* Fixed patch order not guaranteed when `Include` patches get reloaded.
* Improved performance for content packs using tokenized conditions in patches updated on time change.
* Config fields consisting of a numeric range are now formatted as a slider in [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098).

**Update note for mod authors:**  
If you use the `Weather` token, updating the `Format` field to `1.20.0` or later changes its
behavior. See the [migration guide](docs/author-migration-guide.md) for more info.

## 1.19.4
Released 23 January 2021.

* Updated for multi-key bindings in SMAPI 3.9.

## 1.19.3
Released 10 January 2021.

* Fixed `Include` patches skipped if they have multiple `FromFile` values.
* Fixed `FarmType` token returning `Custom` for the beach farm; it now returns `Beach` instead.
* Fixed patches not applied for farmhands in some highly specific cases resulting in an _invalid input arguments_ error.

## 1.19.2
Released 04 January 2021.

* Improved `patch summary` command:
  * Added optional arguments to filter by content packs IDs.
  * Long token values are now truncated to 200 characters by default to improve readability. You can use `patch summary full` to see the full summary.

## 1.19.1
Released 21 December 2020.

* Updated for Stardew Valley 1.5, including...
  * split-screen mode and UI scaling;
  * added `KeyToTheTown` value to `HasWalletItem` token.
* Fixed patch not applied correctly if `FromFile` or `Target` contains a single value with a trailing comma.

## 1.19
Released 05 December 2020. See the [release highlights](https://www.patreon.com/posts/44708077).

* Added [query expressions](docs/author-tokens-guide.md#query-expressions).
* Added support for updating patches [on in-game time change](docs/author-guide.md#update-rate).
* Added support for patches with multiple `FromFile` values.
* Added map patch modes for `"Action": "EditMap"`.
* Added `Time` token.
* Custom mod tokens can now normalize raw values before comparison.
* Fixed `{{DayEvent}}` translating festival names when not playing in English.
* Fixed error when `FromFile` has tokens containing comma-delimited input arguments.

## 1.18.6
Released 21 November 2020.

* Fixed validation for `Include` patches in 1.18.5.

## 1.18.5
Released 21 November 2020.

* Improved error-handling for some content packs with invalid formats.
* Fixed `EditData` patches with multiple targets sometimes applied incorrectly to some targets.

## 1.18.4
Released 04 November 2020.

* Fixed tokens which use input arguments failing to update silently in rare cases.
* Fixed 'collection was modified' error in some cases when patching a data model asset.

## 1.18.3
Released 15 October 2020.

* Added support for setting the default value for an `i18n` token.
* Fixed `i18n` token not accepting named arguments.
* Fixed error-handling for invalid `Include` patches.
* Fixed errors using a dynamic token in some cases when it's set to the value of an immutable token like `{{HasMod |contains=X}}`.

## 1.18.2
Released 13 September 2020.

* `ConfigSchema` options can now have an optional `Description` field, which is shown in UIs like [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098).
* Fixed `TextOperations` incorrectly adding delimiters when there's no previous value.
* Fixed errors sometimes showing "ContentPatcher.Framework.Conditions.TokenString" instead of the intended value.
* Fixed error when using a field reference token as the only input to a token which requires input.

## 1.18.1
Released 13 September 2020.

* Fixed format issue when applying field edits to `Data\Achievements`.

## 1.18
Released 12 September 2020. See the [release highlights](https://www.patreon.com/posts/41527845).

* Added [content pack translation](docs/author-guide.md#translations) support using `i18n` files.
* Added [text operations](docs/author-guide.md#text-operations), which let you change a value instead of replacing it (e.g. append to a map's `Warp` property).
* You can now [configure content packs in-game](README.md#configure-content-packs) if you have [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) installed (thanks to a collaboration with spacechase0!). This works automatically for any content pack that has configuration options, no changes needed by mod authors.
* You can now edit fields via `EditData` for `Data\Achievements` too.
* Patches now update immediately when you change language.
* Fixed `EditData` patches not always updated if they use `FromFile` to load a file containing tokens.
* Fixed patches not always updated for a `Random` token reroll.
* Fixed error text when an `EditData` patch uses an invalid field index.
* Removed support for `FromFile` in `EditData` patches in newer format versions.

**Update note for mod authors:**  
If you use the `FromFile` field with `EditData` patches, updating the `Format` field to `1.18.0` or
later requires changes to your `content.json`. See the
[migration guide](docs/author-migration-guide.md) for more info.

## 1.17.2
Released 28 August 2020.

* Fixed patches not always updated if they depend on mod-provided tokens that incorrectly change outside a context update.

## 1.17.1
Released 19 August 2020.

* Made 'multiple patches want to load asset' errors more user-friendly.
* Fixed error in some cases when warping to a new location as a farmhand in multiplayer.
* Fixed error editing an image previously loaded through the Scale Up mod.

## 1.17
Released 16 August 2020. See the [release highlights for mod authors](https://www.patreon.com/posts/40495753).

* Patches can now optionally [update on location change](docs/author-guide.md#update-rate), including all tokens (not only location-specific tokens).
* Patches can now resize maps automatically using `Action: EditMap` (just patch past the bottom or right edges).
* Added `TargetPathOnly` token (the target field value for the current patch, without the filename).
* Added [`patch reload`](docs/author-guide.md#patch-reload) console command (thanks to spacechase0!).
* Added troubleshooting hints related to update rate in `patch summary` console command.
* Removed legacy token API obsolete since Content Patcher 1.12.
* Fixed ambiguous-method detection in advanced API.
* Internal changes to prepare for realtime content updates.

**Update note for mod authors:**  
If you use the `LocationName` or `IsOutdoors` token/condition, updating the `Format` field to
`1.17.0` or later requires changes to your `content.json`. See the
[migration guide](docs/author-migration-guide.md) for more info.

## 1.16.4
Released 12 August 2020.

* Fixed 'collection was modified' error when unloading `Action: Include` patches.

## 1.16.3
Released 08 August 2020.

* Fixed incorrect token input validation in some cases with 1.16.2.

## 1.16.2
Released 08 August 2020.

* Fixed patches not always unapplied when an `Include` patch changes.
* Fixed error using some tokens within the `contains` input argument.
* Fixed broken error message when multiple load patches apply in 1.16.

## 1.16.1
Released 03 August 2020.

* Fixed some patches not applied correctly in 1.16.

## 1.16
Released 02 August 2020. See the [release highlights for mod authors](https://www.patreon.com/posts/40028155).

* Added [an `Include` action](docs/author-guide.md#include) to load patches from another JSON file. That includes full token support, so you can load files dynamically or conditionally.
* Randomization is now consistent between players, regardless of installed content packs.
* Content packs containing `null` patches are no longer disabled; instead those patches are now skipped with a warning.
* Improved performance when updating very large content packs.
* Fixed boolean/numeric fields rejecting tokens with surrounding whitespace like `"  {{SomeToken}}  "`.
* Fixed auto-generated patch names not normalising path separators.
* Fixed `patch summary` showing duplicate target paths in some cases.
* Fixed string sorting/comparison for some special characters.
* Internal changes to prepare for realtime content updates.

**Update note for mod authors:**  
Using `"Action": "EditData"` with a `FromFile` field is now deprecated, though it still works.
Migrating to an `"Action": "Include"` patch is recommended; it's more flexible and works more
intuitively. (That doesn't apply to `"Action": "EditData"` patches without a `FromFile` field.)

## 1.15.2
Released 21 July 2020.

* Fixed error using `HasFile` with filenames containing commas.
* Fixed broken patches preventing other patches from being applied/updated in rare cases.
* Internal changes to prepare for 1.16.

## 1.15.1
Released 06 July 2020.

* Fixed error loading pre-1.15 content packs that use a token with empty input arguments like `{{token:}}`.

## 1.15
Released 04 July 2020. See the [release highlights for mod authors](https://www.patreon.com/posts/38962480).

* Added [named token arguments](docs/author-tokens-guide.md#global-input-arguments).
* Added a universal `|contains=` argument to search token values.
* Added a universal `|inputSeparator=` argument to allow commas in input values using a custom separator.
* Added a `key` argument to `{{Random}}`.
* Several [player tokens](docs/author-tokens-guide.md#player) now let you choose whether to check the host player, current player, or both.
* Added `HasConversationTopic` token.
* Reduced trace logs when a mod adds many custom tokens.
* Fixed custom tokens added by other mods able to break Content Patcher in some cases.
* Fixed support for tokens in a `From`/`ToArea`'s `Width` and `Height` fields.
* Fixed support for tokens in a `.json` file loaded through `Action: EditData` with a `FromFile` path containing tokens.
* Fixed format migrations not applied to tokens within JSON objects.
* Fixed multiple input arguments allowed for tokens that only recognize one (like `{{HasFile: fileA.png, fileB.png}}`). Doing so now shows an error.

**Update note for mod authors:**  
Updating the `Format` field to `1.15.0` or later requires changes to your `content.json`. See the [migration guide](docs/author-migration-guide.md) for more info.

## 1.14.1
Released 14 May 2020.

* Fixed patches not updating correctly in 1.14 when a changed token is only in their `FromFile` field.

## 1.14
Released 02 May 2020. See the [release highlights for mod authors](https://www.patreon.com/posts/whats-new-in-1-36931803).

* Added `Round` token.
* Added `FromFile` patch token (e.g. so you can do `"HasFile:{{FromFile}}": true`).
* The `patch export` command can now export assets that haven't been loaded yet.
* Fixed `Range` token excluding its upper bound.
* Fixed validation for `Target` fields containing `{{Target}}` and `{{TargetWithoutPath}}` tokens.
* Fixed validation for `Target` fields not shown in `patch summary` in some cases.
* Fixed 'file does not exist' error when the `FromFile` path is ready and doesn't exist, but the patch is disabled by a patch-specific condition.
* Fixed error when removing a map tile without edits.
* Fixed token handling in map tile/property fields.
* Fixed format validation for 1.13 features not applied.

## 1.13
Released 09 March 2020. See the [release highlights for mod authors](https://www.patreon.com/posts/whats-new-in-1-34749703).

* Added support for arithmetic expressions.
* Added support for editing map tiles.
* Added support for editing map tile properties.
* Added support for multi-key bindings (like `LeftShift + F3`).
* `EditMap` patches now also copy layers and layer properties from the source map (thanks to mouse!).
* Patches are now applied in the order listed more consistently.
* Improved logic for matching tilesheets when applying a map patch.
* Fixed incorrect warning when using `HasWalletItem` token in 1.12.

## 1.12
Released 01 February 2020. See the [release highlights for mod authors](https://www.patreon.com/posts/whats-new-in-1-33691875).

* Added advanced API to let other mods add more flexible tokens.
* Added support for mod-provided tokens in `EditData` fields.
* Reduced trace logs when another mod adds a custom token.
* The `patch export` command now exports the asset cached by the game, instead of trying to load it.
* Fixed dialogue and marriage dialogue changes not applied until the next day (via SMAPI 3.2).
* Fixed error when a data model patch uses an invalid token in its fields.
* Fixed whitespace between tokens being ignored (e.g. `{{season}} {{day}}` now outputs `Summer 14` instead of `Summer14`).

## 1.11.1
Released 27 December 2019.

* Mitigated `OutOfMemoryException` issue for some players. (The underlying issue in SMAPI is still being investigated.)
* Reduced performance impact in some cases when warping with content packs which have a large number of seasonal changes.
* Fixed patches being reapplied unnecessarily in some cases.
* Fixed token validation not applied to the entire token string in some cases.
* Fixed `Random` tokens being rerolled when warping if the patch is location-dependent.
* Fixed error when married to an NPC that's not loaded.

## 1.11
Released 15 December 2019. See the [release highlights for mod authors](https://www.patreon.com/posts/whats-new-in-1-1-32382030).

* Added `Lowercase` and `Uppercase` tokens.
* `Random` tokens can have 'pinned keys' to support many new scenarios (see readme).
* `Random` tokens are now bounded for immutable choices (e.g. you can use them in numeric fields if all their choices are numeric).
* `FromArea` and `ToArea` fields can now use tokens (thanks to spacechase0!).
* Optimized asset loading/editing a bit.
* Fixed warning when an `EditData` patch references a file that doesn't exist when that's checked with a `HasFile` condition.
* Fixed `HasFile` token being case-sensitive on Linux/Mac.
* Fixed error if a content pack has a null patch.

## 1.10.1
Released 02 December 2019.

* Updated for Stardew Valley 1.4.0.1.
* Fixed error when an `EditData` patch uses tokens in `FromFile` that aren't available.

## 1.10
Released 26 November 2019. See the [release highlights for mod authors](https://www.patreon.com/posts/whats-new-in-1-1-32382030).

* Updated for Stardew Valley 1.4, including new farm type.
* Added new tokens:
  * `HavingChild` and `Pregnant`: check if an NPC/player is having a child.
  * `HasDialogueAnswer`: the player's selected response IDs for question dialogues (thanks to mus-candidus!).
  * `IsJojaMartComplete`: whether the player bought a Joja membership and completed all Joja bundles.
  * `Random`: a random value from the given list.
  * `Range`: a list of integers between the specified min/max values.
* Added support for editing map properties with `EditMap` patches.
* Added support for using `FromFile` with `EditData` patches.
* Added `patch export` console command, which lets you see what an asset looks like with all changes applied.
* Added `patch parse` console command, which parses an arbitrary tokenizable string and shows the result.
* Added new 'current changes' list for each content pack to `patch summary` output.
* Added world state IDs to the `HasFlag` token.
* Added [`manifest.json` and `content.json` validator](docs/author-guide.md#schema-validator) for content pack authors.
* Content packs can now use mod-provided tokens without a dependency if the patch has an appropriate `HasMod` condition.
* Improved error if a content pack sets a `FromFile` path with invalid characters.
* Fixed `Hearts` and `Relationship` tokens not working for unmet NPCs. They now return `0` and `Unmet` respectively.
* Fixed issue where dynamic tokens weren't correctly updated in some cases if they depend on another dynamic token whose conditions changed. (Thanks to kfahy!)
* Fixed `patch summary` display for mod-provided tokens which require an unbounded input.
* Fixed `patch summary` not showing token input validation errors in some cases.
* Fixed `NullReferenceException` in some cases with invalid `Entries` keys.

## 1.9.2
Released 25 July 2019.

* Fixed `Day` token not allowing zero values.
* Fixed dynamic tokens validated before they're ready.
* Fixed mod-provided tokens called with non-ready inputs in some cases.
* Fixed Linux/Mac players getting `HasFile`-related errors in some cases.

## 1.9.1
Released 12 June 2019.

* Fixed error loading local XNB files in some cases with Content Patcher 1.9.
* Fixed mod-provided tokens being asked for values when they're marked non-ready.

## 1.9
Released 09 June 2019.

* Added API to let other mods create custom tokens and conditions.
* Fixed config parsing errors for some players.
* Fixed tokens not being validated consistently in some cases.
* Fixed a broken warning message.

## 1.8.2
Released 27 May 2019.

* Fixed some patches broken in Content Patcher 1.8.1.
* Fixed `EditMap` working with older format versions.

## 1.8.1
Released 26 May 2019.

* Improved `patch summary`:
  * now tracks the reason a patch wasn't loaded (instead of showing a heuristic guess);
  * added more info for local tokens;
  * simplified some output.
* Improved errors when a local file doesn't exist.
* Fixed patch update bugs in Content Patcher 1.8.

## 1.8
Released 16 May 2019.

* Added new tokens:
  * `IsOutdoors`: whether the player is outdoors.
  * `LocationName`: the name of the player's current location.
  * `Target`: the target field value for the current patch.
  * `TargetWithoutPath`: the target field value for the current patch (only the part after the last path separator).
* Added map patching.
* Added support for list assets in the upcoming Stardew Valley 1.4.
* Improved errors when token parsing fails.
* Fixed patches not applied in some cases.
* Fixed incorrect error message when `Default` and `AllowValues` conflict.
* Fixed confusing errors when a content pack is broken and using an old format version.

Thanks to spacechase0 for contributions to support the new tokens!

## 1.7
Released 08 May 2019.

* Added new tokens:
  * `HasReadLetter`: whether the player has opened a given mail letter.
  * `HasValue`: whether the input argument is non-blank, like `HasValue:{{spouse}}`.
  * `IsCommunityCenterComplete`: whether all bundles in the community center are completed.
  * `IsMainPlayer`: whether the player is the main player.
* Tokens can now be nested (like `Hearts:{{spouse}}`).
* Tokens can now be used almost everywhere (including dynamic token values, condition values, and `Fields` keys).
* Tokens with multiple values can now be used as placeholders.
* Tokens from `config.json` can now be unrestricted (`AllowValues` is now optional).
* Improved input argument validation.
* Added support for new asset structures in the upcoming Stardew Valley 1.4.
* Fixed incorrect error text when dynamic/config tokens conflict.
* Fixed config schema issues logged as `Debug` instead of `Warning`.
* Removed support for the condition value subkey syntax (like `"Relationship": "Abigail:Married"` instead of `"Relationship:Abigail": "Married"`). This only affects one content pack on Nexus.

**Update note for mod authors:**  
Updating the `Format` field to `1.7.0` or later requires changes to your `content.json`. See the [migration guide](docs/author-migration-guide.md) for more info.

## 1.6.5
Released 06 April 2019.

* Fixed `EditData` allowing field values containing `/` (which is the field delimiter).
* Fixed error with upcoming SMAPI 3.0 changes.
* Fixed some broken maps in Stardew Valley 1.3.36 not detected.
* Fixed typo in some errors.
* Internal rewriting to support upcoming features.

## 1.6.4
Released 05 March 2019.

* Added detection for most custom maps broken by Stardew Valley 1.3.36 (they'll now be rejected instead of crashing the game).

## 1.6.3
Released 15 January 2019.

* Fixed some conditions not available for multiplayer farmhands after 1.6.2.

## 1.6.2
Released 04 January 2019.

* Conditions are now checked much sooner when loading a save, so early setup like map debris spawning can be affected conditionally.
* Fixed token subkey form not allowed in boolean fields.
* Updated for changes in the upcoming SMAPI 3.0.

## 1.6.1
Released 08 December 2018.

* Fixed error when a content pack has a patch with no `Target` field.
* Fixed some conditions using subkeys marked invalid incorrectly.

## 1.6
Released 08 December 2018.

* Added new tokens:
  * `DaysPlayed`: the number of in-game days played for the current save.
  * `HasWalletItem`: the [special items in the player wallet](https://stardewvalleywiki.com/Wallet).
  * `SkillLevel`: the player's level for a given skill.
* Added `Wind` value for `Weather` token.
* Added support for matching subkey/value pairs like `"Relationship": "Abigail:Married, Marnie:Friend"`.
* Added support for conditional map edits (via SMAPI 2.9).
* Added support for editing `Data\NPCDispositions` after the NPC is already created (via SMAPI 2.9).
* Improved performance for most content packs.
* Improved `patch summary` format.
* Updated for the upcoming SMAPI 3.0.
* Fixed language token always marked 'not valid in this context'.
* Fixed token strings not validated for format version compatibility.
* Fixed some 1.5 tokens not validated for format version compatibility.

**Update note for mod authors:**  
Updating the `Format` field to `1.6.0` or later requires changes to your `content.json`. See the [migration guide](docs/author-migration-guide.md) for more info.

## 1.5.3
Released 08 November 2018.

* Added `patch summary` hint if `Target` value incorrectly includes a file extension.
* Migrated verbose logs to SMAPI's verbose logging feature.
* Fixed yet another error setting `EditData` entries to `null` since 1.5.

## 1.5.2
Released 29 September 2018.

* Improved `patch summary` output a bit.
* Fixed another error setting `EditData` entries to `null` since 1.5.

## 1.5.1
Released 23 September 2018.

* Added token support in `EditData` keys.
* Fixed error setting `EditData` entries to `null` since 1.5.
* Fixed error using tokens in `Enabled` field since 1.5.

## 1.5
Released 17 September 2018.
* Added support for dynamic tokens defined by the modder.
* Added new tokens:
  * `FarmCave` (the current farm cave type);
  * `FarmhouseUpgrade` (the upgrade level for the main farmhouse);
  * `FarmName` (the farm name);
  * `FarmType` (the farm type like `Standard` or `Wilderness`);
  * `HasFile` (whether a given file path exists in the content pack);
  * `HasProfession` (whether the player has a given profession);
  * `PlayerGender` (the player's gender);
  * `PlayerName` (the player's name);
  * `PreferredPet` (whether the player is a cat or dog person);
  * `Year` (the year number).
* Added subkey form for all tokens, which can be used to enable AND logic and condition negation (see readme).
* Added: you can now use any condition with `Action: Load` patches.
* Added: you can now use tokens in `EditData` entries and fields.
* Added: you can now list multiple values in the `Target` field.
* Added config tokens to `patch summary`.
* Added warning when a config field has `AllowValues` but a patch checks for an unlisted value.
* Removed some early warnings for issues like patch conflicts. That validation required a number of
  restrictions on how conditions and tokens could be used. Based on discussion with content pack
  modders, lifting those restrictions was more valuable than the early validation.
* Removed image preloading, which is no longer needed with SMAPI 2.8+.
* Fixed `patch summary` showing tokens that aren't valid in the current context.

## 1.4.1
Released 26 August 2018.

* Updated for Stardew Valley 1.3.29.
* Fixed broken error message.

## 1.4
Released 01 August 2018.

* Updated for Stardew Valley 1.3 (including multiplayer support).
* Added new tokens:
  * `DayEvent` (the festival name or wedding today);
  * `HasFlag` (the letters or flags set for the current player);
  * `HasMod` (the installed mods and content packs);
  * `HasSeenEvent` (the events seen by the current player);
  * `Hearts:<NPC>` (the relationship type for a given NPC);
  * `Relationship:<NPC>` (the relationship type for a given NPC);
  * `Spouse` (the player's spouse name);
* Added support for deleting entries via `EditData`.
* Added warnings for common mistakes in `patch summary` result.
* Fixed case sensitivity issues in some cases.

## 1.3.1
Released 08 April 2018.

* Added more detailed info to `patch summary` command.
* Improved error handling for image edits.
* Fixed unnecessary warnings when a patch is disabled.
* Fixed error when a content pack's `config.json` has invalid keys.

## 1.3
Released 26 March 2018.

* Added support for patch conditions (with initial support for season, day of month, day of week, and language).
* Added support for content packs having `config.json`.
* Added support for condition/config tokens in `content.json`.
* Added `patch summary` and `patch update` commands to simplify troubleshooting.
* Added trace logs when a content pack loads/edits an asset.
* Added optional verbose logs.
* Added unique patch names (editable via `LogName` field) to simplify troubleshooting.
* Improved error when a patch specifies an invalid source/target area.
* Fixed issue where an exception in one patch prevented other patches from being applied.
* Fixed `Target` not being normalized.
* Fixed errors using debug overlay on Linux/Mac.

## 1.2
Released 09 March 2018.

* Added support for overlaying images.
* Added optional debug mode for modders.
* `FromFile`, `Target`, and map tilesheets are now case-insensitive.
* Fixed null fields not being ignored after warning.

## 1.1
Released 02 March 2018.

* Added `Enabled` field to disable individual patches (thanks to f4iTh!).
* Added support for XNB files in `FromFile`.
* Added support for maps in `FromFile` which reference unpacked PNG tilesheets.

## 1.0
Released 25 February 2018.

* Initial release.
* Added support for replacing assets, editing images, and editing data files.
* Added support for extending spritesheets.
* Added support for locale-specific changes.
