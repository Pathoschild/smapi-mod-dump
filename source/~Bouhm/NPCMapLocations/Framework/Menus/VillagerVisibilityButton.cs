/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Bouhm/stardew-valley-mods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NPCMapLocations.Framework.Models;
using StardewValley;
using StardewValley.Menus;

namespace NPCMapLocations.Framework.Menus
{
    // Mod button for the three main modes
    internal class VillagerVisibilityButton : OptionsElement
    {
        /*********
        ** Fields
        *********/
        private bool IsActive;

        /// <summary>Get the current option value.</summary>
        private readonly Func<VillagerVisibility> GetValue;

        /// <summary>Set the new option value.</summary>
        private readonly Action<VillagerVisibility> SetValue;


        /*********
        ** Accessors
        *********/
        public Rectangle Rect { get; set; }

        /// <summary>The value selected by this button.</summary>
        public VillagerVisibility Id { get; }


        /*********
        ** Public methods
        *********/
        public VillagerVisibilityButton(string label, VillagerVisibility id, Func<VillagerVisibility> get, Action<VillagerVisibility> set)
            : base(label, -1, -1, 9 * Game1.pixelZoom, 9 * Game1.pixelZoom, 1)
        {
            this.Id = id;
            this.GetValue = get;
            this.SetValue = set;
        }

        public override void receiveLeftClick(int x, int y)
        {
            if (!this.IsActive)
            {
                Game1.playSound("drumkit6");
                base.receiveLeftClick(x, y);

                this.SetValue(this.Id);
            }
        }

        public void UpdateState()
        {
            bool shouldBeActive = this.GetValue() == this.Id;

            this.IsActive = shouldBeActive;
            this.greyedOut = !shouldBeActive;
        }

        public override void draw(SpriteBatch b, int slotX, int slotY, IClickableMenu context = null)
        {
            this.UpdateState();

            base.draw(b, slotX - 32, slotY, context);
        }
    }
}
