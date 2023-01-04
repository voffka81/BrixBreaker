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
    public class Vector2
    {
        // Summary:
        //     Gets or sets the x-component of the vector.
        public float X;
        //
        // Summary:
        //     Gets or sets the y-component of the vector.
        public float Y;

        //
        // Summary:
        //     Returns a Vector2 with all of its components set to zero.
        public static Vector2 Zero {
            get
            {
                return new Vector2(0, 0);
            }
        }



        //
        // Summary:
        //     Initializes a new instance of Vector2.
        //
        // Parameters:
        //   x:
        //     Initial value for the x-component of the vector.
        //
        //   y:
        //     Initial value for the y-component of the vector.
        public Vector2(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        //
        // Summary:
        //     Calculates the length of the vector squared.
        //
        // Returns:
        //     The length of the vector squared.
        public float LengthSquared()
        {
            return (float)Math.Sqrt((double)Math.Abs(X + Y));
        }
        //
        // Summary:
        //     Turns the current vector into a unit vector. The result is a vector one unit
        //     in length pointing in the same direction as the original vector.
        public void Normalize()
        {
            float len = LengthSquared();
            X = X / len;
            Y = Y / len;
        }
    }
}
