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
using System.Collections.Generic;
using StardewValley;
using StardewValley.Tools;
using StardewArchipelago.Stardew;

namespace StardewArchipelago.Items.Mail
{
    public class ModLetterActions
    {
        private StardewItemManager _stardewItemManager;
        
        public ModLetterActions(StardewItemManager stardewItemManager)
        {
            _stardewItemManager = stardewItemManager;
        }
        public void AddModLetterActions(Dictionary<string, Action<string>> letterActions)
        {
            letterActions.Add(LetterActionsKeys.DiamondWand, (_) => ReceiveDiamondWand(_stardewItemManager));
        }

        private void ReceiveDiamondWand(StardewItemManager _stardewItemManager)
        {
            Game1.playSound("parry");
            var diamondWandId = _stardewItemManager.GetWeaponByName("Diamond Wand").Id;
            var diamondWand = new MeleeWeapon(diamondWandId);
            Game1.player.holdUpItemThenMessage(diamondWand);
            Game1.player.addItemByMenuIfNecessary(diamondWand);
        } 
    }
}