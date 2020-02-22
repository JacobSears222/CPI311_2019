using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

using CPI311.GameEngine;
namespace Lab03
{
    public class Lab03 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Lab 3-1
        Model model; //3D model
        Matrix world, view, projection;
        SpriteFont font;

        //Control Variable
        Vector3 cameraPosition = new Vector3(0, 0, 5);
        Vector3 torusPosition = new Vector3(0, 0, 0);
        Vector3 torusRotation = new Vector3(0, 0, 0);
        Vector3 torusScale = new Vector3(1, 1, 1);
        bool isPerspective = true;
    

        public Lab03()
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
            //Start game engine
            InputManager.Initialize();
            Time.Initialize();
            //
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
            InputManager.Initialize();
            Time.Initialize();

            model = Content.Load<Model>("Torus");
            font = Content.Load<SpriteFont>("Font");
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

            //Game engine update
            InputManager.Update();
            Time.Update(gameTime);

            if (InputManager.IsKeyDown(Keys.Delete)) torusRotation.X -= 0.05f;

            if (InputManager.IsKeyDown(Keys.Left)) torusPosition += Vector3.Right * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.Right)) torusPosition += Vector3.Left * Time.ElapsedGameTime;

            if (InputManager.IsKeyDown(Keys.Up) && !InputManager.IsKeyDown(Keys.LeftShift)) torusPosition  += Vector3.Forward * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.Down) && !InputManager.IsKeyDown(Keys.LeftShift)) torusPosition += Vector3.Backward * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.Up) && InputManager.IsKeyDown(Keys.LeftShift)) torusScale += Vector3.One * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.Down)&& InputManager.IsKeyDown(Keys.LeftShift)) torusScale -= Vector3.One * Time.ElapsedGameTime;

            if (InputManager.IsKeyDown(Keys.W)) cameraPosition += Vector3.Up * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.S)) cameraPosition += Vector3.Down * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.A)) cameraPosition += Vector3.Right * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.D)) cameraPosition += Vector3.Left * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.Tab)) isPerspective = !isPerspective;

            view = Matrix.CreateLookAt(cameraPosition, cameraPosition + Vector3.Forward, Vector3.Up);
            if (isPerspective)
                projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 1.33f, 0.1f, 1000f);
            else
                projection = Matrix.CreateOrthographicOffCenter(-1, 1, -1, 1, 0.1f, 100f);

            world = Matrix.CreateScale(torusScale) * Matrix.CreateFromYawPitchRoll(torusRotation.X, torusRotation.Y, torusRotation.Z) * Matrix.CreateTranslation(torusPosition);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            model.Draw(world, view, projection);

            spriteBatch.Begin();
            spriteBatch.DrawString(font, "CameraPosition" + cameraPosition, new Vector2(10, 10), Color.White); 
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
