/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using Archery.Framework.Models.Enums;
using System.Collections.Generic;

namespace Archery.Framework.Models.Display
{
    public class DirectionalSpriteModel
    {
        public List<WorldSpriteModel> Up { get; set; } = new List<WorldSpriteModel>();
        public List<WorldSpriteModel> Right { get; set; } = new List<WorldSpriteModel>();
        public List<WorldSpriteModel> Down { get; set; } = new List<WorldSpriteModel>();
        public List<WorldSpriteModel> Left { get; set; } = new List<WorldSpriteModel>();
        public List<WorldSpriteModel> Sideways { get; set; } = new List<WorldSpriteModel>();
        public List<WorldSpriteModel> Any { get; set; } = new List<WorldSpriteModel>();

        public bool IsEmpty()
        {
            return Up.Count == 0 && Right.Count == 0 && Down.Count == 0 && Left.Count == 0 && Sideways.Count == 0 && Any.Count == 0;
        }

        public List<WorldSpriteModel> GetSpritesFromDirection(int direction)
        {
            return GetSpritesFromDirection((Direction)direction);
        }

        public List<WorldSpriteModel> GetSpritesFromDirection(Direction direction)
        {
            if (direction is Direction.Up && Up.Count > 0)
            {
                return Up;
            }
            else if (direction is Direction.Right && Right.Count > 0)
            {
                return Right;
            }
            else if (direction is Direction.Down && Down.Count > 0)
            {
                return Down;
            }
            else if (direction is Direction.Left && Left.Count > 0)
            {
                return Left;
            }
            else if ((direction is Direction.Left || direction is Direction.Right) && Sideways.Count > 0)
            {
                return Sideways;
            }
            else if (Any.Count > 0)
            {
                return Any;
            }

            return new List<WorldSpriteModel>();
        }

        public Direction GetActualDirection(Direction direction)
        {
            if (direction is Direction.Up && Up.Count > 0)
            {
                return Direction.Up;
            }
            else if (direction is Direction.Right && Right.Count > 0)
            {
                return Direction.Right;
            }
            else if (direction is Direction.Down && Down.Count > 0)
            {
                return Direction.Down;
            }
            else if (direction is Direction.Left && Left.Count > 0)
            {
                return Direction.Left;
            }
            else if ((direction is Direction.Left || direction is Direction.Right) && Sideways.Count > 0)
            {
                return Direction.Sideways;
            }

            return Direction.Any;
        }
    }
}
