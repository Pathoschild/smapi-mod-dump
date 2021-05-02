/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace Omegasis.SaveAnywhere.Framework
{
    /// <summary>A subclass of <see cref="ShippingMenu"/> that does everything except save.</summary>
    internal class NewShippingMenu : ShippingMenu
    {
        /*********
        ** Fields
        *********/
        /// <summary>The private field on the shipping menu which indicates the game has already been saved, which prevents it from saving.</summary>
        private readonly IReflectedField<bool> SavedYet;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="items">The shipping bin items.</param>
        /// <param name="reflection">Simplifies access to game code.</param>
        public NewShippingMenu(NetCollection<Item> items, IReflectionHelper reflection)
            : base(items)
        {
            this.SavedYet = reflection.GetField<bool>(this, "savedYet");


            NetCollection<Item> shippingBin = Game1.getFarm().getShippingBin(Game1.player);
            if (Game1.player.useSeparateWallets || !Game1.player.useSeparateWallets && Game1.player.IsMainPlayer)
            {
                int num = 0;
                foreach (Item obj in shippingBin)
                {
                    if (obj is Object)
                        num += (obj as Object).sellToStorePrice(-1L) * obj.Stack;
                }
                Game1.player.Money += num;
                Game1.getFarm().getShippingBin(Game1.player).Clear();
            }

            if (Game1.player.useSeparateWallets && Game1.player.IsMainPlayer)
            {
                foreach (Farmer allFarmhand in Game1.getAllFarmhands())
                {
                    if (!allFarmhand.isActive() && !allFarmhand.isUnclaimedFarmhand)
                    {
                        int num = 0;
                        foreach (Item obj in Game1.getFarm().getShippingBin(allFarmhand))
                        {
                            if (obj is Object)
                                num += (obj as Object).sellToStorePrice(allFarmhand.UniqueMultiplayerID) * obj.Stack;
                        }
                        Game1.player.team.AddIndividualMoney(allFarmhand, num);
                        Game1.getFarm().getShippingBin(allFarmhand).Clear();
                    }
                }
            }


        }

        /// <summary>Overrides some base functionality of the shipping menu to enable proper closing.</summary>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (this.okButton.containsPoint(x, y)) this.exitThisMenu(true);
            base.receiveLeftClick(x, y, playSound);
        }

        /// <summary>Updates the menu during the game's update loop.</summary>
        /// <param name="time">The game time that has passed.</param>
        public override void update(GameTime time)
        {
            this.SavedYet.SetValue(true); // prevent menu from saving
            base.update(time);
        }
    }
}
