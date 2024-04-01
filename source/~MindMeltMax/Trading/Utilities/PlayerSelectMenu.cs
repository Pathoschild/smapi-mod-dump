/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Trading.Utilities
{
    internal class PlayerSelectMenu : IClickableMenu
    {
        private const int BaseIdPlayerButton = 44000;
        private const int IdCloseButton = 50004;

        private Vector2 BasePositionPlayerButtons => new(X + (Width / 2) - 96, Y + (Height / 2) - 32);

        private Farmer _sender;
        private Dictionary<Farmer, bool> _nearbyFarmers;
        private List<ClickableComponent> _buttons;
        private int _timeSinceLastPoll;
        private KeyValuePair<int, Rectangle>? _hovered;
        private string _hoverText;

        public Farmer Sender
        {
            get => _sender;
            set => _sender = value;
        }
        public Dictionary<Farmer, bool> NearbyFarmers
        {
            get => _nearbyFarmers;
            set => _nearbyFarmers = value;
        }

        public int X
        {
            get => xPositionOnScreen;
            set => xPositionOnScreen = value;
        }
        public int Y
        {
            get => yPositionOnScreen;
            set => yPositionOnScreen = value;
        }
        public int Width
        {
            get => width;
            set => width = value;
        }
        public int Height
        {
            get => height;
            set => height = value;
        }

        public PlayerSelectMenu(Farmer sender)
        {
            _sender = sender;
            _nearbyFarmers = new();

            var farmers = ModEntry.IConfig.Global ? Game1.getOnlineFarmers() : sender.currentLocation.farmers;
            foreach (var farmer in farmers.Where(x => x.UniqueMultiplayerID != sender.UniqueMultiplayerID))
                _nearbyFarmers.Add(farmer, false);

            loadViewComponents();
        }

        public PlayerSelectMenu(Farmer sender, Dictionary<Farmer, bool> nearbyFarmers)
        {
            _sender = sender;
            _nearbyFarmers = nearbyFarmers;

            loadViewComponents();
        }

        private void loadViewComponents()
        {
            _buttons = new();

            Width = 400 + borderWidth * 2;
            Height = 250 + borderWidth * 2;

            X = Game1.uiViewport.Width / 2 - (Width / 2);
            Y = Game1.uiViewport.Height / 2 - (Height / 2);

            initializeUpperRightCloseButton();
            upperRightCloseButton.bounds.X = X + Width - 40;
            upperRightCloseButton.bounds.Y = Y + 60;
            upperRightCloseButton.myID = IdCloseButton;

            reloadButtons();
            upperRightCloseButton.downNeighborID = upperRightCloseButton.leftNeighborID = _buttons.Count > 1 ? _buttons[1].myID : (!_buttons.Any() ? -7777 : _buttons[0].myID);
            upperRightCloseButton.upNeighborID = upperRightCloseButton.rightNeighborID = -7777;
        }

        private void reloadButtons()
        {
            int counter = 0;
            foreach (var farmer in _nearbyFarmers)
            {
                _buttons.Add(new(new((int)(BasePositionPlayerButtons.X + (96 * (counter % 2))), (int)(BasePositionPlayerButtons.Y + (96 * (counter / 2))), 64, 64), $"{farmer.Key.UniqueMultiplayerID}")
                {
                    myID = BaseIdPlayerButton + counter,
                    leftNeighborID = counter == 0 ? -7777 : BaseIdPlayerButton + counter - 1,
                    rightNeighborID = counter == _nearbyFarmers.Count - 1 ? -7777 : BaseIdPlayerButton + counter + 1,
                    upNeighborID = counter - 2 < 0 ? upperRightCloseButton.myID : BaseIdPlayerButton + counter - 2,
                    downNeighborID = counter + 2 >= _nearbyFarmers.Count ? -7777 : BaseIdPlayerButton + counter + 2
                });
                ++counter;
            }
        }

        private void assignIds()
        {
            upperRightCloseButton.myID = IdCloseButton;

            for (int i = 0; i < _buttons.Count; i++)
                _buttons[i].myID = BaseIdPlayerButton + i;

            for (int i = 0; i < _buttons.Count; i++) 
            {
                if (i % 2 == 0)
                {
                    _buttons[i].rightNeighborID = i + 1 >= _buttons.Count ? -7777 : _buttons[i + 1].myID;
                    if (i / 2 > 0)
                        _buttons[i].leftNeighborID = _buttons[i - 1].myID;
                    else
                        _buttons[i].leftNeighborID = -7777;
                }
                else
                {
                    _buttons[i].leftNeighborID = i - 1 < 0 ? -7777 : _buttons[i - 1].myID;
                    if (i % 2 == 0 || _buttons.Count < i + 1)
                        _buttons[i].rightNeighborID = _buttons[i + 1].myID;
                    else
                        _buttons[i].rightNeighborID = -7777;
                }

                if (i / 2 == 0)
                {
                    _buttons[i].upNeighborID = IdCloseButton;
                    if (i + 2 >= _buttons.Count)
                        _buttons[i].downNeighborID = -7777;
                    else
                        _buttons[i].downNeighborID = _buttons[i + 2].myID;
                }
                else
                {
                    _buttons[i].upNeighborID = _buttons[i - 2].myID;
                    if (i + 2 >= _buttons.Count)
                        _buttons[i].downNeighborID = -7777;
                    else
                        _buttons[i].downNeighborID = _buttons[i + 2].myID;
                }
            }

            upperRightCloseButton.downNeighborID = upperRightCloseButton.leftNeighborID = _buttons.Count > 1 ? _buttons[1].myID : (!_buttons.Any() ? -7777 : _buttons[0].myID);
            upperRightCloseButton.upNeighborID = upperRightCloseButton.rightNeighborID = -7777;
        }

        private void getClickableComponentList()
        {
            allClickableComponents = new();
            allClickableComponents.AddRange(_buttons);
            allClickableComponents.Add(upperRightCloseButton);
        }

        private ClickableComponent getComponentWithId(int id)
        {
            getClickableComponentList();
            for (int i = 0; i < allClickableComponents.Count; i++)
                if (allClickableComponents[i].myID == id || allClickableComponents[i].myAlternateID == id)
                    return allClickableComponents[i];
            return null;
        }

        public override void snapToDefaultClickableComponent() => currentlySnappedComponent = getComponentWithId(_buttons.Any() ? _buttons[0].myID : upperRightCloseButton.myID);

        public override void setCurrentlySnappedComponentTo(int id)
        {
            currentlySnappedComponent = getComponentWithId(id);
            if (currentlySnappedComponent == null)
            {
                snapToDefaultClickableComponent();
                ModEntry.IMonitor.Log($"Couldn't snap to component with id : {id}, Snapping to default", LogLevel.Warn);
            }
            Game1.playSound("smallSelect");
        }

        public override void setUpForGamePadMode()
        {
            snapToDefaultClickableComponent();
            snapCursorToCurrentSnappedComponent();
        }

        public override void applyMovementKey(int direction)
        {
            if (currentlySnappedComponent == null) snapToDefaultClickableComponent();
            switch (direction)
            {
                case 0: //Up
                    if (currentlySnappedComponent!.upNeighborID < 0) break;
                    setCurrentlySnappedComponentTo(currentlySnappedComponent.upNeighborID);
                    snapCursorToCurrentSnappedComponent();
                    break;
                case 1: //Right
                    if (currentlySnappedComponent!.rightNeighborID < 0) break;
                    setCurrentlySnappedComponentTo(currentlySnappedComponent.rightNeighborID);
                    snapCursorToCurrentSnappedComponent();
                    break;
                case 2: //Down
                    if (currentlySnappedComponent!.downNeighborID < 0) break;
                    setCurrentlySnappedComponentTo(currentlySnappedComponent.downNeighborID);
                    snapCursorToCurrentSnappedComponent();
                    break;
                case 3: //Left
                    if (currentlySnappedComponent!.leftNeighborID < 0) break;
                    setCurrentlySnappedComponentTo(currentlySnappedComponent.leftNeighborID);
                    snapCursorToCurrentSnappedComponent();
                    break;
                default:
                    base.applyMovementKey(direction);
                    break;
            }
        }

        public override void receiveGamePadButton(Buttons b)
        {
            switch (b)
            {
                case Buttons.Back:
                case Buttons.B:
                case Buttons.Y:
                    exitThisMenu();
                    break;
                case Buttons.A:
                    if (currentlySnappedComponent is not null && currentlySnappedComponent.myID == IdCloseButton)
                        goto case Buttons.B;
                    break;
            }

            base.receiveGamePadButton(b);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (upperRightCloseButton.containsPoint(x, y))
            {
                exitThisMenu(playSound);
                return;
            }

            for (int i = 0; i < _buttons.Count; i++)
            {
                if (_buttons[i].containsPoint(x, y))
                {
                    var farmer = _nearbyFarmers.ElementAtOrDefault(i);
                    if (farmer.Key is null || !farmer.Value)
                        break;
                    var sPlayer = ModEntry.IHelper.Multiplayer.GetConnectedPlayer(farmer.Key.UniqueMultiplayerID);
                    if (sPlayer != null && sPlayer.HasSmapi && sPlayer.Mods.Any(x => x.ID == ModEntry.IHelper.ModRegistry.ModID))
                    {
                        ModEntry.IHelper.Multiplayer.SendMessage((NetworkPlayer)Game1.player, MSG_RequestTrade, ModEntry.ModId, new[] { farmer.Key.UniqueMultiplayerID });
                        Game1.activeClickableMenu = new TradeMenu(Game1.player, farmer.Key, true);
                        break;
                    }
                    return;
                }
            }
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            if (_hovered is not null && _buttons.Count > _hovered.Value.Key)
            {
                _buttons[_hovered.Value.Key].bounds = _hovered.Value.Value;
                if (!_buttons[_hovered.Value.Key].containsPoint(x, y))
                {
                    _hovered = null;
                    _hoverText = "";
                }
            }
            for (int i = 0; i < _buttons.Count; i++)
            {
                if (_buttons[i].containsPoint(x, y))
                {
                    _hovered = new(i, _buttons[i].bounds);
                    _hoverText = Game1.getFarmer(Convert.ToInt64(_buttons[i].name)).Name;
                    _buttons[i].bounds = new(_buttons[i].bounds.X - 2, _buttons[i].bounds.Y - 2, _buttons[i].bounds.Width + 4, _buttons[i].bounds.Height + 4);
                }
            }
        }

        public override void update(GameTime time)
        {
            base.update(time);

            if (_timeSinceLastPoll >= 60)
            {
                if (NearbyFarmers.Count != (ModEntry.IConfig.Global ? Game1.getOnlineFarmers() : Sender.currentLocation.farmers).Where(x => x.UniqueMultiplayerID != Sender.UniqueMultiplayerID).Count())
                {
                    NearbyFarmers.Clear();
                    foreach (var farmer in (ModEntry.IConfig.Global ? Game1.getOnlineFarmers() : Sender.currentLocation.farmers).Where(x => x.UniqueMultiplayerID != Sender.UniqueMultiplayerID))
                        NearbyFarmers.Add(farmer, false);
                    reloadButtons();
                    assignIds();
                }

                _timeSinceLastPoll = 0;
                ModEntry.IHelper.Multiplayer.SendMessage("", MSG_PollStatus, ModEntry.ModId, NearbyFarmers.Select(x => x.Key.UniqueMultiplayerID).ToArray());
            }
            else _timeSinceLastPoll++;
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.6f);
            Game1.drawDialogueBox(X, Y, Width, Height, false, true);
            upperRightCloseButton.draw(b);

            if (!_buttons.Any())
            {
                string text = "No players to trade with";
                Vector2 measure = Game1.smallFont.MeasureString(text);
                b.DrawString(Game1.smallFont, text, new(X + (Width / 2) - (measure.X / 2), Y + (Height / 2)), Game1.textColor);
            }

            for (int i = 0; i < _buttons.Count; i++) 
            {
                Vector2 portraitOffset = Vector2.Zero;
                var farmer = _nearbyFarmers.ElementAtOrDefault(i);
                if (farmer.Key is null)
                    continue;
                if (_hovered is not null && _hovered.Value.Key == i)
                    portraitOffset = new(2);
                b.Draw(Game1.mouseCursors, _buttons[i].bounds, new(256, 256, 10, 10), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.88f);
                farmer.Key.FarmerRenderer.drawMiniPortrat(b, new Vector2(_buttons[i].bounds.X + 2, _buttons[i].bounds.Y - 2) + portraitOffset, 0.89f, 3.8f, 0, farmer.Key);
                if (!farmer.Value)
                    b.Draw(Game1.fadeToBlackRect, new Rectangle(_buttons[i].bounds.X + 6, _buttons[i].bounds.Y + 6, _buttons[i].bounds.Width - 12, _buttons[i].bounds.Height - 12), Color.Black * 0.6f);
            }

            if (!string.IsNullOrWhiteSpace(_hoverText))
                drawHoverText(b, _hoverText, Game1.smallFont);

            drawMouse(b);
        }
    }
}
