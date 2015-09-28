using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathExp.Geometry
{
    public class Line
    {
        public Point p1, p2;

        public Line(Point p1, Point p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }

        public static implicit operator Vector2(Line value)
        {
            return (Vector2)value.p2 - (Vector2)value.p1;
        }

        public override bool Equals(object l2)
        {
            Line that = (Line)l2;
            return (this.p1.Equals(that.p1) && this.p2.Equals(that.p2)) || (this.p2.Equals(that.p1) && this.p1.Equals(that.p2));
        }

        public override int GetHashCode()
        {
            return p1.GetHashCode() * p2.GetHashCode();
        }
    }
}
