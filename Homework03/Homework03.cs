using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Threading;
using System.Collections.Generic;

using CPI311.GameEngine;

namespace Homework03
{
    public class Homework03 : Game
    {
        GraphicsDeviceManager graphics;

        Camera camera = new Camera();
        Light light = new Light();
        BoxCollider boxCollider;

        List<GameObject> gameObjects = new List<GameObject>();
        
        Model model;

        Random random;
        SpriteFont font;
        Transform cameraTransform;
        Transform transform = new Transform();

        Texture2D texture;
        SpriteBatch spriteBatch;

        bool textureDisplay;
        bool infoDisplay;
        bool collisionsThread;

        int totalCol = 0;
        int numberOfCollisions = 0;

        int FPS;
        float fps;
        int frames;

        float offsetSpeed = 1;
        float tileAmount = 1;

        Effect effect, offset;
        
        int count = 0;

        public Homework03()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Content";
        }      

        protected override void Initialize()
        {
            InputManager.Initialize();
            Time.Initialize();
            ScreenManager.Initialize(graphics);
            random = new Random();
            Transform lightTransform = new Transform();
            light.Transform = lightTransform;
            lightTransform.LocalPosition = Vector3.Up * 20;
            light.Diffuse = Color.Gainsboro;

            ThreadPool.QueueUserWorkItem(new WaitCallback(CollisionReset));

            base.Initialize();
        }     

        protected override void LoadContent()
        {
            ScreenManager.Setup(false, 1920, 1080);

            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("Font");
            model = Content.Load<Model>("Sphere");
            texture = Content.Load<Texture2D>("DrPepper");
            effect = Content.Load<Effect>("SimpleShading");
            offset = Content.Load<Effect>("Offset");

            foreach (ModelMesh mesh in model.Meshes)      
                foreach (BasicEffect e in mesh.Effects)
                    e.EnableDefaultLighting();

            cameraTransform = new Transform();
            cameraTransform.LocalPosition = new Vector3(0, 0, 20);
            camera.Transform = cameraTransform;
 
            random = new Random();

            boxCollider = new BoxCollider();
            boxCollider.Size = 10;

            ThreadPool.QueueUserWorkItem(new WaitCallback(CollisionReset));

            foreach (GameObject gameObject in GameObject.activeGameObjects) gameObject.Update();
                GameObject.gameStarted = true;
        }

        private void AddGameObject()
        {
            GameObject sphere = new GameObject();
            Transform transform = sphere.Transform;
            SphereCollider sphereCollider = new SphereCollider();
            Rigidbody rigidbody = new Rigidbody();


            sphere.Transform.LocalPosition = Vector3.Zero;
            rigidbody.Transform = transform;

            Renderer renderer = new Renderer(model, sphere.Transform, camera, Content, GraphicsDevice, light, 2, "SimpleShading", 20f, texture);

            Vector3 direction = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
            direction.Normalize();

            rigidbody.Mass = 1;
            rigidbody.Velocity = direction * ((float)random.NextDouble() * 5 + 5);
            rigidbody.Mass = 1f + (float)random.NextDouble();

            sphereCollider.Radius = 1 * transform.LocalScale.Y;
            sphereCollider.Transform = transform;

            sphere.Add<Rigidbody>(rigidbody);
            sphere.Add<Collider>(sphereCollider);
            sphere.Add<Renderer>(renderer);

            gameObjects.Add(sphere);
            count++;
        }

        private void RemoveSphere()
        {
            if (gameObjects.Count > 0)
            {
                gameObjects.RemoveAt(gameObjects.Count - 1);
                count--;
            }
        }

        protected override void UnloadContent() { }

        protected override void Update(GameTime gameTime)
        {
            foreach (GameObject gameObjects in gameObjects)
            {
                gameObjects.Update();
            }
            Time.Update(gameTime);
            InputManager.Update();
            frames++;

            fps += (float)Time.ElapsedGameTime;
            if (fps > 1)
            {
                FPS = frames;
                frames = 0;
                fps = 0;
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))

                Exit();

            if (InputManager.IsKeyPressed(Keys.W)) offsetSpeed += 0.25f;
            if (InputManager.IsKeyPressed(Keys.A)) offsetSpeed -= 0.25f;
            if (InputManager.IsKeyPressed(Keys.S)) tileAmount += 1f;
            if (InputManager.IsKeyPressed(Keys.D)) tileAmount -= (tileAmount > 1) ? 1f : 0;

            if (InputManager.IsKeyPressed(Keys.Up)) AddGameObject();           
            if (InputManager.IsKeyPressed(Keys.Down)) RemoveSphere();
            if (InputManager.IsKeyDown(Keys.Left))
            {
                for (int i = 0; i < gameObjects.Count; i++)
                {
                    gameObjects[i].Rigidbody.Velocity *= 0.75f;
                }
            }

            if (InputManager.IsKeyDown(Keys.Right))
            {
                for (int i = 0; i < gameObjects.Count; i++)
                {
                    gameObjects[i].Rigidbody.Velocity *= 1.25f;
                }
            }

            if (InputManager.IsKeyPressed(Keys.LeftShift)) infoDisplay = !infoDisplay;

            if (InputManager.IsKeyPressed(Keys.LeftControl)) collisionsThread = !collisionsThread;

            if (InputManager.IsKeyPressed(Keys.LeftAlt)) textureDisplay = !textureDisplay;


            Vector3 normal;
            for (int i = 0; i < gameObjects.Count; i++)
            {
                if (boxCollider.Collides(gameObjects[i].Collider, out normal))
                {
                    numberOfCollisions++;
                    if (Vector3.Dot(normal, gameObjects[i].Rigidbody.Velocity) < 0)
                        gameObjects[i].Rigidbody.Impulse +=
                           Vector3.Dot(normal, gameObjects[i].Rigidbody.Velocity) * -2 * normal;
                }
                for (int j = i + 1; j < gameObjects.Count; j++)
                {
                    if (gameObjects[i].Collider.Collides(gameObjects[j].Collider, out normal))
                    {
                        if (Vector3.Dot(normal, gameObjects[i].Rigidbody.Velocity) > 0 &&
                           Vector3.Dot(normal, gameObjects[j].Rigidbody.Velocity) < 0)
                            return;                        
                        Vector3 velocityNormal = Vector3.Dot(normal,
                            gameObjects[i].Rigidbody.Velocity - gameObjects[j].Rigidbody.Velocity) * -2
                               * normal * gameObjects[i].Rigidbody.Mass * gameObjects[j].Rigidbody.Mass;
                        gameObjects[i].Rigidbody.Impulse += velocityNormal / 2;
                        gameObjects[j].Rigidbody.Impulse += -velocityNormal / 2;
                    }
                }
            }

            if (textureDisplay)
            {
                for (int i = 0; i < gameObjects.Count; i++)
                {
                    gameObjects[i].Get<Renderer>().CurrentTechnique = 2;
                }
            }
            else
            {
                for (int i = 0; i < gameObjects.Count; i++)
                {
                    gameObjects[i].Get<Renderer>().CurrentTechnique = 0;
                }
            }
            base.Update(gameTime);
        }    

        protected override void Draw(GameTime gameTime)
        {                        
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.DepthStencilState = new DepthStencilState();

            foreach (GameObject gameObject in gameObjects)
                gameObject.Draw();

            offset.Parameters["height"].SetValue((float)ScreenManager.Height);
            offset.Parameters["offset"].SetValue(offsetSpeed * (float)Time.ElapsedGameTime / 1000);
            offset.Parameters["tile"].SetValue(tileAmount);
            offset.CurrentTechnique.Passes[0].Apply();

            spriteBatch.Begin();
            spriteBatch.Draw(texture, new Rectangle(0, 0, (int)(camera.Size.X * ScreenManager.Width), (int)(camera.Size.Y * ScreenManager.Height)), Color.White);
            spriteBatch.DrawString(font, "Ball count:" + count, new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(font, "Collisions: " + totalCol, new Vector2(10, 30), Color.White);
            spriteBatch.DrawString(font, "FPS: " + (int)FPS, new Vector2(10, 50), Color.White);
            spriteBatch.DrawString(font, "Controls WASD for Tiling", new Vector2(10, 70), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void CollisionReset(Object obj)
        {
            while (collisionsThread)
            {
                totalCol = numberOfCollisions;
                numberOfCollisions = 0;
                System.Threading.Thread.Sleep(1000);
            }
        }

        private void ThreadMethod(Object obj)
        {
            frames = 0;
            System.Threading.Thread.Sleep(1000);
        }
    }
}