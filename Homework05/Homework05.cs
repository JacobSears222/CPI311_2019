using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;

namespace Homework05
{
    public class Homework05 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        TerrainRenderer terrain;
        Effect effect;
        Camera camera, topDownCamera;
        Light light;

        SpriteFont font;
        int hits;
        Player player;
        Random random;
        //Make agent a array list
        Agent agent;
        Agent agent1;
        Agent agent2;
        Box box;
        List<Transform> transforms;
        List<Camera> cameras;

        public Homework05()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        protected override void Initialize()
        {
            Time.Initialize();
            InputManager.Initialize();
            ScreenManager.Initialize(graphics);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            transforms = new List<Transform>();

            cameras = new List<Camera>();
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Font");
            terrain = new TerrainRenderer(Content.Load<Texture2D>("mazeH2"), Vector2.One * 100, Vector2.One * 200);
            terrain.NormalMap = Content.Load<Texture2D>("mazeN2");
            terrain.Transform = new Transform();
            terrain.Transform.LocalScale *= new Vector3(1, 5, 1);
            effect = Content.Load<Effect>("TerrainShader");
            effect.Parameters["AmbientColor"].SetValue(new Vector3(0.2f, 0.2f, 0.2f));
            effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.1f, 0.1f, 0.1f));
            effect.Parameters["SpecularColor"].SetValue(new Vector3(0.2f, 0.2f, 0.2f));
            effect.Parameters["Shininess"].SetValue(20f);

            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.LocalPosition += Vector3.Up * 50;
            camera.Transform.Rotate(Vector3.Right, -MathHelper.PiOver2);
            camera.Position = new Vector2(-0.10f, 0f);
            camera.Size = new Vector2(1f, 1f);
            camera.AspectRatio = camera.Viewport.AspectRatio;

            light = new Light();
            light.Transform = new Transform();
            light.Transform.LocalPosition = Vector3.Backward * 5 + Vector3.Right * 5 + Vector3.Up * 5;
            random = new Random();
            player = new Player(terrain, Content, camera, GraphicsDevice, light);
            agent = new Agent(terrain, Content, camera, GraphicsDevice, light, random);
            agent1 = new Agent(terrain, Content, camera, GraphicsDevice, light, random);
            agent2 = new Agent(terrain, Content, camera, GraphicsDevice, light, random);
            box = new Box(Content, camera, GraphicsDevice, light);

            topDownCamera = new Camera();
            topDownCamera.Transform = new Transform();
            topDownCamera.Transform.LocalPosition += Vector3.Up * 60;
            topDownCamera.Transform.Rotate(Vector3.Right, -MathHelper.PiOver2);
            topDownCamera.Position = new Vector2(0.75f, 0f);
            topDownCamera.Size = new Vector2(0.3f, 0.3f);
            topDownCamera.AspectRatio = topDownCamera.Viewport.AspectRatio;


            cameras.Add(topDownCamera);
            cameras.Add(camera);
        }

        protected override void UnloadContent() { }

        protected override void Update(GameTime gameTime)
        {
            Time.Update(gameTime);
            InputManager.Update();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (InputManager.IsKeyDown(Keys.Up)) camera.Transform.Rotate(Vector3.Right, Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.Down)) camera.Transform.Rotate(Vector3.Left, Time.ElapsedGameTime);

            if(agent.CheckCollision(player))
            {
                hits++;
            }

            if (agent1.CheckCollision(player))
            {
                hits++;
            }
            if (agent2.CheckCollision(player)) hits++;

            
            player.Update();
            agent.Update();
            agent1.Update();
            agent2.Update();
            box.Update();

            Vector3 normal;
            if (player.Get<Collider>().Collides(box.Get<Collider>(), out normal))
            {
                Console.WriteLine("HitBox");
                (box.Get<Renderer>().ObjectModel.Meshes[0].Effects[0] as BasicEffect).DiffuseColor = Color.Red.ToVector3();
            }
            else
            {
                Console.WriteLine("NotHitBox");
                (box.Get<Renderer>().ObjectModel.Meshes[0].Effects[0] as BasicEffect).DiffuseColor = Color.Blue.ToVector3();
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.DepthStencilState = new DepthStencilState();


            foreach (Camera camera in cameras)
            {
                GraphicsDevice.Viewport = camera.Viewport;
                

                effect.Parameters["NormalMap"].SetValue(terrain.NormalMap);
                effect.Parameters["World"].SetValue(terrain.Transform.World);
                effect.Parameters["View"].SetValue(camera.View);
                effect.Parameters["Projection"].SetValue(camera.Projection);
                effect.Parameters["CameraPosition"].SetValue(camera.Transform.Position);
                effect.Parameters["LightPosition"].SetValue(light.Transform.Position);
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    terrain.Draw();
                }


                player.Get<Renderer>().Camera = camera;
                agent.Get<Renderer>().Camera = camera;
                agent1.Get<Renderer>().Camera = camera;
                agent2.Get<Renderer>().Camera = camera;
                box.Get<Renderer>().Camera = camera;
                player.Draw();
                    agent.Draw();
                    agent1.Draw();
                    agent2.Draw();
                    box.Draw();               
            }
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "# of Hits: " + hits, new Vector2(100, 50), Color.Red);
            spriteBatch.DrawString(font, "Time: " + Time.TotalGameTime, new Vector2(100, 70), Color.Red);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
