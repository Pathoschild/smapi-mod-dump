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

namespace ForecasterText.Objects {
    public sealed class Blink {
        private const float MAX = 1.0f;
        private const float MIN = 0.75f;
        public const float SHIFT = 0.01f;
        
        private float Value = Blink.MAX - Blink.SHIFT;
        
        private bool Increasing = true;
        private bool Decreasing {
            get => !this.Increasing;
            set => this.Increasing = !value;
        }
        
        public float Scale => this.Next();
        
        public void Reset() {
            this.Value = Blink.MAX - Blink.SHIFT;
            this.Increasing = true;
        }
        private float Next() {
            if (this.Increasing) {
                this.Value += Blink.SHIFT;
                if (this.Value >= Blink.MAX)
                    this.Decreasing = true;
            } else {
                this.Value -= Blink.SHIFT;
                if (this.Value <= Blink.MIN)
                    this.Increasing = true;
            }
            
            return this.Value;
        }
    }
}
