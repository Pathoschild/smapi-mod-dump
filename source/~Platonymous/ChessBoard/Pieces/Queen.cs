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
using StardewModdingAPI;

namespace ChessBoard.Pieces
{
    internal class Queen : ChessPiece
    {
        public Queen(bool white, Vector2 position, IModHelper helper)
            : base(white, "Queen", position,helper)
        {
            LoadCharacterTexture(helper);
        }
        public override void CalculatePossibleMoves(List<ChessPiece> board)
        {
            base.CalculatePossibleMoves(board);
            AddBishopMovement(board);
            AddRookMovement(board);
        }
        public override ChessPiece Clone(IModHelper helper)
        {
            return new Queen(White, Position, helper);
        }
    }
}
