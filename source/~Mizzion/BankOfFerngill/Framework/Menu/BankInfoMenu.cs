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

namespace BankOfFerngill.Framework.Menu
{
    public class BankInfoMenu : IClickableMenu
    {
        private readonly IMonitor _monitor;
        private readonly ITranslationHelper _i18N;
        private readonly BankData _bankData;

        private const int UiWidth = 1280;
        private const int UiHeight = 760;

        private readonly int _xPos = (int)(Game1.viewport.Width * Game1.options.zoomLevel * (1 / Game1.options.uiScale)) / 2 - 
                                     (UiWidth / 2);

        private readonly int _yPos = (int)(Game1.viewport.Height * Game1.options.zoomLevel * (1 / Game1.options.uiScale)) / 2 -
                                    (UiHeight / 2);

        private readonly ClickableComponent _titleLabel;

        public BankInfoMenu(IMonitor monitor, ITranslationHelper i18N, BankData bankData)
        {
            initialize(_xPos, _yPos, UiWidth, UiHeight);
            
            _monitor = monitor;
            _i18N = i18N; 
            
            _bankData = bankData ?? new BankData();
            
            _titleLabel = new ClickableComponent(new Rectangle(_xPos + 500, _yPos + 96, UiWidth - 400, 128), _i18N.Get("bank.info.title"));
        }
        
        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            Game1.drawDialogueBox(_xPos, _yPos, UiWidth, UiHeight, false, true);
            //Draw Title Label
            
            Utility.drawTextWithShadow(b, _titleLabel.name, Game1.dialogueFont, new Vector2(_titleLabel.bounds.X, _titleLabel.bounds.Y), Color.Black);
            
            var y = 64;
            var total = _bankData.LoanedMoney - _bankData.MoneyPaidBack;
            //i18n.Get("npc_dialogue"+outty, new { player_name = Game1.player.Name });
            //SpriteText.drawStringHorizontallyCenteredAt(b, _titleLabel, XPos + 600, YPos + 120);
            SpriteText.drawString(b, _i18N.Get("bank.info.accountNumber", new{ account_number = Game1.player.UniqueMultiplayerID}), _xPos + 45, _titleLabel.bounds.Y + (y * 1), 999999, -1, 9999, 0.75f, 0.865f, junimoText: false);
            SpriteText.drawString(b, _i18N.Get("bank.info.accountBalance", new{ account_balance = _bankData.MoneyInBank}), _xPos + 45, _titleLabel.bounds.Y + (y * 2), 999999, -1, 9999, 0.75f, 0.865f, junimoText: false);
            SpriteText.drawString(b, _i18N.Get("bank.info.bankInterest", new{ bank_interest = _bankData.BankInterest}), _xPos + 45, _titleLabel.bounds.Y + (y * 3), 999999, -1, 9999, 0.75f, 0.865f, junimoText: false);
            SpriteText.drawString(b, _i18N.Get("bank.info.loanBalance", new{ loan_balance = _bankData.LoanedMoney, loan_owed = ModEntry.FormatNumber(total)}), _xPos + 45, _titleLabel.bounds.Y + (y * 4), 999999, -1, 9999, 0.75f, 0.865f, junimoText: false);
            SpriteText.drawString(b, _i18N.Get("bank.info.loanPaidBack", new{ loan_paid_back = _bankData.MoneyPaidBack}), _xPos + 45, _titleLabel.bounds.Y + (y * 5), 999999, -1, 9999, 0.75f, 0.865f, junimoText: false);
            SpriteText.drawString(b, _i18N.Get("bank.info.loansPaidOff", new{ loans_paid_off = _bankData.NumberOfLoansPaidBack}), _xPos + 45, _titleLabel.bounds.Y + (y * 6), 999999, -1, 9999, 0.75f, 0.865f, junimoText: false);
            SpriteText.drawString(b, _i18N.Get("bank.info.totalLoans", new{ total_loans = _bankData.TotalNumberOfLoans}), _xPos + 45, _titleLabel.bounds.Y + (y * 7), 999999, -1, 9999, 0.75f, 0.865f, junimoText: false);
            SpriteText.drawString(b, _i18N.Get("bank.info.loanInterest", new{ loan_interest = _bankData.LoanInterest}), _xPos + 45, _titleLabel.bounds.Y + (y * 8), 999999, -1, 9999, 0.75f, 0.865f, junimoText: false);
            
            
            //Draw the mouse
            drawMouse(b);
        }

        
    }
}