using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathExp.PathFinder;
using MathExp.Geometry;
using Microsoft.Xna.Framework;

namespace UnitTests
{
    [TestClass]
    public class PathFinderTests
    {
        [TestMethod]
        public void TestDisconnectReturnsNull()
        {
            GeometryCollection geometry = new GeometryCollection();
            Line line = new Line(new Vector2(-1, 0), new Vector2(1, 0));
            geometry.Add(line);
            geometry.Add(new Vector2(2, 0));
            BoundParticle particle = new BoundParticle(line, false, 0.5, 0, 0);
            Path path = PathFinding.PathTo(particle, new Vector2(2, 0), geometry, 0.1f, 0.1f);
            Assert.IsNull(path);
        }

        [TestMethod]
        public void TestBowlSlowdown()
        {
            GeometryCollection geometry = new GeometryCollection();
            Line line = new Line(new Vector2(1, 0), new Vector2(-1, 0));
            geometry.Add(line);
            geometry.Add(new Line(new Vector2(-2, 1), new Vector2(-1, 0)));
            geometry.Add(new Line(new Vector2(2, 1), new Vector2(1, 0)));
            // Shape looks like \_._/*
            BoundParticle particle = new BoundParticle(line, false, 0.5, 0, 0);
            Path path = PathFinding.PathTo(particle, new Vector2(2, 1), geometry, 0.1f, -0.1f);
            // Exact answer: √(80+120√2)-√40
            AssertExtra.AreApproximate(9.4775213623, path.totalTime);
        }

        [TestMethod]
        // gravity is high enough such that swinging back on the flat platform is necessary
        public void TestBowlSwing()
        {
            GeometryCollection geometry = new GeometryCollection();
            Line line = new Line(new Vector2(1, 0), new Vector2(-1, 0));
            geometry.Add(line);
            geometry.Add(new Line(new Vector2(-2, 1), new Vector2(-1, 0)));
            geometry.Add(new Line(new Vector2(2, 1), new Vector2(1, 0)));
            // Shape looks like \_._/*
            BoundParticle particle = new BoundParticle(line, false, 0.5, 0, 0);
            Path path = PathFinding.PathTo(particle, new Vector2(2, 1), geometry, 0.1f, -0.2f);
            Assert.IsNotNull(path);
        }

        [TestMethod]
        public void TestSingleFlatLine()
        {
            GeometryCollection geometry = new GeometryCollection();
            Line line = new Line(new Vector2(-1, 0), new Vector2(1, 0));
            geometry.Add(line);
            BoundParticle particle = new BoundParticle(line, false, 0.5, 0, 0);
            Path path = PathFinding.PathTo(particle, new Vector2(1, 0), geometry, 0.1f, 0.1f);
            AssertExtra.AreApproximate(2 * Math.Sqrt(10), path.totalTime);
            for (float t = 0; t < path.totalTime; t += 0.01f)
            {
                Vector2 pos = path.absolutePosAt(t);
                AssertExtra.AreApproximate(0, pos.Y);
                if (t < Math.Sqrt(10))
                {
                    // when speeding up to approach
                    AssertExtra.AreApproximate(0.1 * t * t / 2, pos.X);
                }else
                {
                    // when slowing down to stop
                    double g = t - Math.Sqrt(10);
                    AssertExtra.AreApproximate(-0.1 * g * g / 2 + Math.Sqrt(10)*0.1*g+0.5, pos.X);
                }
            }
        }

        [TestMethod]
        public void TestMultipleFlatLines()
        {
            int n = 10; // half the total number of points
            GeometryCollection geometry = new GeometryCollection();
            Line line = new Line(new Vector2(-1, 0), new Vector2(1, 0));
            geometry.Add(line);
            MathExp.Geometry.Point target = null;
            for(int i=1; i<n; i++)
            {
                MathExp.Geometry.Point rightMostPoint = new Vector2(i + 1, 0);
                Line leftLine = new Line(new Vector2(i, 0), rightMostPoint);
                Line rightLine = new Line(new Vector2(-i, 0), new Vector2(-i - 1, 0));
                geometry.Add(leftLine);
                geometry.Add(rightLine);
                target = rightMostPoint;
            }
            BoundParticle particle = new BoundParticle(line, false, 0.5, 0, 0);
            Path path = PathFinding.PathTo(particle, target, geometry, 0.1f, 0.1f);
            AssertExtra.AreApproximate(path.totalTime, 2 * Math.Sqrt(10 * n));
            for (float t = 0; t < path.totalTime; t += 0.01f)
            {
                Vector2 pos = path.absolutePosAt(t);
                AssertExtra.AreApproximate(0, pos.Y);
                if (t < Math.Sqrt(10*n))
                {
                    // when speeding up to approach
                    AssertExtra.AreApproximate(0.1 * t * t / 2, pos.X);
                }
                else
                {
                    // when slowing down to stop
                    double g = t - Math.Sqrt(10*n);
                    AssertExtra.AreApproximate(-0.1 * g * g / 2 + Math.Sqrt(10*n) * 0.1 * g + 0.5*n, pos.X);
                }
            }
        }

        [TestMethod]
        public void TestEssentialMath()
        {
            Parabola fullSpeedPath = new Parabola(1, 0, 0);
            Parabola fullRetreatPath = new Parabola(-1, 0, 0);
            Paraboloid speedThenRetreat = fullSpeedPath.FollowedBy(fullRetreatPath);
            float newPartialTime = speedThenRetreat.SpecialShit(1, 0, -1);
            AssertExtra.AreApproximate(newPartialTime, 2);
            float newPartialTime2 = speedThenRetreat.SpecialShit(0, 0, -1);
            AssertExtra.AreApproximate(newPartialTime2, 0);
            float newPartialTime3 = speedThenRetreat.SpecialShit(0, -1, -1);
            AssertExtra.AreApproximate(newPartialTime3, 1 + Math.Sqrt(2));
            float rT = (newPartialTime3 - 1) / (2);
            float lT = newPartialTime3 - rT;
            AssertExtra.AreApproximate(rT, Math.Sqrt(0.5));
            AssertExtra.AreApproximate(lT, 1 + Math.Sqrt(0.5));
        }
    }
}
