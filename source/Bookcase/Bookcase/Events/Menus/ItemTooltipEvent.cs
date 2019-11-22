using StardewValley;
using Microsoft.Xna.Framework.Graphics;

namespace Bookcase.Events {

    /// <summary>
    /// This event is fired every time an item tooltip is drawn. 
    /// 
    /// This event can NOT be canceled!
    /// </summary>
    public class ItemTooltipEvent : Event {

        /// <summary>
        /// The SpriteBatch used to draw the tooltip.
        /// </summary>
        public SpriteBatch Batch { get; private set; }

        /// <summary>
        /// The Item the tooltip is being shown for.
        /// </summary>
        public Item Item { get; private set; }

        private string title;

        /// <summary>
        /// The title of the tooltip.
        /// </summary>
        public string Title {

            get => title;
            set => title = value ?? "";
        }

        private string description;

        /// <summary>
        /// The description of the tootlip.
        /// </summary>
        public string Description {

            get => description;
            set => description = value ?? "";
        }

        /// <summary>
        /// The healing amount to show on the tooltip. Hidden if less than 0.
        /// </summary>
        public int HealAmount { get; set; }

        /// <summary>
        /// The currency symbol to show. Like coins, or festival tokens. Only used if MoneyToShow is greater than -1.
        /// </summary>
        public int CurrencySymbol { get; set; }

        /// <summary>
        /// The amount of money to show at the bottom. Hidden if less than 0.
        /// </summary>
        public int MoneyToShow { get; set; }

        public ItemTooltipEvent(SpriteBatch batch, Item item, string title, string description, int healAmount, int currencySymbol, int moneyToShow) {

            this.Batch = batch;
            this.Item = item;
            this.Title = title;
            this.Description = description;
            this.HealAmount = healAmount;
            this.CurrencySymbol = currencySymbol;
            this.MoneyToShow = moneyToShow;
        }

        /// <summary>
        /// Adds a new line to the description of the item tooltip.
        /// </summary>
        /// <param name="line"></param>
        public void AddLine(string line) {

            this.Description += System.Environment.NewLine + line;
        }
    }
}