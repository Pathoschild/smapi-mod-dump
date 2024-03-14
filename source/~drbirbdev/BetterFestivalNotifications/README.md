**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/drbirbdev/StardewValley**

----

# Better Festival Notifications

A mod for displaying notifications and playing warning sounds when festivals are beginning or ending.

## New in 1.6

Should now work with all custom festivals and passive festivals without needing any integration, so long as those festivals integrate with vanilla correctly.

## Available Configs

Sounds are based on [Game Sound](https://stardewvalleywiki.com/Modding:Audio#Sound) names.

Over notifications don't work for passive festivals, as they are hard-coded to end at 2 am.  If a custom passive festival ends earlier than that, this notification still won't trigger, and warn notifications may happen after the festival has ended.

* PlayStartSound - whether to play notification sound when festival starts.
* StartSound - the notification sound to play.
* WarnHoursAheadOfTime - how many in-game hours before the end of a festival should notifications be played.  2 is a pretty good number here.
* PlayWarnSound - whether to play notification sound when festival is close to ending.
* ShowWarnNotification - whether to display notification text when festival is close to ending. This is vanilla behavior on festival start.
* WarnSound - the notification sound to play.
* PlayOverSound - whether to play notification sound when festival has ended. Doesn't apply to passive festivals, since they are hard-coded to end at 2 am.
* ShowOverNotification - whether to display notification text when festival has ended.
* OverSound - the notification sound to play.

## Translation

Translation is automated by DeepL.  Feel free to PR suggested changes in your language.
