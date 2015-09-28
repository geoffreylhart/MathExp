using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathExp
{
    class MouseListener
    {
        private MouseState prevState = Mouse.GetState();

        public Vector3 Transform(MouseState mouseState)
        {
            if (graphicsDevice != null)
            {
                Vector3 ms = new Vector3(mouseState.X / (float)graphicsDevice.Viewport.Width / 0.5f - 1f, 1f - mouseState.Y / (float)graphicsDevice.Viewport.Height / 0.5f, 0);
                ms = Vector3.Transform(ms, Matrix.Invert(projectionMatrix));
                ms = Vector3.Transform(ms, Matrix.Invert(worldMatrix));
                return ms;
            }else
            {
                return new Vector3(mouseState.X, mouseState.Y, 0);
            }
        }

        internal void Update()
        {
            MouseState newState = Mouse.GetState();
            if (prevState.LeftButton == ButtonState.Released && newState.LeftButton == ButtonState.Pressed)
            {
                if (LeftButtonPress != null)
                {
                    LeftButtonPress(Transform(newState));
                }
            }
            if (prevState.LeftButton == ButtonState.Pressed && newState.LeftButton == ButtonState.Released)
            {
                if (LeftButtonRelease != null)
                {
                    LeftButtonRelease(Transform(newState));
                }
            }
            if (prevState.RightButton == ButtonState.Released && newState.RightButton == ButtonState.Pressed)
            {
                if (RightButtonPress != null)
                {
                    RightButtonPress(Transform(newState));
                }
            }
            if (prevState.RightButton == ButtonState.Pressed && newState.RightButton == ButtonState.Released)
            {
                if (RightButtonRelease != null)
                {
                    RightButtonRelease(Transform(newState));
                }
            }
            if(newState.Free() && (newState.X != prevState.X || newState.Y != prevState.Y))
            {
                if (Move != null)
                {
                    Move(Transform(prevState), Transform(newState));
                }
            }
            if (newState.LeftButton == ButtonState.Pressed)
            {
                if (LeftButtonDrag != null)
                {
                    LeftButtonDrag(Transform(prevState), Transform(newState));
                }
            }
            if (newState.RightButton == ButtonState.Pressed)
            {
                if (RightButtonDrag != null)
                {
                    RightButtonDrag(Transform(prevState), Transform(newState));
                }
            }
            prevState = newState;
        }

        public ButtonAction LeftButtonPress;
        public ButtonAction LeftButtonRelease;
        public DragAction LeftButtonDrag;
        public ButtonAction RightButtonPress;
        public ButtonAction RightButtonRelease;
        public DragAction RightButtonDrag;
        public DragAction Move;
        private GraphicsDevice graphicsDevice;
        private Matrix projectionMatrix;
        private Matrix worldMatrix;
        public delegate void ButtonAction(Vector3 pos);
        public delegate void DragAction(Vector3 start, Vector3 end);

        internal void UpdatePerspective(GraphicsDevice GraphicsDevice, Matrix projectionMatrix, Matrix worldMatrix)
        {
            this.graphicsDevice = GraphicsDevice;
            this.projectionMatrix = projectionMatrix;
            this.worldMatrix = worldMatrix;
        }
    }
}
