using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using CPI311.GameEngine;

namespace Lab06
{
    public class Lab06 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        BoxCollider boxCollider;
        SphereCollider sphere1, sphere2;
        Random random;

        List<Transform> transforms;
        List<Rigidbody> rigidbodies;
        List<Collider> colliders;

        Transform cameraTransform;
        Camera camera;
        Model model;

        public Lab06()
        {
            graphics = new GraphicsDeviceManager(this);
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
            boxCollider = new BoxCollider();
            boxCollider.Size = 10;

            for (int i = 0; i < 2; i++)
            {
                Transform transform = new Transform();
                transform.LocalPosition += new Vector3(i * 5, 0, 0); //avoid overlapping each sphere 
                Rigidbody rigidbody = new Rigidbody();
                rigidbody.Transform = transform;
                rigidbody.Mass = 1;

                Vector3 direction = new Vector3(
                  (float)random.NextDouble(), (float)random.NextDouble(),
                  (float)random.NextDouble());
                direction.Normalize();
                rigidbody.Velocity =
                   direction * ((float)random.NextDouble() * 5 + 5);
                SphereCollider sphereCollider = new SphereCollider();
                sphereCollider.Radius = 2.5f * transform.LocalScale.Y;
                sphereCollider.Transform = transform;

                transforms.Add(transform);
                colliders.Add(sphereCollider);
                rigidbodies.Add(rigidbody);

                base.Initialize();
            }
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            model = Content.Load<Model>("Sphere");

            foreach (ModelMesh mesh in model.Meshes)
                foreach (BasicEffect effect in mesh.Effects)
                    effect.EnableDefaultLighting();

            cameraTransform = new Transform();
            cameraTransform.LocalPosition = Vector3.Backward * 20;
            camera = new Camera();
            camera.Transform = cameraTransform;

            transforms[0].LocalPosition = new Vector3(0, -9.5f, 0);
            rigidbodies[0].Velocity = new Vector3(1, 1, 0);

            boxCollider = new BoxCollider();
            boxCollider.Size = 10;
        }

        protected override void UnloadContent() { }

        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            Time.Update(gameTime);
            InputManager.Update();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            foreach (Rigidbody rigidbody in rigidbodies) rigidbody.Update();

            Vector3 normal; //Updates if a collision occurs
            for (int i = 0; i < transforms.Count; i++)
            {
                if (boxCollider.Collides(colliders[i], out normal))
                {
                    //numberCollisions++;
                    if (Vector3.Dot(normal, rigidbodies[i].Velocity) < 0)
                        rigidbodies[i].Impulse +=
                           Vector3.Dot(normal, rigidbodies[i].Velocity) * -2 * normal;
                }
                for (int j = i + 1; j < transforms.Count; j++)
                {
                    if (colliders[i].Collides(colliders[j], out normal))
                    {
                        //numberCollisions++;
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

            foreach (Transform transform in transforms)
                model.Draw(transform.World, camera.View, camera.Projection);
            spriteBatch.Begin();
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}