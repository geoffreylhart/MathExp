using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathExp
{
    public class Particle
    {
        public Vector2 position, velocity, gravity;

        public Particle(Vector2 p, Vector2 v, Vector2 g)
        {
            position = p;
            velocity = v;
            gravity = g;
        }

        internal void Draw(GraphicsDevice graphicsDevice, BasicEffect basicEffect)
        {
            basicEffect.TextureEnabled = true;
            using (var batch = new SpriteBatch(graphicsDevice))
            {
                batch.Begin(0, null, null, null, null, basicEffect);
                batch.Draw(GlobalTextures.pixelTexture, position, Color.Green);
                batch.End();
            }

            basicEffect.TextureEnabled = false;
        }
    }
}
