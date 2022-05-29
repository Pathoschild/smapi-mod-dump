/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CJBok/SDV-Mods
**
*************************************************/

using System.Collections.Generic;
using CJBCheatsMenu.Framework.Components;
using StardewValley;
using StardewValley.Menus;

namespace CJBCheatsMenu.Framework.Cheats.PlayerAndTools
{
    /// <summary>A cheat which adds various amounts of money to the player.</summary>
    internal class AddMoneyCheat : BaseCheat
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Get the config UI fields to show in the cheats menu.</summary>
        /// <param name="context">The cheat context.</param>
        public override IEnumerable<OptionsElement> GetFields(CheatContext context)
        {
            foreach (int amount in new[] { 100, 1000, 10000, 100000 })
            {
                yield return new CheatsOptionsButton(
                    label: I18n.Add_AmountGold(amount: amount),
                    slotWidth: context.SlotWidth,
                    toggle: () => this.AddMoney(amount)
                );
            }
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Add an amount to the player money.</summary>
        /// <param name="amount">The amount to add.</param>
        private void AddMoney(int amount)
        {
            Game1.player.Money += amount;
            Game1.playSound("coin");
        }
    }
}
