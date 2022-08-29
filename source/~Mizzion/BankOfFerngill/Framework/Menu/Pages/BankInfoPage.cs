/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using BankOfFerngill.Framework.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Minigames;

namespace BankOfFerngill.Framework.Menu.Pages
{
    public class BankInfoPage : IClickableMenu
    {
        private int menuX;
        private int menuY;

        private ITranslationHelper _i18N;
        private IMonitor _monitor;
        private BankData _bankData;
        
        
        //Page stuff
         
        public BankInfoPage(int x, int y, int width, int height, IMonitor monitor, ITranslationHelper i18n, BankData bankData) : base(x, y, width, height)
        {
            menuX = x;
            menuY = y;
            _monitor = monitor;
            _i18N = i18n;
            _bankData = bankData;
        }

        public override void draw(SpriteBatch b)
        {
             var total = _bankData.LoanedMoney - _bankData.MoneyPaidBack;
            //var y = 64;
            
            //i18n.Get("npc_dialogue"+outty, new { player_name = Game1.player.Name });
            //SpriteText.drawStringHorizontallyCenteredAt(b, _titleLabel, XPos + 600, YPos + 120);
            /*
            SpriteText.drawString(b, _i18N.Get("bank.info.accountNumber", new{ account_number = Game1.player.UniqueMultiplayerID}), menuX + 50, menuY + 200 + (y * 1), 999999, -1, 9999, 0.75f, 0.865f, junimoText: false);
            SpriteText.drawString(b, _i18N.Get("bank.info.accountBalance", new{ account_balance = _bankData.MoneyInBank}), menuX + 50, menuY + 200 + (y * 2), 999999, -1, 9999, 0.75f, 0.865f, junimoText: false);
            SpriteText.drawString(b, _i18N.Get("bank.info.bankInterest", new{ bank_interest = _bankData.BankInterest}), menuX + 50, menuY + 200 + (y * 3), 999999, -1, 9999, 0.75f, 0.865f, junimoText: false);
            SpriteText.drawString(b, _i18N.Get("bank.info.loanBalance", new{ loan_balance = _bankData.LoanedMoney, loan_owed = ModEntry.FormatNumber(total)}), menuX + 50, menuY + 200 + (y * 4), 999999, -1, 9999, 0.75f, 0.865f, junimoText: false);
            SpriteText.drawString(b, _i18N.Get("bank.info.loanPaidBack", new{ loan_paid_back = _bankData.MoneyPaidBack}), menuX + 50, menuY + 200 + (y * 5), 999999, -1, 9999, 0.75f, 0.865f, junimoText: false);
            SpriteText.drawString(b, _i18N.Get("bank.info.loansPaidOff", new{ loans_paid_off = _bankData.NumberOfLoansPaidBack}), menuX + 50, menuY + 200 + (y * 6), 999999, -1, 9999, 0.75f, 0.865f, junimoText: false);
            SpriteText.drawString(b, _i18N.Get("bank.info.totalLoans", new{ total_loans = _bankData.TotalNumberOfLoans}), menuX + 50, menuY + 200 + (y * 7), 999999, -1, 9999, 0.75f, 0.865f, junimoText: false);
            SpriteText.drawString(b, _i18N.Get("bank.info.loanInterest", new{ loan_interest = _bankData.LoanInterest}), menuX + 50, menuY + 200 + (y * 8), 999999, -1, 9999, 0.75f, 0.865f, junimoText: false);
            */
             //Lets draw the bank info to the page.
             
            Utility.drawBoldText(b, 
                _i18N.Get("bank.info.accountNumber", new{ account_number = Game1.player.UniqueMultiplayerID}), 
                Game1.dialogueFont, 
                new Vector2(menuX + 50, menuY + 200), 
                Game1.textColor);
            
            Utility.drawBoldText(b, 
                _i18N.Get("bank.info.accountBalance", new{ account_balance = ModEntry.FormatNumber(_bankData.MoneyInBank)}), 
                Game1.dialogueFont, 
                new Vector2(menuX + 50, menuY + 200 + 64), 
                Game1.textColor);
            
            Utility.drawBoldText(b, 
                _i18N.Get("bank.info.bankInterest", new{ bank_interest = _bankData.BankInterest}), 
                Game1.dialogueFont, 
                new Vector2(menuX + 50, menuY + 200  + 128), 
                Game1.textColor);
            
            Utility.drawBoldText(b, 
                _i18N.Get("bank.info.loanBalance", new{ loan_balance = ModEntry.FormatNumber(_bankData.LoanedMoney), loan_owed = ModEntry.FormatNumber(total)}), 
                Game1.dialogueFont, 
                new Vector2(menuX + 50, menuY + 200 + 192), 
                Game1.textColor);
            
            Utility.drawBoldText(b, 
                _i18N.Get("bank.info.loanPaidBack", new{ loan_paid_back = ModEntry.FormatNumber(_bankData.MoneyPaidBack)}), 
                Game1.dialogueFont, 
                new Vector2(menuX + 50, menuY + 200 + 256), 
                Game1.textColor);
            
            Utility.drawBoldText(b, 
                _i18N.Get("bank.info.loansPaidOff", new{ loans_paid_off = _bankData.NumberOfLoansPaidBack}), 
                Game1.dialogueFont, 
                new Vector2(menuX + 50, menuY + 200 + 320), 
                Game1.textColor);
            
            Utility.drawBoldText(b, 
                _i18N.Get("bank.info.totalLoans", new{ total_loans = _bankData.TotalNumberOfLoans}), 
                Game1.dialogueFont, 
                new Vector2(menuX + 50, menuY + 200 + 384), 
                Game1.textColor);
            
            Utility.drawBoldText(b, 
                _i18N.Get("bank.info.loanInterest", new{ loan_interest = _bankData.LoanInterest}), 
                Game1.dialogueFont, 
                new Vector2(menuX + 50, menuY + 200 + 448), 
                Game1.textColor);
            
            //b.DrawString(Game1.dialogueFont, "This is just a test", new Vector2(menuX + 50, menuY + 200 + 512), Game1.textColor);
            drawMouse(b);
        }
    }
}