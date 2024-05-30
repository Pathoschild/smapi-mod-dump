/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/rtrox/LineSprinklersRedux
**
*************************************************/

using GenericModConfigMenu;
using StardewModdingAPI;

namespace LineSprinklersRedux.Framework
{
    internal class GenericModConfigMenuForLineSprinklers
    {
        private readonly Func<ModConfig> GetConfig;

        private readonly Action Reset;

        private readonly Action SaveAndApply;

        private readonly IManifest ModManifest;

        private readonly IModRegistry ModRegistry;

        private readonly ITranslationHelper I18n;
        public GenericModConfigMenuForLineSprinklers(IModRegistry modRegistry, IManifest modManifest, ITranslationHelper i18n, Func<ModConfig> getConfig, Action reset, Action saveAndApply)
        {
            this.ModRegistry = modRegistry;
            this.I18n = i18n;
            this.ModManifest = modManifest;
            this.GetConfig = getConfig;
            this.Reset = reset;
            this.SaveAndApply = saveAndApply;

        }

        public void Register()
        {
            var configMenu = this.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu == null)
                return;

            configMenu.Register(
                mod: this.ModManifest,
                reset: this.Reset,
                save: this.SaveAndApply
            );

            configMenu.AddSectionTitle(this.ModManifest, () => this.I18n.Get("Controls"));
            configMenu.AddKeybindList(
                this.ModManifest,
                name: () => this.I18n.Get("RotateSprinklerKeybind"),
                tooltip: () => this.I18n.Get("RotateSprinklerKeybind.Tooltip"),
                getValue: () => this.GetConfig().RotateSprinklerKeybindList,
                setValue: (value) => this.GetConfig().RotateSprinklerKeybindList = value
            );
        }
    }
}
