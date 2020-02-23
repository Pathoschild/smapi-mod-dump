using System;
using System.Globalization;
using System.Linq;
using LeFauxMatt.CustomChores.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LeFauxMatt.HelpForHire.Models
{
    internal class ChoreHandler
    {
        internal bool IsPurchased { get; set; }
        internal int Price { get; }
        public string ChoreName => Chore.ChoreName;
        public string DisplayName => _displayName.Tokens(HelpForHireMod.CustomChoresApi.GetChoreTokens(ChoreName));
        public string Description => _description.Tokens(HelpForHireMod.CustomChoresApi.GetChoreTokens(ChoreName));
        public int EstimatedCost => ModConfig.Instance.PayPerUnit ? Price * WorkNeeded : Price;
        public int ActualCost => ModConfig.Instance.PayPerUnit ? Price * WorkDone : Price;
        public int WorkNeeded => Convert.ToInt32(_workNeeded.Invoke(), CultureInfo.InvariantCulture);
        public int WorkDone => Convert.ToInt32(_workDone.Invoke(), CultureInfo.InvariantCulture);
        public int ImageWidth => Chore.Image.Width;
        public int ImageHeight => Chore.Image.Height;
        private ChoreData Chore { get; }
        private readonly TranslationData _displayName;
        private readonly TranslationData _description;
        private readonly Func<string> _workNeeded;
        private readonly Func<string> _workDone;

        internal ChoreHandler(ChoreData chore, int price)
        {
            Chore = chore;
            Price = price;

            var choreTokens = HelpForHireMod.CustomChoresApi.GetChoreTokens(chore.ChoreName);
            choreTokens.Add("Mod", () => "HelpForHire");

            // get display name
            _displayName = (
                from translation in chore.Translations
                where translation.Key.Equals("DisplayName", StringComparison.CurrentCultureIgnoreCase)
                      && translation.Filter(choreTokens)
                select translation)?.First();

            // get description
            _description = (
                from translation in chore.Translations
                where translation.Key.Equals("Description", StringComparison.CurrentCultureIgnoreCase)
                      && translation.Filter(choreTokens)
                select translation)?.First();

            // get work needed token
            _workNeeded = (choreTokens.TryGetValue("WorkNeeded", out var workNeededFn)) ? workNeededFn : () => "1";

            // get work done token
            _workDone = (choreTokens.TryGetValue("WorkDone", out var workDoneFn)) ? workDoneFn : () => "1";
        }

        public virtual void DrawInMenu(SpriteBatch b, int x, int y)
        {
            b.Draw(Chore.Image,
                new Vector2(x - ImageWidth / 2, y - ImageHeight / 2),
                new Rectangle(0, 0, ImageWidth, ImageHeight),
                Color.White);
        }
    }
}