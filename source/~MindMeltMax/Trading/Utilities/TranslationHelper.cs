/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.Utilities
{
    internal class TranslationHelper
    {
        private ITranslationHelper _translations;

        public string TradeRequest => _translations.Get("RequestMessage");
        public string TradeAccept => _translations.Get("AcceptRequest");
        public string TradeDecline => _translations.Get("DeclineRequest");
        public string TradeDeclined => _translations.Get("RequestDeclined");
        public string PendingResponse => _translations.Get("PendingResponse");

        public string AcceptOffer => _translations.Get("AcceptOffer");
        public string SendOffer => _translations.Get("SendOffer");
        public string ConfirmOffer => _translations.Get("Confirm");

        public TranslationHelper(IModHelper helper) => _translations = helper.Translation;
    }
}
