using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathExp
{
    static class GlobalTextures
    {
        public static Microsoft.Xna.Framework.Graphics.Texture2D pixelTexture;

        public static void LoadContent(ContentManager content)
        {
            pixelTexture = content.Load<Texture2D>("pixel");
        }
    }
}
