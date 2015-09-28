using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathExp.PathFinder
{
    public class Paraboloid
    {
        private float a;
        private float b;
        private float c;
        private float d;
        private float e;
        private float f;

        // of form ax²+by²+cxy+dx+ey+f
        public Paraboloid(float a, float b, float c, float d, float e, float f)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.e = e;
            this.f = f;
        }

        // returns time to target position and velocity, assuming this was constructed especially by the parabola
        // everything may have to be restructered
        public float SpecialShit(float d, float f, float ifError)
        {
            // b=c,d or e, apparently
            float l = -2 * b;
            float r = 2 * a;
            float partial = (l * c * c + 2 * r * l * d + r * f * f) * (l + r);
            if (partial >= 0)
            {
                return (float) Math.Sqrt(partial) / (l * r) - f / l - c / r;
            }else
            {
                return ifError;
            }
        }
    }
}
