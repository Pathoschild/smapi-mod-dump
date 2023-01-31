/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cdaragorn/Ui-Info-Suite
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;

namespace UIInfoSuite.UIElements
{
    class ExperiencePointDisplay
    {
        private int _alpha = 100;
        private float _experiencePoints;
        private Vector2 _position;

        public ExperiencePointDisplay(float experiencePoints, Vector2 position)
        {
            _position = position;
            _experiencePoints = experiencePoints;
        }

        public void Draw()
        {
            _position.Y -= 0.5f;
            --_alpha;
            Game1.drawWithBorder(
                "Exp " + _experiencePoints,
                Color.DarkSlateGray * (_alpha / 100f),
                Color.PaleTurquoise * (_alpha / 100f),
                Utility.ModifyCoordinatesForUIScale(new Vector2(_position.X - 28, _position.Y - 130)),
                0.0f,
                0.8f,
                0.0f);
        }

        public bool IsInvisible
        {
            get { return _alpha < 3; }
        }
    }
}
