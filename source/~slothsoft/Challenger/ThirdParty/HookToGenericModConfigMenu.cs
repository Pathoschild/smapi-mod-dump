/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-challenger
**
*************************************************/

namespace Slothsoft.Challenger.ThirdParty; 

internal static class HookToGenericModConfigMenu {
    
    public static void Apply(ChallengerMod challengerMod) {
        // get Generic Mod Config Menu's API (if it's installed)
        var configMenu = challengerMod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is null)
            return;

        // register mod
        configMenu.Register(
            mod: challengerMod.ModManifest,
            reset: () => challengerMod.Config = new ChallengerConfig(),
            save: () => challengerMod.Helper.WriteConfig(challengerMod.Config)
        );

        // add some config options
        configMenu.AddKeybind(
            mod: challengerMod.ModManifest,
            name: () => challengerMod.Helper.Translation.Get("ChallengerConfig.ButtonOpenMenu.Name"),
            tooltip: () => challengerMod.Helper.Translation.Get("ChallengerConfig.ButtonOpenMenu.Description"),
            getValue: () => challengerMod.Config.ButtonOpenMenu,
            setValue: value => challengerMod.Config.ButtonOpenMenu = value
        );
    }
}