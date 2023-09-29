/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Core.ConfigMenu;

#region using directives

using System.Linq;
using DaLion.Shared.Integrations.GenericModConfigMenu;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration.</summary>
internal sealed partial class GenericModConfigMenu : GenericModConfigMenuIntegration<GenericModConfigMenu, ModConfig>
{
    private static bool _reload;

    /// <summary>Initializes a new instance of the <see cref="GenericModConfigMenu"/> class.</summary>
    internal GenericModConfigMenu()
        : base(ModHelper.ModRegistry, Manifest)
    {
    }

    /// <inheritdoc />
    protected override void BuildMenu()
    {
        this.SetTitleScreenOnlyForNextOptions(true);
        if (!LocalData.InitialSetupComplete)
        {
            this.AddParagraph(I18n.Gmcm_Core_Initial);
        }
        else
        {
            this.AddParagraph(I18n.Gmcm_Core_Choose);
        }

        this.AddModuleSelectionOption();
        if (!LocalData.InitialSetupComplete)
        {
            return;
        }

        this
            .AddHorizontalRule()
            .SetTitleScreenOnlyForNextOptions(false)
            .AddMultiPageLinkOption(
                getOptionName: I18n.Gmcm_Core_Modules,
                pages: EnumerateModules().Skip(1).Where(m => m._ShouldEnable).ToArray(),
                getPageId: module => module.Namespace,
                getPageName: module => module.Name,
                getColumnsFromWidth: _ => 2);

        // add page contents
        if (Config.EnableProfessions)
        {
            this.AddProfessionOptions();
        }

        if (Config.EnableCombat)
        {
            this.AddCombatOptions();
        }

        if (Config.EnableTools)
        {
            this.AddToolOptions();
        }

        if (Config.EnablePonds)
        {
            this.AddPondOptions();
        }

        if (Config.EnableTaxes)
        {
            this.AddTaxOptions();
        }

        if (Config.EnableTweex)
        {
            this.AddMiscOptions();
        }

        this.OnFieldChanged((_, _) => { _reload = true; });
    }

    /// <inheritdoc />
    protected override ModConfig GetConfig()
    {
        return Config;
    }

    /// <inheritdoc />
    protected override void ResetConfig()
    {
        Config = new ModConfig();
    }

    /// <inheritdoc />
    protected override void SaveAndApply()
    {
        ModHelper.WriteConfig(Config);
        Config.Log();
        if (!_reload)
        {
            return;
        }

        this.Reload();
        _reload = false;
    }

    /// <summary>Adds a new instance of <see cref="ModuleSelectionOption"/> to this mod menu.</summary>
    private GenericModConfigMenu AddModuleSelectionOption()
    {
        this.AssertRegistered();
        new ModuleSelectionOption(this.Reload).AddToMenu(this.ModApi, this.ConsumerManifest);
        return this;
    }
}
