**Gift Taste Helper** is a [Stardew Valley](http://stardewvalley.net/) mod that shows a helpful
tooltip with a villager's favourite gifts when you point at them on the calendar or (thanks to
[dreamsicl](https://github.com/dreamsicl)) on the social page. It won't show the two universal
loved items.

## New "Progression" Mode

![Gift progression gif](images/progressive_gifts.gif)

In this mode you will only be shown the loved gifts that you have already given to an NPC. This mode can be enabled via the config (see below).

Note that you can freely turn this mode on/off and your progress will be kept. You can also switch between sharing known gifts between saves and having unique ones per save seamlessly too.

Also note that only the gifts you have given after downloading GiftTasteHelper 2.6 or later will be tracked.

---

![Calendar preview image](images/calendar_example.png?raw=true)

![Social page preview image](images/social_page_example.png?raw=true)

### Support for all locales!

![Locale support preview image](images/locale_support.png?raw=true)

Special thanks to the following for translation help:
* Sasara
* Missundomiel
* Google Translate

Also special thanks to PathosChild for help with fixes when I didn't have time and general help.

## Contents
* [Install](#install)
* [Configure](#configure)
* [Future features](#future-features)
* [See also](#see-also)

## Install
1. [Install the latest version of SMAPI](https://smapi.io/).
2. [Install this mod from Nexus mods](http://www.nexusmods.com/stardewvalley/mods/229/).
3. Run the game using SMAPI.


## Configure
The mod will work fine out of the box, but you can tweak its settings by editing the `config.json`
file in a text editor. (The file might not appear until you've run the game once with the mod.)

Available settings:

setting           | what it affects
:---------------- | :------------------
`ShowOnCalendar` | Default `true`. Whether the tooltip should be displayed on the calendar.
`ShowOnSocialPage` | Default `true`. Whether the tooltip should be displayed on the social page.
`ShowOnlyKnownGifts` | Default `false`. Only show the loved gifts that you have given to that NPC.
`ShareKnownGiftsWithAllSaves` | Default `true`. Should known loved gifts be shared between saves or unique per save. Ignored unless ShowOnlyKnownGifts is true.
`HideTooltipWhenNoGiftsKnown` | Default `false`. Hide the tooltip if you don't know any of that NPC's loved gifts. Ignored unless ShowOnlyKnownGifts is true.
`MaxGiftsToDisplay` | Default unlimited. The maximum number of gifts to list in the tooltip (or `0` for unlimited).
`ShowUniversalGifts` | Default `true`. Should the universally loved gifts be shown on the tooltip.
`ColorizeUniversalGiftNames` | Default `false`. Color universal gifts blue.
`ShowGiftsForUnmetNPCs` | Default `false`. Show gift info for NPCs you haven't met yet.

## Future features
* Display more than just loved gifts (if I can find a way to do it without cluttering the UI).

## See also
* [Nexus mod](http://www.nexusmods.com/stardewvalley/mods/229)
* [Discussion thread](http://community.playstarbound.com/threads/npc-gift-taste-helper.112180/)
