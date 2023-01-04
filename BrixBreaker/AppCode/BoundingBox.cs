using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CrackOut
{
    public class BoundingBox
    {
        public Vector2 min = Vector2.Zero;
        public Vector2 max = Vector2.Zero;

        public BoundingBox(Vector2 min, Vector2 max)
        {
            this.min = min;
            this.max.X = min.X + max.X;
            this.max.Y = min.Y + max.Y;
        }

        public bool Intersects(BoundingBox box)
        {
            return (box.min.X <= this.max.X && box.max.X >= this.min.X &&
                box.min.Y <= this.max.Y && box.max.Y >= this.min.Y);
        }
    }
}
