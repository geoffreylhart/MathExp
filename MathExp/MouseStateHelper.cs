using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathExp
{
    public static class MouseStateHelper
    {
        public static Boolean Free(this MouseState state)
        {
            return state.LeftButton == ButtonState.Released && state.RightButton == ButtonState.Released;
        }
    }
}
