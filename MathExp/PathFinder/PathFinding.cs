using MathExp.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathExp.PathFinder
{
    public class PathFinding
    {
        public static Path PathTo(BoundParticle start, Point end, GeometryCollection geometry, float accel, float gravity)
        {
            // construct a new critical point
            Point newPoint = start.position();
            Line line1 = new Line(newPoint, start.boundTo.p1);
            Line line2 = new Line(newPoint, start.boundTo.p2);
            geometry.Add(line1);
            geometry.Add(line2);
            geometry.Remove(start.boundTo);

            Queue<PathCriticalPoint> bfs = new Queue<PathCriticalPoint>();
            // map time function for two attached points
            Dictionary<PathCriticalPoint, PointFunction> dict = new Dictionary<PathCriticalPoint, PointFunction>();
            PathCriticalPoint state1 = new PathCriticalPoint(line1, true);
            PathCriticalPoint state2 = new PathCriticalPoint(line2, true);
            dict[state1] = new PointFunction(0);
            dict[state2] = new PointFunction(0);
            bfs.Enqueue(state1);
            bfs.Enqueue(state2);
            // bfs will simply search through steps, and will not continue a branch if it doesn't improve anything
            while (bfs.Count > 0)
            {
                var head = bfs.Dequeue();
                // TODO: currently not going back to previously visited points, as the logic isn't correct for it
                var currPoint = head.firstPoint ? head.line.p1 : head.line.p2;
                foreach (var nextLine in geometry.LinesAttachedTo(currPoint).Where(l => !l.Equals(head.line)))
                {
                    var nextPoint = (nextLine.p1.Equals(currPoint)) ? nextLine.p2 : nextLine.p1;
                    var nextState = new PathCriticalPoint(nextLine, nextLine.p1.Equals(nextPoint));
                    PointFunction timings = new PointFunction(currPoint, nextPoint, dict[head], accel, gravity);
                    WorkWithTimings(end, geometry, bfs, dict, nextState, timings);
                    // also attempt to return along that path
                    var nextState2 = new PathCriticalPoint(nextLine, nextLine.p1.Equals(currPoint));
                    PointFunction timings2 = PointFunction.Return(currPoint, nextPoint, dict[head], accel, gravity);
                    WorkWithTimings(end, geometry, bfs, dict, nextState2, timings2);
                }
            }
            Path answer = null;
            if (ContainsAnyState(dict, end, geometry, 0))
            {
                answer = BestOfAnyState(dict, end, geometry, 0);
            }
            geometry.Remove(newPoint);
            geometry.Add(start.boundTo);
            return answer;
        }

        private static void WorkWithTimings(Point end, GeometryCollection geometry, Queue<PathCriticalPoint> bfs, Dictionary<PathCriticalPoint, PointFunction> dict, PathCriticalPoint nextState, PointFunction timings)
        {
            if (dict.ContainsKey(nextState))
            {
                double lowestChange = dict[nextState].improved(timings);
                if (!double.IsInfinity(lowestChange))
                {
                    if (!ContainsAnyState(dict, end, geometry, 0) || lowestChange < BestOfAnyState(dict, end, geometry, 0).totalTime)
                    {
                        bfs.Enqueue(nextState);
                    }
                }
            }
            else
            {
                double lowestChange = (timings.timings.Count > 0) ? timings.timings.OrderBy(p => p.Value.totalTime).First().Value.totalTime : double.PositiveInfinity;
                dict[nextState] = timings;
                if (!ContainsAnyState(dict, end, geometry, 0) || lowestChange < BestOfAnyState(dict, end, geometry, 0).totalTime)
                {
                    bfs.Enqueue(nextState);
                }
            }
        }

        private static Boolean ContainsAnyState(Dictionary<PathCriticalPoint, PointFunction> dict, Point p, GeometryCollection geom, int key)
        {
            foreach(var line in geom.LinesAttachedTo(p))
            {
                PathCriticalPoint state = new PathCriticalPoint(line, line.p1.Equals(p));
                if(dict.ContainsKey(state) && dict[state].timings.ContainsKey(key))
                {
                    return true;
                }
            }
            return false;
        }

        private static Path BestOfAnyState(Dictionary<PathCriticalPoint, PointFunction> dict, Point p, GeometryCollection geom, int key)
        {
            Path best = null;
            foreach (var line in geom.LinesAttachedTo(p))
            {
                PathCriticalPoint state = new PathCriticalPoint(line, line.p1.Equals(p));
                if (dict.ContainsKey(state) && dict[state].timings.ContainsKey(key))
                {
                    if(best==null || dict[state].timings[key].totalTime>best.totalTime)
                    {
                        best = dict[state].timings[key];
                    }
                }
            }
            return best;
        }
    }
}
