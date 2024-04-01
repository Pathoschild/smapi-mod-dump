/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace StardewAquarium.Menus
{
    public class DonateFishMenu : InventoryMenu
    {
        private readonly IModHelper _helper;
        private readonly IMonitor _monitor;
        private bool _donated;
        private bool _pufferchickDonated;

        private static int PufferChickID { get => ModEntry.JsonAssets?.GetObjectId(ModEntry.PufferChickName) ?? -1; }

        public DonateFishMenu(IModHelper translate, IMonitor monitor) : base(Game1.viewport.Width / 2 - 768 / 2, Game1.viewport.Height / 2 + 36, true, null, Utils.IsUnDonatedFish, 36, 3)
        {
            this.showGrayedOutSlots = true;
            this._helper = translate;
            this._monitor = monitor;
            this.exitFunction = () => Utils.DonationMenuExit(this._donated, this._pufferchickDonated);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            var item = this.getItemAt(x, y);
            if (!Utils.IsUnDonatedFish(item))
                return;

            if (Utils.DonateFish(item))
            {
                this._donated = true;
                Game1.playSound("newArtifact");
                item.Stack--;
                if (item.Stack == 0)
                    Game1.player.removeItemFromInventory(item);

                if (item.ParentSheetIndex == PufferChickID)
                {
                    Game1.playSound("openChest");
                    this._pufferchickDonated = true;
                }

                var mp = this._helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
                mp.globalChatInfoMessage("StardewAquarium.FishDonated", new[] { Game1.player.Name, item.Name });
            }
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);
            if (!Game1.options.showMenuBackground)
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
            else
                base.drawBackground(b);

            string title = this._helper.Translation.Get("DonationMenuTitle");
            SpriteText.drawStringWithScrollCenteredAt(b, title, Game1.viewport.Width / 2,
                Game1.viewport.Height / 2 - 128, title, 1f, -1, 0, 0.88f, false);

            Game1.drawDialogueBox(this.xPositionOnScreen - 64, this.yPositionOnScreen - 160, this.width + 128, this.height + 192, false, true);

            base.draw(b);
            this.drawMouse(b);
        }
    }
}
