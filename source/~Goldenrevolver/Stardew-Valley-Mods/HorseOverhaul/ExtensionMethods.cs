/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

using StardewValley;
using StardewValley.Buildings;
using StardewValley.Characters;
using System;

namespace HorseOverhaul
{
    internal static class ExtensionMethods
    {
        internal static string GetNPCNameForDisplay(this NPC npc, HorseOverhaul mod)
        {
            if (!string.IsNullOrWhiteSpace(npc?.getName()))
            {
                return npc.getName().Trim();
            }
            else
            {
                return mod.Helper.Translation.Get("UnknownName");
            }
        }

        public static bool IsTractor(this Horse horse)
        {
            return horse != null && horse.modData.ContainsKey("Pathoschild.TractorMod");
        }

        public static bool IsTractorGarage(this Stable stable)
        {
            return stable != null && stable.buildingType.Value == "Pathoschild.TractorMod_Stable";
        }

        internal static bool MouseOrPlayerIsInRange(this Character chara, Farmer who, int mouseX, int mouseY, bool ignoreMousePosition)
        {
            if (!ignoreMousePosition)
            {
                return Utility.distance(mouseX, chara.Position.X, mouseY, chara.Position.Y) <= 70;
            }
            else
            {
                var playerPos = who.StandingPixel;
                var charaPos = chara.StandingPixel;

                int xDistance = Math.Abs(playerPos.X - charaPos.X);
                int yDistance = Math.Abs(playerPos.Y - charaPos.Y);

                return who.FacingDirection switch
                {
                    Game1.up => playerPos.Y > charaPos.Y && xDistance < 48,
                    Game1.down => playerPos.Y < charaPos.Y && xDistance < 48,
                    Game1.right => playerPos.X < charaPos.X && yDistance < 48,
                    Game1.left => playerPos.X > charaPos.X && yDistance < 48,
                    _ => false,
                };
            }
        }
    }
}