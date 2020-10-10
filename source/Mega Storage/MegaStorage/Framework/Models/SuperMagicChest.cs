/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/Stardew-MegaStorage
**
*************************************************/

using MegaStorage.Framework.Persistence;
using Microsoft.Xna.Framework;

namespace MegaStorage.Framework.Models
{
    public class SuperMagicChest : CustomChest
    {
        public override int Capacity => StateManager.MainChest == this ? int.MaxValue : 0;
        public SuperMagicChest() : this(Vector2.Zero) { }
        public SuperMagicChest(Vector2 tileLocation)
            : base(
                ChestType.SuperMagicChest,
                tileLocation)
        {
            name = "Super Magic Chest";
            EnableRemoteStorage = true;
        }
    }
}
