using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathExp.Geometry
{
    public class NullablePoint
    {
        private Point p;
        public NullablePoint(Point p)
        {
            this.p = p;
        }

        public NullablePoint()
        {
            p = null;
        }

        public static implicit operator NullablePoint(Point value)
        {
            return new NullablePoint(value);
        }

        public static implicit operator Point(NullablePoint value)
        {
            return value.p;
        }

        public Vector3 Position { get { return p.v.Position; } set { if (p != null)p.v.Position = value; } }
        public Color Color { get { return p.v.Color; } set { if (p != null)p.v.Color = value; } }

        public bool isNull { get { return p == null; } }
    }
}
