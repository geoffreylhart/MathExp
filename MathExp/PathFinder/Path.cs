using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathExp.PathFinder
{
    public class Path
    {
        public float totalTime;
        public float time;
        public Path prev;
        Vector2 pos;
        Vector2 v;
        Vector2 a;

        public Path(float totalTime, Vector2 pos, Vector2 v, Vector2 a, Path prev)
        {
            this.totalTime = totalTime;
            this.pos = pos;
            this.v = v;
            this.a = a;
            this.prev = prev;
            this.time = totalTime - ((prev == null) ? 0 : prev.totalTime);
        }

        // these do not use absolute time
        internal Vector2 posAt(float t)
        {
            return pos + Vector2.Multiply(v, t) + Vector2.Multiply(a, t * t / 2);
        }

        // these do not use absolute time
        internal Vector2 vAt(float t)
        {
            return v + Vector2.Multiply(a, t);
        }

        public Vector2 absolutePosAt(float t)
        {
            if(t<totalTime-time)
            {
                return prev.absolutePosAt(t);
            }
            return posAt((float)(t-(totalTime-time)));
        }
    }
}
