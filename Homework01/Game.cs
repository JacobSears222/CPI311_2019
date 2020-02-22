using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;

namespace Homework01
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        ProgressBar timeBar, distanceBar;
        float time = 90f, distanceWalked = 0;
        AnimatedSprite player;
        Sprite bonus;
        SpriteFont font;

        Vector2 playerDestination = Vector2.Zero;
        Vector2 playerDirection = Vector2.UnitX;
        bool isFacing = false;

        Random random = new Random();

        public Game()
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
            Time.Initialize();
            InputManager.Initialize();
            base.Initialize();
            this.IsMouseVisible = true;

            timeBar.Scale = new Vector2(2, 1);
            distanceBar.Scale = new Vector2(2, 1);

            player.Position = new Vector2(300, 100);
            timeBar.Position = new Vector2(50, 50);
            distanceBar.Position = new Vector2(150, 50);
            bonus.Position = new Vector2(400, 400);
        }

        protected override void LoadContent()
        {
            InputManager.Initialize();
            Time.Initialize();

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            bonus = new Sprite(Content.Load<Texture2D>("Square"));
            player = new AnimatedSprite(Content.Load<Texture2D>("explorer"), 8);
            timeBar = new ProgressBar(Content.Load<Texture2D>("Square"), 1, 2);
            distanceBar = new ProgressBar(Content.Load<Texture2D>("Square"));
            font = Content.Load<SpriteFont>("Font");

            distanceBar.FillColor = Color.Green;
            timeBar.FillColor = Color.Red;

            //timeBar = Time.ElapsedGameTime
            player.Update();
        }

        protected override void UnloadContent() { }

        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            Time.Update(gameTime);
            InputManager.Update();
            player.Update();

            Vector2 playerPrePos = player.Position;
           
            if (InputManager.IsKeyDown(Keys.Left))
            {
                player.Position += Vector2.UnitX * Time.ElapsedGameTime * -100f;
                AnimatedSprite.currentRow = 2;
            }
            else if (InputManager.IsKeyDown(Keys.Right))
            {
                player.Position -= Vector2.UnitX * Time.ElapsedGameTime * -100f;
                AnimatedSprite.currentRow = 3;
            }

            else if (InputManager.IsKeyDown(Keys.Up))
            {
                player.Position -= Vector2.UnitY * Time.ElapsedGameTime * 100f;
                AnimatedSprite.currentRow = 0;
            }
            if (InputManager.IsKeyDown(Keys.Down))
            {
                player.Position += Vector2.UnitY * Time.ElapsedGameTime * 100f;
                AnimatedSprite.currentRow = 1;
            }
            
            if (Vector2.Distance(player.Position, bonus.Position) < bonus.Texture.Width)
            {
                bonus.Position = new Vector2(random.Next(200), random.Next(600));
                time += 60;
            }

            time = time - Time.ElapsedGameTime;
            timeBar.Value = time / 90f;
            timeBar.Update();

            distanceWalked += Math.Abs((player.Position - playerPrePos).Length());
            distanceBar.Value = distanceWalked / 1000f;
            if(Math.Abs((player.Position - bonus.Position).Length()) < 50)
            {
                time = Math.Min(time + 5, 90);
                int randX = random.Next(0, GraphicsDevice.Viewport.Width);
                int randY = random.Next(0, GraphicsDevice.Viewport.Height);
                bonus.Position = new Vector2(randX, randY);
                timeBar.Update();
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            bonus.Draw(spriteBatch);
            player.Draw(spriteBatch);
            timeBar.Draw(spriteBatch);
            distanceBar.Draw(spriteBatch);
            spriteBatch.DrawString(font, "Time:" + timeBar.Value, new Vector2(20, 10), Color.White);
            spriteBatch.DrawString(font, "Distance:" + distanceBar.Value, new Vector2(120, 10), Color.White);
            spriteBatch.DrawString(font, "*" + player.Position, new Vector2(300, 10), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
