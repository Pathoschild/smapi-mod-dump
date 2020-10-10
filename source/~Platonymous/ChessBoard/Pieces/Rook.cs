/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace ChessBoard.Pieces
{
    public class Rook : ChessPiece
    {
        public bool canCastle = true;
        public Rook(bool white, Vector2 position, IModHelper helper)
            : base(white,"Rook",position,helper, false)
        {
            LoadCharacterTexture(helper);
        }
        public override void CalculatePossibleMoves(List<ChessPiece> board)
        {
            base.CalculatePossibleMoves(board);
            AddRookMovement(board);
        }
        public override ChessPiece Clone(IModHelper helper)
        {
            return new Rook(White, Position, helper);
        }
    }
}
