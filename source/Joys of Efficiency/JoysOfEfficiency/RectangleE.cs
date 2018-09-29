namespace JoysOfEfficiency
{
    internal class RectangleE
    {
        public float X { get; }
        public float Y { get; }
        public float Width { get; }
        public float Height { get; }

        internal RectangleE(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        internal bool IsInternalPoint(float x, float y) => x >= X && x <= X + Width && y >= Y && y < Y + Height;
    }
}
