/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using Archipelago.MultiClient.Net.MessageLog.Messages;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archipelago.MultiClient.Net;
using Microsoft.Xna.Framework;

namespace StardewArchipelago.Archipelago
{
    public class HintHelper
    {
        private bool _canAffordHintYesterday;

        public HintHelper()
        {
            _canAffordHintYesterday = false;
        }

        public void GiveHintTip(ArchipelagoSession session)
        {
            if (!TryGetCurrentHintCost(session, out var hintCost))
            {
                return;
            }

            var canAffordHintToday = session.RoomState.HintPoints >= hintCost;
            if (!_canAffordHintYesterday && canAffordHintToday)
            {
                Game1.chatBox?.addMessage($"You can now afford a hint. Syntax: '!hint [itemName]'", Color.Gold);
            }

            _canAffordHintYesterday = canAffordHintToday;
        }

        private static bool TryGetCurrentHintCost(ArchipelagoSession session, out int hintCost)
        {
            hintCost = session.RoomState.HintCost;
            if (hintCost <= 0)
            {
                hintCost = (int)Math.Max(0M,
                    session.Locations.AllLocations.Count * 0.01M *
                    session.RoomState.HintCostPercentage);

                if (hintCost <= 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
