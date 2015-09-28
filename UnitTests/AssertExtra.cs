using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    public static class AssertExtra
    {
        public static void AreApproximate(double expected, double actual)
        {
            if (Math.Abs(expected - actual)/expected > 0.1)
            {
                Assert.AreEqual(expected, actual);
            }
        }
    }
}
