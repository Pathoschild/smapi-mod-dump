**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/purrplingcat/PurrplingMod**

----

# Experimental features

From version 0.12.0 the mod brings experimental features. This features are disabled by default and player can enable it in [config.json](configuration.md). See the `Experimental` section in config file.

## Declaration

If we are not sure about compatibility and stability and we need a test it in production, we mark these features as experimental and disable them by default. If player wants to test it or play with them, they can enable it in configuration. Please remember these experimental features are REALLY EXPERIMENTAL AND USE IT ON YOUR OWN RISK if you enable them.

### Make experimental features as stable

We make experimental as stable feature and enable it by default when:

- It was tested by users
- No critical issues not caused
- Found bugs with this feature was fixed

Some features which was experimental you can still disable in configuration file if you don't need them. This rule is not solid rule and not include all ex-experimental features as optional. It's features which solves any problem in gameplay. Some features can be in next versions disabled by user, but one day the option for disable it may be removed.

## Current experimental features

- **Swimsuits** This feature allows your companions to change to swimsuit if you enter to bathroom with them. This feature is experimental, because swimsuits are incomplete and still WIP. Some companions hasn't own swimsuits and enter the pool in their daily clothes. List of NPCs with own swimsuit: *Abigail, Alex, Haley, Emily, Sam*. For enable set `UseSwimsuits` to `true` in experimental section in config file.

## See also

- [Getting started](getting-started.md)
- [Configuration](configuration.md)
