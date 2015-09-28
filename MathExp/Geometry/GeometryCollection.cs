using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathExp.Geometry
{
    public class GeometryCollection
    {
        private List<Point> points;
        private List<Line> lines;

        public GeometryCollection(List<VertexPositionColor> vertices, List<short> lines)
        {
            points = vertices.Select(v => new Point(v)).ToList();
            this.lines = new List<Line>();
            for (int i = 0; i < lines.Count / 2; i++)
            {
                this.lines.Add(new Line(points[lines[i * 2]], points[lines[i * 2+1]]));
            }
        }

        public GeometryCollection()
        {
            points = new List<Point>();
            lines = new List<Line>();
        }

        public VertexPositionColor[] GetPointsAsArray()
        {
            return points.Select(p => p.v).ToArray();
        }

        public short[] GetLinesAsIndexArray()
        {
            Dictionary<Point, short> hash = new Dictionary<Point,short>();
            for(short i=0; i< points.Count; i++)
            {
                hash.Add(points[i], i);
            }
            short[] index = new short[lines.Count * 2];
            for (int i = 0; i < lines.Count; i++)
            {
                index[i * 2] = hash[lines[i].p1];
                index[i * 2+1] = hash[lines[i].p2];
            }
            return index;
        }

        public Point SnapToClosePoint(Vector3 v)
        {
            Point answer = null;
            float minDistance = float.MaxValue;
            for (int i = 0; i < points.Count; i++)
            {
                float distance = Vector3.Distance(points[i].v.Position, v);
                if (distance < 10 && distance < minDistance)
                {
                    minDistance = distance;
                    answer = points[i];
                }
            }
            return answer;
        }

        public void Add(Point selected, Point selected2)
        {
            if(selected != null && selected2 != null && selected!=selected2)
            {
                lines.Add(new Line(selected, selected2));
                if (!points.Contains(selected))
                {
                    points.Add(selected);
                }
                if (!points.Contains(selected2))
                {
                    points.Add(selected2);
                }
            }
        }

        public void Draw(GraphicsDevice graphicsDevice, BasicEffect basicEffect)
        {
            if (this.lines.Count > 0)
            {
                foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                        PrimitiveType.LineList,
                        this.GetPointsAsArray(),
                        0,  // vertex buffer offset to add to each element of the index buffer
                        this.points.Count,  // number of vertices in pointList
                        this.GetLinesAsIndexArray(),  // the index buffer
                        0,  // first index element to read
                        this.lines.Count   // number of primitives to draw
                    );
                }
            }

            basicEffect.TextureEnabled = true;
            using (var batch = new SpriteBatch(graphicsDevice))
            {
                batch.Begin(0, null, null, null, null, basicEffect);
                foreach (var vertex in this.GetPointsAsArray())
                {
                    batch.Draw(GlobalTextures.pixelTexture, new Vector2(vertex.Position.X - 3, vertex.Position.Y - 3), vertex.Color);
                }
                batch.End();
            }

            basicEffect.TextureEnabled = false;
        }

        public void Add(Point vertexPositionColor)
        {
            if (!points.Contains(vertexPositionColor))
            {
                points.Add(vertexPositionColor);
            }
        }

        public void Remove(Point selected)
        {
            points.Remove(selected);
            lines = lines.Where(l => l.p1 != selected && l.p2 != selected).ToList();
        }

        public void Remove(Line selected)
        {
            lines.Remove(selected);
        }

        public void Add(Line line)
        {
            Add(line.p1);
            Add(line.p2);
            if (!lines.Contains(line))
            {
                lines.Add(line);
            }
        }

        public BoundParticle FirstCollision(Particle particle)
        {
            var v = particle.velocity;
            var p = particle.position;
            if (v.X == 0 && v.Y == 0) return null;
            BoundParticle collidedWith = null;
            double minT = Double.MaxValue;
            foreach (Line line in lines)
            {
                // calculate t, the amount of time it would take to collide
                // p+v*t=line.p1+(line.p2-line.p1)*g
                // p.x+v.x*t=line.p1.x+(line.p2.x-line.p1.x)*g
                // p.y+v.y*t=line.p1.y+(line.p2.y-line.p1.y)*g
                
                // (line.p1.x+(line.p2.x-line.p1.x)*g)*v.y-(line.p1.y+(line.p2.y-line.p1.y)*g)*v.x=p.x*v.y-p.y*v*x
                // g=(p.x*v.y-p.y*v.x-v.y*line.p1.x+line.p1.y*v.x)/((line.p2.x-line.p1.x)*v.y-(line.p2.y-line.p1.y)*v.x)
                double g = (p.X*v.Y-p.Y*v.X-v.Y*line.p1.v.Position.X+line.p1.v.Position.Y*v.X)/((line.p2.v.Position.X-line.p1.v.Position.X)*v.Y-(line.p2.v.Position.Y-line.p1.v.Position.Y)*v.X);
                double t = (line.p1.v.Position.Y + (line.p2.v.Position.Y - line.p1.v.Position.Y) * g - p.Y) / v.Y;
                if(t>=0 && g>=0 && g<=1 && t<minT && t<=1)
                {
                    minT = t;
                    double gv = Vector2.Dot(v, (Vector2)line.p2 - line.p1) / (((Vector2)line.p2 - line.p1).LengthSquared());
                    double ga = Vector2.Dot(particle.gravity, line) / ((Vector2)line).LengthSquared();
                    collidedWith = new BoundParticle(line, v.CrossProduct(line) > 0, g, gv, ga);
                }
            }
            return collidedWith;
        }

        public IEnumerable<Line> LinesAttachedTo(Point point)
        {
            return lines.Where(l => l.p1.Equals(point) || l.p2.Equals(point));
        }
    }
}
