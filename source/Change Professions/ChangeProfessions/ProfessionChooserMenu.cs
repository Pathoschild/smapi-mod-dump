/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aleksanderwaagr/Stardew-ChangeProfessions
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace ChangeProfessions
{

    // Extracted from LevelUpMenu.cs
    public class ProfessionChooserMenu : IClickableMenu
    {
        public event Action<int> OnChangedProfession;

        private readonly IModHelper _modHelper;
        private readonly int _oldProfessionId;
        private readonly int[] _professionsToChoose;

        private Color _leftProfessionColor = Game1.textColor;
        private Color _rightProfessionColor = Game1.textColor;
        private List<string> _leftProfessionDescription = new List<string>();
        private List<string> _rightProfessionDescription = new List<string>();
        public ClickableComponent LeftProfession;
        public ClickableComponent RightProfession;

        public ProfessionChooserMenu(IModHelper modHelper, int oldProfessionId, int[] professionIdsToChoose)
            : base(Game1.viewport.Width / 2 - 384, Game1.viewport.Height / 2 - 256, 768, 512)
        {
            _modHelper = modHelper;
            _oldProfessionId = oldProfessionId;
            _professionsToChoose = professionIdsToChoose;
            InitMenu();
            modHelper.Events.Input.ButtonReleased += InputOnButtonReleased;
        }

        private void InputOnButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (IsCloseButton(e.Button))
            {
                _modHelper.Events.Input.ButtonReleased -= InputOnButtonReleased;
                return;
            }

            if (!IsAcceptButton(e.Button))
                return;

            var chosenProfessionId = GetChosenProfession();
            if (chosenProfessionId == null)
                return;

            _modHelper.Events.Input.ButtonReleased -= InputOnButtonReleased;

            if (chosenProfessionId != _oldProfessionId)
            {
                OnChangedProfession?.Invoke(chosenProfessionId.Value);
            }

            ReturnToSkillsPage();
        }

        private bool IsAcceptButton(SButton button)
        {
            return button == SButton.MouseLeft || button == SButton.ControllerA;
        }

        private bool IsCloseButton(SButton button)
        {
            return button == SButton.Escape || button == SButton.E || button == SButton.ControllerB;
        }

        private void ReturnToSkillsPage()
        {
            var mainMenu = new GameMenu();
            mainMenu.changeTab(1);
            Game1.activeClickableMenu = mainMenu;
        }

        private int? GetChosenProfession()
        {
            if (Game1.getMouseY() <= yPositionOnScreen + 192 || Game1.getMouseY() >= yPositionOnScreen + height)
            {
                return null;
            }
            if (Game1.getMouseX() > xPositionOnScreen && Game1.getMouseX() < xPositionOnScreen + width / 2)
            {
                return _professionsToChoose[0];
            }
            if (Game1.getMouseX() > xPositionOnScreen + width / 2 && Game1.getMouseX() < xPositionOnScreen + width)
            {
                return _professionsToChoose[1];
            }
            return null;
        }

        private void InitMenu()
        {
            _leftProfessionDescription = LevelUpMenu.getProfessionDescription(_professionsToChoose[0]);
            _rightProfessionDescription = LevelUpMenu.getProfessionDescription(_professionsToChoose[1]);
            _leftProfessionColor = _oldProfessionId == _professionsToChoose[0] ? Color.Green : Game1.textColor;
            _rightProfessionColor = _oldProfessionId == _professionsToChoose[1] ? Color.Green : Game1.textColor;
            Game1.player.completelyStopAnimatingOrDoingAction();
            Game1.player.freezePause = 100;
            height = 512;
            if (!Game1.options.SnappyMenus)
                return;
            var mouseHeight = (int)(height / 1.5);
            LeftProfession = new ClickableComponent(new Rectangle(xPositionOnScreen, yPositionOnScreen + 128, width / 2, mouseHeight), "")
            {
                myID = 102,
                rightNeighborID = 103
            };
            RightProfession = new ClickableComponent(new Rectangle(width / 2 + xPositionOnScreen, yPositionOnScreen + 128, width / 2, mouseHeight), "")
            {
                myID = 103,
                leftNeighborID = 102
            };
            populateClickableComponentList();
            currentlySnappedComponent = LeftProfession;
            snapCursorToCurrentSnappedComponent();
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), Color.Black * 0.5f);
            b.Draw(Game1.mouseCursors, new Vector2(xPositionOnScreen + width / 2 - 116, yPositionOnScreen - 32 + 12),
                new Rectangle(363, 87, 58, 22), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);
            drawHorizontalPartition(b, yPositionOnScreen + 192);
            drawVerticalIntersectingPartition(b, xPositionOnScreen + width / 2 - 32, yPositionOnScreen + 192);
            var text = Game1.content.LoadString("Strings\\UI:LevelUp_ChooseProfession");
            b.DrawString(Game1.smallFont, text, new Vector2(xPositionOnScreen + width / 2 - Game1.smallFont.MeasureString(text).X / 2f,
                yPositionOnScreen + 64 + spaceToClearTopBorder), Game1.textColor);
            b.DrawString(Game1.dialogueFont, _leftProfessionDescription[0], new Vector2(xPositionOnScreen + spaceToClearSideBorder + 32,
                yPositionOnScreen + spaceToClearTopBorder + 160), _leftProfessionColor);
            b.Draw(Game1.mouseCursors, new Vector2(xPositionOnScreen + spaceToClearSideBorder + width / 2 - 112,
                yPositionOnScreen + spaceToClearTopBorder + 160 - 16), new Rectangle(_professionsToChoose[0] % 6 * 16,
                624 + _professionsToChoose[0] / 6 * 16, 16, 16), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            for (var index = 1; index < _leftProfessionDescription.Count; ++index)
                b.DrawString(Game1.smallFont,
                    Game1.parseText(_leftProfessionDescription[index], Game1.smallFont, width / 2 - 64),
                    new Vector2(xPositionOnScreen - 4 + spaceToClearSideBorder + 32,
                        yPositionOnScreen + spaceToClearTopBorder + 128 + 8 + 64 * (index + 1)), _leftProfessionColor);
            b.DrawString(Game1.dialogueFont, _rightProfessionDescription[0], new Vector2(xPositionOnScreen + spaceToClearSideBorder + width / 2,
                yPositionOnScreen + spaceToClearTopBorder + 160), _rightProfessionColor);
            b.Draw(Game1.mouseCursors, new Vector2(xPositionOnScreen + spaceToClearSideBorder + width - 128,
                yPositionOnScreen + spaceToClearTopBorder + 160 - 16), new Rectangle(_professionsToChoose[1] % 6 * 16, 624 + _professionsToChoose[1] / 6 * 16, 16, 16),
                Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            for (var index = 1; index < _rightProfessionDescription.Count; ++index)
                b.DrawString(Game1.smallFont,
                    Game1.parseText(_rightProfessionDescription[index], Game1.smallFont, width / 2 - 48),
                    new Vector2(xPositionOnScreen - 4 + spaceToClearSideBorder + width / 2,
                        yPositionOnScreen + spaceToClearTopBorder + 128 + 8 + 64 * (index + 1)), _rightProfessionColor);
            drawMouse(b);
        }

    }
}
