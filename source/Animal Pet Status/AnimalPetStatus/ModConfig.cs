/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hakej/Animal-Pet-Status
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace AnimalPetStatus
{
    public class ModConfig
    {
        public Vector2 Position { get; set; } = new Vector2(10, 10);
        public bool IsActive { get; set; } = true;
        public SButton ToggleButton = SButton.P;
        public SButton MoveButton = SButton.L;
    }
}
