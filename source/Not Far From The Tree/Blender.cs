/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GStefanowich/SDV-NFFTT
**
*************************************************/

/*
 * This software is licensed under the MIT License
 * https://github.com/GStefanowich/SDV-NFFTT
 *
 * Copyright (c) 2019 Gregory Stefanowich
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace NotFarFromTheTree {
    public static class Blender {
        public static Dictionary<string, Color> NPC_EYES = new Dictionary<string, Color> {
            ["Abigail"] = new Color(0, 147, 116),
            ["Alex"] = new Color(62, 147, 0),
            ["Caroline"] = new Color(40, 91, 87),
            ["Clint"] = new Color(0, 66, 165),
            ["Demetrius"] = new Color(99, 25, 29),
            ["Elliott"] = new Color(1, 94, 24),
            ["Emily"] = new Color(2, 48, 139),
            ["Evelyn"] = new Color(),
            ["George"] = new Color(),
            ["Gus"] = new Color(144, 72, 0),
            ["Haley"] = new Color(41, 120, 244),
            ["Harvey"] = new Color(93, 165, 93),
            ["Jodi"] = new Color(145, 85, 241),
            ["Kent"] = new Color(143, 136, 133),
            ["Leah"] = new Color(71, 28, 150),
            ["Lewis"] = new Color(91, 54, 52),
            ["Linus"] = new Color(255, 143, 43),
            ["Marnie"] = new Color(142, 51, 0),
            ["Maru"] = new Color(195, 122, 248),
            ["Pam"] = new Color(136, 30, 12),
            ["Penny"] = new Color(35, 75, 37),
            ["Pierre"] = new Color(103, 0, 36),
            ["Robin"] = new Color(62, 124, 68),
            ["Sam"] = new Color(99, 98, 104),
            ["Sandy"] = new Color(56, 4, 41),
            ["Sebastian"] = new Color(72, 53, 110),
            ["Shane"] = new Color(43, 74, 56),
            ["Willy"] = new Color(32, 81, 76),
            ["Wizard"] = new Color(128, 111, 255)
        };
        public static Dictionary<string, Color> NPC_HAIR = new Dictionary<string, Color> {
            ["Abigail"] = new Color(178, 93, 246),
            ["Alex"] = new Color(153, 62, 0),
            ["Caroline"] = new Color(70, 153, 81),
            ["Clint"] = new Color(119, 43, 16),
            ["Demetrius"] = new Color(73, 7, 36),
            ["Elliott"] = new Color(253, 153, 48),
            ["Emily"] = new Color(27, 14, 246),
            ["Evelyn"] = new Color(203, 186, 197),
            ["George"] = new Color(183, 144, 139),
            ["Gus"] = new Color(111, 49, 0),
            ["Haley"] = new Color(255, 252, 56),
            ["Harvey"] = new Color(119, 51, 39),
            ["Jodi"] = new Color(249, 119, 79),
            ["Kent"] = new Color(179, 81, 48),
            ["Leah"] = new Color(254, 116, 35),
            ["Lewis"] = new Color(146, 121, 117),
            ["Linus"] = new Color(180, 158, 156),
            ["Marnie"] = new Color(148, 44, 0),
            ["Maru"] = new Color(145, 40, 77),
            ["Pam"] = new Color(238, 146, 47),
            ["Penny"] = new Color(255, 171, 71),
            ["Pierre"] = new Color(209, 97, 50),
            ["Robin"] = new Color(224, 74, 33),
            ["Sam"] = new Color(255, 255, 150),
            ["Sandy"] = new Color(211, 10, 77),
            ["Sebastian"] = new Color(57, 30, 71),
            ["Shane"] = new Color(93, 35, 88),
            ["Willy"] = new Color(91, 72, 60),
            ["Wizard"] = new Color(112, 52, 237)
        };
        public static Dictionary<string, int> NPC_SKIN = new Dictionary<string, int> {
            ["Abigail"] = 23,
            ["Alex"] = 0,
            ["Caroline"] = 0,
            ["Clint"] = 0,
            ["Demetrius"] = 5,
            ["Elliott"] = 22,
            ["Emily"] = 0,
            ["Evelyn"] = 0,
            ["George"] = 0,
            ["Gus"] = 1,
            ["Haley"] = 0,
            ["Harvey"] = 0,
            ["Jodi"] = 0,
            ["Kent"] = 22,
            ["Leah"] = 23,
            ["Lewis"] = 0,
            ["Linus"] = 0,
            ["Marnie"] = 23,
            ["Maru"] = 8,
            ["Pam"] = 0,
            ["Penny"] = 23,
            ["Pierre"] = 22,
            ["Robin"] = 0,
            ["Sam"] = 0,
            ["Sandy"] = 0,
            ["Sebastian"] = 0,
            ["Shane"] = 22,
            ["Willy"] = 0,
            ["Wizard"] = 0
        };
        private const int BLEND = 11;
        
        /**
         * If two Parents (Farmer, NPC) or (Farmer, Farmer) are the same sex
         */
        public static bool AreAdopters( Character parent1, Character parent2 ) {
            return Blender.GetSex(parent1) == Blender.GetSex(parent2);
        }
        
        /**
         * Verify that Parents are adults (Cannot blend with child NPCs: Jas, Vicent, Leo...)
         */
        public static bool IsOfAge( params Character[] characters ) {
            foreach (Character character in characters)
                if (character is NPC npc && npc.Age == NPC.child)
                    return false;
            return true;
        }
        
        /**
         * Get the sex of a Farmer or NPC
         */
        private static int GetSex( Character character ) {
            if (character is NPC npc)
                return npc.Gender;
            if (character is Farmer farmer)
                return farmer.IsMale ? NPC.male : NPC.female;
            return -1;
        }
        
        /**
         * Blend two colors randomly
         */
        public static Color Blend( this Color start, Color end ) {
            int rand = Game1.random.Next(1, Blender.BLEND),
                incR = (end.R - start.R) / (Blender.BLEND - 1),
                incG = (end.G - start.G) / (Blender.BLEND - 1),
                incB = (end.B - start.B) / (Blender.BLEND - 1);
            
            Color[] colors = new Color[Blender.BLEND];
            for ( int i = 0; i < Blender.BLEND; i++ ) {
                colors[i] = new Color(
                    start.R + (incR * i),
                    start.G + (incG * i),
                    start.B + (incB * i)
                );
                
                ModEntry.MONITOR.Log($"{i}. {colors[i]}", LogLevel.Info);
            }
            
            return colors[rand - 1];
        }
        
        /**
         * Create a random skin color using two parents
         */
        public static int SkinColor( Character parent1, Character parent2 ) {
            if (Blender.AreAdopters(parent1, parent2))
                return Blender.NaturalSkinColor();
            return 0;
        }
        
        /**
         * Get a natural (Not blue, red, purple) skin color, in the case of adoptees
         */
        private static int NaturalSkinColor() {
            // TODO: Add a randomizer for skin colors
            return 0;
        }
        
        /**
         * Get the skin color of a farmer or NPC
         */
        public static int GetSkinColor( Character character ) {
            // If character is a Farmer, return their skin color
            if (character is Farmer farmer)
                return farmer.skinColor;
            
            // If character is an NPC, return their skin color
            if (character is NPC npc && Blender.NPC_SKIN.TryGetValue(npc.Name, out int color))
                return color;
            
            // Create a new random skin color (Fallback)
            return Blender.NaturalSkinColor();
        }
        
        /**
         * Get a random hair color using two parents
         */
        public static Color HairColor( Character parent1, Character parent2 ) => Blender.AreAdopters(parent1, parent2) ? Blender.NaturalHairColor() : Blender.GetHairColor(parent1).Blend(Blender.GetHairColor(parent2));
        
        /**
         * Get a natural (Not blue, red, purple) hair color, in the case of adoptees
         */
        private static Color NaturalHairColor() {
            // TODO: Add a randomizer for hair colors
            return Color.Brown;
        }
        
        /**
         * Get the hair color of a Farmer or NPC
         */
        public static Color GetHairColor( Character character ) {
            // If character is a Farmer, return their hair color
            if (character is Farmer farmer)
                return farmer.hairstyleColor.Value;
            
            // If character is an NPC, return their hair color
            if (character is NPC npc && Blender.NPC_HAIR.TryGetValue(npc.Name, out Color color))
                return color;
            
            // Create a new random hair color (Fallback)
            return Blender.NaturalHairColor();
        }
        
        /**
         * Get a random hair color using two parents
         */
        public static Color EyeColor( Character parent1, Character parent2 ) => Blender.AreAdopters(parent1, parent2) ? Blender.NaturalEyeColor() : Blender.GetEyeColor(parent1).Blend(Blender.GetEyeColor(parent2));
        
        /**
         * Get a natural (Not orange, pink, purple) eye color, in the case of adoptees
         */
        private static Color NaturalEyeColor() {
            // TODO: Add a randomizer for Hair Colors
            return Color.Blue;
        }
        
        /**
         * Get the eye color of a Farmer or NPC
         */
        public static Color GetEyeColor( Character character ) {
            // If character is a Farmer, return their eye color
            if (character is Farmer farmer)
                return farmer.newEyeColor.Value;
            
            // If character is an NPC, return their eye color
            if (character is NPC npc && Blender.NPC_EYES.TryGetValue(npc.Name, out Color color))
                return color;
            
            // Create a new random eye color (Fallback)
            return Blender.NaturalEyeColor();
        }
    }
}