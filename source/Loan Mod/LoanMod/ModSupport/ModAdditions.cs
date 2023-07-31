/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/legovader09/SDVLoanMod
**
*************************************************/

using GenericModConfigMenu;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System.IO;
using LoanMod.Common;
using LoanMod.Common.Constants;

namespace LoanMod
{
    public partial class ModEntry
    {
        private IMobilePhoneApi _mobileApi;
        private void AddModFunctions()
        {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>(ModConstants.ModConfigMenu);
            if (configMenu != null)
            {
                // register mod
                configMenu.Register(
                    mod: ModManifest,
                    reset: () => _config = new ModConfig(),
                    save: () => Helper.WriteConfig(_config)
                );

                MainSection(configMenu);
                DurationSection(configMenu);
                InterestSection(configMenu);
                LegacyMoneySection(configMenu);
                AdvancedSelection(configMenu);
            }

            _mobileApi = Helper.ModRegistry.GetApi<IMobilePhoneApi>(ModConstants.MobilePhoneMod);

            if (_mobileApi == null || !_config.AddMobileApp) return;

            var appIcon = Helper.ModContent.Load<Texture2D>(Path.Combine(ModConstants.AssetFolder, ModConstants.AppIcon));
            var success = _mobileApi.AddApp(Helper.ModRegistry.ModID, I18n.App_Name(), () => StartBorrow(1, ModConstants.BorrowAmountKey), appIcon);
            Monitor.Log($"Loaded phone app successfully: {success}", LogLevel.Debug);
        }

        private void AdvancedSelection(IGenericModConfigMenuApi configMenu)
        {
            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => "Advanced Options"
            );

            configMenu.AddParagraph(
                mod: ModManifest,
                text: () => "Only modify these values if you need to."
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Use Input Box vs Dialog Boxes",
                tooltip: () => "Uses input box to enter a custom amount of money to borrow.",
                getValue: () => _config.CustomMoneyInput,
                setValue: value => _config.CustomMoneyInput = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Reset Loan Profile",
                tooltip: () => "Resets the loan profile on the next save file you load. NOTE: This removes your current loan, but does not deduct the loaned money from you.",
                getValue: () => _config.Reset,
                setValue: value => _config.Reset = value
            );
        }

        private void MainSection(IGenericModConfigMenuApi configMenu)
        {
            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => "Main Options"
            );

            configMenu.AddKeybind(
                mod: ModManifest,
                getValue: () => _config.LoanButton,
                setValue: value => _config.LoanButton = value,
                name: () => "Change Keybind",
                tooltip: () => "Change the button to press to open the loan menu."
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => _config.LatePaymentChargeRate,
                setValue: value => _config.LatePaymentChargeRate = value,
                name: () => "Late Payment Interest Rate",
                min: 0f,
                interval: 100f
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => _config.MaxBorrowAmount,
                setValue: value => _config.MaxBorrowAmount = (int)value,
                name: () => "Maximum Borrow Amount",
                min: 100f,
                interval: 100f
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => _config.AddMobileApp,
                setValue: value => _config.AddMobileApp = value,
                name: () => "Add Mobile App"
            );
        }
        
        private void LegacyMoneySection(IGenericModConfigMenuApi configMenu)
        {
            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => "Money Amount Dialog"
            );

            configMenu.AddParagraph(
                mod: ModManifest,
                text: () => "Only modify these values if you DO NOT have \"Use Input Box\" checked in the advanced section."
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => _config.MoneyAmount1,
                setValue: value => _config.MoneyAmount1 = (int)value,
                name: () => "Money Amount 1",
                min: 0f,
                interval: 100f
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => _config.MoneyAmount2,
                setValue: value => _config.MoneyAmount2 = (int)value,
                name: () => "Money Amount 2",
                min: 0f,
                interval: 100f
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => _config.MoneyAmount3,
                setValue: value => _config.MoneyAmount3 = (int)value,
                name: () => "Money Amount 3",
                min: 0f,
                interval: 100f
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => _config.MoneyAmount4,
                setValue: value => _config.MoneyAmount4 = (int)value,
                name: () => "Money Amount 4",
                min: 0f,
                interval: 100f
            );
        }
        
        private void DurationSection(IGenericModConfigMenuApi configMenu)
        {
            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => "Loan Duration Menu Options"
            );

            configMenu.AddParagraph(
                mod: ModManifest,
                text: () => "Value in days that define the duration of the loan."
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => _config.DayLength1,
                setValue: value => _config.DayLength1 = value,
                name: () => "Duration Option 1"
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => _config.DayLength2,
                setValue: value => _config.DayLength2 = value,
                name: () => "Duration Option 2"
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => _config.DayLength3,
                setValue: value => _config.DayLength3 = value,
                name: () => "Duration Option 3"
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => _config.DayLength4,
                setValue: value => _config.DayLength4 = value,
                name: () => "Duration Option 4"
            );
        }
        
        private void InterestSection(IGenericModConfigMenuApi configMenu)
        {
            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => "Interest Menu Options"
            );

            configMenu.AddParagraph(
                mod: ModManifest,
                text: () => "Each value corresponds to the Loan Duration menu above."
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => _config.InterestModifier1,
                setValue: value => _config.InterestModifier1 = value,
                name: () => "Interest Modifier 1"
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => _config.InterestModifier2,
                setValue: value => _config.InterestModifier2 = value,
                name: () => "Interest Modifier 2"
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => _config.InterestModifier3,
                setValue: value => _config.InterestModifier3 = value,
                name: () => "Interest Modifier 3"
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => _config.InterestModifier4,
                setValue: value => _config.InterestModifier4 = value,
                name: () => "Interest Modifier 4"
            );
        }
    }
}
