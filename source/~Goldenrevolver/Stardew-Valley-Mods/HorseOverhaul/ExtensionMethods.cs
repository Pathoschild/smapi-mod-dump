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

        public static bool IsAboutEqualTo(this float firstValue, float secondValue)
        {
            return firstValue >= (secondValue - 0.1f) && firstValue <= (secondValue + 0.1f);
        }

        private const int thinHorseXOffset = 12;

        internal static bool WithinRangeOfPlayer(this Character chara, HorseOverhaul mod, Farmer who)
        {
            int charX = chara.StandingPixel.X;

            if (chara is Horse && mod.Config.ThinHorse)
            {
                charX -= thinHorseXOffset;
            }

            if (Math.Abs(charX - who.StandingPixel.X) <= mod.Config.MaximumSaddleBagAndFeedRange)
            {
                return Math.Abs(chara.StandingPixel.Y - who.StandingPixel.Y) <= mod.Config.MaximumSaddleBagAndFeedRange;
            }

            return false;
        }

        internal static bool MouseOrPlayerIsInRange(this Character chara, HorseOverhaul mod, Farmer who, int mouseX, int mouseY, bool ignoreMousePosition)
        {
            if (!ignoreMousePosition)
            {
                int mouseMargin = 44;
                int charYOffset = 40;

                var charX = chara.StandingPixel.X;

                if (chara is Horse)
                {
                    mouseMargin = 70;

                    if (mod.Config.ThinHorse)
                    {
                        charX -= thinHorseXOffset;
                    }
                }
                else if (chara is Pet)
                {
                    charYOffset = 24;
                }

                return Utility.distance(mouseX, charX, mouseY, chara.StandingPixel.Y - charYOffset) <= mouseMargin;
            }
            else
            {
                var playerPos = who.StandingPixel;
                var charaPos = chara.StandingPixel;

                var charX = charaPos.X;

                if (chara is Horse && mod.Config.ThinHorse)
                {
                    charX -= thinHorseXOffset;
                }

                int xDistance = Math.Abs(playerPos.X - charX);
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