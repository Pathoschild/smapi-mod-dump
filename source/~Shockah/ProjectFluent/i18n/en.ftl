mod-name = { MOD_NAME() }
content-patcher-mod-name = { MOD_NAME("Pathoschild.ContentPatcher") }

config-contentPatcherPatchingMode = { content-patcher-mod-name } patching
    .tooltip =
        Controls how and if { content-patcher-mod-name } gets patched to support { mod-name }.
        > { config-contentPatcherPatchingMode.Disabled }:
        {"   "}Content Patcher will not be patched.
        > { config-contentPatcherPatchingMode.PatchFluentToken }:
        {"   "}Content Patcher will be patched to directly allow the usage of the {"{{"}Fluent{"}}"} token.
        > { config-contentPatcherPatchingMode.PatchAllTokens }:
        {"   "}Content Patcher will be patched to directly allow the usage of any tokens registered for each mod.

        Technical details:
        By default (as of { content-patcher-mod-name } version 2.x), { content-patcher-mod-name } mods
        have to spell out the whole name of the Fluent localization token, including their ID.
        Enabling the "Patch all tokens" is discouraged, but if you are working on your own C# mod
        which adds tokens for { content-patcher-mod-name } mods, it will allow those to also be used directly.
    .Disabled = Disabled
    .PatchFluentToken = Only patch {"{{"}Fluent{"}}"}
    .PatchAllTokens = Patch all tokens

config-localeOverride = Locale override
    .tooltip =
        Allows you to override the current locale used by { mod-name }.
        Enter either a built-in locale (listed below), or a different (mod) locale.
        Leave empty to use the game locale.
        
config-localeOverrideSubtitle =
    Built-in values:
    { $Values }

config-developerMode = Developer mode
    .tooltip =
        If enabled:
        {"*"} You will be warned if any mods are accessing i18n translations before { mod-name } is ready
           (those mods may not be fully translatable).
        {"*"} You will be warned if a localized mod is missing, even if the developer disabled this warning.

        The game has to be restarted before those features take effect.