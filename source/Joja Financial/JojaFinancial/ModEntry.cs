/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/JojaFinancial
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using NermNermNerm.Stardew.LocalizeFromSource;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Shops;

using static NermNermNerm.Stardew.LocalizeFromSource.SdvLocalize;

namespace StardewValleyMods.JojaFinancial
{
    public class ModEntry
        : Mod, ISimpleLog
    {
        private const string StartLoanEventCommand = "JojaFinance.StartLoan";
        private const string MorrisOffersLoanEvent = "JojaFinance.MorrisOffer";

        public static ModConfig Config = null!;
        public ModConfigMenu ConfigMenu = new ModConfigMenu();

        public VGame1 Game1 { get; private set; }
        public Loan Loan { get; }
        public GeneratedMail GeneratedMail { get; }

        protected JojaPhoneHandler PhoneHandler { get; }

        public ModEntry()
            : this(new VGame1(), new Loan(), new JojaPhoneHandler(), new GeneratedMail())
        { }

        public ModEntry(VGame1 game1, Loan loan, JojaPhoneHandler phoneHandler, GeneratedMail generatedMail)
        {
            this.Game1 = game1;
            this.Loan = loan;
            this.PhoneHandler = phoneHandler;
            this.GeneratedMail = generatedMail;
        }

        public override void Entry(IModHelper helper)
        {
            Initialize(this, I("en"));

            Config = this.Helper.ReadConfig<ModConfig>();
            this.ConfigMenu.Entry(this);

            this.Helper.Events.Content.AssetRequested += this.OnAssetRequested;
            Event.RegisterCommand(StartLoanEventCommand, this.StartLoan);

            this.Loan.Entry(this);
            this.PhoneHandler.Entry(this);
            this.GeneratedMail.Entry(this);
        }

        private void StartLoan(Event @event, string[] args, EventContext context)
        {
            try
            {
                this.Loan.InitiateLoan(new LoanScheduleTwoYear());
            }
            finally
            {
                ++@event.currentCommand;
            }
        }

        private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Events/Farm"))
            {
                e.Edit((data) =>
                {
                    var dict = data.AsDictionary<string, string>().Data;
                    dict[IF($"{MorrisOffersLoanEvent}/t 600 930/w sunny/d Mon Tue")] = SdvEvent($@"
continue
64 15
farmer 64 15 2 Morris 65 16 0

setskipactions addItem (BC)214
skippable

speak Morris ""Welcome to the Valley!  It is my pleasure to welcome you to our community on behalf of the whole Joja Team!#$b#Please accept this telephone as a housewarming gift from your friends at your local Jojamart!""
addItem (BC)214
speak Morris ""While I'm here, I thought I'd tell you about a SPECIAL OFFER, EXCLUSIVELY for new residents of Stardew Valley!#$b#We'd you to have a complete Wallpaper and Furniture Catalog for ABSOLUTELY NO MONEY DOWN and NO PAYMENTS for TWO SEASONS!$1""
faceDirection Morris 3
speak Morris ""...mumble mumble...  usurious interests rates...  mumble mumble...  unfair fees... mumble mumble... draconian penalties...  mumble mumble...$3""
faceDirection Morris 0
speak Morris ""SO ARE YOU READY TO START LIVING IN COMFORT??!  Sure you are!  Just sign this contract and I'll have that furniture catalog shipped right out!""
quickQuestion #Morris, I am so ready to start living my dream!#Ermm..  I need time to think about that(break)emote Morris 32\speak Morris ""Great!  The Joja corporation is ready to enable you to live the way you want to, NOW!  Let the future take care of itself, am I right??!  I'll have those catalogs shipped out tonight!""\JojaFinance.StartLoan(break)emote Morris 12\speak Morris ""That's very... responsible of you - financial decisions like this should be undertaken with careful thought.$3#$b#JojaFinancial is ready whenever you are!  Just use your complimentary Joja Phone to call our offices after you've thought it over!""
faceDirection Morris 1
pause 200
faceDirection Morris 0
speak Morris ""Once again, Welcome to Stardew Valley and we look forward to seeing you at your local neighborhood JojaMart!""
pause 200
faceDirection Morris 1
end fade
").Replace("\r", "").Replace("\n", "/");
                });
            }
        }

        [NoStrict]
        public List<StardewValley.Object> GetConfiguredCatalogs()
        {
            void issueError(string message)
            {
                StardewValley.Game1.chatBox?.addErrorMessage(IF($"JojaFinancial's configuration is bad: {message}"));
                this.LogError($"Bad configuration: {message}");
            }
            void issueWarning(string message)
            {
                StardewValley.Game1.chatBox?.addErrorMessage(IF($"JojaFinancial's configuration is suspicious: {message}"));
                this.LogWarning($"Suspicious configuration: {message}");
            }

            List<StardewValley.Object> result = new();

            (bool isEnabled, string qiid)[] pairs = [
                (Config.UseRobinsFurnitureCatalogue, "(F)1226"),
                (Config.UsePierresWallpaperCatalogue, "(F)1308"),
                (Config.UseJojaCatalogue, "(F)JojaCatalogue"),
                (Config.UseWizardCatalogue, "(F)WizardCatalogue"),
                (Config.UseJunimoCatalogue, "(F)JunimoCatalogue"),
                (Config.UseRetroCatalogue, "(F)RetroCatalogue")];
            var stockQiids = pairs.Where(p => p.isEnabled).Select(p => p.qiid);

            string?[] givenModQiids = [Config.ModCatalog1, Config.ModCatalog2, Config.ModCatalog3, Config.ModCatalog4, Config.ModCatalog5, Config.ModCatalog6];
            var modIds = givenModQiids.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s!);

            foreach (string qiid in stockQiids.Union(modIds).Distinct())
            {
                var item = this.Game1.CreateObject(qiid, 1);
                if (item is null || item.Name == "ErrorItem")
                {
                    issueError($"'{qiid}' Is not a known Stardew Valley object.");
                }
                else
                {
                    if (item.Price <= 10000)
                    {
                        int shopPrice = this.GetShopPrice(item);
                        if (shopPrice > 0 && shopPrice <= 10000)
                        {
                            issueWarning($"Warning: '{qiid}' ('{item.DisplayName}') is priced at {item.Price}, which seems cheap.  It was found in a shop for {shopPrice}, which is also seems too cheap.");
                        }
                        else if (shopPrice == 0)
                        {
                            issueWarning($"Warning: '{qiid}' ('{item.DisplayName}') is priced at {item.Price}, which seems cheap.  There isn't any shop that carries it either.");
                        }
                        else
                        {
                            item.Price = shopPrice;
                        }
                    }

                    result.Add(item);
                }
            }

            if (!result.Any())
            {
                issueError("No valid catalog entries supplied - defaulting to the base game catalogs.");
                result.Add(this.Game1.CreateObject("(F)1226", 1)!);
                result.Add(this.Game1.CreateObject("(F)1308", 1)!);
            }

            return result;
        }

        private int GetShopPrice(StardewValley.Object item)
        {
            foreach (var value in DataLoader.Shops(StardewValley.Game1.content).Values)
            {
                foreach (ShopItemData shopItemData in value.Items)
                {
                    if (shopItemData.ItemId == item.QualifiedItemId)
                    {
                        this.LogTrace($"Found {item.QualifiedItemId} in {value.Owners.FirstOrDefault()?.Name}'s shop at {shopItemData.Price}");
                        return shopItemData.Price;
                    }
                }
            }

            return 0;
        }

        public void WriteToLog(string message, LogLevel level, bool isOnceOnly)
        {
            if (isOnceOnly)
            {
                this.Monitor.LogOnce(message, level);
            }
            else
            {
                this.Monitor.Log(message, level);
            }
        }
    }
}
