**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/KediDili/Creaturebook**

----

Hello there! I believe you're here to see either how to do translations for a content packs, or Creaturebook itself. If you aren't familiar with i18n files, see this wiki page first: https://stardewvalleywiki.com/Modding:Translations

## Translations for a content pack
A content pack shouldn't have a variety of things in their i18n files, only their creatures' translatable display names and descriptions/fun facts. Which are formatted like below:

```
{
  "<Chapter's CreatureNamePrefix>.<Creature's ID>_name": "Creature name",
  "<Chapter's CreatureNamePrefix>.<Creature's ID>_desc": "Creature description/fun fact",
}
```

If we would like to have a live example...


```
{
  "Flutter.32_name": "Anomalous Bluetail",
  "Flutter.32_desc": "Fact about this blue, yellow and brown beautiful moth",
}
```

## Translations for Creaturebook itself
Opposite to a content pack's, Creaturebook has more than just names and descriptions in translation files.
See the file here:

Translate it to any language, then either reach to me from Discord (`KediDili#4678`) or send a PR here on GitHub. Any translators will be credited in Nexus/ModDrop page, patch notes and this readme file.
