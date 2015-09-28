using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace MathExp
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Vector3 cameraOffset, cameraVelocity;
        Matrix viewMatrix, projectionMatrix, worldMatrix;
        float cameraSpeed;
        LineEnvironment environment;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            cameraOffset = Vector3.Zero;
            cameraVelocity = Vector3.Zero;
            cameraSpeed = 5f;

            viewMatrix = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 1.0f), Vector3.Zero, Vector3.Up);
            projectionMatrix = Matrix.CreateOrthographicOffCenter(0, (float)GraphicsDevice.Viewport.Width, (float)GraphicsDevice.Viewport.Height, 0, 1.0f, 1000.0f);
            worldMatrix = Matrix.Identity;
            this.IsMouseVisible = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            environment = new LineEnvironment("test1.txt");
            GlobalTextures.LoadContent(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                environment.Save("test1.txt");
                Exit();
            }

            KeyboardState keyState = Keyboard.GetState();
            cameraVelocity = Vector3.Zero;
            if (keyState.IsKeyDown(Keys.Up))
            {
                cameraVelocity += Vector3.UnitY * cameraSpeed;
            }
            if (keyState.IsKeyDown(Keys.Left))
            {
                cameraVelocity += Vector3.UnitX * cameraSpeed;
            }
            if (keyState.IsKeyDown(Keys.Down))
            {
                cameraVelocity -= Vector3.UnitY * cameraSpeed;
            }
            if (keyState.IsKeyDown(Keys.Right))
            {
                cameraVelocity -= Vector3.UnitX * cameraSpeed;
            }
            cameraOffset += cameraVelocity;
            worldMatrix = Matrix.CreateTranslation(cameraOffset);
            environment.UpdatePerspective(GraphicsDevice, projectionMatrix, worldMatrix);
            environment.Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            BasicEffect basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.View = viewMatrix;
            basicEffect.Projection = projectionMatrix;
            basicEffect.World = worldMatrix;
            basicEffect.VertexColorEnabled = true;
            environment.Draw(GraphicsDevice, basicEffect);
            base.Draw(gameTime);
            spriteBatch.Begin();

            spriteBatch.End();
        }
    }
}
