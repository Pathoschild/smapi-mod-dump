**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/atravita-mods/SpecialOrdersExtended**

----

# SpecialOrdersExtended

Adds additional tags that can be used to constrain the appearance of special orders on the Special Order board.

#### For users:

Just download the .zip and unzip into your Mods file. That's it!

#### For modders:

tl;dr Special orders are cached on Sunday night, making it hard to use CP tokens to control how Special Orders show up. I wanted to write a special order that would challenge you to gather a certain number of blackberries, and wrote this so that one special order would only show up in the one week in the year where it's actually, you know, possible.

A full list of tags available, both in vanilla and from this mod, can be found here: https://github.com/atravita-mods/SpecialOrdersExtended/blob/master/SpecialOrdersExtended/docs/Tags.MD. If there are other tags you want, let me know in the Issues tab!

Because the vanilla game is programmed to return "false" for any tag it doesn't understand, negated versions of each tag are also provided, letting you use `!tag` for any tag you want to default to "true". This way, if a user does NOT install this mod and you're using a tag that is part of it, you can control whether or not the tag will default to true or false. For example: If you want a quest to show up on Y2 or later if this mod is installed, and to just show up at any time regardless, use `!year1`. If you want the quest to not show up at all if this mod is not installed, use `atleastyear_2`.

If you think it'd be nice if players had, say, experience killing serpents before embarking on your quest, but it's not vital, use something like `!haskilled_Serpent_under_50`. If this mod's installed, the special order will not appear before 50 serpents are killed. If this mod isn't, it'll show up regardless.

Additionally, adds two console commands to help debug special orders. `check_tag` will tell you the current value of the tag in question, and `special_order_pool` will list every special order and tell you if the special order can currently be selected and why if not. `list_available_stats` will also list all the stats available to the `stats` tag.

Finally, adds dialogue keys. Their documentation can be found here: https://github.com/atravita-mods/SpecialOrdersExtended/blob/master/SpecialOrdersExtended/docs/DialogueKeys.MD.

**Technical:** Uses Harmony to patch `SpecialOrder.CheckTag` and `NPC:checkForNewCurrentDialogue`. Requires SMAPI.

Changelog: https://github.com/atravita-mods/SpecialOrdersExtended/blob/master/SpecialOrdersExtended/docs/CHANGELOG.MD

#### Known issues:
1. I suspect (but am not absolutely sure) that if you complete two ship orders at the same time, only one will finish. This should only rarely happen in vanilla because there's only a few quests with ship requirements or have multi-week duration, but if you add a lot of multi-week ship orders you may run into this as well. This mod doesn't affect that part of the code though - it just patches CheckTag.
