/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Berisan/SpawnMonsters
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using static Spawn_Monsters.Monsters.MonsterData;

namespace Spawn_Monsters
{
    /// <summary>
    /// Represents a single monster in one component.
    ///</summary>
    public class ClickableMonsterComponent : ClickableComponent
    {

        private readonly int StartFrame;
        private readonly int NumberOfFrames;
        private readonly float Interval;
        private Color color;
        public AnimatedSprite Sprite { get; }
        public Monster Monster { get; }
        public int HeightLevel { get; }
        public int WidthLevel { get; }


        public ClickableMonsterComponent(Monster monster, string textureName, int xPosition, int yPosition, int width, int height, int spriteWidth = 16, int spriteHeight = 24, int startFrame = 0, int numberOfFrames = 4, float interval = 100, Color color = default)
            : base(new Rectangle(xPosition, yPosition, width, height), textureName) {

            if (monster != Monster.CursedDoll) {
                Sprite = new AnimatedSprite($"Characters\\Monsters\\{textureName}", startFrame, spriteWidth, spriteHeight);

                StartFrame = startFrame;
                NumberOfFrames = numberOfFrames;
                Interval = interval;
            }

            Monster = monster;
            this.color = color;

            // Small workaround: Spider's texture is 32x32 but is actually 8x8
            HeightLevel = Monster == Monster.Spider ? 2 : spriteHeight / 8;
            WidthLevel = Monster == Monster.Spider ? 2 : spriteWidth / 8;
        }


        public void PerformHoverAction(int x, int y) {
            if (Monster != Monster.CursedDoll && Monster != Monster.PutridGhost && Monster != Monster.RoyalSerpent && Monster != Monster.Spiker) {
                if (containsPoint(x, y)) {
                    Sprite.Animate(Game1.currentGameTime, StartFrame, NumberOfFrames, Interval);
                } else {
                    if (Sprite.CurrentFrame != StartFrame) {
                        Sprite.CurrentFrame = StartFrame;
                    }
                }
            }
        }

        public void Draw(SpriteBatch b) {
            //b.Draw(Game1.staminaRect, bounds, HeightLevel == 2 ? Color.LightBlue : HeightLevel == 3 ? Color.LightYellow : HeightLevel == 4 ? Color.PaleVioletRed : Color.White);

            if (Monster == Monster.CursedDoll) {
                b.Draw(Game1.objectSpriteSheet, new Rectangle(bounds.X, bounds.Y, 16 * 4, 16 * 4), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 103, 16, 16)), new Color(255, 50, 50));
            } else if (Monster == Monster.PrismaticSlime) {
                Sprite.draw(b, new Vector2(bounds.X, bounds.Y), 1, 0, 0, Utility.GetPrismaticColor(348, 5f), false, 4);
            } else if (Monster == Monster.Spider) {
                Sprite.draw(b, new Vector2(bounds.X, bounds.Y), 1, 8, 16, (color == default ? Color.White : color), false, 4);
            } else {
                Sprite.draw(b, new Vector2(bounds.X, bounds.Y), 1, 0, 0, (color == default ? Color.White : color), false, 4);
            }
        }
    }
}
