/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/MysticalBuildings
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CaveOfMemories.Framework.GameLocations;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CaveOfMemories.Framework.UI
{
    internal class CharacterSelectionMenu : IClickableMenu
    {
        public ClickableTextureComponent forwardButton;
        public ClickableTextureComponent backButton;
        public List<ClickableTextureComponent> availablePortraits = new List<ClickableTextureComponent>();
        public List<NPC> eventableCharacters = new List<NPC>();

        private string _hoverText = "";

        private int _startingRow = 0;
        private int _texturesPerRow = 4;
        private int _maxRows = 2;
        private double _initialClickCooldown = 300;

        private Farmer _farmer;
        private CaveOfMemoriesLocation _caveOfMemories;

        public CharacterSelectionMenu(Farmer who, CaveOfMemoriesLocation caveOfMemories) : base(0, 0, 832, 576, showUpperRightCloseButton: true)
        {
            // Set up menu structure
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.fr)
            {
                base.height += 64;
            }

            Vector2 topLeft = Utility.getTopLeftPositionForCenteringOnScreen(base.width, base.height);
            base.xPositionOnScreen = (int)topLeft.X;
            base.yPositionOnScreen = (int)topLeft.Y;

            _farmer = who;
            _caveOfMemories = caveOfMemories;

            var drawingScale = 4f;
            var widthOffsetScale = 3;
            var sourceRect = new Rectangle(0, 0, Game1.daybg.Width, 96);

            for (int r = 0; r < _maxRows; r++)
            {
                for (int c = 0; c < _texturesPerRow; c++)
                {
                    var componentId = c + r * _texturesPerRow;
                    availablePortraits.Add(new ClickableTextureComponent(new Rectangle(base.xPositionOnScreen + componentId % _texturesPerRow * 64 * widthOffsetScale, base.yPositionOnScreen + sourceRect.Height / 2 + componentId + (r * sourceRect.Height) + (r > 0 ? 128 : 0) - 32, 64 * 4, 64 * 4), null, new Rectangle(), drawingScale, false)
                    {
                        myID = componentId,
                        downNeighborID = componentId + _texturesPerRow,
                        upNeighborID = r >= _texturesPerRow ? componentId - _texturesPerRow : -1,
                        rightNeighborID = c == 5 ? 9997 : componentId + 1,
                        leftNeighborID = c > 0 ? componentId - 1 : 9998
                    });
                }
            }

            backButton = new ClickableTextureComponent(new Rectangle(base.xPositionOnScreen - 64, base.yPositionOnScreen + 8, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f)
            {
                myID = 9998,
                rightNeighborID = 0
            };
            forwardButton = new ClickableTextureComponent(new Rectangle(base.xPositionOnScreen + base.width + 64 - 48, base.yPositionOnScreen + base.height - 48, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f)
            {
                myID = 9997
            };

            // Get the NPCs to display
            List<NPC> characters = new List<NPC>();
            Utility.getAllCharacters(characters);
            foreach (var npc in characters.OrderBy(n => n.displayName))
            {
                if (npc.Portrait is not null && npc.CanSocialize is true)
                {
                    eventableCharacters.Add(npc);
                }
            }

            // Call snap functions
            if (Game1.options.SnappyMenus)
            {
                base.populateClickableComponentList();
                snapToDefaultClickableComponent();
            }
        }

        public override void performHoverAction(int x, int y)
        {
            _hoverText = String.Empty;
            if (Game1.IsFading())
            {
                return;
            }

            for (int i = 0; i < availablePortraits.Count; i++)
            {
                var textureIndex = i + _startingRow * _texturesPerRow;
                if (textureIndex < eventableCharacters.Count && availablePortraits[i].containsPoint(x, y))
                {
                    var targetNpc = eventableCharacters[textureIndex];

                    _hoverText = $"{targetNpc.displayName}";
                    if (Game1.player.friendshipData.ContainsKey(targetNpc.Name) is false)
                    {
                        _hoverText = $"???";
                    }
                }
            }

            forwardButton.tryHover(x, y, 0.2f);
            backButton.tryHover(x, y, 0.2f);
        }

        public override void receiveKeyPress(Keys key)
        {
            if (key == Keys.Escape && base.readyToClose())
            {
                base.exitThisMenu();
            }
        }

        public override void update(GameTime time)
        {
            base.update(time);

            if (_initialClickCooldown > 0)
            {
                _initialClickCooldown -= time.ElapsedGameTime.TotalMilliseconds;
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = false)
        {
            if (_initialClickCooldown > 0)
            {
                return;
            }

            if (Game1.activeClickableMenu == null)
            {
                return;
            }

            for (int i = 0; i < availablePortraits.Count; i++)
            {
                ClickableTextureComponent c = availablePortraits[i];
                var textureIndex = i + _startingRow * _texturesPerRow;
                if (textureIndex < eventableCharacters.Count && c.containsPoint(x, y))
                {
                    var targetNpc = eventableCharacters[textureIndex];
                    if (Game1.player.friendshipData.ContainsKey(targetNpc.Name) is false)
                    {
                        Game1.addHUDMessage(new HUDMessage(CaveOfMemories.i18n.Get("Dialogue.Memory.DontKnow"), null));
                    }
                    else if (_caveOfMemories.GetEventsForNPC(targetNpc).Count == 0)
                    {
                        Game1.addHUDMessage(new HUDMessage(String.Format(CaveOfMemories.i18n.Get("Dialogue.Memory.NoEvents"), targetNpc.displayName), null));
                    }
                    else
                    {
                        Game1.activeClickableMenu = new EventSelectionMenu(targetNpc, _caveOfMemories.GetEventsForNPC(targetNpc), _caveOfMemories);
                    }

                    base.exitThisMenu();
                    return;
                }
            }

            if (_startingRow > 0 && backButton.containsPoint(x, y))
            {
                _startingRow--;
                Game1.playSound("shiny4");
                return;
            }
            if ((_maxRows + _startingRow) * _texturesPerRow < eventableCharacters.Count && forwardButton.containsPoint(x, y))
            {
                _startingRow++;
                Game1.playSound("shiny4");
                return;
            }

            base.receiveLeftClick(x, y, playSound);
        }

        public override void receiveScrollWheelAction(int direction)
        {
            base.receiveScrollWheelAction(direction);
            if (direction > 0 && _startingRow > 0)
            {
                _startingRow--;
                Game1.playSound("shiny4");
            }
            else if (direction < 0 && (_maxRows + _startingRow) * _texturesPerRow < eventableCharacters.Count)
            {
                _startingRow++;
                Game1.playSound("shiny4");
            }
        }

        public override void draw(SpriteBatch b)
        {
            if (!Game1.dialogueUp && !Game1.IsFading())
            {
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
                SpriteText.drawStringWithScrollCenteredAt(b, CaveOfMemories.i18n.Get("Menu.Character.Title"), base.xPositionOnScreen + base.width / 4, base.yPositionOnScreen - 64);
                IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), base.xPositionOnScreen, base.yPositionOnScreen, base.width, base.height, Color.White, 4f);

                for (int i = 0; i < availablePortraits.Count; i++)
                {
                    availablePortraits[i].item = null;
                    availablePortraits[i].texture = null;

                    Color overlayColor = Color.White;
                    var textureIndex = i + _startingRow * _texturesPerRow;
                    if (textureIndex < eventableCharacters.Count)
                    {
                        var targetNpc = eventableCharacters[textureIndex];

                        availablePortraits[i].texture = targetNpc.Portrait;
                        availablePortraits[i].sourceRect = new Rectangle(0, 0, 64, 64);
                        availablePortraits[i].scale = 2f;

                        if (Game1.player.friendshipData.ContainsKey(targetNpc.Name) is false)
                        {
                            overlayColor = Color.Black;
                        }
                        else if (_caveOfMemories.GetEventsForNPC(targetNpc).Count == 0)
                        {
                            overlayColor = Color.Gray;
                        }
                    }

                    this.availablePortraits[i].draw(b, overlayColor, 0.87f);
                }
            }

            if (_startingRow > 0)
            {
                backButton.draw(b);
            }
            if ((_maxRows + _startingRow) * _texturesPerRow < eventableCharacters.Count)
            {
                forwardButton.draw(b);
            }

            // Draw hover text
            if (!_hoverText.Equals(""))
            {
                b.End();
                b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
                IClickableMenu.drawHoverText(b, _hoverText, Game1.smallFont);
            }

            Game1.mouseCursorTransparency = 1f;
            base.drawMouse(b);
        }
    }
}