using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

using CPI311.GameEngine;

namespace Lab02
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Lab02 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        //Lab02
        Sprite sprite;
        //KeyboardState prevState;
        float phase = 0.0f;

        public Lab02()
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
            //prevState = Keyboard.GetState();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            sprite = new Sprite(Content.Load<Texture2D>("Square"));
            InputManager.Initialize();
            Time.Initialize();

            sprite.Position = new Vector2(200, 200);
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
                Exit();
            //Lab02-1
            /*KeyboardState currentState = Keyboard.GetState();
            if (currentState.IsKeyDown(Keys.Left) &&
                    prevState.IsKeyUp(Keys.Left))
                sprite.Position += Vector2.UnitX * -5;
            if (currentState.IsKeyDown(Keys.Right) &&
                    prevState.IsKeyUp(Keys.Right))
                sprite.Position += Vector2.UnitX * 5;
            if (currentState.IsKeyDown(Keys.Up) &&
                   prevState.IsKeyUp(Keys.Up))
                sprite.Position += Vector2.UnitY * -5;
            if (currentState.IsKeyDown(Keys.Down) &&
                    prevState.IsKeyUp(Keys.Down))
                sprite.Position += Vector2.UnitY * 5;
            prevState = currentState;
            */

            //Lab02-2
            InputManager.Update();
            Time.Update(gameTime);
            phase += Time.ElapsedGameTime;
            if (InputManager.IsKeyPressed(Keys.Left))
                sprite.Position += Vector2.UnitX * -5;
            if (InputManager.IsKeyPressed(Keys.Right))
                sprite.Position += Vector2.UnitX * 5;
            if (InputManager.IsKeyPressed(Keys.Up))
                sprite.Position += Vector2.UnitY * -5;
            if (InputManager.IsKeyPressed(Keys.Down))
                sprite.Position += Vector2.UnitY * 5;
            if (InputManager.IsKeyDown(Keys.Space))
                sprite.Rotation += 0.05f;

            sprite.Position = new Vector2(200, 200) + new Vector2((float)(phase * 100 * Math.Cos(phase)),
                (float)(phase * 100 * Math.Sin(phase)));

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            sprite.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
