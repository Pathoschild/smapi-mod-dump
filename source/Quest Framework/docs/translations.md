**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/purrplingcat/QuestFramework**

----

# Translations

You can use i18n in your content packs for Quest Framework

## Translation tokens

Some text fields in content pack JSON file(s) support translation tokens `%i18n:<key>`. For this token you can use texts in `i18n` folder json files. For more info about translations see [Stardew Modding wiki](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Translation).

## Supported fields

I18n translation tokens are supported in all text fields (strings).

## Example

```js
// File: quests.json
{
  "Format": "1.0",
  "Quests": [
    {
      "Name": "abigail_amethyst", // No id needed, will be automatically generated
      "Type": "ItemDelivery", // Vanilla quest type
      "Title": "%i18n:abigail_amethyst.title",
      "Description": "Abigail is very hungry. She wants to eat something special from mines.",
      "Objective": "Bring amethyst to Abigail",
      "Reward": 150, // 150g
      "Cancelable": true,
      "Trigger": "Abigail 66", // Bring amethyst to Abby
      "ReactionText": "Oh, this looks so delicious. I am really hungry, thank you, @!$h"
    }
  ]
  "Offers:" [
    {
      "QuestName": "abigail_amethyst",
      "OfferedBy": "NPC",
      "OfferDetails": {
        "NpcName": "Abigail", // Speak with Abigail to get this quest
        "DialogueText": "%i18n:abigail_amethyst.offer.dialogueText"
      },
      "When": {
        "Seasons": "summer fall",
        "not:QuestAcceptedInPeriod": "season year"
      }
    },
  ]
}
```

```js
// File: i18n/default.json
{
  "abigail_amethyst.title": "The purple lunch",
  "abigail_amethyst.offer.dialogueText": "I have a craving for something special.#$b#@, can you bring me amethyst?"
}
```
