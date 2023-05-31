/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Core.ConfigMenu;

#region using directives

using DaLion.Shared.Integrations.GenericModConfigMenu;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration.</summary>
internal sealed partial class GenericModConfigMenu
{
    /// <summary>Register the config menu for SLNGS.</summary>
    private void AddSlingshotOptions()
    {
        this
            .AddPage(OverhaulModule.Slingshots.Namespace, I18n.Gmcm_Slngs_Heading)

            .AddSectionTitle(I18n.Gmcm_General_Heading)
            .AddCheckbox(
                I18n.Gmcm_Slngs_Enablerebalance_Title,
                I18n.Gmcm_Slngs_Enablerebalance_Desc,
                config => config.Slingshots.EnableRebalance,
                (config, value) => config.Slingshots.EnableRebalance = value)
            .AddCheckbox(
                I18n.Gmcm_Slngs_Enablecriticalhits_Title,
                I18n.Gmcm_Slngs_Enablecriticalhits_Desc,
                config => config.Slingshots.EnableCriticalHits,
                (config, value) => config.Slingshots.EnableCriticalHits = value)
            .AddCheckbox(
                I18n.Gmcm_Slngs_Enableenchantments_Title,
                I18n.Gmcm_Slngs_Enableenchantments_Desc,
                config => config.Slingshots.EnableEnchantments,
                (config, value) => config.Slingshots.EnableEnchantments = value)
            .AddCheckbox(
                I18n.Gmcm_Slngs_Enablespecialmove_Title,
                I18n.Gmcm_Slngs_Enablespecialmove_Desc,
                config => config.Slingshots.EnableSpecialMove,
                (config, value) => config.Slingshots.EnableSpecialMove = value)
            .AddCheckbox(
                I18n.Gmcm_Slngs_Disablegraceperiod_Title,
                I18n.Gmcm_Slngs_Disablegraceperiod_Desc,
                config => config.Slingshots.DisableGracePeriod,
                (config, value) => config.Slingshots.DisableGracePeriod = value)
            .AddCheckbox(
                I18n.Gmcm_Slngs_Bullseyereplacescursor_Title,
                I18n.Gmcm_Slngs_Bullseyereplacescursor_Desc,
                config => config.Slingshots.BullseyeReplacesCursor,
                (config, value) => config.Slingshots.BullseyeReplacesCursor = value)
            .AddCheckbox(
                I18n.Gmcm_Slngs_Enableinfinityslingshot_Title,
                I18n.Gmcm_Slngs_Enableinfinityslingshot_Desc,
                config => config.Slingshots.EnableInfinitySlingshot,
                (config, value) => config.Slingshots.EnableInfinitySlingshot = value)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Controls_Heading)
            .AddCheckbox(
                I18n.Gmcm_Controls_Facemousecursor_Title,
                I18n.Gmcm_Controls_Facemousecursor_Desc,
                config => config.Slingshots.FaceMouseCursor,
                (config, value) => config.Slingshots.FaceMouseCursor = value)
            .AddCheckbox(
                I18n.Gmcm_Controls_Enableautoselection_Title,
                () => I18n.Gmcm_Controls_Enableautoselection_Desc(I18n.Gmcm_Slngs_Slingshot().ToLowerInvariant()),
                config => config.Slingshots.EnableAutoSelection,
                (config, value) =>
                {
                    config.Slingshots.EnableAutoSelection = value;
                    if (!value)
                    {
                        SlingshotsModule.State.AutoSelectableSlingshot = null;
                    }
                })
            .AddKeyBinding(
                I18n.Gmcm_Controls_Selectionkey_Title,
                () => I18n.Gmcm_Controls_Selectionkey_Desc(I18n.Gmcm_Slngs_Slingshot()),
                config => config.Slingshots.SelectionKey,
                (config, value) => config.Slingshots.SelectionKey = value)
            .AddColorPicker(
                I18n.Gmcm_Controls_Selectionbordercolor_Title,
                () => I18n.Gmcm_Controls_Selectionbordercolor_Desc(I18n.Gmcm_Slngs_Slingshot().ToLowerInvariant()),
                config => config.Slingshots.SelectionBorderColor,
                (config, value) => config.Slingshots.SelectionBorderColor = value,
                Color.Magenta,
                colorPickerStyle: (uint)IGenericModConfigMenuOptionsApi.ColorPickerStyle.RGBSliders)
            .AddNumberField(
                I18n.Gmcm_Controls_Autoselectionrange_Title,
                () => I18n.Gmcm_Controls_Autoselectionrange_Desc(I18n.Gmcm_Slngs_Slingshot().ToLowerInvariant()),
                config => (int)config.Slingshots.AutoSelectionRange,
                (config, value) => config.Slingshots.AutoSelectionRange = (uint)value,
                1,
                9)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Movement_Heading)
            .AddCheckbox(
                I18n.Gmcm_Movement_Slickmoves_Title,
                I18n.Gmcm_Movement_Slickmoves_Desc,
                config => config.Slingshots.SlickMoves,
                (config, value) => config.Slingshots.SlickMoves = value);
    }
}
