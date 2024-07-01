/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Brandon22Adams/ToolPouch
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace ToolPouch.UI
{
    internal class ToolPouchMenu : IClickableMenu
    {
        private List<ToolPouchButton> buttons = new List<ToolPouchButton>();
        protected int buttonRadius;
        protected int lowestButtonY = -1;
        protected int animtime;
        protected int age;
        protected float animProgress = 0f;

        protected bool gamepadMode;
        protected bool moved = false;
        protected int moveAge = 0;
        protected int delay = 250;

        protected int wheelIndex = -1;

        IModHelper Helper;
        IMonitor Monitor;
        ModConfig Config;
        ModEntry Mod;

        public ToolPouchMenu(IModHelper helper, ModEntry mod, ModConfig config, IMonitor Monitor)
        {
            Helper = helper;
            Mod = mod;
            Config = config;
            this.Monitor = Monitor;

            animtime = Config.AnimationMilliseconds;
            buttonRadius = 26;

            width = 256;
            height = 256;
            xPositionOnScreen = (int)(Game1.viewport.Width / 2 - width / 2f);
            yPositionOnScreen = (int)(Game1.viewport.Height / 2 - height / 2f);
            snapToPlayerPosition();
        }

        public void updateToolList(SortedDictionary<Item, int> dict)
        {
            //essentially re init the menu
            gamepadMode = false;
            buttons.Clear();
            age = 0;
            animProgress = 0f;
            delay = 250;
            moved = false;

            foreach (KeyValuePair<Item, int> kv in dict)
            {
                buttons.Add(new ToolPouchButton(kv.Value, kv.Key, Helper));
                if (kv.Value == Game1.player.CurrentToolIndex)
                {
                    wheelIndex = buttons.Count - 1;
                    buttons[wheelIndex].select();
                }
            }

            if (dict.Count < Config.BagCapacity)
            {
                buttons.Add(new ToolPouchButton(Config.BagCapacity - 1, null, Helper));
                if (Config.BagCapacity - 1 == Game1.player.CurrentToolIndex)
                {
                    wheelIndex = buttons.Count - 1;
                    buttons[wheelIndex].select();
                }
            }

            snapToPlayerPosition();
        }

        public int closeAndReturnSelected()
        {
            exitThisMenu();
            if (wheelIndex < 0) return wheelIndex;
            return buttons[wheelIndex].getIndex();
        }

        public override void update(GameTime time)
        {
            age += time.ElapsedGameTime.Milliseconds;
            if (age > animtime)
            {
                animProgress = 1f;
            }
            else if (animtime > 0)
            {
                animProgress = age / (float)animtime;
            }
            if (wheelIndex >= 0) buttons[wheelIndex].select();

            snapToPlayerPosition();
            Vector2 offset = default;
            float xState = 0;
            float yState = 0;

            if (Config.LeftStickSelection)
            {
                xState = Game1.input.GetGamePadState().ThumbSticks.Left.X;
                yState = Game1.input.GetGamePadState().ThumbSticks.Left.Y;
            }
            else
            {
                xState = Game1.input.GetGamePadState().ThumbSticks.Right.X;
                yState = Game1.input.GetGamePadState().ThumbSticks.Right.Y;
            }

            if (!gamepadMode && Game1.options.gamepadControls && (Math.Abs(xState) > 0.5f || Math.Abs(yState) > 0.5f))
            {
                gamepadMode = true;
            }

            if (gamepadMode)
            {
                if (Math.Abs(xState) > 0.5f || Math.Abs(yState) > 0.5f)
                {
                    offset = new Vector2(xState, yState);
                    offset.Y *= -1f;
                    offset.Normalize();
                    float highest_dot = -1f;
                    int tempIndex = 0;
                    for (int j = 0; j < buttons.Count; j++)
                    {
                        float dot = Vector2.Dot(value2: new Vector2(buttons[j].bounds.Center.X - (xPositionOnScreen + width / 2f), buttons[j].bounds.Center.Y - (yPositionOnScreen + height / 2f)), value1: offset);

                        if (dot > highest_dot)
                        {
                            highest_dot = dot;
                            tempIndex = j;

                        }
                    }
                    if (wheelIndex >= 0)
                    {
                        buttons[wheelIndex].deSelect();
                    }
                    wheelIndex = tempIndex;
                    buttons[wheelIndex].select();
                }
                else
                {
                    Mod.swapItem(closeAndReturnSelected());
                }
            }

        }

        public override void performHoverAction(int x, int y)
        {
            if (gamepadMode)
            {
                return;
            }

            x = (int)Utility.ModifyCoordinateFromUIScale(x);
            y = (int)Utility.ModifyCoordinateFromUIScale(y);

            for (int i = 0; i < buttons.Count; i++)
            {
                if (buttons[i].containsPoint(x, y))
                {
                    wheelIndex = i;
                    buttons[wheelIndex].select();
                    return;
                }
                else
                {
                    buttons[i].deSelect();
                    wheelIndex = -1;
                }
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            x = (int)Utility.ModifyCoordinateFromUIScale(x);
            y = (int)Utility.ModifyCoordinateFromUIScale(y);
            Mod.swapItem(closeAndReturnSelected());
            base.receiveLeftClick(x, y, playSound);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            wheelIndex = -1;
            receiveLeftClick(x, y, playSound);
        }

        protected void snapToPlayerPosition()
        {
            if (Game1.player != null)
            {
                Vector2 player_position = Game1.player.getLocalPosition(Game1.viewport) + new Vector2(-width / 2f, -height / 2f);
                xPositionOnScreen = (int)player_position.X + 32;
                yPositionOnScreen = (int)player_position.Y - 64;
                if (xPositionOnScreen + width > Game1.viewport.Width)
                {
                    xPositionOnScreen -= xPositionOnScreen + width - Game1.viewport.Width;
                }
                if (xPositionOnScreen < 0)
                {
                    xPositionOnScreen -= xPositionOnScreen;
                }
                if (yPositionOnScreen + height > Game1.viewport.Height)
                {
                    yPositionOnScreen -= yPositionOnScreen + height - Game1.viewport.Height;
                }
                if (yPositionOnScreen < 0)
                {
                    yPositionOnScreen -= yPositionOnScreen;
                }
                positionRadial();
            }
        }

        private void positionRadial()
        {
            updateButtonRadius();
            lowestButtonY = -1;
            int x;
            int y;
            for (int i = 0; i < buttons.Count; i++)
            {
                ClickableTextureComponent button = buttons[i];
                float radians = Utility.Lerp(0f, (float)Math.PI * 2f, i / (float)buttons.Count);
                x = (int)(xPositionOnScreen + width / 2 + (int)(Math.Cos(radians) * buttonRadius) * 4 - button.bounds.Width / 2f);
                y = (int)(yPositionOnScreen + height / 2 + (int)((0.0 - Math.Sin(radians)) * buttonRadius) * 4 - button.bounds.Height / 2f);
                button.bounds.X = x;
                button.bounds.Y = y;
                if (lowestButtonY < y) lowestButtonY = y;
            }
        }

        private void updateButtonRadius()
        {
            if (buttons.Count > 6)
            {
                buttonRadius = buttons.Count * 5;
            }
            else
            {
                buttonRadius = 26;
            }
            buttonRadius = (int)(animProgress * buttonRadius);

        }

        public override void draw(SpriteBatch b)
        {
            Game1.StartWorldDrawInUI(b);



            foreach (ToolPouchButton button in buttons)
            {
                button.draw(b, (float)Math.Pow(animProgress, 8), Config.UseBackdrop);
            }

            if (!gamepadMode)
            {
                Game1.mouseCursorTransparency = 1f;
                drawMouse(b);
            }


            if (animProgress >= 1 && wheelIndex >= 0)
            {
                SpriteText.drawStringWithScrollCenteredAt(b, buttons[wheelIndex].toolName(), xPositionOnScreen + width / 2, lowestButtonY + buttons[wheelIndex].bounds.Height + 20);

            }
            Game1.EndWorldDrawInUI(b);
        }
    }
}