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
        ** Properties
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
        }

        /// <summary>
        /// Overrides some base functionality of the shipping menu to enable proper closing.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="playSound"></param>
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
