using System;
using System.Linq;
using StardewModdingAPI;
using TehPers.CoreMod.Api.ContentPacks;
using TehPers.CoreMod.Api.ContentPacks.Tokens;
using TehPers.CoreMod.Api.Environment;
using TehPers.CoreMod.Api.Extensions;
using TehPers.CoreMod.Api.Structs;

namespace TehPers.CoreMod.ContentPacks.Tokens {
    internal class SeasonToken : IToken {
        private Season _lastSeason = Season.None;

        public SeasonToken(IMod mod) {
            mod.Helper.Events.GameLoop.DayStarted += (sender, args) => this.CheckSeason();
        }

        private void CheckSeason() {
            Season season = SDateTime.Today.Season;

            if (season != this._lastSeason) {
                this._lastSeason = season;
                this.Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        public string GetValue(ITokenHelper helper, string[] arguments) {
            if (arguments.Any()) {
                throw new ArgumentException("Season token doesn't accept any arguments.");
            }

            return Context.IsWorldReady ? SDateTime.Today.Season.GetName() : "null";
        }
        public bool IsValidInContext(IContext context) {
            return context.CanChange;
        }

        public event EventHandler Changed;
    }
}