using MathExp.Geometry;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathExp.PathFinder
{
    public class PointFunction
    {
        static private float PRECISION = 100;
        public Dictionary<int, Path> timings = new Dictionary<int, Path>();
        private Path p;

        public PointFunction(Geometry.Point p1, Geometry.Point p2, PointFunction old, float accel, float gravity)
        {
            Vector2 path = ((Vector2)p2-(Vector2)p1);
            Vector2 normalized = path;
            normalized.Normalize();
            float g = gravity * path.Y / path.Length();
            // r and l have been specifically framed this way, and affect several equations
            float r = g + accel;
            float l = -g + accel;
            float d = path.Length();
            foreach(var pair in old.timings)
            {
                // is positive if it assists acceleration towards p2 (aka right)
                float b = FromKey(pair.Key);
                // collisions reduce speed
                Vector2 prevPath = (Vector2)p1 - pair.Value.posAt(0);
                if (prevPath != Vector2.Zero)
                {
                    b *= Math.Abs(Vector2.Dot(path, prevPath)) / (path.Length() * prevPath.Length());
                }
                Parabola fullSpeedPath = new Parabola(r, b, 0);
                Parabola fullRetreatPath = new Parabola(-l, b, 0);
                Paraboloid speedThenRetreat = fullSpeedPath.FollowedBy(fullRetreatPath);
                int lowerBound = ToKey(fullRetreatPath.SpeedAt(d, 0));
                int upperBound = ToKey(fullSpeedPath.SpeedAt(d, 0));
                for (int i = lowerBound; i <= upperBound; i++)
                {
                    float f = FromKey(i);
                    float newPartialTime = speedThenRetreat.SpecialShit(d, f, -1);
                    if (newPartialTime >= 0)
                    {
                        float newT = newPartialTime + pair.Value.totalTime;
                        if (!timings.ContainsKey(i) || timings[i].totalTime > newT)
                        {
                            float rT = (l * newPartialTime + f - b) / (l + r);
                            float lT = newPartialTime - rT;
                            timings[i] = new Path(newT-lT, p1, b * normalized, r * normalized, pair.Value);
                            timings[i] = new Path(newT, timings[i].posAt(rT), timings[i].vAt(rT), -l * normalized, timings[i]);
                        }
                    }
                }
            }
        }

        private static int ToKey(double x)
        {
            return (int)Math.Round(x * PRECISION);
        }

        private static float FromKey(int x)
        {
            return x / PRECISION;
        }

        public PointFunction(float p)
        {
            timings[ToKey(p)] = new Path(0, Vector2.Zero, Vector2.Zero, Vector2.Zero, null);
        }

        public PointFunction()
        {
        }

        internal double improved(PointFunction newTimings)
        {
            double improved = double.PositiveInfinity;
            foreach (var pair in newTimings.timings)
            {
                if (!timings.ContainsKey(pair.Key) || timings[pair.Key].totalTime > pair.Value.totalTime)
                {
                    timings[pair.Key] = pair.Value;
                    improved = Math.Min(pair.Value.totalTime, improved);
                }
            }
            return improved;
        }

        internal static PointFunction Return(Geometry.Point p1, Geometry.Point p2, PointFunction old, float accel, float gravity)
        {
            // either you accelerate fully away and then return to get a higher speed, or you try to backpedel and then brace yourself to get a lower speed
            PointFunction answer = new PointFunction();
            Vector2 path = ((Vector2)p2 - (Vector2)p1);
            Vector2 normalized = path;
            normalized.Normalize();
            float g = gravity * path.Y / path.Length();
            // r and l have been specifically framed this way, and affect several equations
            float r = g + accel;
            float l = -g + accel;
            float d = (p1.v.Position - p2.v.Position).Length();
            foreach (var pair in old.timings)
            {
                // is positive if it assists acceleration towards p2 (aka right)
                float b = FromKey(pair.Key);
                // collisions reduce speed
                Vector2 l1 = (Vector2)p2 - (Vector2)p1;
                Vector2 l2 = (Vector2)p1 - pair.Value.posAt(0);
                if (l2 != Vector2.Zero)
                {
                    b *= Math.Abs(Vector2.Dot(l1, l2)) / (l1.Length() * l2.Length());
                }
                // old
                /*int lowerBound = ToKey(b);
                int upperBound = ToKey(Math.Sqrt(Math.Max(b * b + 2 * r * d, 0)));
                float newPartialTime = 2 * b / l;
                if (true)
                {
                    float newT = newPartialTime + pair.Value.totalTime;
                    int i = ToKey(b);
                    if (!answer.timings.ContainsKey(i) || answer.timings[i].totalTime > newT)
                    {
                        answer.timings[i] = new Path(newT, p1, b * normalized, -l * normalized, pair.Value);
                    }
                 }*/
                // if trying to speed up first
                Parabola fullSpeedPath = new Parabola(r, b, 0);
                Parabola fullRetreatPath = new Parabola(-l, b, 0);
                Paraboloid speedThenRetreat = fullSpeedPath.FollowedBy(fullRetreatPath);
                int lowerBound = ToKey(b);
                int upperBound = ToKey(Math.Sqrt(2*l*d));
                //int upperBound = ToKey();
                for (int i = lowerBound; i <= upperBound; i++)
                {
                    float f = FromKey(i);
                    float newPartialTime = speedThenRetreat.SpecialShit(0, -f, -1);
                    if (newPartialTime >= 0)
                    {
                        float newT = newPartialTime + pair.Value.totalTime;
                        if (!answer.timings.ContainsKey(i) || answer.timings[i].totalTime > newT)
                        {
                            //float rT = newPartialTime * 0.27f;
                            //float rT = (l * newPartialTime + f - b) / (l + r);
                            //f=b+rt-lg
                            //T=t+g
                            //lT+f=b+rt+lt
                            //t=(lT+f-b)/(l+r)
                            float rT = (l * newPartialTime - f - b) / (l + r);
                            float lT = newPartialTime - rT;
                            answer.timings[i] = new Path(newT - lT, p1, b * normalized, r * normalized, pair.Value);
                            answer.timings[i] = new Path(newT, answer.timings[i].posAt(rT), answer.timings[i].vAt(rT), -l * normalized, answer.timings[i]);
                        }
                    }
                }
            }
            return answer;
        }
    }
}
