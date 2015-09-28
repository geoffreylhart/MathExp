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
    public class GeometryTests
    {
        [TestMethod]
        public void TestLinesAttaching()
        {
            GeometryCollection geometry = new GeometryCollection();
            geometry.Add(new Line(new Vector2(-1, 0), new Vector2(0, 0)));
            geometry.Add(new Line(new Vector2(0, 0), new Vector2(1, 0)));
            Assert.AreEqual(1, geometry.LinesAttachedTo(new Vector2(-1, 0)).Count());
            Assert.AreEqual(2, geometry.LinesAttachedTo(new Vector2(0, 0)).Count());
            Assert.AreEqual(1, geometry.LinesAttachedTo(new Vector2(1, 0)).Count());
        }

        [TestMethod]
        public void TestPointHashing()
        {
            GeometryCollection geometry = new GeometryCollection();
            geometry.Add(new Vector2(0, 0));
            geometry.Add(new Vector2(0, 0));
            Assert.AreEqual(1, geometry.GetPointsAsArray().Length);
        }

        [TestMethod]
        public void TestLineHashing()
        {
            GeometryCollection geometry = new GeometryCollection();
            geometry.Add(new Line(new Vector2(-1, 0), new Vector2(1, 0)));
            geometry.Add(new Line(new Vector2(-1, 0), new Vector2(1, 0)));
            geometry.Add(new Line(new Vector2(1, 0), new Vector2(-1, 0)));
            geometry.Add(new Line(new Vector2(1, 0), new Vector2(-1, 0)));
            Assert.AreEqual(2, geometry.GetPointsAsArray().Length);
            Assert.AreEqual(2, geometry.GetLinesAsIndexArray().Length);
        }
    }
}
