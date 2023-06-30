/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GStefanowich/SDV-Forecaster
**
*************************************************/

/*
 * This software is licensed under the MIT License
 * https://github.com/GStefanowich/SDV-Forecaster
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

using System.Globalization;
using StardewValley;

namespace ForecasterText.Objects.Addons {
    public static class CharacterEmoji {
        public static string GetName(this Character character)
            => character switch {
                NPC npc => npc.getName(),
                _ => character.Name
            };
        
        public static bool HasEmoji(this Character character)
            => GetEmoji(character.GetName()) is not null;
        
        public static bool HasEmoji(string name)
            => GetEmoji(name) is not null;
        
        public static uint? GetEmoji(string name)
            => name.ToLower(CultureInfo.InvariantCulture) switch {
                "abigail" => 154u,
                "penny" => 155u,
                "maru" => 156u,
                "leah" => 157u,
                "haley" => 158u,
                "emily" => 159u,
                "alex" => 160u,
                "shane" => 161u,
                "sebastian" => 162u,
                "sam" => 163u,
                "harvey" => 164u,
                "elliot" => 165u,
                "sandy" => 166u,
                "evelyn" => 167u,
                "marnie" => 168u,
                "caroline" => 169u,
                "robin" => 170u,
                "pierre" => 171u,
                "pam" => 172u,
                "jodi" => 173u,
                "lewis" => 174u,
                "linus" => 175u,
                "marlon" => 176u,
                "willy" => 177u,
                "wizard" => 178u,
                "morris" => 179u,
                "jas" => 180u,
                "vincent" => 181u,
                "krobus" => 182u,
                "dwarf" => 183u,
                "gus" => 184u,
                "gunther" => 185u,
                "george" => 186u,
                "demetrius" => 187u,
                "clint" => 188u,
                _ => null
            };
    }
}
