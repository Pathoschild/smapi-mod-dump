/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/6135/StardewValley.ProfitCalculator
**
*************************************************/

using Microsoft.Xna.Framework.Input;
using StardewValley;
using System;
using System.Linq;

namespace ProfitCalculator.ui
{
    /// <summary>
    /// Option for uints in the options menu. Extends TextOption to allow for easy input of uints.
    /// </summary>
    public class UIntOption : TextOption
    {
        /// <summary> The maximum value of the uintbox. </summary>
        private readonly Func<uint> Max;

        /// <summary> The minimum value of the uintbox. </summary>
        protected readonly Func<uint> Min;

        /// <summary> Whether the uintbox should clamp the value to the min and max. </summary>
        protected readonly bool EnableClamping;

        /// <summary> Whether the option is Valid. </summary>
        public bool IsValid => int.TryParse(this.ValueGetter(), out _);

        /// <summary>
        /// Creates a new uint option.
        /// </summary>
        /// <param name="x"> The x position of the option. </param>
        /// <param name="y"> The y position of the option. </param>
        /// <param name="name"> The name of the option. </param>
        /// <param name="label"> The label of the option. </param>
        /// <param name="valueGetter"> The function to get the value of the option. </param>
        /// <param name="max"> The function to get the maximum value of the option. </param>
        /// <param name="min"> The function to get the minimum value of the option. </param>
        /// <param name="valueSetter"> The function to set the value of the option. </param>
        /// <param name="enableClamping"> Whether the uintbox should clamp the value to the min and max. </param>
        public UIntOption(
            int x,
            int y,
            Func<string> name,
            Func<string> label,
            Func<uint> valueGetter,
            Func<uint> max,
            Func<uint> min,
            Action<string> valueSetter,
            bool enableClamping = true
        ) : base(x, y, name, label, () => valueGetter().ToString(), valueSetter)
        {
            this.Max = max;
            this.Min = min;
            this.EnableClamping = enableClamping;
        }

        /// <inheritdoc />
        protected override void ReceiveInput(string str)
        {
            bool valid = true;
            //number uintbox not clamped should be able to take any positive number, to the max of uint, and 0.
            //Should not be able to take negative numbers and should not be able to take decimals or empty string, if char not valid then dont add it to string
            for (int i = 0; i < str.Length; ++i)
            {
                char c = str[i];
                if (!char.IsDigit(c) && !(c == '-' && this.ValueGetter() == "" && i == 0))
                {
                    valid = false;
                    break;
                }
            }
            if (!valid)
                return;
            //if the parsed string equals to utin 0 then set to 0, this should allow for easy clearing of the uintbox by typing 0 and being able to type a new number after that
            if (uint.Parse(this.ValueGetter() + str) == 0)
            {
                this.ValueSetter("0");
                return;
            }
            //if clamping is enabled then clamp the value to the min and max
            if (this.EnableClamping)
            {
                uint val = Math.Clamp(uint.Parse(this.ValueGetter() + str), this.Min(), this.Max());
                this.ValueSetter(val.ToString());
            }
            else
            {
                this.ValueSetter(this.ValueGetter() + str);
            }
        }

        /// <inheritdoc />
        public override void RecieveCommandInput(char command)
        {
            if (command == '\b' && this.ValueGetter().Length > 0)
            {
                Game1.playSound("tinyWhip");
                //if length is 1 then set to 0 or if multiple 0s then set to 0, else remove last char
                if (this.ValueGetter().Length == 1 || this.ValueGetter().All(c => c == '0'))
                    this.ValueSetter("0");
                else
                    this.ValueSetter(this.ValueGetter()[..^1]);
            }
        }

        /// <inheritdoc />
        public override void RecieveSpecialInput(Keys key)
        {
            if (key == Keys.Up)
            {
                uint val = Math.Clamp(uint.Parse(this.ValueGetter()) + 1, this.Min(), this.Max());
                this.ValueSetter(val.ToString());
            }
            else if (key == Keys.Down)
            {
                uint val = Math.Clamp(uint.Parse(this.ValueGetter()) - 1, this.Min(), this.Max());
                this.ValueSetter(val.ToString());
            }
        }

        /// <inheritdoc />
        public override void BeforeReceiveLeftClick(int x, int y)
        {
            base.BeforeReceiveLeftClick(x, y);
            if (!this.Selected && this.EnableClamping)
                this.ValueSetter(
                    Math.Clamp(
                        uint.Parse(this.ValueGetter()),
                        this.Min(),
                        this.Max()
                    ).ToString()
                );
        }
    }
}