using MathExp.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathExp.PathFinder
{
    public class PathCriticalPoint
    {
        // a critical point will be coming out of a line from either endpoint, and (potentially) on either side of the line
        public Line line;
        public Boolean firstPoint;

        public PathCriticalPoint(Line line, bool firstPoint)
        {
            this.line = line;
            this.firstPoint = firstPoint;
        }

        public override bool Equals(object p2)
        {
            PathCriticalPoint that = (PathCriticalPoint)p2;
            return this.line.Equals(that.line) && (this.firstPoint==that.firstPoint);
        }

        public override int GetHashCode()
        {
            return line.GetHashCode() * (firstPoint ? 1 : -1);
        }
    }
}
