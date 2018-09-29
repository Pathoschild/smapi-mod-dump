using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GiftTasteHelper.Framework
{
    internal class SVector2
    {
        /*********
        ** Accessors
        *********/
        public static SVector2 Zero => new SVector2();

        public float X { get; set; }
        public float Y { get; set; }

        public int XInt
        {
            get => (int)this.X;
            set => this.X = value;
        }
        public int YInt
        {
            get => (int)this.Y;
            set => this.Y = value;
        }


        /*********
        ** Public methods
        *********/
        public SVector2()
        {
            this.X = 0;
            this.Y = 0;
        }

        public SVector2(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public SVector2(int x, int y)
        {
            this.XInt = x;
            this.YInt = y;
        }

        public SVector2(Vector2 v)
        {
            this.X = v.X;
            this.Y = v.Y;
        }

        public SVector2(Point p)
        {
            this.XInt = p.X;
            this.YInt = p.Y;
        }

        public bool IsZero()
        {
            return this == SVector2.Zero;
        }

        public Vector2 ToVector2()
        {
            return new Vector2(this.X, this.Y);
        }

        public Point ToPoint()
        {
            return new Point(this.XInt, this.YInt);
        }

        public static SVector2 Max(SVector2 a, SVector2 b)
        {
            return (a.X > b.X && a.Y > b.Y) ? a : b;
        }

        public static SVector2 Min(SVector2 a, SVector2 b)
        {
            return (a.X > b.X && a.Y > b.Y) ? b : a;
        }

        public static SVector2 MeasureString(string s, SpriteFont font)
        {
            return new SVector2(font.MeasureString(s));
        }

        public override bool Equals(object other)
        {
            return (this == (SVector2)other);
        }

        public override string ToString()
        {
            return "{" + this.X + ", " + this.Y + "}";
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region operators
        public static SVector2 operator +(SVector2 a, SVector2 b)
        {
            return new SVector2(a.X + b.X, a.Y + b.Y);
        }

        public static SVector2 operator -(SVector2 value)
        {
            return new SVector2(-value.X, -value.Y);
        }

        public static SVector2 operator -(SVector2 a, SVector2 b)
        {
            return new SVector2(a.X - b.X, a.Y - b.Y);
        }

        public static SVector2 operator *(SVector2 a, SVector2 b)
        {
            return new SVector2(a.X * b.X, a.Y * b.Y);
        }

        public static SVector2 operator *(SVector2 v, float scaleFactor)
        {
            return new SVector2(v.X * scaleFactor, v.Y * scaleFactor);
        }

        public static SVector2 operator *(float scaleFactor, SVector2 v)
        {
            return new SVector2(v.X * scaleFactor, v.Y * scaleFactor);
        }

        public static SVector2 operator /(SVector2 v, SVector2 b)
        {
            return new SVector2(v.X / b.X, v.Y / b.Y);
        }

        public static SVector2 operator /(SVector2 v, float divider)
        {
            return new SVector2(v.X / divider, v.Y / divider);
        }

        public static bool operator ==(SVector2 a, SVector2 b)
        {
            return (a.X == b.X && a.Y == b.Y);
        }

        public static bool operator !=(SVector2 a, SVector2 b)
        {
            return !(a == b);
        }
        #endregion operators
    }
}
