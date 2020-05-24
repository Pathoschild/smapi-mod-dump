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

- **Fight over companion** This feature fixes the annoying request dialogue while you are fighting with a monster and companion is too near you. For enable set `FightThruCompanion` to `true` in experimental section in config file.
- **Use harmony patch for event checks** This feature fixes some problems with SVE with the Marlon's invitation event. If you play this mod with SVE, it's recommended to enable this feature despite it'S experimental. For enable set `UseCheckForEventsPatch` to `true` in experimental section in config file.

## See also

- [Getting started](getting-started.md)
- [Configuration](configuration.md)
