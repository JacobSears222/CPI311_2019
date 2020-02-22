using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

using CPI311.GameEngine;

namespace Lab08
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Lab08 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        SoundEffect gunSound;
        SoundEffect soundInstance;

        Model model;
        Texture2D texture;
        Camera camera, topDownCamera;
        List<Transform> transforms;
        List<Collider> colliders;
        List<Camera> cameras;

        Effect effect;

        SphereCollider sphereCollider = new SphereCollider();
        Transform transform = new Transform();

        public Lab08()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Content";

            ScreenManager.Initialize(graphics);
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Time.Initialize();
            InputManager.Initialize();
            ScreenManager.Initialize(graphics);
            IsMouseVisible = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ScreenManager.Setup(true, 1920, 1080);

            gunSound = Content.Load<SoundEffect>("Gun");
            model = Content.Load<Model>("Sphere");
            font = Content.Load<SpriteFont>("Font");
            texture = Content.Load<Texture2D>("Square");
            effect = Content.Load<Effect>("SimpleShading");

            transforms = new List<Transform>();
            colliders = new List<Collider>();
            cameras = new List<Camera>();

            Transform transform = new Transform();
            transform.LocalPosition = new Vector3(0, 0, 0);
            sphereCollider.Transform = new Transform();
            sphereCollider.Radius = .85f;
            sphereCollider.Transform.LocalPosition = new Vector3(-1.26f,1.57f,0);
            colliders.Add(sphereCollider);
            transforms.Add(transform);

            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.LocalPosition = Vector3.Backward * 5;
            camera.Position = new Vector2(0.1f, 0f);
            camera.Size = new Vector2(0.5f, 1f);

            topDownCamera = new Camera();
            topDownCamera.Transform = new Transform();
            topDownCamera.Transform.LocalPosition = Vector3.Up * 10;
            topDownCamera.Transform.Rotate(Vector3.Right, -MathHelper.PiOver2);
            topDownCamera.Position = new Vector2(0.5f, 0f);
            topDownCamera.Size = new Vector2(0.5f, 1f);
            

            cameras.Add(topDownCamera);
            cameras.Add(camera);

            foreach (ModelMesh mesh in model.Meshes)
                foreach (BasicEffect effect in mesh.Effects)
                    effect.EnableDefaultLighting();
        }

        protected override void UnloadContent() { }

        protected override void Update(GameTime gameTime)
        {
            Time.Update(gameTime);
            InputManager.Update();

            transforms[0].Rotate(Vector3.UnitY, Time.ElapsedGameTime);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Ray ray = camera.ScreenPointToWorldRay(InputManager.GetMousePosition());
            foreach (Collider collider in colliders)
            {
                if (collider.Intersects(ray) != null)
                {
                    effect.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector3());
                    (model.Meshes[0].Effects[0] as BasicEffect).DiffuseColor = Color.Blue.ToVector3();

                    if (InputManager.IsMousePressed(0))
                    {
                        {
                            SoundEffectInstance instance = gunSound.CreateInstance();
                            instance.IsLooped = false;
                            instance.Volume = 0.4f;
                            instance.Pitch = 0.99f;
                            instance.Play();
                        }
                    }
                }
                else
                {
                    effect.Parameters["DiffuseColor"].SetValue(Color.Blue.ToVector3());
                    (model.Meshes[0].Effects[0] as BasicEffect).DiffuseColor = Color.Red.ToVector3();
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.Viewport = camera.Viewport;

            foreach (Camera camera in cameras)
            {
                GraphicsDevice.DepthStencilState = new DepthStencilState();
                GraphicsDevice.Viewport = camera.Viewport;

                spriteBatch.Begin();
                spriteBatch.DrawString(font, "Sphere?" + sphereCollider.Transform.LocalPosition, new Vector2(10, 0), Color.White);
                spriteBatch.DrawString(font, "Tranform of Sphere?" + transform.LocalPosition, new Vector2(10, 20), Color.White);
                spriteBatch.DrawString(font, "Sphere?" + sphereCollider.Radius, new Vector2(10, 40), Color.White);
                spriteBatch.DrawString(font, "Sphere?" + sphereCollider.Transform, new Vector2(10, 60), Color.White);
                spriteBatch.DrawString(font, "Mouse Pos: " + InputManager.GetMousePosition(), new Vector2(10, 80), Color.White);


                spriteBatch.End();

                foreach (Transform transform in transforms)
                {
                    model.Draw(transform.World, camera.View, camera.Projection);
                }
            }
            base.Draw(gameTime);
        }
    }
}
