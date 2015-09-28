using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathExp.Geometry
{
    public class BoundParticle
    {
        public Line boundTo;
        public Boolean onCW;
        public double g;
        public double gv;
        public double ga;

        public BoundParticle(Line boundTo, bool onCW, double g, double gv, double ga)
        {
            this.boundTo = boundTo;
            this.onCW = onCW;
            this.g = g;
            this.gv = gv;
            this.ga = ga;
        }

        internal void Draw(GraphicsDevice graphicsDevice, BasicEffect basicEffect)
        {
            basicEffect.TextureEnabled = true;
            using (var batch = new SpriteBatch(graphicsDevice))
            {
                batch.Begin(0, null, null, null, null, basicEffect);
                batch.Draw(GlobalTextures.pixelTexture, boundTo.p1 + Vector2.Multiply(((Vector2)boundTo.p2-boundTo.p1), (float)g), Color.Green);
                batch.End();
            }

            basicEffect.TextureEnabled = false;
        }

        internal Point position()
        {
            return (Point)(boundTo.p1+Vector2.Multiply((Vector2)boundTo.p2-boundTo.p1, (float)g));
        }
    }
}
