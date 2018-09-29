using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoreMultiplayerInfo.EventHandlers;
using MoreMultiplayerInfo.Helpers;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;

namespace MoreMultiplayerInfo
{
    public class PlayerInformationMenu : IClickableMenu
    {
        public long PlayerId;

        private readonly IModHelper _helper;

        private readonly ModConfigOptions _configOptions;

        private Farmer Player => PlayerHelpers.GetPlayerWithUniqueId(PlayerId);

        private InventoryMenu _inventory;

        private PlayerSkillInfo _skillInfo;
        private PlayerEquipmentInfo _equipmentInfo;
        private PlayerHealthInfo _healthBar;
        private ClickableTextureComponent _optionsIcon;

        private Vector2 _locationPos;

        private Vector2 _lastActionPos;
        private Vector2 _lastActionTimePos;

        private static int Width => 850;
        private static int Height => 580;

        private static int Xposition => (Game1.viewport.Width / 2) - (Width / 2);
        private static int Yposition => (Game1.viewport.Height / 2) - (Height / 2);

        private static Item HoveredItem { get; set; }

        private static string HoverText { get; set; }

        private static int GenericHeightSpacing => 25;

        public PlayerInformationMenu(long playerUniqueMultiplayerId, IModHelper helper) : base(Xposition, Yposition,
            Width, Height, true)
        {
            PlayerId = playerUniqueMultiplayerId;
            _helper = helper;

            GraphicsEvents.Resize += Resize;

                _configOptions = ConfigHelper.GetOptions();

            if (!_configOptions.ShowInventory)
            {
                this.height -= 200;
            }
        }

        private void Resize(object sender, EventArgs e)
        {
            this.xPositionOnScreen = Xposition;
            this.yPositionOnScreen = Yposition;
        }

        public override void draw(SpriteBatch b)
        {
            Game1.mouseCursor = 0;

            DrawBackground(b);

            DrawTitle(b);

            DrawInventory(b);

            DrawLocationInfo(b);

            DrawSkills(b);

            /* DrawEquipment(b); */

            /* DrawHealth(b); */

            DrawLastActionInfo(b);

            DrawHoverText(b);

            DrawOptionsIcon(b);

            drawMouse(b);
        }

        private void DrawOptionsIcon(SpriteBatch b)
        {
            var zoom = (int) (Game1.pixelZoom * 0.75f);

            _optionsIcon = new ClickableTextureComponent("", new Rectangle(xPositionOnScreen + 25, yPositionOnScreen + 25, 17 * zoom, 17 * zoom), "", "", Game1.mouseCursors, new Rectangle(162, 440, 17, 17), zoom, false);

            _optionsIcon.draw(b);
        }



        private void DrawHealth(SpriteBatch b)
        {
            _healthBar = new PlayerHealthInfo(Player.health, Player.maxHealth, (int) Player.Stamina, Player.maxStamina,
                new Rectangle(_skillInfo.xPositionOnScreen + _skillInfo.Width, _skillInfo.yPositionOnScreen, 45, 11));
            _healthBar.draw(b);
        }

        private void DrawSkills(SpriteBatch b)
        {
            _skillInfo = new PlayerSkillInfo(Player,
                new Vector2(_inventory.xPositionOnScreen,
                    _inventory.yPositionOnScreen + _inventory.height + GenericHeightSpacing));
            _skillInfo.draw(b);
        }

        private void DrawLocationInfo(SpriteBatch b)
        {
            

            var yPos = _inventory.yPositionOnScreen + _inventory.height + GenericHeightSpacing;

            var text = $"Location: {LocationHelper.GetFriendlyLocationName(Player.currentLocation.Name)} ";

            if (PlayerHelpers.IsPlayerOffline(PlayerId))
            {
                text = "(Offline)";
            }

            var font = Game1.smallFont;

            var textWidth = font.MeasureString(text).X;

            var xPos = _inventory.xPositionOnScreen + (_inventory.width * 3 / 4) - (textWidth / 2);

            _locationPos = new Vector2(xPos, yPos);

            b.DrawString(font, text, _locationPos, Color.Black);
        }

        private void DrawLastActionInfo(SpriteBatch b)
        {
            if (PlayerHelpers.IsPlayerOffline(PlayerId)) return;

            var font = Game1.smallFont;

            var yPos = _locationPos.Y + font.LineSpacing + GenericHeightSpacing;
            var action = PlayerStateWatcher.GetLastActionForPlayer(PlayerId);

            var text = action.GetDisplayText();

            var textWidth = font.MeasureString(text).X;

            var xPos = _inventory.xPositionOnScreen + (_inventory.width * 3 / 4);

            _lastActionPos = new Vector2(xPos - (textWidth / 2), yPos);

            b.DrawString(font, text, _lastActionPos, Color.Black);

            var lastActionTimeText = action.GetWhenDisplay();

            var timeTextWidth = font.MeasureString(lastActionTimeText).X;

            yPos += font.LineSpacing;

            _lastActionTimePos = new Vector2(xPos - (timeTextWidth / 2), yPos);

            b.DrawString(font, lastActionTimeText, _lastActionTimePos, Color.Black);

        }

        private void DrawTitle(SpriteBatch b)
        {
            var text = $"{Player.Name}'s Info";

            var font = Game1.dialogueFont;

            var titleWidth = font.MeasureString(text).X;

            var xPos = xPositionOnScreen + (Width / 2) - (titleWidth / 2);

            b.DrawString(font, text, new Vector2(xPos, this.yPositionOnScreen + 25), Color.Black);
        }

        private void DrawBackground(SpriteBatch b)
        {
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18),
                this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, Color.White, Game1.pixelZoom);

            this.upperRightCloseButton.draw(b);
        }

        private void DrawHoverText(SpriteBatch b)
        {
            if (!string.IsNullOrEmpty(HoverText))
            {
                IClickableMenu.drawToolTip(b, HoveredItem?.getDescription() ?? HoverText, HoverText, HoveredItem);
            }
        }


        private void DrawInventory(SpriteBatch b)
        {
            _inventory = new InventoryMenu(this.xPositionOnScreen + 38, this.yPositionOnScreen + 100, false,
                Player.Items)
            {
                showGrayedOutSlots = true
            };

            if (_configOptions.ShowInventory)
            {
                _inventory.draw(b);
            }
            else
            {
                _inventory.height = 0;
            }

        }

        public override void performHoverAction(int x, int y)
        {
            Game1.mouseCursor = 0;

            HoveredItem = null;
            HoverText = string.Empty;

            if (_inventory != null && _inventory.isWithinBounds(x, y))
            {
                SetHoverTextFromInventory(x, y);
            }

            if (_optionsIcon != null && _optionsIcon.containsPoint(x, y))
            {
                HoverText = "Configure Mod Settings";
                Game1.mouseCursor = 9;
            }

            base.performHoverAction(x, y);
        }

        private void SetHoverTextFromInventory(int x, int y)
        {
            foreach (ClickableComponent c in _inventory.inventory)
            {
                if (c != null && c.containsPoint(x, y))
                {
                    var item = _inventory.getItemFromClickableComponent(c);
                    if (item == null)
                        continue;
                    HoveredItem = item;
                    HoverText = $"{item.Name}";
                }
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (this.upperRightCloseButton.containsPoint(x, y))
            {
                UnloadMenu(playSound);
            }

            if (this._optionsIcon.containsPoint(x, y))
            {
                var optionsMenu = new OptionsMenu<ModConfigOptions>(_helper, 500, 425, PlayerId, ConfigHelper.GetOptions());

                Game1.activeClickableMenu = optionsMenu;
            }


            base.receiveLeftClick(x, y, playSound);
        }

        private void UnloadMenu(bool playSound)
        {
            this.exitFunction = new IClickableMenu.onExit(() =>
            {
                // do something here
            });

            this.exitThisMenu(playSound);

            Game1.onScreenMenus.Remove(this);
            GraphicsEvents.Resize -= Resize;
        }
        

        
    }
}
