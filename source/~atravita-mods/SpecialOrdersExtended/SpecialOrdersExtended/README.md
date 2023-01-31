**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/atravita-mods/StardewMods**

----

Special Orders Extended
===============================

Adds additional tags that can be used to constrain the appearance of special orders on the Special Order board.

### Install

1. Install the latest version of [SMAPI](https://smapi.io).
2. Download and install [AtraCore](https://www.nexusmods.com/stardewvalley/mods/12932).
2. Download this mod and unzip it into `Stardew Valley/Mods`.
3. Run the game using SMAPI.

### Configuration
Run SMAPI at least once with this mod installed to generate the `config.json`, or use [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) to configure.

* `SurpressUnnecessaryBoardUpdates` prevents the board from trying to update if it's not unlocked yet. Disable this if it causes issues.
* `UseTagCache` will use a small internal cache to prevent recalculating tags often.
* `AvoidRepeatingQiOrders` will cause completed Qi orders to never appear on the board UNLESS you've finished all of them. Primarily put this one in because I got tired of waiting for Danger in the Deep to show up.

### For modders:

tl;dr Special orders are cached on Sunday night, making it hard to use CP tokens to control how Special Orders show up. I wanted to write a special order that would challenge you to gather a certain number of blackberries, and wrote this so that one special order would only show up in the one week in the year where it's actually, you know, possible.

A full list of tags available, both in vanilla and from this mod, can be found [here](docs/Tags.MD). If there are other tags you want, let me know by opening an issue!

Because the vanilla game is programmed to return `false` for any tag it doesn't understand, negated versions of each tag are also provided, letting you use `!tag` for any tag you want to default to `true`. This way, if a user does NOT install this mod and you're using a tag that is part of it, you can control whether or not the tag will default to true or false. For example: If you want a quest to show up on Y2 or later if this mod is installed, and to just show up at any time regardless, use `!year1`. If you want the quest to not show up at all if this mod is not installed, use `atleastyear_2`.

If you think it'd be nice if players had, say, experience killing serpents before embarking on your quest, but it's not vital, use something like `!haskilled_Serpent_under_50`. If this mod's installed, the special order will not appear before 50 serpents are killed. If this mod isn't, it'll show up regardless.

Adds a single event command: `atravita_addSpecialOrder <specialordername>`, which lets you add a special order in an event.

Additionally, adds two console commands to help debug special orders. `check_tag` will tell you the current value of the tag in question, and `special_order_pool` will list every special order and tell you if the special order can currently be selected and why if not. `list_available_stats` will also list all the stats available to the `stats` tag.

Also adds a way to override the duration of special orders. Edit the asset `Mods/atravita_SpecialOrdersExtended_DurationOverride`, which is a `string->int` dictionary corresponding to the special order quest keys to the duration. Use `-1` to refer to an infinite duration.

Finally, adds dialogue keys. Their documentation can be found [here](docs/DialogueKeys.MD).

[Changelog](docs/CHANGELOG.MD).

### Known issues:
1. I suspect (but am not absolutely sure) that if you complete two ship orders at the same time, only one will finish. This should only rarely happen in vanilla because there's only a few quests with ship requirements or have multi-week duration, but if you add a lot of multi-week ship orders you may run into this as well. This mod doesn't affect that part of the code though - it just patches CheckTag.
