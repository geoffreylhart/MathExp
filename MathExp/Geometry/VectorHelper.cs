using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathExp.Geometry
{
    public static class VectorHelper
    {
        public static float CrossProduct(this Vector2 v1, Vector2 v2)
        {
            return v1.X * v2.Y - v1.Y * v2.X; 
        }
    }
}
