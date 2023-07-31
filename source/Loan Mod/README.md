**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/legovader09/SDVLoanMod**

----

## Loan Mod
Loan Mod updated for .NET 5.0.
This is the repository for [Loan Mod](https://www.nexusmods.com/stardewvalley/mods/3882). 

### What is Loan Mod?
Loan Mod is a mod that lets you loan money for a period of time, with daily repayment installments. From interest rates, to the duration of the loan, almost everything can be customised; either via the `config.json` file or via the [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) mod.

### Installation
-   [Install the latest version of SMAPI](https://smapi.io/).
-   Install Loan Mod [from Nexus Mods](https://www.nexusmods.com/stardewvalley/mods/3882).
-   Run the game using `SMAPI`.

### Using the Mod
- Using the mod is very straight-forward! Simply press `L`, or open the app on the [Mobile Phone](https://www.nexusmods.com/stardewvalley/mods/6523) mod.
- You will be then prompted with an input of how much money you would like to borrow, and the duration you would like to borrow for along with the interest amount for that time period.

### Config File
Here is a list of config options and their purpose:

| Key | Default Value | Purpose |
| :- | :--: | - |
| LoanButton | L | This is the keyboard key to press to open the menu. |
| CustomMoneyInput | true | If true, the player is presented with a number input dialog, if false `MoneyAmount1-4` will be used instead, shown in a multiple choice dialog. |
| MaxBorrowAmount | 1000000 | The maximum amount of money that can be entered in the borrow dialog. This number should not exceed `99,999,999` as the game will throw an error. |
| LatePaymentChargeRate | 0.1 (10%) | This is the interest rate that will be added on to the money owed, if payments should fail to go through for `2` consecutive days. |
| InterestModifier1-4 | 0.5, 0.25, 0.1, 0.05 | These are the interest rates that correspond with the `DayLength1-4` settings below. |
| DayLength1-4 | 3, 7, 14, 28 | Sets the duration options of the loan. These values can be anything, but at minimum must be `1`. |
| MoneyAmount1-4 | 500, 1000, 5000, 10000 | Sets multiple choice option for amount of money to borrow. **Note:** these values are only relevant if `CustomMoneyInput` is set to `false`, this setting is obsolete, and should not be used unless preferred over a number input dialog. |
| Reset | false | Should the mod be bugged in any way with your save file, set this to `true`, then load the save again, and the mod should create a new Loan Profile. |
| AddMobileApp | true | If true, will add an app icon to the mobile phone. (if [Mobile Phone](https://www.nexusmods.com/stardewvalley/mods/6523) is installed.) |

Note: By default, the shorter the loan is the higher the interest rate will be, however some people prefer this the other way around. If this is the case for you, I'd recommend changing the interest rate values accordingly. The interest value corresponds to the DayLength setting, so *DayLength1* matches with *InterestModifier1*, and so on.

### Languages Translated
| Language   | Credit                  | Link |
| :--------- | :------------           | :--- |
| Korean     | Credits to [jooarose](https://www.nexusmods.com/users/56707037), [SNP0301](https://github.com/SNP0301)   | [X](LoanMod/i18n/ko.json) |
| Thai       | Credit to [warmblanket](https://github.com/ellipszist/StardewMods)  | [X](LoanMod/i18n/th.json) |
| Chinese    | Credit to [Puffeeydii](https://www.nexusmods.com/stardewvalley/users/122749553)   | [X](LoanMod/i18n/zh.json) |
| German    | Credit to [legovader09](https://github.com/legovader09)   | [X](LoanMod/i18n/de.json) |
| Russian    | Credit to [Sharaj](https://steamcommunity.com/id/Sharaj/myworkshopfiles)   | [X](LoanMod/i18n/ru.json) |
| Japanese    | Credit to Machina_Ex   | [X](LoanMod/i18n/ja.json) |

### Contributing
- If you would like to contribute with language translations, please follow the steps below:
1. Fork this repository.
2. Create a new branch (based off `dev` for the latest changes), ideally called `translations/{language code}` (e.g. translations/th for Thai).
3. If adding a new language, duplicate the [default.json](LoanMod/i18n/default.json) file, rename this to [your language code](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Translation#File_structure)`.json` (e.g. de.json for German)
4. Make the relevant translations, please ensure to leave {{tokens}} untouched, they can be moved around but must not be changed. As they are what the game swaps out for values.
6. Push up your work and then create a pull request into the `dev`  branch for approval.
- If you don't like using git, or GitHub, feel free to [email](mailto:dcdominoes@gmail.com) me with the translation file!
- You will be credited and added to the table above, as a thanks for your contribution. Only if you want to be, of course.
