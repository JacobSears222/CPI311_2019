using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Threading;

using CPI311.GameEngine;

namespace Lab07
{
    public class Lab07 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        BoxCollider boxCollider;
        Random random;

        List<Transform> transforms;
        List<Rigidbody> rigidbodies;
        List<Collider> colliders;
        List<Renderer> renderers;

        Transform cameraTransform;
        Camera camera;
        Model model;
        SpriteFont font;
        Light light;

        bool haveThreadRunning = true;
        int lastSecondCollisions = 0;
        int numberOfCollisions = 0;

        public Lab07()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Time.Initialize();
            InputManager.Initialize();

            random = new Random();
            transforms = new List<Transform>();
            rigidbodies = new List<Rigidbody>();
            colliders = new List<Collider>();
            renderers = new List<Renderer>();
            boxCollider = new BoxCollider();
            boxCollider.Size = 10;

            for (int i = 0; i < 2; i++)
            {
                Transform transform = new Transform();
                transform.LocalPosition += new Vector3(i * 5, 0, 0); //avoid overlapping each sphere 
                Rigidbody rigidbody = new Rigidbody();
                rigidbody.Transform = transform;
                rigidbody.Mass = 1;

                Vector3 direction = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
                direction.Normalize();
                rigidbody.Velocity = direction * ((float)random.NextDouble() * 5 + 5);
                SphereCollider sphereCollider = new SphereCollider();
                sphereCollider.Radius = 2.5f * transform.LocalScale.Y;
                sphereCollider.Transform = transform;

                transforms.Add(transform);
                colliders.Add(sphereCollider);
                rigidbodies.Add(rigidbody);

                ThreadPool.QueueUserWorkItem(new WaitCallback(CollisionReset));

                base.Initialize();
            }
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Font");
            model = Content.Load<Model>("Sphere");
            Texture2D texture = Content.Load<Texture2D>("Square");
            AddSphere();

            foreach (ModelMesh mesh in model.Meshes)
                foreach (BasicEffect effect in mesh.Effects)
                    effect.EnableDefaultLighting();

            cameraTransform = new Transform();
            cameraTransform.LocalPosition = Vector3.Backward * 20;
            camera = new Camera();
            camera.Transform = cameraTransform;

            transforms[0].LocalPosition = new Vector3(0, -9.5f, 0);
            rigidbodies[0].Velocity = new Vector3(1, 1, 0);

            ThreadPool.QueueUserWorkItem(new WaitCallback(CollisionReset));
        }

        protected override void UnloadContent() { }

        protected override void Update(GameTime gameTime)
        {
            Time.Update(gameTime);
            InputManager.Update();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (InputManager.IsKeyPressed(Keys.Space))
                AddSphere();

            foreach (Rigidbody rigidbody in rigidbodies) rigidbody.Update();

            Vector3 normal; //Updates if a collision occurs
            for (int i = 0; i < transforms.Count; i++)
            {
                if (boxCollider.Collides(colliders[i], out normal))
                {
                    numberOfCollisions++;
                    if (Vector3.Dot(normal, rigidbodies[i].Velocity) < 0)
                        rigidbodies[i].Impulse +=
                           Vector3.Dot(normal, rigidbodies[i].Velocity) * -2 * normal;
                }
                for (int j = i + 1; j < transforms.Count; j++)
                {
                    if (colliders[i].Collides(colliders[j], out normal))
                    {
                        if (Vector3.Dot(normal, rigidbodies[i].Velocity) > 0 &&
                           Vector3.Dot(normal, rigidbodies[j].Velocity) < 0)     
                            return;
                        numberOfCollisions++;
                        Vector3 velocityNormal = Vector3.Dot(normal,
                            rigidbodies[i].Velocity - rigidbodies[j].Velocity) * -2
                               * normal * rigidbodies[i].Mass * rigidbodies[j].Mass;
                        rigidbodies[i].Impulse += velocityNormal / 2;
                        rigidbodies[j].Impulse += -velocityNormal / 2;
                    }
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.DepthStencilState = new DepthStencilState();

            //for (int i = 0; i < renderers.Count; i++) renderers[i].Draw();
            for (int i = 0; i < transforms.Count; i++)
            {
                Transform transform = transforms[i];
                float speed = rigidbodies[i].Velocity.Length();
                float speedValue = MathHelper.Clamp(speed / 20f, 0, 1);
                (model.Meshes[0].Effects[0] as BasicEffect).DiffuseColor =
                                           new Vector3(speedValue, speedValue, 1);
                model.Draw(transform.World, camera.View, camera.Projection);
            }

            foreach (Transform transform in transforms)
                model.Draw(transform.World, camera.View, camera.Projection);
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Collision: " + lastSecondCollisions, Vector2.Zero, Color.Black);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void AddSphere()
        {
            Transform transform = new Transform();
            transform.Position += Vector3.Zero;
            
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = transform;
            rigidbody.Mass = 1;

            Vector3 direction = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
            direction.Normalize();
            rigidbody.Velocity = direction * ((float)random.NextDouble() * 5 + 5);

            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = 1.0f * transform.LocalScale.Y;
            sphereCollider.Transform = transform;

            transforms.Add(transform);
            colliders.Add(sphereCollider);
            rigidbodies.Add(rigidbody);
            Texture2D texture = Content.Load<Texture2D>("Square");
            Renderer renderer = new Renderer(model, transform, camera, Content, GraphicsDevice, light, 0, "SimpleShading", 20f, texture);
            renderers.Add(renderer);

        }

        private void CollisionReset(Object obj)
        {
            while (haveThreadRunning)
            {
                lastSecondCollisions = numberOfCollisions;
                numberOfCollisions = 0;
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}