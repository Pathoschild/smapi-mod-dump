**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/TehPers/StardewValleyMods**

----

# TehCore

For **SDV** 1.5.5+ and **SMAPI** 3.13.0+

A core mod with the goal of enabling high levels of compatibility between mods.

Features:

- Expose complex mod APIs. TehCore adds dependency injection for mods through [Ninject]. Need to
  expose an `IComplexApi` interface that SMAPI's mod registry doesn't support? Expose it through
  your mod's service container and anyone can request it easily and even have it automatically
  injected into their classes if they want.
- Support items of any type with namespaced keys! By using the namespace registry, mods can create
  items of any type (objects, weapons, tools, etc.) and even create items added through Json Assets
  or Dynamic Game Assets through string-based keys.
  - Use namespaced keys in your configs! You can let users create content packs for your mods using
    any item exposed through an item namespace by using namespaced keys.
  - Mods can also register their own namespaces. Add support for your custom items to **every**
    TehCore-based mod by registering a custom namespace for them. You can even make TehCore an
    optional dependency while doing this!
- Create commented JSON files. Enhance your configs by adding descriptions to each of the settings.

## API

The API docs are still a work in progress. Check back soon for updated docs.

## Source code & License

This mod is licensed under MIT License. The source code and full license text can be found on the [GitHub repository][github repo].

[ninject]: https://github.com/ninject/ninject/wiki
[github repo]: https://github.com/TehPers/StardewValleyMods/tree/full-rewrite
