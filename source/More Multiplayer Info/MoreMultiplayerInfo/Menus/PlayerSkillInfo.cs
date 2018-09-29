using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace MoreMultiplayerInfo
{
    public class PlayerSkillInfo : IClickableMenu
    {
        private readonly Farmer _player;
        private readonly Vector2 _position;

        private List<Tuple<string, int>> _skillLevels;

        public int Width { get; set; }

        private SpriteFont _font => Game1.smallFont;

        public PlayerSkillInfo(Farmer player, Vector2 position)
        {
            _player = player;
            _position = position;
            yPositionOnScreen = Convert.ToInt32(position.Y);
            xPositionOnScreen = Convert.ToInt32(position.X);


            _skillLevels = new List<Tuple<string, int>>
            {
                new Tuple<string, int>("Farming", player.farmingLevel),
                new Tuple<string, int>("Foraging", player.foragingLevel),
                new Tuple<string, int>("Mining", player.miningLevel),
                new Tuple<string, int>("Combat", player.combatLevel),
                new Tuple<string, int>("Fishing", player.fishingLevel),
            };
        }


        public override void draw(SpriteBatch b)
        {
            Width = 0;

            for (var idx = 0; idx < _skillLevels.Count; idx++)
            {
                var skill = _skillLevels[idx];

                var yOffset = idx * 35;

                var text = $"{skill.Item1}: Lv. {skill.Item2}";

                var textWidth = Convert.ToInt32(_font.MeasureString(text).X);

                Width = Math.Max(textWidth, Width);

                b.DrawString(Game1.smallFont, text, new Vector2(_position.X, _position.Y + yOffset), Color.Black);
            }

            base.draw(b);
        }
    }
}