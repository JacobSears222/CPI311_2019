using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CPI311.GameEngine
{
    public class Pacman : GameObject
    {
        public AStarSearch search;

        Ghost ghost { get; set; }
        public int Lives { get; set; }
        public int Score { get; set; }
        public int CollectedPills { get; set; }
        public bool IsDead { get; set; }
        public int GhostEatCount { get; internal set; }
        public TerrainRenderer Terrain { get; set; }
        public Vector3 Position = Vector3.Zero;

        protected Point origin;

        Effect effect;

        public Pacman(TerrainRenderer terrain, ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light) : base()
        {

            Terrain = terrain;

            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);

            Texture2D texture = Content.Load<Texture2D>("PacManSquare");
            Renderer renderer = new Renderer(Content.Load<Model>("Sphere"), Transform, camera, Content, graphicsDevice, light, 1, "SimpleShading", 20f, texture);
            Add<Renderer>(renderer);

            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = renderer.ObjectModel.Meshes[0].BoundingSphere.Radius;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);
        }

        public Vector3 GetGridPosition(Vector3 gridPos)
        {
            float gridW = Terrain.size.X / search.Cols;
            float gridH = Terrain.size.Y / search.Rows;
            return new Vector3(gridW * gridPos.X + gridW / 2 - Terrain.size.X / 2, 0, gridH * gridPos.Z + gridH / 2 - Terrain.size.Y / 2);
        }

        public override void Update()
        {                      

            if (InputManager.IsKeyDown(Keys.W))
            {
                this.Transform.LocalPosition += this.Transform.Forward * Time.ElapsedGameTime * 10f;
            }
            if (InputManager.IsKeyDown(Keys.S))
            {
                this.Transform.LocalPosition += this.Transform.Backward * Time.ElapsedGameTime * 10f;
            }
            if (InputManager.IsKeyDown(Keys.A))
            {
                this.Transform.LocalPosition += this.Transform.Left * Time.ElapsedGameTime * 10f;
            }
            if (InputManager.IsKeyDown(Keys.D))
            {
                this.Transform.LocalPosition += this.Transform.Right * Time.ElapsedGameTime * 10f;
            }

            if (this.Transform.Position.X > 75 && this.Transform.Position.Z < -1 && this.Transform.Position.Z > -6)
            {
                this.Transform.LocalPosition -= Vector3.UnitX * 150;
            }
            if (this.Transform.Position.X < -75 && this.Transform.Position.Z < -1 && this.Transform.Position.Z > -6)
            {
                this.Transform.LocalPosition += Vector3.UnitX * 150;
            }

            if (this.Transform.Position.X > 75 && this.Transform.Position.Z <= -45)
            {
                this.Transform.LocalPosition = new Vector3(75, this.Transform.Position.Y, this.Transform.Position.Z);
            }
            if (this.Transform.Position.X < -75 && this.Transform.Position.Z >= 45) 
            {
                this.Transform.LocalPosition = new Vector3(-75, this.Transform.Position.Y, this.Transform.Position.Z);
            }
            
            if (this.Transform.Position.Z > 45)
            {
                this.Transform.LocalPosition = new Vector3(this.Transform.Position.X, this.Transform.Position.Y, 45);
            }
            if (this.Transform.Position.Z < -45)
            {
                this.Transform.LocalPosition = new Vector3(this.Transform.Position.X, this.Transform.Position.Y, -45);
            }

            if (this.Transform.Position.X > 75)
            {
                this.Transform.LocalPosition = new Vector3(75, this.Transform.Position.Y, this.Transform.Position.Z);
            }
            if (this.Transform.Position.X < -75)
            {
                this.Transform.LocalPosition = new Vector3(-75, this.Transform.Position.Y, this.Transform.Position.Z);
            }
            if (this.Transform.Position.Y > 1)
            {
                this.Transform.LocalPosition = new Vector3(this.Transform.Position.X, 1, this.Transform.Position.Z);
            }

            
            base.Update();
        }
    }
}
