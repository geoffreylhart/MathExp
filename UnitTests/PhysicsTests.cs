using MathExp;
using MathExp.Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestClass]
    public class PhysicsTests
    {
        [TestMethod]
        public void TestAngledImpacts()
        {
            // Try all four rotations around the origin
            foreach(int xSign in new []{-1, 1})
            {
                foreach (int ySign in new[] { -1, 1 })
                {
                    GeometryCollection geometry = new GeometryCollection();
                    Line line = new Line(new Vector2(0, 0), new Vector2(xSign, ySign));
                    geometry.Add(line);
                    // position just 0.5 above the line
                    Particle p = new Particle(new Vector2(0.5f * xSign, (ySign + 1) / 2), new Vector2(0, -1), new Vector2(0, -0.1f));
                    BoundParticle impact = geometry.FirstCollision(p);
                    Assert.AreEqual<Line>(line, impact.boundTo);
                    AssertExtra.AreApproximate(0.5, impact.g);
                    AssertExtra.AreApproximate(-ySign * 0.5, impact.gv);
                    AssertExtra.AreApproximate(-ySign * 0.5 / 10, impact.ga);
                }
            }
        }
    }
}
