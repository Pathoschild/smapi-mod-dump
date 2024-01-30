/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using BankOfFerngill.Framework;
using BankOfFerngill.Framework.Configs;
using BankOfFerngill.Framework.Data;
using BankOfFerngill.Framework.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace BankOfFerngill
{
    public class ModEntry : Mod
    {
        private BoFConfig _config;
        private ITranslationHelper _i18N;
        
        private IModEvents _events;
        private MobilePhoneApi _mobileApi;
        private IGenericModConfigMenuApi _cfgMenu;
        private BankData _bankData;
        private bool _debugging;
        private int _maxLoan;
        private int _loanOwed;
        private int _lostAmt;
        private int _gainedAmt;
        private int _giftAmt;

        private readonly List<Vector2> _vaultCoords = new()
        {
            new Vector2(54,1), 
            new Vector2(54,2),
            new Vector2(54,3),
            new Vector2(54,4),
            new Vector2(54,5),
            new Vector2(55,1), 
            new Vector2(55,2),
            new Vector2(55,3),
            new Vector2(55,4),
            new Vector2(55,5),
            new Vector2(56,1), 
            new Vector2(56,2),
            new Vector2(56,3),
            new Vector2(56,4),
            new Vector2(56,5),
            new Vector2(57,1), 
            new Vector2(57,2),
            new Vector2(57,3),
            new Vector2(57,4),
            new Vector2(57,5),
            
        };

        private readonly List<Vector2> _deskCords = new()
        {
            new Vector2(48, 4),
            new Vector2(48, 5),
            new Vector2(48, 6),
            new Vector2(49, 4),
            new Vector2(49, 5),
            new Vector2(49, 6),
            new Vector2(50, 4),
            new Vector2(50, 5),
            new Vector2(50, 6),
            new Vector2(51, 4),
            new Vector2(51, 5),
            new Vector2(51, 6)
        };

        private readonly List<Vector2> _jojaMartCoords = new()
        {
            new Vector2(24, 23),
            new Vector2(24, 24)
        }; 
        
        
        /// <summary>
        /// The entry method
        /// </summary>
        /// <param name="helper">Helper interface</param>
        public override void Entry(IModHelper helper)
        {
            //Load the config file
            _config = helper.ReadConfig<BoFConfig>();
            
            //Set up translations
            _i18N = Helper.Translation;
            
            //Load Events
            _events = helper.Events;

            //Events
            _events.Input.ButtonPressed += OnButtonPressed; //Event that triggers when a button is pressed.
            _events.GameLoop.SaveLoaded += OnSaveLoaded; //Event that triggers when a Save is loaded.
            //_events.GameLoop.Saving += OnSaving; //Event that triggers before a save is saved.
            _events.GameLoop.SaveCreated += OnSaveCreated; //Event that triggers when a new save is created.
            _events.GameLoop.DayEnding += OnDayEnding; //Event that triggers when the day ends.
            _events.GameLoop.Saved += OnSaved; //Event that triggers after a save is saved.
            _events.GameLoop.GameLaunched += OnGameLaunched; //Event that triggers when the game is launched.
            _events.Content.AssetRequested += OnAssetRequested; //Event that triggers when the game is requesting an asset.
            
        }
        
        //Event Methods

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            //Generic Mod Config Menu.
            #region Generic Moc Config Menu
            _cfgMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (_cfgMenu is null) return;
            
            //Register mod
            _cfgMenu.Register(
                mod: ModManifest,
                reset: () => _config =  new BoFConfig(),
                save: () => Helper.WriteConfig(_config)
                );
            
            _cfgMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => "Bank of Ferngill Settings",
                tooltip: null
                );
            
            _cfgMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Base Banking Interest",
                tooltip: () => "The base value that gets calculated into the daily interest for any money in the bank",
                getValue: () => _config.BaseBankingInterest,
                setValue: value => _config.BaseBankingInterest = value
            );
            
            _cfgMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => _config.EnableRandomEvents,
                setValue: value => _config.EnableRandomEvents = value,
                name: () => "Enable Random Events",
                tooltip:() => "Enable Random Events, that happens during the nightly update. Win money, Lose money and so on. (Coming Soon)"
            );
            
            _cfgMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => _config.EnableVaultRoomDeskActivation,
                setValue: value => _config.EnableVaultRoomDeskActivation = value,
                name: () => "Click desk for Bank",
                tooltip:() => "Enable if you want to bypass needing the vault room completed to activate the bank."
            );
            
            _cfgMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => "Loan Settings",
                tooltip: null
                );
            
            _cfgMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Loan Interest",
                tooltip: () => "The base value that gets calculated into the interest for any loan you take out.",
                getValue: () => _config.LoanSettings.LoanBaseInterest,
                setValue: value => _config.LoanSettings.LoanBaseInterest = value
            );
            
            _cfgMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => _config.LoanSettings.PayBackLoanDaily,
                setValue: value => _config.LoanSettings.PayBackLoanDaily = value,
                name: () => "Pay Back a Portion of Loan Daily",
                tooltip:() => "If enabled you will pay back a portion of the loan each night. If possible"
            );
            
            _cfgMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Percentage of Loan to Pay Back Daily",
                tooltip: () => "Percent of Loan Paid Daily",
                getValue: () => _config.LoanSettings.PercentageOfLoanToPayBackDaily,
                setValue: value => _config.LoanSettings.PercentageOfLoanToPayBackDaily = value
            );
            
            _cfgMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => _config.LoanSettings.EnableUnlimitedLoansAtOnce,
                setValue: value => _config.LoanSettings.EnableUnlimitedLoansAtOnce = value,
                name: () => "Unlimited Loans",
                tooltip:() => "If enabled you will be able to have any number of loans at once."
            );
            
            _cfgMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => "Hard Mode Settings",
                tooltip: null
                );
            
            _cfgMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => _config.HardModSettings.EnableHarderMode,
                setValue: value => _config.HardModSettings.EnableHarderMode = value,
                name: () => "Enable Hard Mode",
                tooltip:() => "If enabled you will have debt that needs to be paid back before you can deposit any money."
            );
            _cfgMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Starting Debt",
                tooltip: () => "How much debt to start with.",
                getValue: () => _config.HardModSettings.HowFarInDebtAtStart,
                setValue: value => _config.HardModSettings.HowFarInDebtAtStart = value
            );
            _cfgMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => _config.HardModSettings.BypassHavingToRepayDebtFirst,
                setValue: value => _config.HardModSettings.BypassHavingToRepayDebtFirst = value,
                name: () => "Bypass Repaying Debt First",
                tooltip:() => "If enabled you can deposit without first repaying your debt."
            );
            #endregion
            
            #region MobilePhone

            _mobileApi = Helper.ModRegistry.GetApi<MobilePhoneApi>("aedenthorn.MobilePhone");
            if (_mobileApi != null)
            {
                var appIcon = Helper.ModContent.Load<Texture2D>(Path.Combine("assets", "bank_phone_icon.png"));
                var hasLoaded = _mobileApi.AddApp(Helper.ModRegistry.ModID, "Bank of Ferngill",DoBanking, appIcon);
                Monitor.Log($"Loaded Bank of Ferngills Mobile Phone app successfully: {hasLoaded}", LogLevel.Debug);
            }

            #endregion

        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;

            if (e.IsDown(SButton.F5))
            {
                _config = Helper.ReadConfig<BoFConfig>();
                Monitor.Log("The config file was reloaded.");
            }

            if (e.IsDown(SButton.NumPad4) && _debugging)
            {
                DoBanking();
            }

            if (e.IsDown(SButton.F10) && _debugging)
            {
                var stockTanked = 0;
                var stockRose = 0;
                var customerAppreciation = 0;
                var accountHacked = 0;
                var debtPaid = 0;
                var nothingDone = 0;
                
                //DoRandomEvent();
                for (var i = 0; i < 27; i++)
                {
                    var ra = Game1.random.NextDouble();
                    var luck = Game1.player.DailyLuck;
                   
                    var str = "";
                    
                    if(ra < 0.05 + luck)
                        str = $"< {0.05f + luck} = Debt Paid";
                    else if (ra < 0.07f + luck)
                        str = $"< {0.07f + luck} = Account Hacked";
                    else if (ra < 0.10f + luck)
                        str = $"< {0.10f + luck} = Customer Appreciation";
                    else if (ra < 0.15f + luck)
                        str = $"< {0.15f + luck} = Stock Rose";
                    else if (ra < 0.20f + luck)
                        str = $"< {0.20f + luck} = Stock Tanked";
                    Monitor.Log($"Random was: {ra} {str}",LogLevel.Alert);
                    
                    if(ra < 0.05 + luck)
                        debtPaid++;
                    else if (ra < 0.07f + luck)
                        accountHacked++;
                    else if (ra < 0.10f + luck)
                        customerAppreciation++;
                    else if (ra < 0.15f + luck)
                        stockRose++;
                    else if (ra < 0.20f + luck)
                        stockTanked++;
                    else
                        nothingDone++;
                    /*
                    switch (ra)
                    {
                        case < 0.05f:
                            debtPaid++;
                            break;
                        case < 0.07f:
                            accountHacked++;
                            break;
                        case < 0.10f:
                            customerAppreciation++;
                            break;
                        case < 0.15f:
                            stockRose++;
                            break;
                        case < 0.20f:
                            stockTanked++;
                            break;
                    }*/
                }
                Monitor.Log($"StockTanked: {stockTanked}, StockRose: {stockRose}, CustomerAppreciation: {customerAppreciation}, AccountHacked: {accountHacked}, DebtPaid: {debtPaid}, NothingDone: {nothingDone}", LogLevel.Warn);
                Monitor.Log($"Lets check CalculatePercentage 10. {CalculatePercentage(10)}, 25. {CalculatePercentage(25)}, 50. {CalculatePercentage(50)} , 75. {CalculatePercentage(75)}");
            }

            if (e.IsDown(SButton.NumPad3) && _debugging)
            {
                Game1.activeClickableMenu = new BankTabbedMenu(MenuTab.BankInfo, Monitor, _i18N, _bankData, false);
            }
            
            if(e.IsDown(SButton.Escape) && Game1.isQuestion)// && Game1.activeClickableMenu is BankMenu or BankInfoMenu)
            {
                Game1.exitActiveMenu();
            }
            if (e.IsDown(SButton.MouseRight) && 
                ((Game1.currentLocation.Name.Contains("Community") && ((_vaultCoords.Contains(Game1.currentCursorTile) && Game1.player.mailReceived.Contains("ccVault")) || (_config.EnableVaultRoomDeskActivation && _deskCords.Contains(Game1.currentCursorTile)))) || 
                (Game1.currentLocation.Name.Contains("JojaMart") && ((_jojaMartCoords.Contains(Game1.currentCursorTile) && Game1.player.mailReceived.Contains("jojaVault")) || (_config.EnableVaultRoomDeskActivation && _jojaMartCoords.Contains(Game1.currentCursorTile))))))
            {
                DoBanking();
            }
        }

        private void OnSaveCreated(object sender, SaveCreatedEventArgs e)
        {
            _bankData = new BankData();
            
            //Lets see if we should add debt.
            if (_config.HardModSettings.EnableHarderMode)
            {
                _bankData.LoanedMoney += _config.HardModSettings.HowFarInDebtAtStart;
                _bankData.TotalNumberOfLoans++;
                _bankData.WasHardModeDebtAdded = true;
            }
        }
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            _bankData = Helper.Data.ReadSaveData<BankData>(ModManifest.UniqueID) ?? new BankData();

            _bankData.BankInterest = _config.BaseBankingInterest > 0 ? _config.BaseBankingInterest : 0;
            _bankData.LoanInterest = _config.LoanSettings.LoanBaseInterest > 0 ? _config.LoanSettings.LoanBaseInterest : 0;
            
            _loanOwed = 0;
            _maxLoan = 0;
            
            //Check to see if hardmode is enabled. If it is, add the debt.
            if (_config.HardModSettings.EnableHarderMode && !_bankData.WasHardModeDebtAdded)
            {
                _bankData.LoanedMoney += _config.HardModSettings.HowFarInDebtAtStart;
                _bankData.WasHardModeDebtAdded = true;
            }
            //Enable or disable Debugging Based on FarmerName
            _debugging = Game1.player.Name.Contains("Vrillion");
        }

        private void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            //Write the bank data to the save before it actually saves.
            if (_bankData is null) return;
            
            if(_bankData.MoneyInBank > 0)
                _bankData.MoneyInBank += CalculateInterest(_bankData.MoneyInBank, _bankData.BankInterest);

            var loanOwed = _bankData.LoanedMoney - _bankData.MoneyPaidBack;
            
            if (_config.LoanSettings.PayBackLoanDaily && Game1.player.Money >=
                CalculateInterest(_bankData.LoanedMoney, _config.LoanSettings.PercentageOfLoanToPayBackDaily) && loanOwed > 0)
            {
                var total = loanOwed > CalculateInterest(_bankData.LoanedMoney, _config.LoanSettings.PercentageOfLoanToPayBackDaily)
                    ? CalculateInterest(_bankData.LoanedMoney, _config.LoanSettings.PercentageOfLoanToPayBackDaily)
                    : loanOwed;
                _bankData.MoneyPaidBack += total;
                    
                Game1.player.Money -= total;

                if (_bankData.MoneyPaidBack == _bankData.LoanedMoney && _bankData.TotalNumberOfLoans > 0)
                {
                    _bankData.MoneyPaidBack = 0;
                    _bankData.LoanedMoney = 0;
                    _bankData.TotalNumberOfLoans = 0;
                    _bankData.NumberOfLoansPaidBack++;
                }
                    
            }
                
            //Do Random Event.
            if(_config.EnableRandomEvents && _bankData.MoneyInBank > 0)
                DoRandomEvent();
                
            //Write save data.
            Helper.Data.WriteSaveData(ModManifest.UniqueID, _bankData);


        }

        private void OnSaved(object sender, SavedEventArgs e)
        {
            
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/mail"))
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    //Now we add the mail.
                    data["bankStockTanked"] = _i18N.Get("bank.events.stockTanked", new {player_name = Game1.player.Name, lost_amt = _lostAmt });
                    data["bankStockRose"] = _i18N.Get("bank.events.stockRose", new {player_name = Game1.player.Name, gain_amt = _gainedAmt });
                    data["bankCustomerAppreciation"] = _i18N.Get("bank.events.customerAppreciation", new {player_name = Game1.player.Name, gift_amt = _giftAmt });
                    data["bankAccountHacked"] = _i18N.Get("bank.events.accountHacked", new {player_name = Game1.player.Name });
                    data["bankDebtPaid"] = _i18N.Get("bank.events.debtPaid", new {player_name = Game1.player.Name });
                });
            }
        }
        
        #region Custom Methods
        private void DoBanking()
        {
            if (Game1.activeClickableMenu is not null)
                return;

            _mobileApi?.SetAppRunning(true);

            _maxLoan = CalculateInterest(Convert.ToInt32(Game1.player.totalMoneyEarned), 10);
            _loanOwed = _bankData.LoanedMoney - _bankData.MoneyPaidBack;
                
            //Dialogue Responses
            var mainResponses = new List<Response> { new("BankInfo", "Bank Account Information") };
            if (_bankData.MoneyInBank > 0)
            {
                mainResponses.Add(new Response("Withdraw", "Make a Withdraw"));
            }

            if (!_config.HardModSettings.EnableHarderMode || 
                (_config.HardModSettings.EnableHarderMode && _config.HardModSettings.BypassHavingToRepayDebtFirst) || 
                (_config.HardModSettings.EnableHarderMode && _bankData.LoanedMoney == 0 && !_config.HardModSettings.BypassHavingToRepayDebtFirst))
            {
                mainResponses.Add(new Response("Deposit", "Make a Deposit"));
            }
            
            if (_bankData.LoanedMoney == 0 || _config.LoanSettings.EnableUnlimitedLoansAtOnce)
            {
                mainResponses.Add(new Response("GetLoan", "Get a Loan"));
            }

            if (_bankData.LoanedMoney > 0)
            {
                mainResponses.Add(new Response("PayBackLoan", "Make a Loan Payment"));
            }

            mainResponses.Add(new Response("Nothing", "Nothing Now")); 
            //Now we create the dialogue
            Game1.currentLocation.createQuestionDialogue("Choose an Option from below.", mainResponses.ToArray(), BankProcessAnswer
                /*delegate(Farmer _, string whichAnswer)
                {
                    Game1.activeClickableMenu = whichAnswer switch
                    {
                        "BankInfo" => new BankInfoMenu(Monitor, Helper.Translation, _bankData),
                        "Withdraw" => new BankMenu(Monitor, Helper.Translation, _config, _bankData, DoWithdraw, _i18N.Get("bank.withdraw.title"), FormatString(_i18N.Get("bank.withdraw.description", new { player_name = Game1.player.Name, bank_balance = _bankData.MoneyInBank}), 1280)),
                        "Deposit" => new BankMenu(Monitor, Helper.Translation, _config, _bankData, DoDeposit, _i18N.Get("bank.deposit.title"), FormatString(_i18N.Get("bank.deposit.description", new { player_name = Game1.player.Name, bank_balance = _bankData.MoneyInBank}), 1280)),                        
                        "GetLoan" => new BankMenu(Monitor, Helper.Translation, _config, _bankData, DoGetLoan, _i18N.Get("bank.getLoan.title"), FormatString(_i18N.Get("bank.getLoan.description", new { player_name = Game1.player.Name, loan_interest = _bankData.LoanInterest, max_loan = _maxLoan, total_money_earned = Game1.player.totalMoneyEarned }), 1280)),
                        "PayBackLoan" => new BankMenu(Monitor, Helper.Translation, _config, _bankData, DoPayLoan, _i18N.Get("bank.payLoan.title"), FormatString(_i18N.Get("bank.payLoan.description", new { player_name = Game1.player.Name, loan_owed = _loanOwed }), 1280)),
                        _ => Game1.activeClickableMenu = null
                    };
                }*/);
            _mobileApi?.SetAppRunning(false);

        }

        private void BankProcessAnswer(Farmer who, string whichAnswer)
        {
            if(whichAnswer == "BankInfo")
            {
                Game1.activeClickableMenu = new BankInfoMenu(Monitor, Helper.Translation, _bankData);
            }
            else if(whichAnswer == "Withdraw")
            {
                Game1.activeClickableMenu = new BankMenu(Monitor, Helper.Translation, _config, _bankData, DoWithdraw, _i18N.Get("bank.withdraw.title"), FormatString(_i18N.Get("bank.withdraw.description", new { player_name = Game1.player.Name, bank_balance = _bankData.MoneyInBank }), 1280));
            }
            else if (whichAnswer == "Deposit")
            {
                Game1.activeClickableMenu = new BankMenu(Monitor, Helper.Translation, _config, _bankData, DoDeposit, _i18N.Get("bank.deposit.title"), FormatString(_i18N.Get("bank.deposit.description", new { player_name = Game1.player.Name, bank_balance = _bankData.MoneyInBank }), 1280));
            }
            else if (whichAnswer == "GetLoan")
            {
                Game1.activeClickableMenu = new BankMenu(Monitor, Helper.Translation, _config, _bankData, DoGetLoan, _i18N.Get("bank.getLoan.title"), FormatString(_i18N.Get("bank.getLoan.description", new { player_name = Game1.player.Name, loan_interest = _bankData.LoanInterest, max_loan = _maxLoan, total_money_earned = Game1.player.totalMoneyEarned }), 1280));
            }
            else if (whichAnswer == "PayBackLoan")
            {
                Game1.activeClickableMenu = new BankMenu(Monitor, Helper.Translation, _config, _bankData, DoPayLoan, _i18N.Get("bank.payLoan.title"), FormatString(_i18N.Get("bank.payLoan.description", new { player_name = Game1.player.Name, loan_owed = _loanOwed }), 1280));
            }
            else if(whichAnswer == "Nothing")
            {
                return;               
            }
            else
            {
                return;
            }
        }
        
        //Private Methods
        private void DoRandomEvent()
        {
            //Coming Soon
            
            var rand = Game1.random.Next(0, 100);
            //var chance = GetPercentage(rand, 100);
            _lostAmt = GetRandomAmt(1, GetRandomAmt(1, CalculateInterest(_bankData.MoneyInBank > 0 ? _bankData.MoneyInBank : 0, _bankData.BankInterest)));
            _gainedAmt = GetRandomAmt(1, GetRandomAmt(1, CalculateInterest(_bankData.MoneyInBank > 0 ? _bankData.MoneyInBank : 0, _bankData.BankInterest)));
            _giftAmt = GetRandomAmt(1, GetRandomAmt(1, CalculateInterest(_bankData.MoneyInBank > 0 ? _bankData.MoneyInBank : 0, _bankData.BankInterest)));
            var hackedAmt = GetPercentage(_bankData.MoneyInBank, 75);
            
            if(_debugging)
                Monitor.Log($"Random: {rand} and check Value was {0.05 + Game1.player.DailyLuck}");
            
            //Now we invalidate the mail, this way the correct values get added.
            Helper.GameContent.InvalidateCache("Data/mail");
            
            //Now we do our calculations for events.
            if (rand > 10)
            {
                if (_debugging)
                    Monitor.Log($"Rand was: {rand}", LogLevel.Info);
                return;
            }
            //Random Chance passed, we can now decide which event to run
            var eventRandom = Game1.random.Next(1, 6);
                
            switch (eventRandom)
            {
                case 1:
                {
                    Game1.player.mailForTomorrow.Add("bankStockTanked");
                    if (_bankData.MoneyInBank > _lostAmt)
                        _bankData.MoneyInBank -= _lostAmt;
                    if (_debugging)
                        Monitor.Log($"StockTanked Lost: {_lostAmt}G");
                    break;
                }
                case 2:
                {
                    Game1.player.mailForTomorrow.Add("bankStockRose");
                    _bankData.MoneyInBank += _gainedAmt;
                    if(_debugging)
                        Monitor.Log($"StockRose Gained: {_gainedAmt}G");
                    break;
                }
                case 3:
                {
                    Game1.player.mailForTomorrow.Add("bankAccountHacked");
                    _bankData.MoneyInBank -= hackedAmt;
                    if(_debugging)
                        Monitor.Log($"AccountHacked Lost: {hackedAmt}");
                    break;
                }
                case 4:
                {
                    Game1.player.mailForTomorrow.Add("bankCustomerAppreciation");
                    _bankData.MoneyInBank += _giftAmt;
                    if(_debugging)
                        Monitor.Log($"CustomerAppreciation Gained: {_giftAmt}G");
                    break;
                }
                case 5:
                {
                    Game1.player.mailForTomorrow.Add("bankDebtPaid");
                    if (_bankData.LoanedMoney - _bankData.MoneyPaidBack > 0)
                    {
                        _bankData.LoanedMoney = 0;
                        _bankData.MoneyPaidBack = 0;
                        _bankData.TotalNumberOfLoans = 0;
                    }
                    if(_debugging)
                        Monitor.Log($"DebtPaid ForgivenAmt: {_bankData.LoanedMoney}G");
                    break;
                }
                default:
                {
                    if(_debugging)
                        Monitor.Log($"(Rand: {rand}) Nothing was triggered in the event");
                    break;
                }
            }
        }
        
        
        //Bank Menus
        private void DoWithdraw(string val)
        {
            try
            {
                var withdrawAmt = int.Parse(val);
                var haveEnoughMoney = _bankData.MoneyInBank >= withdrawAmt;
                if (!haveEnoughMoney)
                {
                    Game1.showGlobalMessage(_i18N.Get("bank.notEnoughMoneyInBank", new { amt = FormatNumber(withdrawAmt)}));
                    return;
                }
                if (withdrawAmt > 0)
                {
                    _bankData.MoneyInBank -= withdrawAmt;
                    Game1.player.Money += withdrawAmt;
                    Game1.exitActiveMenu();
                    Game1.showGlobalMessage(_i18N.Get("bank.withdraw.doWithdraw", new { amt = FormatNumber(withdrawAmt)}));
                }
                else
                {
                    Monitor.Log($"{val} wasn't a valid int.");
                }
            }
            catch (Exception ex)
            {
                Game1.showGlobalMessage(_i18N.Get("bank.nonNumeric"));
                Monitor.Log($"An error was thrown.\r\n {ex}");
            }
            
        }
        
        private void DoDeposit(string val)
        {
            try
            {
                var depositAmt = int.Parse(val);
                var haveEnoughMoney = Game1.player.Money >= depositAmt;
                if (!haveEnoughMoney)
                {
                    Game1.showGlobalMessage(_i18N.Get("bank.notEnoughMoneyOnPlayer", new { amt = FormatNumber(depositAmt) }));
                    return;
                }

                if (_config.HardModSettings.EnableHarderMode && (_bankData.LoanedMoney - _bankData.MoneyPaidBack) > 0 && !_config.HardModSettings.BypassHavingToRepayDebtFirst)
                {
                    Game1.showGlobalMessage(_i18N.Get("bank.hardmode.cantdeposit"));
                    return;
                }
                
                if (depositAmt > 0)
                {
                    _bankData.MoneyInBank += depositAmt;
                    Game1.player.Money -= depositAmt;
                    Game1.exitActiveMenu();
                    Game1.showGlobalMessage(_i18N.Get("bank.deposit.doDeposit", new { amt = FormatNumber(depositAmt) }));
                }
                else
                {
                    Monitor.Log($"{val} wasn't a valid int.");
                }
            }
            catch (Exception ex)
            {
                //Game1.exitActiveMenu();
                Game1.showGlobalMessage(_i18N.Get("bank.nonNumeric"));
                Monitor.Log($"An error was thrown.\r\n {ex}");
            }
            
        }
        
        private void DoGetLoan(string val)
        {
            try
            {
                int loanAmt = int.Parse(val);
                if (loanAmt > _maxLoan)
                {
                    Game1.showGlobalMessage(_i18N.Get("bank.getLoan.canGetThatMuch", new { amt = loanAmt}));
                }
                else if (_bankData.LoanedMoney - _bankData.MoneyPaidBack > 0 && !_config.LoanSettings.EnableUnlimitedLoansAtOnce)
                {
                    Game1.showGlobalMessage(_i18N.Get("bank.getLoan.stillOwe", new { loan_owned = _bankData.LoanedMoney - _bankData.MoneyPaidBack}));
                }
                else
                {
                    _bankData.LoanedMoney = CalculateInterest(loanAmt, _bankData.LoanInterest) + loanAmt;
                    _bankData.TotalNumberOfLoans++;
                    Game1.player.Money += loanAmt;
                    Game1.exitActiveMenu();
                    Game1.showGlobalMessage(_i18N.Get("bank.getLoan.loanTaken", new {loan = loanAmt, loan_interest = CalculateInterest(loanAmt, _bankData.LoanInterest)} ));
                }
            }
            catch (Exception ex)
            {
                Game1.showGlobalMessage(_i18N.Get("bank.nonNumeric"));
                Monitor.Log($"An error was thrown. \r\n{ex}");
            }
        }
        
        private void DoPayLoan(string val)
        {
            try
            {
                int amtLoanPay = int.Parse(val);
                if (amtLoanPay > _bankData.LoanedMoney - _bankData.MoneyPaidBack)
                {
                    Game1.showGlobalMessage(_i18N.Get("bank.payLoan.DontOweThatMuch",new { loan_owed = _bankData.LoanedMoney - _bankData.MoneyPaidBack }));
                }
                else if (Game1.player.Money < amtLoanPay)
                {
                    Game1.showGlobalMessage(_i18N.Get("bank.notEnoughMoneyOnPlayer", new { amt = FormatNumber(amtLoanPay) }));
                }
                else
                {
                    _loanOwed = _bankData.LoanedMoney - _bankData.MoneyPaidBack;
                    var total = _loanOwed > CalculateInterest(_bankData.LoanedMoney, _config.LoanSettings.PercentageOfLoanToPayBackDaily)
                        ? CalculateInterest(_bankData.LoanedMoney, _config.LoanSettings.PercentageOfLoanToPayBackDaily)
                        : _loanOwed;
                    
                    _bankData.MoneyPaidBack += amtLoanPay; //LoanedMoney -= amtLoanPay;
                    Game1.player.Money -= amtLoanPay;
                    if (_bankData.MoneyPaidBack == _bankData.LoanedMoney)
                    {
                        _bankData.LoanedMoney = 0;
                        _bankData.MoneyPaidBack = 0;
                        _bankData.NumberOfLoansPaidBack++;
                        if (_bankData.TotalNumberOfLoans > 0)
                            _bankData.TotalNumberOfLoans--;
                        else
                            _bankData.TotalNumberOfLoans = 0;
                    }
                    string s = _bankData.LoanedMoney > 0 ? _i18N.Get("bank.payLoan.payTowards", new { amt = FormatNumber(amtLoanPay), loan_balance = _bankData.LoanedMoney - _bankData.MoneyPaidBack}) : _i18N.Get("bank.payLoan.paidOff");
                    Game1.exitActiveMenu();
                    Game1.showGlobalMessage(s);
                }

            }
            catch (Exception ex)
            {
                Game1.showGlobalMessage(_i18N.Get("bank.nonNumeric"));
                Monitor.Log($"An error was thrown.\r\n {ex}");
            }
        }
        
        #endregion
        
        
        //Private Methods
        private int GetPercentage(int initialValue, int percentage)
        {
            return (initialValue * percentage) / 100;
        }
        
        private int GetRandomAmt(int min, int max)
        {
            return Game1.random.Next(min, max);
        }
        
        //Static Methods

        private static int CalculatePercentage(int value, int total = 100)
        {
            return (value / total) * 100 > 0 ? (value / total) * 100 : 0;
        }
        private static int CalculateInterest(int val, int interest)
        {
            return (val * interest / 100);
        }

        private static string FormatString(string val, int width)
        {
            int maxWidth = width;//UiWidth - 10;
            string outer = "";

            foreach (string ori in val.Replace("\r\n", "\n").Split('\n'))
            {
                var line = "";
                foreach (var word in ori.Split(' '))
                {
                    if (line == "")
                    {
                        line = word;
                    }
                    else if (Game1.smallFont.MeasureString(line + " " + word).X <= maxWidth)
                    {
                        line += "\n " + word;
                    }
                    else
                    {
                        outer = line;
                        line = word;
                    }
                }

                if (line != "")
                {
                    outer = line;
                }
            }

            return outer;
        }
        
        public static string FormatNumber(int val)
        {
            return $"{val:#,0}";
        }
        
    }
    
    }