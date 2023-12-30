/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Linq;
using System.Collections.Generic;
using StardewValley;
using StardewValley.Tools;

namespace StardewArchipelago.Items.Mail
{
    public class ModLetterActions
    {
        public void AddModLetterActions(Dictionary<string, Action<string>> letterActions)
        {
            letterActions.Add(LetterActionsKeys.DiamondWand, (_) => ReceiveDiamondWand());
        }

        private void ReceiveDiamondWand()
        {
            Game1.playSound("parry");
            var weaponData = Game1.content.Load<Dictionary<int, string>>("Data\\weapons");
            var id = weaponData.FirstOrDefault(x => x.Value.Contains("Diamond Wand"));
            var diamondWand = new MeleeWeapon(id.Key);
            Game1.player.holdUpItemThenMessage(diamondWand);
            Game1.player.addItemByMenuIfNecessary(diamondWand);
        } 
    }
}