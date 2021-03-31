/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

using ImJustMatt.SlimeFramework.Framework.Controllers;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley.Monsters;

namespace ImJustMatt.SlimeFramework.Framework.Extensions
{
    internal static class GreenSlimeExtensions
    {
        private static IReflectionHelper _reflection;

        public static void Init(IReflectionHelper reflection)
        {
            _reflection = reflection;
        }

        public static void MakeCustomSlime(this GreenSlime slime, SlimeController slimeController)
        {
            slime.Name = "";
            slime.reloadSprite();
            slime.Sprite.SpriteHeight = 24;
            slime.Sprite.UpdateSourceRect();
            _reflection.GetMethod(slime, "parseMonsterInfo").Invoke(slime.Name);
            slime.color.Value = Color.White;
        }
    }
}