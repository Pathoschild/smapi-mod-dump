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

using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace ForecasterText.Objects {
    /// <summary>
    /// A strictly integer verson of the <see cref="Vector2"/>
    /// </summary>
    public struct Vector2I {
        [DataMember]
        public int X { get; init; }
        
        [DataMember]
        public int Y { get; init; }
        
        public bool More(Vector2I vector)
            => vector.X >= this.X && vector.Y >= this.Y;
        public bool Less(Vector2I vector)
            => vector.X <= this.X && vector.Y <= this.Y;
        public bool IsIn(Rectangle bounds) {
            return this.X >= bounds.X
                && this.Y >= bounds.Y
                && this.X <= bounds.X + bounds.Width
                && this.Y <= bounds.Y + bounds.Height;
        }
        
        public static implicit operator Vector2I(Vector2 vector2) => new() {
            X = (int)vector2.X,
            Y = (int)vector2.Y
        };
        public static implicit operator Vector2(Vector2I vector2) => new() {
            X = vector2.X,
            Y = vector2.Y
        };
        public static Vector2I operator +(Vector2I a, Vector2I b) => new() {
            X = a.X + b.X,
            Y = a.Y + b.Y
        };
        public static Vector2I operator -(Vector2I a, Vector2I b) => new() {
            X = a.X - b.X,
            Y = a.Y - b.Y
        };
    }
}
