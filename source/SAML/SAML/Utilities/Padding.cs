/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/SAML
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAML.Utilities
{
    public readonly struct Padding
    {
        public static readonly Padding Zero = new();

        public int Left { get; }

        public int Right { get; }

        public int Top { get; }

        public int Bottom { get; }

        public Padding() => Left = Right = Top = Bottom = 0;

        public Padding(int value) => Left = Right = Top = Bottom = value;

        public Padding(int width, int height)
        {
            Left = Right = width;
            Top = Bottom = height;
        }

        public Padding(int left, int right, int top, int bottom)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }

        public Padding(Padding other)
        {
            Left = other.Left;
            Right = other.Right;
            Top = other.Top;
            Bottom = other.Bottom;
        }

        public static Padding operator +(Padding left, int right) { return new(left.Left + right, left.Right + right, left.Top + right, left.Bottom + right); }

        public static Padding operator +(Padding left, Padding right) { return new(left.Left + right.Left, left.Right + right.Right, left.Top + right.Top, left.Bottom + right.Bottom); }

        public static Padding operator -(Padding left, int right) { return new(left.Left - right, left.Right - right, left.Top - right, left.Bottom - right); }

        public static Padding operator -(Padding left, Padding right) { return new(left.Left - right.Left, left.Right - right.Right, left.Top - right.Top, left.Bottom - right.Bottom); }

        public static Padding operator *(Padding left, int right) { return new(left.Left * right, left.Right * right, left.Top * right, left.Bottom * right); }

        public static Padding operator *(Padding left, Padding right) { return new(left.Left * right.Left, left.Right * right.Right, left.Top * right.Top, left.Bottom * right.Bottom); }

        public static Padding operator /(Padding left, int right) { return new(left.Left / right, left.Right / right, left.Top / right, left.Bottom / right); }

        public static Padding operator /(Padding left, Padding right) { return new(left.Left / right.Left, left.Right / right.Right, left.Top / right.Top, left.Bottom / right.Bottom); }
    }
}
