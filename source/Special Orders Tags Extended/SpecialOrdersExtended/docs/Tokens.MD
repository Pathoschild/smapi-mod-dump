**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/atravita-mods/SpecialOrdersExtended**

----

Tokens
=======

New in 1.0.4

Tokens for Content Patcher have been added that reveal the current active quests, the current available quests, all completed quests, and the current active special orders rules.

| Token | Results |
| ----------- | ----------- |
| `{{atravita.SpecialOrdersExtended/Current}}` | Currently active orders. |
| `{{atravita.SpecialOrdersExtended/Available}}` | Available orders |
| `{{atravita.SpecialOrdersExtended/Completed}}` | Completed orders|
| `{{atravita.SpecialOrdersExtended/CurrentRules}}` | Currently active special order rules|
| `{{atravita.SpecialOrdersExtended/RecentCompleted}}` | Orders completed within the last seven days |

The usual Content Patcher inputs (`|contains=`, `|valueAt=`, etc) work, see [Content Patcher documentation](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-tokens-guide.md#global-input-arguments).

#### A note on custom dropboxes.

Custom dropboxes are quite easy to add with map edits. Just add `action Dropbox Dropboxname` to a tile. That said: it's fairly easy to accidentally overwrite someone else's custom dropbox, because they're just simple map edits. In order to avoid that, I use the tag `dropboxRoom` to prevent my orders from showing up at all if they might conflict with someone else's dropbox, and only patch in my dropbox when my order is active, using the `Current` token with an update rate of at least `OnLocationChange`.