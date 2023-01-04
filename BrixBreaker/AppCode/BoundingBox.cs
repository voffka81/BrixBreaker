namespace CrackOut
{
    public class BoundingBox
    {
        private Vector2 _min = Vector2.Zero;
        private Vector2 _max = Vector2.Zero;
        private Vector2 _size = Vector2.Zero;
        public Vector2 Min => _min;
        public Vector2 Max => _max;

        public BoundingBox(Vector2 min, Vector2 size)
        {
            _size = size;
            Update(min);
        }

        public void Update(Vector2 min)
        {
            _min = min;
            _max.X = min.X + _size.X;
            _max.Y = min.Y + _size.Y;
        }

        public bool Intersects(BoundingBox box)
        {
            return (box.Min.X <= _max.X && box.Max.X >= _min.X &&
                box.Min.Y <= _max.Y && box.Max.Y >= _min.Y);
        }
    }
}
