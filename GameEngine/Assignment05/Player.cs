using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CPI311.GameEngine
{
    public class Player : GameObject
    {
        public TerrainRenderer Terrain { get; set; }
        public Vector3 Position = Vector3.Zero;

        Effect effect;

        public Player(TerrainRenderer terrain, ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light) : base()
        {

            Terrain = terrain;

            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);

            Texture2D texture = Content.Load<Texture2D>("Square");
            Renderer renderer = new Renderer(Content.Load<Model>("Sphere"), Transform, camera, Content, graphicsDevice, light, 1, "SimpleShading", 20f, texture);
            Add<Renderer>(renderer);

            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = renderer.ObjectModel.Meshes[0].BoundingSphere.Radius;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);
        }

        public override void Update()
        {
            // Control the player

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
            // change the Y position corresponding to the terrain (maze)
            this.Transform.LocalPosition = new Vector3(this.Transform.LocalPosition.X, Terrain.GetAltitude(this.Transform.LocalPosition), this.Transform.LocalPosition.Z) + Vector3.Up;

            base.Update();
        }
    }
}
