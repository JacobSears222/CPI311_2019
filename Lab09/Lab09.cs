using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;

namespace Lab09
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Lab09 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        AStarSearch search;
        List<Vector3> path;

        int size = 20;

        Random random = new Random();
        
        Camera camera;
        Model cube;
        Model sphere;

        public Lab09()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Time.Initialize();
            InputManager.Initialize();
            ScreenManager.Initialize(graphics);

            search = new AStarSearch(20, 20);

            foreach (AStarNode node in search.Nodes)
                if (random.NextDouble() < 0.2)
                    search.Nodes[random.Next(20), random.Next(20)].Passable = false;

            search.Start = search.Nodes[0, 0];
            search.Start.Passable = true;
            search.End = search.Nodes[size -1, size -1];
            search.End.Passable = true;

            search.Search(); // A search is made here.

            path = new List<Vector3>();
            AStarNode current = search.End;
            while (current != null)
            {
                path.Insert(0, current.Position);
                current = current.Parent;
            }
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            sphere = Content.Load<Model>("Sphere");
            cube = Content.Load<Model>("cube");


            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.LocalPosition = Vector3.One * 10;
            camera.Transform.Rotate(Vector3.Right, -MathHelper.PiOver2);
        }

        protected override void UnloadContent() { }

        protected override void Update(GameTime gameTime)
        {
            Time.Update(gameTime);
            InputManager.Update();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();                 

            if (InputManager.IsKeyPressed(Keys.Space))
            {
                search.Start = search.Nodes[random.Next(search.Cols), random.Next(search.Rows)];
                search.End = search.Nodes[random.Next(search.Cols), random.Next(search.Rows)];

                while (!(search.Start = search.Nodes[random.Next(search.Cols), random.Next(search.Rows)]).Passable);
                while (!(search.End = search.Nodes[random.Next(search.Cols), random.Next(search.Rows)]).Passable);

                search.Search();
                path.Clear();
                AStarNode current = search.End;
                while (current != null)
                {
                    path.Insert(0, current.Position);
                    current = current.Parent;
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Matrix view = camera.View;
            Matrix projection = camera.Projection;

            foreach (AStarNode node in search.Nodes)
                if (!node.Passable)
                    cube.Draw(Matrix.CreateScale(0.5f, 0.05f, 0.5f) * Matrix.CreateTranslation(node.Position), view, projection);

            foreach (Vector3 position in path)
                sphere.Draw(Matrix.CreateScale(0.1f, 0.1f, 0.1f) * Matrix.CreateTranslation(position), view, projection);

            base.Draw(gameTime);
        }
    }
}
