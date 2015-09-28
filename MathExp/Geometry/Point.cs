using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathExp.Geometry
{
    public class Point
    {
        public VertexPositionColor v;

        public Point(VertexPositionColor v)
        {
            this.v = v;
        }

        public static implicit operator Point(VertexPositionColor value)
        {
            return new Point(value);
        }

        public static implicit operator Point(Vector2 value)
        {
            return new Point(new VertexPositionColor(new Vector3(value.X, value.Y, 0), Color.White));
        }

        public static implicit operator Vector2(Point value)
        {
            return new Vector2(value.v.Position.X, value.v.Position.Y);
        }

        public override bool Equals(object p2)
        {
            Point that = (Point) p2;
            return this.v.Position == that.v.Position;
        }

        public override int GetHashCode()
        {
            return this.v.Position.GetHashCode();
        }
    }
}
