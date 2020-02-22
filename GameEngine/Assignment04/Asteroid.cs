using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CPI311.GameEngine
{
    public class Asteroid : GameObject
    {
        Asteroid[] asteroidList = new Asteroid[GameConstants.NumAsteroids];

        Random random = new Random();

        public bool isActive { get; set; }

        public Vector3 position;
        public Vector3 direction;
        public float speed;

        public Asteroid(ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light) : base()
        {
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);
           

            Texture2D texture = Content.Load<Texture2D>("Square");
            Renderer renderer = new Renderer(Content.Load<Model>("asteroid4"), Transform, camera, Content, graphicsDevice, light, 1, null, 20f, texture);
            Add<Renderer>(renderer);

            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = renderer.ObjectModel.Meshes[0].BoundingSphere.Radius;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);

            isActive = false;
        }

        public override void Update()
        {
            if (this.Transform.Position.X > GameConstants.PlayfieldSizeX)
            {
                this.Transform.LocalPosition -= Vector3.UnitX * 2 * GameConstants.PlayfieldSizeX;
            }
            if (this.Transform.Position.X < -GameConstants.PlayfieldSizeX)
            {
                this.Transform.LocalPosition += Vector3.UnitX * 2 * GameConstants.PlayfieldSizeX;
            }
            if (this.Transform.Position.Y > GameConstants.PlayfieldSizeY)
            {
                this.Transform.LocalPosition -= Vector3.UnitY * 2 * GameConstants.PlayfieldSizeY;
            }
            if (this.Transform.Position.Y < -GameConstants.PlayfieldSizeY)
            {
                this.Transform.LocalPosition += Vector3.UnitY * 2 * GameConstants.PlayfieldSizeY;
            }

            base.Update();
        }

        public void Update(float delta)
        {
            position += direction * speed * GameConstants.AsteroidSpeedAdjustment * delta;
            if (position.X > GameConstants.PlayfieldSizeX) position.X -= 2 * GameConstants.PlayfieldSizeX;
            if (position.X < -GameConstants.PlayfieldSizeX) position.X += 2 * GameConstants.PlayfieldSizeX;
            if (position.Y > GameConstants.PlayfieldSizeY) position.Y -= 2 * GameConstants.PlayfieldSizeY;
            if (position.Y < -GameConstants.PlayfieldSizeY) position.Y += 2 * GameConstants.PlayfieldSizeY;
        }

        public override void Draw()
        {
            if (isActive) base.Draw();
        }
    }
}
