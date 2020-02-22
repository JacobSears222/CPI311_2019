using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;

namespace Homework2
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Homework2 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        Model earth, sun, moon, mercury, ground, firstPersonModel;

        Transform earthTransform = new Transform();
        Transform sunTransform = new Transform();
        Transform moonTransform = new Transform();
        Transform mercuryTransform = new Transform();
        Transform groundTransform = new Transform();

        Transform cameraTransform = new Transform();
        Transform firstPersonTransform = new Transform();
        Transform thirdPersonTransform = new Transform();
        Transform firstPersonModelTransform = new Transform();

        Transform firstPersonCameraTransform = new Transform();

        Camera firstPerson = new Camera();
        Camera thirdPerson = new Camera();

        Matrix projection;
        float fieldOfView = MathHelper.PiOver2;

        float sunRotation = .2f;
        float earthRotation = 1;
        float moonRotation = 1;

        bool isFirstPerson = false;

        Random random = new Random();

        public Homework2()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
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

            groundTransform.LocalPosition = new Vector3(0, -10, 0);
            groundTransform.LocalScale = Vector3.One * 10f;

            sunTransform.LocalPosition = new Vector3(0, 0, 0);
            sunTransform.LocalScale = Vector3.One * 5f;

            mercuryTransform.LocalPosition = new Vector3(3, 0, 0);
            mercuryTransform.LocalScale = Vector3.One * 2f / 5f;

            earthTransform.LocalPosition = new Vector3(7, 0, 0);
            earthTransform.LocalScale = Vector3.One * 3f / 5f;

            moonTransform.LocalPosition = new Vector3(3f, 0, 0);
            moonTransform.LocalScale = Vector3.One * 1f / 5f;

            firstPersonTransform.LocalPosition = new Vector3(1, 0 ,16);

            firstPerson.Transform = firstPersonTransform;
            firstPersonCameraTransform.LocalPosition = firstPersonTransform.LocalPosition;

            firstPersonModelTransform.LocalPosition = firstPersonTransform.LocalPosition;

            thirdPersonTransform.LocalPosition = Vector3.Up * 50;
            thirdPersonTransform.Rotate(Vector3.Right, MathHelper.PiOver2);

            //thirdPersonTransform.LocalPosition = new Vector3(0, 50, 50);
            //thirdPersonTransform.Rotate(Vector3.Right, MathHelper.PiOver2);
            thirdPerson.Transform = thirdPersonTransform;
            thirdPerson.Transform.Rotate(Vector3.UnitX, MathHelper.Pi);


            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            sun = Content.Load<Model>("Sphere");
            earth = Content.Load<Model>("Sphere");
            moon = Content.Load<Model>("Sphere");
            mercury = Content.Load<Model>("Sphere");
            font = Content.Load<SpriteFont>("Font");
            firstPersonModel = Content.Load<Model>("Sphere");
        
            ground = Content.Load<Model>("Plane");

            firstPersonModelTransform.LocalPosition = firstPersonTransform.LocalPosition;

            mercuryTransform.Parent = sunTransform;
            earthTransform.Parent = sunTransform;
            moonTransform.Parent = earthTransform;

            // Set the mouse
            this.IsMouseVisible = true;
        }

        protected override void UnloadContent() { }

        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Time.Update(gameTime);
            InputManager.Update();

            float speed = 5;

            projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, 1.33f, 0.1f, 100f); 

            earthTransform.Rotate(Vector3.Up, earthRotation * Time.ElapsedGameTime);

            sunTransform.Rotate(Vector3.Up, sunRotation * Time.ElapsedGameTime);

            moonTransform.Rotate(Vector3.Up, moonRotation * Time.ElapsedGameTime);

            if (InputManager.IsKeyPressed(Keys.Tab))
            {
                isFirstPerson = !isFirstPerson;
                firstPersonModelTransform.LocalPosition = firstPersonCameraTransform.LocalPosition;
            }


            if (isFirstPerson)
                firstPersonModelTransform.LocalPosition = firstPersonCameraTransform.LocalPosition;

            if (!isFirstPerson)
            {
                if (InputManager.IsKeyDown(Keys.W)) thirdPersonTransform.LocalPosition += Vector3.Forward * Time.ElapsedGameTime * speed;
                if (InputManager.IsKeyDown(Keys.S)) thirdPersonTransform.LocalPosition += Vector3.Backward * Time.ElapsedGameTime * speed;
                if (InputManager.IsKeyDown(Keys.A)) thirdPersonTransform.LocalPosition += Vector3.Left * Time.ElapsedGameTime * speed;
                if (InputManager.IsKeyDown(Keys.D)) thirdPersonTransform.LocalPosition += Vector3.Right * Time.ElapsedGameTime * speed;
            }
            else
            {
                if (InputManager.IsKeyDown(Keys.W)) firstPersonTransform.LocalPosition += Vector3.Forward * Time.ElapsedGameTime * speed;
                if (InputManager.IsKeyDown(Keys.S)) firstPersonTransform.LocalPosition += Vector3.Backward * Time.ElapsedGameTime * speed;
                if (InputManager.IsKeyDown(Keys.A)) firstPersonTransform.LocalPosition += Vector3.Left * Time.ElapsedGameTime * speed;
                if (InputManager.IsKeyDown(Keys.D)) firstPersonTransform.LocalPosition += Vector3.Right * Time.ElapsedGameTime * speed;
            }

            if (InputManager.IsKeyDown(Keys.Left)) firstPersonTransform.Rotate(Vector3.Up, sunRotation * Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.Right)) firstPersonTransform.Rotate(Vector3.Down, sunRotation * Time.ElapsedGameTime);
            
            // Mouse Controls
            MouseState state = Mouse.GetState();

                // Get angle
                Vector3 direction = new Vector3(state.Position.X - GraphicsDevice.Viewport.Width / 2, 0, 0);

                if (state.LeftButton == ButtonState.Pressed)
                    firstPerson.Transform.LocalPosition += Vector3.Normalize(firstPerson.Transform.Forward + direction / 100) / 2;
                if (state.RightButton == ButtonState.Pressed)
                    firstPerson.Transform.LocalPosition += Vector3.Normalize(firstPerson.Transform.Backward + direction / 100) / 2;

            // For both cameras
            if (InputManager.IsKeyDown(Keys.LeftShift))
            {
                if (firstPerson.FieldOfView <= MathHelper.PiOver2)
                {
                    firstPerson.FieldOfView += .5f * Time.ElapsedGameTime;
                    thirdPerson.FieldOfView += .5f * Time.ElapsedGameTime;
                }
            }
            if (InputManager.IsKeyDown(Keys.Space))
            {
                if (firstPerson.FieldOfView >= .5f)
                {
                    firstPerson.FieldOfView -= .5f * Time.ElapsedGameTime;
                    thirdPerson.FieldOfView -= .5f * Time.ElapsedGameTime;
                }
            }

            if (InputManager.IsKeyPressed(Keys.Q)) sunRotation -= 0.1f;
            if (InputManager.IsKeyPressed(Keys.E)) sunRotation += 0.1f;

            base.Update(gameTime);
        }

        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            
            Matrix view = firstPerson.View;
            Matrix projection = firstPerson.Projection;
            firstPersonModelTransform.LocalPosition = new Vector3(0,5,0);


            if (!isFirstPerson)
            {
                firstPersonModel.Draw(firstPersonModelTransform.World, firstPerson.View, projection);
                ground.Draw(groundTransform.World, thirdPerson.View, projection);
                sun.Draw(sunTransform.World, thirdPerson.View, projection);
                earth.Draw(earthTransform.World, thirdPerson.View, projection);
                moon.Draw(moonTransform.World, thirdPerson.View, projection);
                mercury.Draw(mercuryTransform.World, thirdPerson.View, projection);
                firstPersonModel.Draw(firstPersonModelTransform.World, firstPerson.View, projection);
            }
            else
            {
                ground.Draw(groundTransform.World, firstPerson.View, projection);
                sun.Draw(sunTransform.World, firstPerson.View, projection);
                earth.Draw(earthTransform.World, firstPerson.View, projection);
                moon.Draw(moonTransform.World, firstPerson.View, projection);
                mercury.Draw(mercuryTransform.World, firstPerson.View, projection);
            }

            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Moon Rotation " + moonTransform.Rotation, new Vector2(0, 10), Color.Black);
            spriteBatch.DrawString(font, "Moon Position " + moonTransform.Position, new Vector2(0, 30), Color.Black);
            spriteBatch.DrawString(font, "Earth Rotation " + earthTransform.Rotation, new Vector2(0, 50), Color.Black);
            spriteBatch.DrawString(font, "Earth Position " + moonTransform.Position, new Vector2(0, 70), Color.Black);
            spriteBatch.DrawString(font, "Mercury Position " + moonTransform.Position, new Vector2(0, 90), Color.Black);
            spriteBatch.DrawString(font, "Sun Rotation " + sunTransform.Rotation, new Vector2(0, 110), Color.Black);
            spriteBatch.DrawString(font, "Sun Position " + sunTransform.Position, new Vector2(0, 130), Color.Black);
            spriteBatch.DrawString(font, "Sphere? " + firstPersonModelTransform.LocalPosition, new Vector2(0, 150), Color.Black);
            spriteBatch.DrawString(font, "1st Person Camera " + firstPerson.Transform.LocalPosition, new Vector2(0, 170), Color.Black);
            spriteBatch.DrawString(font, "3rd Person Camera " + thirdPersonTransform.LocalPosition, new Vector2(0, 190), Color.Black);
            spriteBatch.DrawString(font, "WASD for moving planets", new Vector2(600, 10), Color.Black);
            spriteBatch.DrawString(font, "LeftShift for increasing FOV", new Vector2(600, 30), Color.Black);
            spriteBatch.DrawString(font, "Space for decreasing FOV", new Vector2(600, 50), Color.Black);
            spriteBatch.DrawString(font, "Q/E for increasing/decreasing sun rotation speed", new Vector2(600, 70), Color.Black);
            spriteBatch.DrawString(font, "Tab to change views", new Vector2(600, 90), Color.Black);
            spriteBatch.DrawString(font, "Left/right to rotate", new Vector2(600, 110), Color.Black);
            spriteBatch.DrawString(font, "Mouse left click/right click to increase/decrease FOV", new Vector2(600, 130), Color.Black);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}