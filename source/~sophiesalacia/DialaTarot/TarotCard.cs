/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace DialaTarotCSharp
{
    internal class TarotCard
    {
        public Texture2D Texture;
        internal static readonly Dictionary<int, string> Names = new()
        {
            [1] = "The Sun",
            [2] = "The Three of Wands",
            [3] = "The Ace of Cups",
            [4] = "The Empress",
            [5] = "The Moon",
            [6] = "The Lovers",
            [7] = "The Tower, Reversed",
            [8] = "The Ace of Pentacles",
            [9] = "The Three of Swords, Reversed",
            [10] = "The Wheel of Fortune"
        };

        private static readonly Dictionary<int, string> Descriptions = new()
        {
            [1] = "Warmth, happiness, and success follow you. You're invincible.",
            [2] = "Indicates growth moving forward. Your hard work will pay off.",
            [3] = "A new beginning is here. Open yourself up and be vulnerable.",
            [4] = "This is a sign for you to be kind to yourself. Embrace your creativity and quirks.",
            [5] = "You might be experiencing confusion or anxiety. Take it slow today.",
            [6] = "Pure harmony and love in your path. Show your truest feelings.",
            [7] = "Something looms in the horizon. Don't be afraid. Embrace the change.",
            [8] = "Abundance and prosperity is on the way. Welcome all opportunities.",
            [9] = "Allow yourself to recover and grieve the past. Let go. Don't look back.",
            [10] = "Luck is on your side today. Embrace what you cannot control."
        };

        private static readonly Dictionary<int, Action> Buffs = new()
        {
            [1] = () =>
            {
                Buff buff = new(21)
                {
                    millisecondsDuration = 5 * 60 * 1000,
                    source = "The Sun",
                    displaySource = "The Sun"
                };
                Game1.buffsDisplay.addOtherBuff(buff);
            },
            [2] = () =>
            {
                Buff buff = new(0, 0, 3, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 14 * 60, "The Three of Wands",
                    "The Three of Wands")
                {
                    millisecondsDuration = 20 * 60 * 1000
                };
                Game1.buffsDisplay.addOtherBuff(buff);
            },
            [3] = () =>
            {
                Buff buff = new(0, 0, 0, 0, 0, 0, 0, 0, 0,
                1, 0, 0, 14 * 60, "Ace of Cups", "Ace of Cups")
                {
                    millisecondsDuration = 20 * 60 * 1000
                };
                Game1.buffsDisplay.addOtherBuff(buff);
            },
            [4] = () =>
            {
                Buff buff = new(0, 0, 0, 0, 3, 0, 0, 0, 0,
                0, 0, 0, 14 * 60, "The Empress", "The Empress")
                {
                    millisecondsDuration = 20 * 60 * 1000
                };
                Game1.buffsDisplay.addOtherBuff(buff);
            },
            [5] = () =>
            {
                Buff buff = new(17)
                {
                    millisecondsDuration = 20 * 60 * 1000,
                    source = "The Moon",
                    displaySource = "The Moon"
                };
                Game1.buffsDisplay.addOtherBuff(buff);
            },
            [6] = () =>
            {
                Buff buff = new(0, 0, 0, 0, 0, 0, 0, 50, 0,
                0, 0, 0, 14 * 60, "The Lovers", "The Lovers")
                {
                    millisecondsDuration = 20 * 60 * 1000
                };
                Game1.buffsDisplay.addOtherBuff(buff);
            },
            [7] = () =>
            {
                Buff buff = new(26)
                {
                    millisecondsDuration = 20 * 60 * 1000,
                    source = "The Tower, Reversed",
                    displaySource = "The Tower, Reversed"
                };
                Game1.buffsDisplay.addOtherBuff(buff);
            },
            [8] = () =>
            {
                Buff buff = new(0, 0, 0, 0, 0, 3, 0, 0, 0,
                    0, 0, 0, 14 * 60, "The Ace of Pentacles", "The Ace of Pentacles")
                {
                    millisecondsDuration = 20 * 60 * 1000
                };
                Game1.buffsDisplay.addOtherBuff(buff);
            },
            [9] = () =>
            {
                Buff buff = new(0, 0, 0, 0, 0, 0, 0, 50, 0,
                0, 0, 0, 14 * 60, "The Three of Swords, Reversed", "The Three of Swords, Reversed")
                {
                    millisecondsDuration = 20 * 60 * 1000
                };
                Game1.buffsDisplay.addOtherBuff(buff);
            },
            [10] = () =>
            {

                Buff buff = new(0, 0, 0, 0, 3, 0, 0, 0, 0,
                    0, 0, 0, 14 * 60, "The Wheel of Fortune", "The Wheel of Fortune")
                {
                    millisecondsDuration = 20 * 60 * 1000
                };
                Game1.buffsDisplay.addOtherBuff(buff);
            },
        };

        public string Name;
        public string Description;
        public Action Buff;

        public TarotCard(int num)
        {
            Texture = Game1.content.Load<Texture2D>($"Sophie.DialaTarot/Card{num}");
            Name = Names[num];
            Description = Descriptions[num];
            Buff = Buffs[num];
        }
    }
}
