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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanMod
{
    public partial class ModEntry
    {
        private IMobilePhoneApi mobileApi;
        private void AddModFunctions()
        {
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu != null)
            {
                // register mod
                configMenu.Register(
                    mod: this.ModManifest,
                    reset: () => this.Config = new ModConfig(),
                    save: () => this.Helper.WriteConfig(this.Config)
                );

                MainSection(configMenu);
                DurationSection(configMenu);
                InterestSection(configMenu);
                LegacyMoneySection(configMenu);
                AdvancedSelection(configMenu);
            }

            mobileApi = this.Helper.ModRegistry.GetApi<IMobilePhoneApi>("aedenthorn.MobilePhone");
            if (mobileApi != null)
            {
                Texture2D appIcon = Helper.ModContent.Load<Texture2D>(Path.Combine("assets", "app_icon.png"));
                bool success = mobileApi.AddApp(Helper.ModRegistry.ModID, "Loans", () => StartBorrow(1, "Key_Amount"), appIcon);
                Monitor.Log($"loaded phone app successfully: {success}", LogLevel.Debug);
            }
        }

        private void AdvancedSelection(IGenericModConfigMenuApi configMenu)
        {
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Advanced Options"
            );

            configMenu.AddParagraph(
                mod: this.ModManifest,
                text: () => "Only modify these values if you absolutely have to."
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Use Input Box vs Dialog Boxes",
                tooltip: () => "Uses input box to enter a custom amount of money to borrow.",
                getValue: () => this.Config.CustomMoneyInput,
                setValue: value => this.Config.CustomMoneyInput = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Reset Loan Profile",
                tooltip: () => "Resets the loan profile on the next save file you load.",
                getValue: () => this.Config.Reset,
                setValue: value => this.Config.Reset = value
            );
        }

        private void MainSection(IGenericModConfigMenuApi configMenu)
        {
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Main Options"
            );

            configMenu.AddKeybind(
                mod: this.ModManifest,
                getValue: () => this.Config.LoanButton,
                setValue: value => this.Config.LoanButton = value,
                name: () => "Change Keybind",
                tooltip: () => "Change the button to press to open the loan menu."
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                getValue: () => this.Config.LatePaymentChargeRate,
                setValue: value => this.Config.LatePaymentChargeRate = (float)value,
                name: () => "Late Payment Interest Rate",
                min: 0f,
                interval: 100f
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                getValue: () => this.Config.MaxBorrowAmount,
                setValue: value => this.Config.MaxBorrowAmount = (int)value,
                name: () => "Maximum Borrow Amount",
                min: 100f,
                interval: 100f
            );
        }
        private void LegacyMoneySection(IGenericModConfigMenuApi configMenu)
        {
            configMenu.AddSectionTitle(
                   mod: this.ModManifest,
                   text: () => "Money Amount Dialog"
               );

            configMenu.AddParagraph(
                mod: this.ModManifest,
                text: () => "Only modify these values if you DO NOT have \"Use Input Box\" checked."
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                getValue: () => this.Config.MoneyAmount1,
                setValue: value => this.Config.MoneyAmount1 = (int)value,
                name: () => "Money Amount 1",
                min: 0f,
                interval: 100f
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                getValue: () => this.Config.MoneyAmount2,
                setValue: value => this.Config.MoneyAmount2 = (int)value,
                name: () => "Money Amount 2",
                min: 0f,
                interval: 100f
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                getValue: () => this.Config.MoneyAmount3,
                setValue: value => this.Config.MoneyAmount3 = (int)value,
                name: () => "Money Amount 3",
                min: 0f,
                interval: 100f
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                getValue: () => this.Config.MoneyAmount4,
                setValue: value => this.Config.MoneyAmount4 = (int)value,
                name: () => "Money Amount 4",
                min: 0f,
                interval: 100f
            );
        }
        private void DurationSection(IGenericModConfigMenuApi configMenu)
        {
            configMenu.AddSectionTitle(
                      mod: this.ModManifest,
                      text: () => "Loan Duration Menu Options"
                  );

            configMenu.AddParagraph(
                mod: this.ModManifest,
                text: () => "Value in days that define the duration of the loan."
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                getValue: () => this.Config.DayLength1,
                setValue: value => this.Config.DayLength1 = (int)value,
                name: () => "Duration Option 1"
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                getValue: () => this.Config.DayLength2,
                setValue: value => this.Config.DayLength2 = (int)value,
                name: () => "Duration Option 2"
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                getValue: () => this.Config.DayLength3,
                setValue: value => this.Config.DayLength3 = (int)value,
                name: () => "Duration Option 3"
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                getValue: () => this.Config.DayLength4,
                setValue: value => this.Config.DayLength4 = (int)value,
                name: () => "Duration Option 4"
            );
        }
        private void InterestSection(IGenericModConfigMenuApi configMenu)
        {
            configMenu.AddSectionTitle(
                      mod: this.ModManifest,
                      text: () => "Interest Menu Options"
                  );

            configMenu.AddParagraph(
                mod: this.ModManifest,
                text: () => "Each value corresponds to the Loan Duration menu above."
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                getValue: () => this.Config.InterestModifier1,
                setValue: value => this.Config.InterestModifier1 = (float)value,
                name: () => "Interest Modifier 1"
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                getValue: () => this.Config.InterestModifier2,
                setValue: value => this.Config.InterestModifier2 = (float)value,
                name: () => "Interest Modifier 2"
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                getValue: () => this.Config.InterestModifier3,
                setValue: value => this.Config.InterestModifier3 = (float)value,
                name: () => "Interest Modifier 3"
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                getValue: () => this.Config.InterestModifier4,
                setValue: value => this.Config.InterestModifier4 = (float)value,
                name: () => "Interest Modifier 4"
            );
        }
    }
}
