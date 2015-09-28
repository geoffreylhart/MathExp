using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathExp.PathFinder
{
    public class Parabola
    {
        private float a;
        private float v;
        private float p;

        public Parabola(float a, float v, float p)
        {
            this.a = a;
            this.v = v;
            this.p = p;
        }

        public float SpeedAt(float d)
        {
            // ½at²+vt+p=d
            // t = (-v±√(v²-2a(p-d)))/a
            // answer = v+at
            // answer = ±√(v²-2a(p-d))
            return (float) Math.Sqrt(v * v + 2 * a * (d-p));
        }

        public float SpeedAt(float d, float ifError)
        {
            float partial = v * v + 2 * a * (d - p);
            if(partial>0)
            {
                return (float) Math.Sqrt(partial);
            }else
            {
                return ifError;
            }
        }

        public Paraboloid FollowedBy(Parabola that)
        {
            Paraboloid combined = new Paraboloid(this.a/2, that.a/2, this.v, this.v, that.v, this.p+ that.p);
            return combined;
        }
    }
}
